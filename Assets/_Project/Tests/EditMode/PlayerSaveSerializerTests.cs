using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Core;
using Game.Core.Save;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class PlayerSaveSerializerTests
    {
        private PlayerSaveSerializer serializer;

        [SetUp]
        public void SetUp() => serializer = new PlayerSaveSerializer();

        [Test]
        public void RoundTrip_FullyPopulatedSave_KeepsEveryField()
        {
            var original = CreateFullyPopulatedSave();
            var json = serializer.Serialize(original);

            var restored = serializer.Deserialize(json);

            Assert.That(JToken.DeepEquals(JObject.Parse(serializer.Serialize(restored)), JObject.Parse(json)), Is.True);
            Assert.That(JObject.Parse(json).Properties().Select(property => property.Name),
                Is.EquivalentTo(typeof(PlayerSave).GetFields(BindingFlags.Public | BindingFlags.Instance).Select(field => field.Name)));
        }

        [Test]
        public void RoundTrip_FullyPopulatedSave_KeepsCollectionsAndEnums()
        {
            var restored = serializer.Deserialize(serializer.Serialize(CreateFullyPopulatedSave()));

            Assert.That(restored.Balances, Is.EquivalentTo(new Dictionary<CurrencyType, long>
            {
                { CurrencyType.Light, 1234567890123L },
                { CurrencyType.Fragments, 15 },
                { CurrencyType.Pearls, 250 },
                { CurrencyType.LampOil, 7 }
            }));
            Assert.That(restored.MetaLevels["lens"], Is.EqualTo(10));
            Assert.That(restored.Islands["gulls_rest"].BossDefeated, Is.True);
            Assert.That(restored.Islands["gulls_rest"].BuildingLevels["lantern_hut"], Is.EqualTo(3));
            Assert.That(restored.Islands["gulls_rest"].ConsecutiveDefeats, Is.EqualTo(2));
            Assert.That(restored.UnlockedKeepers, Is.EquivalentTo(new[] { "orrin", "sela" }));
            Assert.That(restored.OwnedCosmetics, Is.EquivalentTo(new[] { "ember_lighthouse" }));
            Assert.That(restored.EquippedCosmetics["lighthouse"], Is.EqualTo("ember_lighthouse"));
            Assert.That(restored.LogbookEntries, Is.EquivalentTo(new[] { "first_light" }));
            Assert.That(restored.OwnedProducts, Is.EquivalentTo(new[] { "remove_ads", "starter_pack" }));
            Assert.That(restored.Quests.Active.Select(quest => quest.QuestId), Is.EqualTo(new[] { "defeat_enemies", "finish_runs" }));
            Assert.That(restored.Ads.DailyRewardedCounts["extra_lamp_oil"], Is.EqualTo(2));
            Assert.That(restored.Season.ClaimedFreeTiers, Is.EquivalentTo(new[] { 1, 2, 5 }));
            Assert.That(restored.Tutorial.CompletedSteps, Is.EquivalentTo(new[] { "aim", "flare" }));
            Assert.That(restored.ActiveRun.IslandId, Is.EqualTo("gulls_rest"));
            Assert.That(restored.Settings.Quality, Is.EqualTo(GraphicsQuality.High));
            Assert.That(restored.Settings.MusicVolume, Is.EqualTo(0.35f));
        }

        [Test]
        public void Serialize_Enums_WrittenAsNames()
        {
            var json = JObject.Parse(serializer.Serialize(CreateFullyPopulatedSave()));

            Assert.That(((JObject)json[nameof(PlayerSave.Balances)]).Properties().Select(property => property.Name),
                Is.EquivalentTo(new[] { "Light", "Fragments", "Pearls", "LampOil" }));
            Assert.That(json[nameof(PlayerSave.Settings)][nameof(PlayerSettings.Quality)].Value<string>(), Is.EqualTo("High"));
        }

        [Test]
        public void RoundTrip_NewSave_KeepsNullOptionalFields()
        {
            var restored = serializer.Deserialize(serializer.Serialize(new PlayerSave()));

            Assert.That(restored.ActiveRun, Is.Null);
            Assert.That(restored.EquippedKeeper, Is.Null);
            Assert.That(restored.Settings.Language, Is.Null);
        }

        [Test]
        public void Deserialize_EmptyObject_ReturnsSafeDefaults()
        {
            var restored = serializer.Deserialize("{}");

            Assert.That(restored.Version, Is.EqualTo(PlayerSave.CurrentVersion));
            Assert.That(JToken.DeepEquals(JObject.Parse(serializer.Serialize(restored)), JObject.Parse(serializer.Serialize(new PlayerSave()))), Is.True);
        }

        [Test]
        public void Deserialize_NestedObjectsWithMissingFields_UseDefaults()
        {
            const string json = "{\"Version\":1,\"Settings\":{\"MusicVolume\":0.5},\"Islands\":{\"gulls_rest\":{\"BossDefeated\":true}},\"Energy\":{}}";

            var restored = serializer.Deserialize(json);

            Assert.That(restored.Settings.MusicVolume, Is.EqualTo(0.5f));
            Assert.That(restored.Settings.SfxVolume, Is.EqualTo(new PlayerSettings().SfxVolume));
            Assert.That(restored.Settings.Haptics, Is.EqualTo(new PlayerSettings().Haptics));
            Assert.That(restored.Settings.ScreenShake, Is.EqualTo(new PlayerSettings().ScreenShake));
            Assert.That(restored.Settings.Quality, Is.EqualTo(GraphicsQuality.Auto));
            Assert.That(restored.Islands["gulls_rest"].BuildingLevels, Is.Not.Null.And.Empty);
            Assert.That(restored.Energy.Amount, Is.Zero);
            Assert.That(restored.Daily, Is.Not.Null);
            Assert.That(restored.HighestIsland, Is.EqualTo(1));
        }

        [Test]
        public void Deserialize_ExplicitNulls_UseDefaults()
        {
            const string json = "{\"Balances\":null,\"Islands\":null,\"UnlockedKeepers\":null,\"Energy\":null,\"Settings\":null,\"HighestIsland\":null}";

            var restored = serializer.Deserialize(json);

            Assert.That(restored.Balances, Is.Not.Null.And.Empty);
            Assert.That(restored.Islands, Is.Not.Null.And.Empty);
            Assert.That(restored.UnlockedKeepers, Is.Not.Null.And.Empty);
            Assert.That(restored.Energy, Is.Not.Null);
            Assert.That(restored.Settings.SfxVolume, Is.EqualTo(new PlayerSettings().SfxVolume));
            Assert.That(restored.HighestIsland, Is.EqualTo(1));
        }

        [Test]
        public void Deserialize_UnknownField_IsIgnored()
        {
            var restored = serializer.Deserialize("{\"Version\":1,\"RemovedField\":42,\"RunsCompleted\":3}");

            Assert.That(restored.RunsCompleted, Is.EqualTo(3));
        }

        [Test]
        public void Deserialize_V1WithFakeMigrationToV2_AppliesMigration()
        {
            var serializerV2 = new PlayerSaveSerializer(2, new ISaveMigration[] { new RenameRunsFinishedMigration() });
            const string v1Json = "{\"Version\":1,\"RunsFinished\":12,\"Balances\":{\"Light\":500}}";

            var restored = serializerV2.Deserialize(v1Json);

            Assert.That(restored.Version, Is.EqualTo(2));
            Assert.That(restored.RunsCompleted, Is.EqualTo(12));
            Assert.That(restored.Balances[CurrencyType.Light], Is.EqualTo(500));
            Assert.That(JObject.Parse(serializerV2.Serialize(restored)).ContainsKey("RunsFinished"), Is.False);
        }

        [Test]
        public void Deserialize_V2WithFakeMigrationToV2_LeavesDataUntouched()
        {
            var serializerV2 = new PlayerSaveSerializer(2, new ISaveMigration[] { new RenameRunsFinishedMigration() });

            var restored = serializerV2.Deserialize("{\"Version\":2,\"RunsCompleted\":4,\"RunsFinished\":99}");

            Assert.That(restored.RunsCompleted, Is.EqualTo(4));
        }

        [Test]
        public void Deserialize_NewerVersion_ThrowsSaveDataException() =>
            Assert.Throws<SaveDataException>(() => serializer.Deserialize($"{{\"Version\":{PlayerSave.CurrentVersion + 1}}}"));

        [TestCase("")]
        [TestCase("   ")]
        [TestCase("not json")]
        [TestCase("{\"Version\":1,")]
        [TestCase("[]")]
        [TestCase("null")]
        [TestCase("{\"RunsCompleted\":\"many\"}")]
        [TestCase("{\"Balances\":{\"Gems\":5}}")]
        public void Deserialize_InvalidContent_ThrowsSaveDataException(string json) =>
            Assert.Throws<SaveDataException>(() => serializer.Deserialize(json));

        private static PlayerSave CreateFullyPopulatedSave() => new PlayerSave
        {
            Balances = new Dictionary<CurrencyType, long>
            {
                { CurrencyType.Light, 1234567890123L },
                { CurrencyType.Fragments, 15 },
                { CurrencyType.Pearls, 250 },
                { CurrencyType.LampOil, 7 }
            },
            MetaLevels = new Dictionary<string, int> { { "lens", 10 }, { "keepers_luck", 4 } },
            HighestIsland = 2,
            Islands = new Dictionary<string, IslandState>
            {
                {
                    "gulls_rest",
                    new IslandState
                    {
                        BossDefeated = true,
                        BuildingLevels = new Dictionary<string, int> { { "lantern_hut", 3 }, { "net_loft", 1 } },
                        ConsecutiveDefeats = 2
                    }
                }
            },
            UnlockedKeepers = new HashSet<string> { "orrin", "sela" },
            EquippedKeeper = "sela",
            OwnedCosmetics = new HashSet<string> { "ember_lighthouse" },
            EquippedCosmetics = new Dictionary<string, string> { { "lighthouse", "ember_lighthouse" } },
            LogbookEntries = new HashSet<string> { "first_light" },
            OwnedProducts = new HashSet<string> { "remove_ads", "starter_pack" },
            Energy = new EnergyState { Amount = 4, LastRegenTimestamp = 1760000000000L, BonusAboveCap = 2 },
            Daily = new DailyState { CycleDay = 5, LastClaimTimestamp = 1760000100000L, Streak = 9, ForgivenDayTimestamp = 1759900000000L },
            Quests = new QuestState
            {
                AssignedTimestamp = 1760000200000L,
                Active = new List<QuestProgress>
                {
                    new QuestProgress { QuestId = "defeat_enemies", Progress = 40, Claimed = false },
                    new QuestProgress { QuestId = "finish_runs", Progress = 3, Claimed = true }
                },
                CompletionBonusClaimed = true
            },
            Ads = new AdCounters
            {
                CountersDayTimestamp = 1760000300000L,
                DailyRewardedCounts = new Dictionary<string, int> { { "extra_lamp_oil", 2 }, { "light_cache_x2", 1 } },
                LastInterstitialTimestamp = 1760000400000L,
                RunsSinceLastInterstitial = 1
            },
            Season = new SeasonState
            {
                SeasonId = "s1",
                Xp = 830,
                ClaimedFreeTiers = new HashSet<int> { 1, 2, 5 },
                ClaimedPremiumTiers = new HashSet<int> { 1 }
            },
            Tutorial = new TutorialState { TutorialRunCompleted = true, CompletedSteps = new HashSet<string> { "aim", "flare" } },
            ActiveRun = new RunInProgress { IslandId = "gulls_rest", Wave = 3, StartedTimestamp = 1760000500000L, LampOilCharged = true },
            RunsCompleted = 27,
            StarterPackOffered = true,
            LastSeenTimestamp = 1760000600000L,
            LightCacheTimestamp = 1760000700000L,
            Settings = new PlayerSettings
            {
                MusicVolume = 0.35f,
                SfxVolume = 0.8f,
                Haptics = false,
                LeftHanded = true,
                ScreenShake = false,
                Quality = GraphicsQuality.High,
                Language = "fr"
            }
        };

        private sealed class RenameRunsFinishedMigration : ISaveMigration
        {
            private const string LegacyField = "RunsFinished";

            public int FromVersion => 1;

            public void Apply(JObject save)
            {
                var legacyValue = save[LegacyField];
                if (legacyValue == null) return;
                save[nameof(PlayerSave.RunsCompleted)] = legacyValue;
                save.Remove(LegacyField);
            }
        }
    }
}

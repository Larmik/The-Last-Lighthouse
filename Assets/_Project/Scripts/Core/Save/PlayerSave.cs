using System;
using System.Collections.Generic;

namespace Game.Core.Save
{
    [Serializable]
    public sealed class PlayerSave
    {
        public const int CurrentVersion = 1;

        public int Version = CurrentVersion;
        public Dictionary<CurrencyType, long> Balances = new();
        public Dictionary<string, int> MetaLevels = new();
        public int HighestIsland = 1;
        public Dictionary<string, IslandState> Islands = new();
        public HashSet<string> UnlockedKeepers = new();
        public string EquippedKeeper;
        public HashSet<string> OwnedCosmetics = new();
        public Dictionary<string, string> EquippedCosmetics = new();
        public HashSet<string> LogbookEntries = new();
        public HashSet<string> OwnedProducts = new();
        public EnergyState Energy = new();
        public DailyState Daily = new();
        public QuestState Quests = new();
        public AdCounters Ads = new();
        public SeasonState Season = new();
        public TutorialState Tutorial = new();
        public RunInProgress ActiveRun;
        public int RunsCompleted;
        public bool StarterPackOffered;
        public long LastSeenTimestamp;
        public long LightCacheTimestamp;
        public PlayerSettings Settings = new();
    }
}

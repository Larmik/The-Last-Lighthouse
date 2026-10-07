namespace Game.Services.Ads
{
    public static class AdPlacements
    {
        public const string Revive = "revive";
        public const string DoubleRewards = "double_rewards";
        public const string ExtraLampOil = "extra_lamp_oil";
        public const string LightCacheX2 = "light_cache_x2";
        public const string Reroll = "reroll";

        public static bool IsRewarded(string placement) =>
            placement is Revive or DoubleRewards or ExtraLampOil or LightCacheX2 or Reroll;
    }
}

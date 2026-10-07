namespace Game.Services.Purchases
{
    public static class ProductIds
    {
        public const string RemoveAds = "remove_ads";
        public const string StarterPack = "starter_pack";
        public const string PearlsS = "pearls_s";
        public const string PearlsM = "pearls_m";
        public const string PearlsL = "pearls_l";
        public const string PearlsXl = "pearls_xl";
        public const string PearlsXxl = "pearls_xxl";

        public static bool IsConsumable(string productId) =>
            productId is PearlsS or PearlsM or PearlsL or PearlsXl or PearlsXxl;
    }
}

using System;

namespace Game.Core.Save
{
    [Serializable]
    public sealed class PlayerSettings
    {
        public float MusicVolume = 1f;
        public float SfxVolume = 1f;
        public bool Haptics = true;
        public bool LeftHanded;
        public bool ScreenShake = true;
        public GraphicsQuality Quality = GraphicsQuality.Auto;
        public string Language;
    }
}

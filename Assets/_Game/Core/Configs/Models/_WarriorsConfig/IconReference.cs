using _Game.Core.Configs.Models._AgeConfig;
using UnityEngine.AddressableAssets;

namespace _Game.Core.Configs.Models._WarriorsConfig
{
    public class IconReference : IIconReference
    {
        public AssetReference Atlas { get; }
        public string IconName { get; }
        
        public IconReference(AssetReference atlas, string iconName)
        {
            Atlas = atlas;
            IconName = iconName;
        }
    }
}
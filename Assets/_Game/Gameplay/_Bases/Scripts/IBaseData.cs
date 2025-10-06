using UnityEngine.AddressableAssets;

namespace _Game.Gameplay._Bases.Scripts
{
    public interface IBaseData
    {
        public AssetReference BaseAssetReference { get; }
        public float CoinsAmount { get; }
        public int Layer { get; }
        float Health { get; }
    }
}
using UnityEngine.AddressableAssets;

namespace _Game.Gameplay._Bases.Scripts
{
    public abstract class BaseDataDecorator : IBaseData
    {
        protected readonly IBaseData _baseData;

        protected BaseDataDecorator(IBaseData baseData) => _baseData = baseData;
        public AssetReference BaseAssetReference => _baseData.BaseAssetReference;
        public virtual float CoinsAmount => _baseData.CoinsAmount;
        public int Layer => _baseData.Layer;
        public virtual float Health => _baseData.Health;
    }
}
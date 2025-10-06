using UnityEngine.AddressableAssets;

namespace _Game.Gameplay._Bases.Scripts
{
    public class BaseData : IBaseData
    {
        public AssetReference BaseAssetReference { get; private set; }
        public int  Layer { get; private set; }
        public float Health { get; private set; }
        public float CoinsAmount { get; private set; }
        
        public class BaseDataBuilder
        {
            AssetReference _baseAssetReference;
            int _layer;
            float _health;
            float _coinsAmount;

            public BaseDataBuilder WithAssetReference(AssetReference baseAssetReference)
            {
                _baseAssetReference = baseAssetReference;
                return this;
            }
            public BaseDataBuilder WithLayer(int layer)
            {
                _layer = layer;
                return this;
            }
            public BaseDataBuilder WithHealth(float health)
            {
                _health = health;
                return this;
            }
            
            public BaseDataBuilder WithCoins(float coinsAmount)
            {
                _coinsAmount = coinsAmount;
                return this;
            }

            public BaseData Build()
            {
                var baseData = new BaseData
                {
                    BaseAssetReference = _baseAssetReference,
                    Layer = _layer,
                    Health = _health,
                    CoinsAmount = _coinsAmount
                };
                return baseData;
            }
        }
    }
}
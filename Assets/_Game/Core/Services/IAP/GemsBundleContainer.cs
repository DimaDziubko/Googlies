using System.Collections.Generic;

namespace _Game.Core.Services.IAP
{
    public class GemsBundleContainer
    {
        private readonly Dictionary<string, GemsBundle> _gemsBundles = new();
        public void AddOrUpdate(string productId, GemsBundle gemsBundle) => 
            _gemsBundles[productId] = gemsBundle;

        public GemsBundle Get(string productId) => 
            _gemsBundles.ContainsKey(productId) ? _gemsBundles[productId] : null;
        
        public bool TryGetValue(string productId, out GemsBundle bundle) => 
            _gemsBundles.TryGetValue(productId, out bundle);

        public bool Contains(string productId) => 
            _gemsBundles.ContainsKey(productId);

        public void Clear() => 
            _gemsBundles.Clear();
        
        public IEnumerable<GemsBundle> GetAll() => 
            _gemsBundles.Values;
    }
}
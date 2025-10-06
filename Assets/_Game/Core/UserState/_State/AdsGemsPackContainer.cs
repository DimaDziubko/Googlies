using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace _Game.Core.UserState._State
{
    public class AdsGemsPackContainer : IAdsGemsPackContainer
    {
        [ShowInInspector]
        private readonly Dictionary<int, AdsGemsPackState> _adsGemsPacks = new();

        public IReadOnlyDictionary<int, AdsGemsPackState> AdsGemsPacks => _adsGemsPacks;

        public void AddPack(int packId, AdsGemsPackState pack)
        {
            if (_adsGemsPacks.ContainsKey(packId))
            {
                _adsGemsPacks[packId] = pack;
            }
            else
            {
                _adsGemsPacks.Add(packId, pack);
            }
        }

        public bool TryGetPack(int packId, out AdsGemsPackState pack)
        {
            return _adsGemsPacks.TryGetValue(packId, out pack);
        }

        public void RemovePack(int packId)
        {
            if (_adsGemsPacks.ContainsKey(packId))
            {
                _adsGemsPacks.Remove(packId);
            }
        }

        public void ClearAllPacks()
        {
            _adsGemsPacks.Clear();
        }
    }

    public interface IAdsGemsPackContainer
    {
        public void AddPack(int packId, AdsGemsPackState pack);
        public bool TryGetPack(int packId, out AdsGemsPackState pack);
        public void RemovePack(int packId);
        public void ClearAllPacks();
        
        IReadOnlyDictionary<int, AdsGemsPackState> AdsGemsPacks { get;}
    }
}

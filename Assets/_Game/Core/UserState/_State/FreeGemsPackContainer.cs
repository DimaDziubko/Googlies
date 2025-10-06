using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace _Game.Core.UserState._State
{
    public class FreeGemsPackContainer : IFreeGemsPackContainer
    {
        [ShowInInspector]
        private readonly Dictionary<int, FreeGemsPackState> _freeGemsPacks = new();

        public IReadOnlyDictionary<int, FreeGemsPackState> FreeGemsPacks => _freeGemsPacks;

        public void AddPack(int packId, FreeGemsPackState pack) => 
            _freeGemsPacks[packId] = pack;

        public bool TryGetPack(int packId, out FreeGemsPackState pack) => 
            _freeGemsPacks.TryGetValue(packId, out pack);

        public void RemovePack(int packId)
        {
            if (_freeGemsPacks.ContainsKey(packId))
            {
                _freeGemsPacks.Remove(packId);
            }
        }

        public void ClearAllPacks() => 
            _freeGemsPacks.Clear();
    }
}
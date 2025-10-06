using System.Collections.Generic;

namespace _Game.Core.UserState._State
{
    public interface IFreeGemsPackContainer
    {
        public void AddPack(int packId, FreeGemsPackState pack);
        public bool TryGetPack(int packId, out FreeGemsPackState pack);
        public void RemovePack(int packId);
        public void ClearAllPacks();
        IReadOnlyDictionary<int, FreeGemsPackState> FreeGemsPacks { get;}
    }
}
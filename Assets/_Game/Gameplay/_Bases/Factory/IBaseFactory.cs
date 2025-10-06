using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Gameplay._Bases.Factory
{
    public interface IBaseFactory
    {
        UniTask<Base> GetAsync(Faction faction);
        public void Reclaim(Base @base);
        void Cleanup();
    }
}
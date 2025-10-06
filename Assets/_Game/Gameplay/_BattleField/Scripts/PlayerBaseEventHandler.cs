using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class PlayerBaseEventHandler : IBaseEventHandler
    {
        private readonly IBattleTriggersManager _battleTriggersManager;
        private readonly IVFXProxy _vfxProxy;

        public PlayerBaseEventHandler(
            IBattleTriggersManager battleTriggersManager,
            IVFXProxy vfxProxy)
        {
            _battleTriggersManager = battleTriggersManager;
            _vfxProxy = vfxProxy;
        }

        public void OnBaseHit(Base @base, float damage, float maxHealth)
        {
            _vfxProxy.SpawnDamageTextLeft(@base.DamageTextPosition, $"<style=SpecialGrey>{damage.ToCompactFormat(10)}</style>");
        }

        public void OnBaseDestructionStarted(Base @base) =>
            _battleTriggersManager.BaseDestructionStarted(Faction.Player, @base);

        public void OnBaseDestructionCompleted(Base @base) =>
            _battleTriggersManager.BaseDestructionCompleted(Faction.Player, @base);
    }
}
using System.Collections.Generic;
using _Game.Gameplay.BattleLauncher;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core.CustomKernel
{
    public sealed class GameKernel : MonoKernel
    {
        [ShowInInspector]
        [Inject]
        private IGameManager _gameManager;
        
        [ShowInInspector]
        [Inject(Optional = true, Source = InjectSources.Local)]
        private List<IGameTickable> _tickables = new();
        
        [ShowInInspector]
        [Inject(Optional = true, Source = InjectSources.Local)]
        private List<IGameFixedTickable> _fixedTickables = new();
        
        [ShowInInspector]
        [Inject(Optional = true, Source = InjectSources.Local)]
        private List<IGameLateTickable> _lateTickables = new();


        public override void Update()
        {
            base.Update();

            if (_gameManager.State == BattleState.Play || _gameManager.State == BattleState.Stop)
            {
                float deltaTime = Time.deltaTime;
                foreach (var tickable in _tickables)
                {
                    tickable.Tick(deltaTime);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_gameManager.State == BattleState.Play || _gameManager.State == BattleState.Stop)
            {
                float deltaTime = Time.fixedDeltaTime;
                foreach (var fixedTickable in _fixedTickables)
                {
                    fixedTickable.FixedTick(deltaTime);
                }
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            
            if (_gameManager.State == BattleState.Play || _gameManager.State == BattleState.Stop)
            {
                float deltaTime = Time.deltaTime;
                foreach (var lateTickable in _lateTickables)
                {
                    lateTickable.LateTick(deltaTime);
                }
            }
        }
    }
}
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._Coins.Factory;
using _Game.Gameplay._Coins.Scripts;
using UnityEngine;
using UnityUtils;

namespace _Game.Gameplay._BattleField.Scripts
{
    public struct CoinLogicTimer
    {
        public float Amount;
        public float Timer;
    }

    public class LootCoinSpawner : ILootCoinSpawner
    {
        private readonly ICoinFactory _coinFactory;
        private readonly IAudioService _audioService;
        private readonly ICoinCounter _coinCounter;
        private readonly IMyLogger _logger;

        private Vector3 _coinCounterPosition;

        private readonly List<CoinLogicTimer> _logicTimers = new(100);

        private const float DELAY_PER_COIN = 2.5f;

        private float _testCounter;

        public LootCoinSpawner(
            ICoinFactory coinFactory,
            IAudioService audioService,
            ICoinCounter coinCounter,
            IMyLogger logger)
        {
            _coinFactory = coinFactory;
            _audioService = audioService;
            _coinCounter = coinCounter;
            _logger = logger;
        }

        public void Init(Vector3 coinCounterPosition) =>
            _coinCounterPosition = coinCounterPosition;

        public void OnStartBattle()
        {
            _coinFactory.Warmup();
        }

        void ILootCoinSpawner.SpawnLootCoin(Vector3 position, float amount)
        {
            _logicTimers.Add(new CoinLogicTimer { Amount = amount, Timer = DELAY_PER_COIN });

            _audioService.PlayCoinDropSound();
            
            LootFlyingReward lootCoin = _coinFactory.GetLootCoin();
            if (lootCoin != null && lootCoin.OrNull() != null)
            {
                lootCoin.Position = position;
                lootCoin.Init(_coinCounterPosition);
                lootCoin.Jump();
            }
            
            _testCounter += amount;
            
            _logger.Log($"LOOT COINS AMOUNT: {_testCounter}", DebugStatus.Info);
        }

        public void OnCoinCollected(float amount)
        {
            _audioService.PlayCoinCollectSound();
            _coinCounter.AddCoins(amount);
        }

        public void GameUpdate(float deltaTime)
        {
            for (int i = _logicTimers.Count - 1; i >= 0; i--)
            {
                var timer = _logicTimers[i];
                timer.Timer -= deltaTime;
                if (timer.Timer <= 0)
                {
                    OnCoinCollected(timer.Amount);
                    _logicTimers[i] = _logicTimers[^1];
                    _logicTimers.RemoveAt(_logicTimers.Count - 1);
                }
                else
                {
                    _logicTimers[i] = timer;
                }
            }
        }

        public void Cleanup()
        {
            _logicTimers.Clear();
            _testCounter = 0;
        }
    }
}
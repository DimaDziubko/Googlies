using System;
using _Game.Core._Logger;
using _Game.Gameplay._Battle.Scripts;
using _Game.UI._Environment.Factory;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Environment
{
    public class BattleEnvironmentController
    {
        private readonly IEnvironmentFactory _factory;
        private readonly IMyLogger _logger;
        
        private BattleEnvironment _currentBattleEnvironment;

        private bool _isUpdating;
        
        public BattleEnvironmentController(
            IEnvironmentFactory factory,
            IMyLogger logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async UniTask ShowEnvironment(string envKey)
        {
            if (_isUpdating)
            {
                _logger.Log("ShowEnvironment call was skipped — already updating", DebugStatus.Warning);
                return;
            }

            _isUpdating = true;

            try
            {
                _logger.Log($"SHOW ENVIRONMENT: {envKey} IS CURRENT NULL {_currentBattleEnvironment == null} ",
                    DebugStatus.Warning);

                var newEnvironment = await _factory.Get(envKey);

                if (_currentBattleEnvironment != null && _currentBattleEnvironment.gameObject != null)
                {
                    _currentBattleEnvironment?.Hide();
                }

                _currentBattleEnvironment = newEnvironment;
                _currentBattleEnvironment.Show();
            }
            catch (Exception e)
            {
                var newEnvironment = await _factory.Get(envKey);
                _currentBattleEnvironment = newEnvironment;
                _currentBattleEnvironment.Show();
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }
}
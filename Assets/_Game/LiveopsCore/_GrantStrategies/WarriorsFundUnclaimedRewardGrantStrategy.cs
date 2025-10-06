using System;
using System.Collections;
using _Game.Core.Boosts;
using _Game.Gameplay._RewardProcessing;
using _Game.Gameplay.Common;
using _Game.LiveopsCore.Models.WarriorsFund;
using UnityEngine;

namespace _Game.LiveopsCore._GrantStrategies
{
    public class WarriorsFundUnclaimedRewardGrantStrategy : IGameEventUnclaimedRewardGrantStrategy
    {
        private const int CLAIM_DELAY_SECONDS = 0;
        public event Action<int> Complete;

        private readonly WarriorsFundEvent _event;
        private readonly RewardProcessingService _rewardProcessing;
        private readonly ICoroutineRunner _runner;

        public WarriorsFundUnclaimedRewardGrantStrategy(
            WarriorsFundEvent @event,
            RewardProcessingService rewardProcessing,
            ICoroutineRunner runner)
        {
            _event = @event;
            _rewardProcessing = rewardProcessing;
            _runner = runner;
        }
        
        public bool Execute()
        {
            if (_event != null)
            {
                _event.ClaimUnclaimedRewardsRequested += OnClaimUnclaimedRewardsRequested;
                return true;
            }

            return false;
        }

        private void OnClaimUnclaimedRewardsRequested() => 
            _runner.StartCoroutine(ClaimAfterDelay(CLAIM_DELAY_SECONDS));
        
        public void UnExecute()
        {
            _event.ClaimUnclaimedRewardsRequested -= OnClaimUnclaimedRewardsRequested;
            Complete?.Invoke(_event.Id);
        }

        public void Cleanup() => 
            _event.ClaimUnclaimedRewardsRequested -= OnClaimUnclaimedRewardsRequested;
        
        
        private IEnumerator ClaimAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            foreach (var reward in _event.UnclaimedRewards)
            {
                _rewardProcessing.Process(reward, ItemSource.LeaderPass);
            }
            
            _event.OnUnclaimedRewardsClaimed();
            
            Complete?.Invoke(_event.Id);
        }
    }
}
using System;
using _Game.Core._Logger;
using _Game.Core._Time;

namespace _Game.Core.Services._TimeBasedRecoveryCalculator
{
    public class TimeBasedRecoveryCalculator
    {
        private readonly IMyLogger _logger;

        public TimeBasedRecoveryCalculator(IMyLogger logger)
        {
            _logger = logger;
        }
        
        public bool CalculateRecoveredUnits(int currentCount, int maxCount, int recoverTimeMinutes, DateTime lastUseTime, out int recoveredUnits)
        {
            recoveredUnits = 0;
            
            if (currentCount >= maxCount)
            {
                return true;
            }

            DateTime now = GlobalTime.UtcNow;
            TimeSpan timeSinceLastUse = now - lastUseTime;

            if (now.Date > lastUseTime.Date)
            {
                recoveredUnits = maxCount - currentCount;
                return true;
            }
            
            if (timeSinceLastUse.TotalMinutes < 0)
            {
                timeSinceLastUse = TimeSpan.Zero;
            }

            int recoverableUnits = Math.Max(0, (int)(timeSinceLastUse.TotalMinutes / recoverTimeMinutes));
            recoveredUnits = Math.Min(recoverableUnits, maxCount - currentCount);

            return currentCount + recoveredUnits >= maxCount;
        }
        
        public float CalculateTimeUntilNextRecoverySeconds(int recoveringTimeMinutes, DateTime lastUseTime)
        {
            DateTime now = GlobalTime.UtcNow;
            
            TimeSpan recoveryInterval = TimeSpan.FromMinutes(recoveringTimeMinutes);
            
            TimeSpan timeForNextUnit = recoveryInterval - (now - lastUseTime);
            
            float secondsUntilNextRecovery = (float)Math.Max(0, timeForNextUnit.TotalSeconds);

            return secondsUntilNextRecovery;
        }
        
        public DateTime CalculateNewLastUseTime(DateTime lastUseTime, int recoveredUnits, int recoverTimeMinutes) => 
            lastUseTime.AddMinutes(recoveredUnits * recoverTimeMinutes);
    }
}
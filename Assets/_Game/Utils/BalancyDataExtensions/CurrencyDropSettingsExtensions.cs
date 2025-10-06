using System.Collections.Generic;
using _Game.Gameplay._Units.Scripts;
using _Game.LiveopsCore._GameEventCurrencyManagement;
using Balancy.Models;

namespace _Game.Utils.BalancyDataExtensions
{
    public static class CurrencyDropSettingsExtensions
    {
        public static CurrencyDropSettings ToLocal(this CurrencyDropSettingsB remote)
        {
            var settings = new CurrencyDropSettings()
            {
                BattleModeSettings = remote.BattleModeSettings.ToLocal(),
                ZombieRushModeSettings = remote.ZombieRushModeSettings.ToLocal(),
                LegendsModeSettings = remote.LegendsSettings.ToLocal(),
                BossesModeSettings = remote.BossesSettings.ToLocal()
            };
            
            return settings;
        }
        
        public static Dictionary<UnitType, UnitLootDropSettingsWithDropChance> ToLocal(this UnitLootDropSettingsWithDropChanceB[] remoteSettings)
        {
            var localSettings = new Dictionary<UnitType, UnitLootDropSettingsWithDropChance>();

            foreach (var remoteConfig in remoteSettings)
            {
                localSettings.TryAdd(remoteConfig.UnitType.ToLocal(), new UnitLootDropSettingsWithDropChance()
                {
                    UnitType = remoteConfig.UnitType.ToLocal(), 
                    DropChance = remoteConfig.DropChance,
                    Amount = remoteConfig.Amount
                });
            }
            
            return localSettings;
        }

        public static UnitType ToLocal(this UnitTypeB remote)
        {
            return remote switch
            {
                UnitTypeB.Light => UnitType.Light,
                UnitTypeB.Medium => UnitType.Medium,
                UnitTypeB.Heavy => UnitType.Heavy,
                UnitTypeB.Boss => UnitType.Boss,
                _ => UnitType.Light,
            };
        }
    }
}
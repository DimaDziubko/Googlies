using System.Collections.Generic;
using _Game.Gameplay._Units.Scripts;

namespace _Game.LiveopsCore._GameEventCurrencyManagement
{
    public class CurrencyDropSettings
    {
        public Dictionary<UnitType, UnitLootDropSettingsWithDropChance> BattleModeSettings;
        public Dictionary<UnitType, UnitLootDropSettingsWithDropChance> ZombieRushModeSettings;
        public Dictionary<UnitType, UnitLootDropSettingsWithDropChance> LegendsModeSettings;
        public Dictionary<UnitType, UnitLootDropSettingsWithDropChance> BossesModeSettings;

        public static CurrencyDropSettings CreateDefault()
        {
            return new CurrencyDropSettings()
            {
                BattleModeSettings = new Dictionary<UnitType, UnitLootDropSettingsWithDropChance>()
                {
                    {
                        UnitType.Light,
                        new UnitLootDropSettingsWithDropChance()
                            { UnitType = UnitType.Light, DropChance = 0.5f, Amount = 1 }
                    },
                    {
                        UnitType.Medium,
                        new UnitLootDropSettingsWithDropChance()
                            { UnitType = UnitType.Medium, DropChance = 0.5f, Amount = 1 }
                    },
                    {
                        UnitType.Heavy,
                        new UnitLootDropSettingsWithDropChance()
                            { UnitType = UnitType.Heavy, DropChance = 0.5f, Amount = 1 }
                    },
                },
                
                ZombieRushModeSettings  = new Dictionary<UnitType, UnitLootDropSettingsWithDropChance>()
                {
                    {
                        UnitType.Light,
                        new UnitLootDropSettingsWithDropChance()
                            { UnitType = UnitType.Light, DropChance = 0.5f, Amount = 1 }
                    },
                    {
                        UnitType.Medium,
                        new UnitLootDropSettingsWithDropChance()
                            { UnitType = UnitType.Medium, DropChance = 0.5f, Amount = 1 }
                    },
                    {
                        UnitType.Heavy,
                        new UnitLootDropSettingsWithDropChance()
                            { UnitType = UnitType.Heavy, DropChance = 0.5f, Amount = 1 }
                    },
                },
                
                LegendsModeSettings = new Dictionary<UnitType, UnitLootDropSettingsWithDropChance>()
                {
                    {
                        UnitType.Light,
                        new UnitLootDropSettingsWithDropChance()
                            { UnitType = UnitType.Light, DropChance = 0.5f, Amount = 1 }
                    },
                    {
                        UnitType.Medium,
                        new UnitLootDropSettingsWithDropChance()
                            { UnitType = UnitType.Medium, DropChance = 0.5f, Amount = 1 }
                    },
                    {
                        UnitType.Heavy,
                        new UnitLootDropSettingsWithDropChance()
                            { UnitType = UnitType.Heavy, DropChance = 0.5f, Amount = 1 }
                    },
                },
                
                BossesModeSettings = new Dictionary<UnitType, UnitLootDropSettingsWithDropChance>()
                {
                    {
                        UnitType.Boss,
                        new UnitLootDropSettingsWithDropChance()
                            { UnitType = UnitType.Boss, DropChance = 1.0f, Amount = 5 }
                    }
                },
            };
        }
    }
}
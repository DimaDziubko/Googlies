using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class CurrencyDropSettingsB : BaseModel
	{

		[JsonProperty]
		private string[] unnyIdBattleModeSettings;
		[JsonProperty]
		private string[] unnyIdBossesSettings;
		[JsonProperty]
		private string[] unnyIdZombieRushModeSettings;
		[JsonProperty]
		private string[] unnyIdLegendsSettings;


		[JsonIgnore]
		public Models.UnitLootDropSettingsWithDropChanceB[] BattleModeSettings
		{
			get
			{
				if (unnyIdBattleModeSettings == null)
					return new Models.UnitLootDropSettingsWithDropChanceB[0];
				var battleModeSettings = new Models.UnitLootDropSettingsWithDropChanceB[unnyIdBattleModeSettings.Length];
				for (int i = 0;i < unnyIdBattleModeSettings.Length;i++)
					battleModeSettings[i] = DataEditor.GetModelByUnnyId<Models.UnitLootDropSettingsWithDropChanceB>(unnyIdBattleModeSettings[i]);
				return battleModeSettings;
			}
		}

		[JsonIgnore]
		public Models.UnitLootDropSettingsWithDropChanceB[] BossesSettings
		{
			get
			{
				if (unnyIdBossesSettings == null)
					return new Models.UnitLootDropSettingsWithDropChanceB[0];
				var bossesSettings = new Models.UnitLootDropSettingsWithDropChanceB[unnyIdBossesSettings.Length];
				for (int i = 0;i < unnyIdBossesSettings.Length;i++)
					bossesSettings[i] = DataEditor.GetModelByUnnyId<Models.UnitLootDropSettingsWithDropChanceB>(unnyIdBossesSettings[i]);
				return bossesSettings;
			}
		}

		[JsonIgnore]
		public Models.UnitLootDropSettingsWithDropChanceB[] ZombieRushModeSettings
		{
			get
			{
				if (unnyIdZombieRushModeSettings == null)
					return new Models.UnitLootDropSettingsWithDropChanceB[0];
				var zombieRushModeSettings = new Models.UnitLootDropSettingsWithDropChanceB[unnyIdZombieRushModeSettings.Length];
				for (int i = 0;i < unnyIdZombieRushModeSettings.Length;i++)
					zombieRushModeSettings[i] = DataEditor.GetModelByUnnyId<Models.UnitLootDropSettingsWithDropChanceB>(unnyIdZombieRushModeSettings[i]);
				return zombieRushModeSettings;
			}
		}

		[JsonIgnore]
		public Models.UnitLootDropSettingsWithDropChanceB[] LegendsSettings
		{
			get
			{
				if (unnyIdLegendsSettings == null)
					return new Models.UnitLootDropSettingsWithDropChanceB[0];
				var legendsSettings = new Models.UnitLootDropSettingsWithDropChanceB[unnyIdLegendsSettings.Length];
				for (int i = 0;i < unnyIdLegendsSettings.Length;i++)
					legendsSettings[i] = DataEditor.GetModelByUnnyId<Models.UnitLootDropSettingsWithDropChanceB>(unnyIdLegendsSettings[i]);
				return legendsSettings;
			}
		}

	}
#pragma warning restore 649
}
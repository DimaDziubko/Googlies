using System;
using Balancy.Data;
using System.Collections.Generic;

namespace Balancy
{
#pragma warning disable 649

	public partial class DataEditor
	{

		private static void LoadSmartObject(string userId, string name, string key, Action<ParentBaseData> callback)
		{
			switch (name)
			{
				case "GameProfile":
				{
					SmartStorage.LoadSmartObject<Data.GameProfile>(userId, key, responseData =>
					{
						callback?.Invoke(responseData.Data);
					});
					break;
				}
				case "SmartObjects.UnnyProfile":
				{
					SmartStorage.LoadSmartObject<Data.SmartObjects.UnnyProfile>(userId, key, responseData =>
					{
						callback?.Invoke(responseData.Data);
					});
					break;
				}
				default:
					UnnyLogger.Critical("No SmartObject found by name " + name);
					break;
			}
		}

		static partial void MoveAllData(string userId)
		{
			MigrateSmartObject(userId, "GameProfile");
			MigrateSmartObject(userId, "UnnyProfile");
		}

		static partial void TransferAllSmartObjectsFromLocalToCloud(string userId)
		{
			TransferSmartObjectFromLocalToCloud<Data.GameProfile>(userId);
			TransferSmartObjectFromLocalToCloud<Data.SmartObjects.UnnyProfile>(userId);
		}

		static partial void ResetAllSmartObjects(string userId)
		{
			ResetSmartObject<Data.GameProfile>(userId);
			ResetSmartObject<Data.SmartObjects.UnnyProfile>(userId);
		}

		static partial void PreloadAllSmartObjects(string userId, bool skipServerLoading)
		{
			SmartStorage.LoadSmartObject<Data.GameProfile>(userId, null, skipServerLoading);
		}

		public static List<Models.CardItemB> CardItemBS { get; private set; }
		public static List<Models.WarriorsFundB> WarriorsFundBS { get; private set; }
		public static List<Models.EventIAPs> EventIAPs { get; private set; }
		public static List<Models.CustomGameEvent> CustomGameEvents { get; private set; }
		public static List<Models.MoneyItemB> MoneyItemBS { get; private set; }
		public static List<Models.KeyItemB> KeyItemBS { get; private set; }
		public static List<Models.ClassicOfferB> ClassicOfferBS { get; private set; }
		public static List<Models.BattlePassB> BattlePassBS { get; private set; }

		static partial void PrepareGeneratedData() {
			CardItemBS = DataManager.ParseList<Models.CardItemB>();
			ParseDictionary<Models.CurrencyDropSettingsB>();
			WarriorsFundBS = DataManager.ParseList<Models.WarriorsFundB>();
			EventIAPs = DataManager.ParseList<Models.EventIAPs>();
			CustomGameEvents = DataManager.ParseList<Models.CustomGameEvent>();
			MoneyItemBS = DataManager.ParseList<Models.MoneyItemB>();
			ParseDictionary<Models.BattlePassPointB>();
			ParseDictionary<Models.ConditionalPrice>();
			ParseDictionary<Models.ConditionalRewardList>();
			ParseDictionary<Models.UserProgressComponent>();
			KeyItemBS = DataManager.ParseList<Models.KeyItemB>();
			ClassicOfferBS = DataManager.ParseList<Models.ClassicOfferB>();
			ParseDictionary<Models.UnitLootDropSettingsWithDropChanceB>();
			ParseDictionary<Models.WarriorsFundItemB>();
			BattlePassBS = DataManager.ParseList<Models.BattlePassB>();
			SmartStorage.SetLoadSmartObjectMethod(LoadSmartObject);
		}
	}
#pragma warning restore 649
}
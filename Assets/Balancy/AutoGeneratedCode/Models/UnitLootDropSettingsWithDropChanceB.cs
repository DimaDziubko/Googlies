using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class UnitLootDropSettingsWithDropChanceB : BaseModel
	{



		[JsonProperty("unitType")]
		public readonly Models.UnitTypeB UnitType;

		[JsonProperty("dropChance")]
		public readonly float DropChance;

		[JsonProperty("amount")]
		public readonly int Amount;

	}
#pragma warning restore 649
}
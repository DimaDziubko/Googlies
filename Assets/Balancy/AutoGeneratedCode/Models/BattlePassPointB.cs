using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class BattlePassPointB : BaseModel
	{



		[JsonProperty("freeReward")]
		public readonly Models.SmartObjects.ItemWithAmount FreeReward;

		[JsonProperty("premiumReward")]
		public readonly Models.SmartObjects.ItemWithAmount PremiumReward;

		[JsonProperty("objective")]
		public readonly int Objective;

	}
#pragma warning restore 649
}
using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class WarriorsFundItemB : BaseModel
	{

		[JsonProperty]
		private string unnyIdObjective;


		[JsonIgnore]
		public Models.UserProgressComponent Objective => DataEditor.GetModelByUnnyId<Models.UserProgressComponent>(unnyIdObjective);

		[JsonProperty("freeReward")]
		public readonly Models.SmartObjects.ItemWithAmount FreeReward;

		[JsonProperty("premiumReward")]
		public readonly Models.SmartObjects.ItemWithAmount PremiumReward;

	}
#pragma warning restore 649
}
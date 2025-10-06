using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ConditionalRewardList : BaseModel
	{

		[JsonProperty]
		private string unnyIdCondition;


		[JsonProperty("id")]
		public readonly int Id;

		[JsonProperty("rewards")]
		public readonly Models.SmartObjects.ItemWithAmount[] Rewards;

		[JsonIgnore]
		public Models.SmartObjects.Conditions.Logic Condition => DataEditor.GetModelByUnnyId<Models.SmartObjects.Conditions.Logic>(unnyIdCondition);

	}
#pragma warning restore 649
}
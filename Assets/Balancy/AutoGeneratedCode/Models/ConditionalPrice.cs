using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ConditionalPrice : BaseModel
	{

		[JsonProperty]
		private string unnyIdCondition;


		[JsonProperty("price")]
		public readonly Models.SmartObjects.Price Price;

		[JsonIgnore]
		public Models.SmartObjects.Conditions.Logic Condition => DataEditor.GetModelByUnnyId<Models.SmartObjects.Conditions.Logic>(unnyIdCondition);

	}
#pragma warning restore 649
}
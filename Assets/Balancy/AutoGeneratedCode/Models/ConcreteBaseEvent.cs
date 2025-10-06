using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public abstract class ConcreteBaseEvent : BaseModel
	{

		[JsonProperty]
		private string unnyIdThreshold;


		[JsonProperty("gameEventType")]
		public readonly Models.GameEventTypeB GameEventType;

		[JsonIgnore]
		public Models.UserProgressComponent Threshold => DataEditor.GetModelByUnnyId<Models.UserProgressComponent>(unnyIdThreshold);

	}
#pragma warning restore 649
}
using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class CustomGameEvent : SmartObjects.GameEvent
	{

		[JsonProperty]
		private string unnyIdConcreteEvent;


		[JsonProperty("showOrder")]
		public readonly int ShowOrder;

		[JsonProperty("gameEventPanel")]
		public readonly Models.GameEventPanelTypeB GameEventPanel;

		[JsonIgnore]
		public Models.ConcreteBaseEvent ConcreteEvent => DataEditor.GetModelByUnnyId<Models.ConcreteBaseEvent>(unnyIdConcreteEvent);

		[JsonProperty("slotTypeB")]
		public readonly Models.GameEventSlotTypeB SlotTypeB;

		[JsonProperty("sortOrder")]
		public readonly int SortOrder;

		[JsonProperty("showcaseCondition")]
		public readonly Models.ShowcaseConditionB ShowcaseCondition;

	}
#pragma warning restore 649
}
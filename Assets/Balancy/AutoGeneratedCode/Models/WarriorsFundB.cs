using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class WarriorsFundB : ConcreteBaseEvent
	{

		[JsonProperty]
		private string[] unnyIdItems;
		[JsonProperty]
		private string unnyIdEventIAP;


		[JsonIgnore]
		public Models.WarriorsFundItemB[] Items
		{
			get
			{
				if (unnyIdItems == null)
					return new Models.WarriorsFundItemB[0];
				var items = new Models.WarriorsFundItemB[unnyIdItems.Length];
				for (int i = 0;i < unnyIdItems.Length;i++)
					items[i] = DataEditor.GetModelByUnnyId<Models.WarriorsFundItemB>(unnyIdItems[i]);
				return items;
			}
		}

		[JsonIgnore]
		public Models.EventIAPs EventIAP => DataEditor.GetModelByUnnyId<Models.EventIAPs>(unnyIdEventIAP);

	}
#pragma warning restore 649
}
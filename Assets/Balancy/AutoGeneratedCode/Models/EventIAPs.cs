using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class EventIAPs : SmartObjects.StoreItem
	{

		[JsonProperty]
		private string[] unnyIdConditionalPrice;


		[JsonProperty("allPrices")]
		public readonly Models.SmartObjects.Price[] AllPrices;

		[JsonIgnore]
		public Models.ConditionalPrice[] ConditionalPrice
		{
			get
			{
				if (unnyIdConditionalPrice == null)
					return new Models.ConditionalPrice[0];
				var conditionalPrice = new Models.ConditionalPrice[unnyIdConditionalPrice.Length];
				for (int i = 0;i < unnyIdConditionalPrice.Length;i++)
					conditionalPrice[i] = DataEditor.GetModelByUnnyId<Models.ConditionalPrice>(unnyIdConditionalPrice[i]);
				return conditionalPrice;
			}
		}

	}
#pragma warning restore 649
}
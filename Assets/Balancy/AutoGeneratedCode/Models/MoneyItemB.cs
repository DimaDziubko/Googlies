using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class MoneyItemB : SmartObjects.Item
	{



		[JsonProperty("crurrencyType")]
		public readonly Models.CurrencyTypeB CrurrencyType;

	}
#pragma warning restore 649
}
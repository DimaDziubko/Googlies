using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class KeyItemB : SmartObjects.Item
	{



		[JsonProperty("dungeonType")]
		public readonly Models.DungeonTypeB DungeonType;

	}
#pragma warning restore 649
}
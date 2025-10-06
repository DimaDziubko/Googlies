using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class UserProgressComponent : BaseModel
	{



		[JsonProperty("timeline")]
		public readonly int Timeline;

		[JsonProperty("age")]
		public readonly int Age;

		[JsonProperty("maxBattle")]
		public readonly int MaxBattle;

	}
#pragma warning restore 649
}
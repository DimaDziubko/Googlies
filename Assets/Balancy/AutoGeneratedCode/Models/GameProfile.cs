using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class GameProfile : ParentBaseData
	{

		[JsonProperty]
		private Data.Progress progress;
		[JsonProperty]
		private Data.Engagement engagement;


		[JsonIgnore]
		public Data.Progress Progress => progress;

		[JsonIgnore]
		public Data.Engagement Engagement => engagement;

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref progress);
			ValidateData(ref engagement);
		}

		public static GameProfile Instantiate()
		{
			return Instantiate<GameProfile>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "Progress", Progress, null, cache);
			AddCachedItem(path + "Engagement", Engagement, null, cache);
		}
	}
#pragma warning restore 649
}
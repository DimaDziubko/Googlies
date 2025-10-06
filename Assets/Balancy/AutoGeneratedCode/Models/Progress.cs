using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class Progress : BaseData
	{

		[JsonProperty]
		private int timeline;
		[JsonProperty]
		private int age;
		[JsonProperty]
		private int maxBattle;


		[JsonIgnore]
		public int Timeline
		{
			get => timeline;
			set {
				if (UpdateValue(ref timeline, value))
					_cache?.UpdateStorageValue(_path + "Timeline", timeline);
			}
		}

		[JsonIgnore]
		public int Age
		{
			get => age;
			set {
				if (UpdateValue(ref age, value))
					_cache?.UpdateStorageValue(_path + "Age", age);
			}
		}

		[JsonIgnore]
		public int MaxBattle
		{
			get => maxBattle;
			set {
				if (UpdateValue(ref maxBattle, value))
					_cache?.UpdateStorageValue(_path + "MaxBattle", maxBattle);
			}
		}

		protected override void InitParams() {
			base.InitParams();

		}

		public static Progress Instantiate()
		{
			return Instantiate<Progress>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "Timeline", timeline, newValue => Timeline = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "Age", age, newValue => Age = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "MaxBattle", maxBattle, newValue => MaxBattle = Utils.ToInt(newValue), cache);
		}
	}
#pragma warning restore 649
}
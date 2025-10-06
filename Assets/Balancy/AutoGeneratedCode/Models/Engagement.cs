using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class Engagement : BaseData
	{

		[JsonProperty]
		private float activity_7;
		[JsonProperty]
		private float activity_14;
		[JsonProperty]
		private int battlesCompleted;
		[JsonProperty]
		private float activity_21;
		[JsonProperty]
		private float activity_28;


		[JsonIgnore]
		public float Activity_7
		{
			get => activity_7;
			set {
				if (UpdateValue(ref activity_7, value))
					_cache?.UpdateStorageValue(_path + "Activity_7", activity_7);
			}
		}

		[JsonIgnore]
		public float Activity_14
		{
			get => activity_14;
			set {
				if (UpdateValue(ref activity_14, value))
					_cache?.UpdateStorageValue(_path + "Activity_14", activity_14);
			}
		}

		[JsonIgnore]
		public int BattlesCompleted
		{
			get => battlesCompleted;
			set {
				if (UpdateValue(ref battlesCompleted, value))
					_cache?.UpdateStorageValue(_path + "BattlesCompleted", battlesCompleted);
			}
		}

		[JsonIgnore]
		public float Activity_21
		{
			get => activity_21;
			set {
				if (UpdateValue(ref activity_21, value))
					_cache?.UpdateStorageValue(_path + "Activity_21", activity_21);
			}
		}

		[JsonIgnore]
		public float Activity_28
		{
			get => activity_28;
			set {
				if (UpdateValue(ref activity_28, value))
					_cache?.UpdateStorageValue(_path + "Activity_28", activity_28);
			}
		}

		protected override void InitParams() {
			base.InitParams();

		}

		public static Engagement Instantiate()
		{
			return Instantiate<Engagement>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "Activity_7", activity_7, newValue => Activity_7 = Utils.ToFloat(newValue), cache);
			AddCachedItem(path + "Activity_14", activity_14, newValue => Activity_14 = Utils.ToFloat(newValue), cache);
			AddCachedItem(path + "BattlesCompleted", battlesCompleted, newValue => BattlesCompleted = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "Activity_21", activity_21, newValue => Activity_21 = Utils.ToFloat(newValue), cache);
			AddCachedItem(path + "Activity_28", activity_28, newValue => Activity_28 = Utils.ToFloat(newValue), cache);
		}
	}
#pragma warning restore 649
}
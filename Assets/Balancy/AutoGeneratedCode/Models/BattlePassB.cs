using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class BattlePassB : ConcreteBaseEvent
	{

		[JsonProperty]
		private string unnyIdEventIAP;
		[JsonProperty]
		private string unnyIdCurrencyDropSettings;
		[JsonProperty]
		private string[] unnyIdPoints;


		[JsonProperty("repeat")]
		public readonly bool Repeat;

		[JsonIgnore]
		public Models.EventIAPs EventIAP => DataEditor.GetModelByUnnyId<Models.EventIAPs>(unnyIdEventIAP);

		[JsonProperty("breakDuration")]
		public readonly int BreakDuration;

		[JsonProperty("pointsCell")]
		public readonly Models.CurrencyTypeB PointsCell;

		[JsonProperty("startDate")]
		public readonly UnnyDate StartDate;

		[JsonProperty("duration")]
		public readonly int Duration;

		[JsonIgnore]
		public Models.CurrencyDropSettingsB CurrencyDropSettings => DataEditor.GetModelByUnnyId<Models.CurrencyDropSettingsB>(unnyIdCurrencyDropSettings);

		[JsonIgnore]
		public Models.BattlePassPointB[] Points
		{
			get
			{
				if (unnyIdPoints == null)
					return new Models.BattlePassPointB[0];
				var points = new Models.BattlePassPointB[unnyIdPoints.Length];
				for (int i = 0;i < unnyIdPoints.Length;i++)
					points[i] = DataEditor.GetModelByUnnyId<Models.BattlePassPointB>(unnyIdPoints[i]);
				return points;
			}
		}

	}
#pragma warning restore 649
}
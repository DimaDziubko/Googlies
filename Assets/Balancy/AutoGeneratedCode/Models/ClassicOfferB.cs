using Newtonsoft.Json;
using System;

namespace Balancy.Models
{
#pragma warning disable 649

	public class ClassicOfferB : ConcreteBaseEvent
	{

		[JsonProperty]
		private string[] unnyIdConditionalRewards;
		[JsonProperty]
		private string unnyIdIAP;


		[JsonProperty("startDate")]
		public readonly UnnyDate StartDate;

		[JsonProperty("duration")]
		public readonly int Duration;

		[JsonProperty("repeat")]
		public readonly bool Repeat;

		[JsonProperty("breakDuration")]
		public readonly int BreakDuration;

		[JsonProperty("purchaseLimit")]
		public readonly int PurchaseLimit;

		[JsonProperty("discount")]
		public readonly int Discount;

		[JsonIgnore]
		public Models.ConditionalRewardList[] ConditionalRewards
		{
			get
			{
				if (unnyIdConditionalRewards == null)
					return new Models.ConditionalRewardList[0];
				var conditionalRewards = new Models.ConditionalRewardList[unnyIdConditionalRewards.Length];
				for (int i = 0;i < unnyIdConditionalRewards.Length;i++)
					conditionalRewards[i] = DataEditor.GetModelByUnnyId<Models.ConditionalRewardList>(unnyIdConditionalRewards[i]);
				return conditionalRewards;
			}
		}

		[JsonIgnore]
		public Models.EventIAPs IAP => DataEditor.GetModelByUnnyId<Models.EventIAPs>(unnyIdIAP);

	}
#pragma warning restore 649
}
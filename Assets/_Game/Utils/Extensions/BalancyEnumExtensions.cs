using _Game.Core.UserState._State;
using _Game.LiveopsCore._Enums;
using Balancy.Models;

namespace _Game.Utils.Extensions
{
    public static class BalancyEnumExtensions
    {
        public static CurrencyType ToLocal(this CurrencyTypeB remoteType)
        {
            return remoteType switch
            {
                CurrencyTypeB.Coins => CurrencyType.Coins,
                CurrencyTypeB.Gems => CurrencyType.Gems,
                CurrencyTypeB.SkillPotion => CurrencyType.SkillPotion,
                CurrencyTypeB.LeaderPassPoint => CurrencyType.LeaderPassPoint,
                _ => CurrencyType.Gems
            };
        }

        public static GameEventType ToLocal(this GameEventTypeB remoteType)
        {
            return remoteType switch
            {
                GameEventTypeB.BattlePass => GameEventType.BattlePass,
                GameEventTypeB.WarriorsFund => GameEventType.WarriorsFund,
                GameEventTypeB.ClassicOffer => GameEventType.ClassicOffer,
                _ => GameEventType.None
            };
        }

        public static GameEventPanelType ToLocal(this GameEventPanelTypeB remoteType)
        {
            return remoteType switch
            {
                GameEventPanelTypeB.Left => GameEventPanelType.Left,
                GameEventPanelTypeB.Right => GameEventPanelType.Right,
                _ => GameEventPanelType.Right
            };
        }

        public static ShowcaseCondition ToLocal(this ShowcaseConditionB remoteType)
        {
            return remoteType switch
            {
                ShowcaseConditionB.OnStart => ShowcaseCondition.OnStart,
                ShowcaseConditionB.OnUpdate => ShowcaseCondition.OnUpdate,
                ShowcaseConditionB.New => ShowcaseCondition.New,
                _ => ShowcaseCondition.Never
            };
        }

        public static GameEventSlotType ToLocal(this GameEventSlotTypeB remoteType)
        {
            return remoteType switch
            {
                GameEventSlotTypeB.Slot_1 => GameEventSlotType.Slot1,

                _ => GameEventSlotType.Slot1
            };
        }

        public static UserProgress ToLocal(this UserProgressComponent component)
        {
            return new UserProgress()
            {
                TimelineNumber = component.Timeline,
                AgeNumber = component.Age,
                MaxBattleNumber = component.MaxBattle,
            };
        }
    }
}
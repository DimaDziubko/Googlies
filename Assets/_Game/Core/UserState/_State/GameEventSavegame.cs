using System;
using System.Collections.Generic;
using _Game.Core.UserState._State._GameEvents;
using _Game.LiveopsCore;
using _Game.LiveopsCore._Enums;
using _Game.LiveopsCore.Models.WarriorsFund;

namespace _Game.Core.UserState._State
{
    public class GameEventSavegame
    {
        public int Id;
        public string EventName;
        public int Layer;
        public DateTime StartDateUtc;
        public DateTime EndDateUtc;
        public int BadgeTier;
        public GameEventType EventType;
        public GameEventSlotType SlotType;
        public GameEventPanelType PanelType;
        public int SortOrder;
        public bool IsOnBreak;
        public List<int> Offers;
        public bool IsHiden;
        public PurchaseDataState PurchaseData;

        public BattlePassSavegame BattlePassSavegame;
        public WarriorsFundSavegame WarriorsFundSavegame;
        public ClassicOfferSavegame ClassicOfferSavegame;

        public GameEventShowcaseSavegamne ShowcaseSave;

        internal void SetEndTime(DateTime endTime)
        {
            EndDateUtc = endTime;
        }

        internal void SetStartTime(DateTime startTime)
        {
            StartDateUtc = startTime;
        }

        internal void SetHiden(bool state)
        {
            IsHiden = state;
        }
    }
}
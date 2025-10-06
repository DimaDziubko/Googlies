using _Game.Core.UserState._State;
using _Game.LiveopsCore._Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.LiveopsCore
{
    public abstract class GameEventBase
    {
        [ShowInInspector, ReadOnly]
        public string Name { get; protected set; }
        public int Id { get; protected set; }
        public GameEventShowcaseToken ShowcaseToken { get; protected set; }
        public virtual GameEventType Type { get; protected set; }

        public Sprite Icon { get; protected set; }

        public GameEventSavegame Save { get; protected set; }

        public GameEventPanelType PanelType => Save.PanelType;
        public GameEventSlotType SlotType => Save.SlotType;
        public int SortOrder => Save.SortOrder;

        public virtual bool IsLocked { get; protected set; }
        public bool IsOnBreak => Save.IsOnBreak;
        public virtual bool HasUnclaimedRewards() => false;
    }
}
using System;
using _Game.LiveopsCore._Enums;

namespace _Game.LiveopsCore
{
    [Serializable]
    public struct SlotBinding
    {
        public GameEventSlotType SlotType;
        public GameEventListView ListView;
    }
}
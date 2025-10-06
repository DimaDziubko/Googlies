using System;
using _Game.Gameplay.Common.Scripts;
using UnityEngine;

namespace _Game.UI.Common.Scripts
{
    [Serializable]
    public class RaceSpriteSelect
    {
        public Race Race;
        public Sprite Sprite;

        public RaceSpriteSelect(Race race, Sprite sprite)
        {
            Race = race;
            Sprite = sprite;
        }
    }
}

﻿using System.Collections.Generic;
using _Game.UI._CardsGeneral._Cards.Scripts;
using UnityEngine;

namespace _Game.Utils._Static
{
    public static class CardColorMapper
    { 
        private static readonly Dictionary<CardType, Color> _cardColors = new Dictionary<CardType, Color>
        {
            { CardType.Rare, new Color(27f/255f, 164f/255f, 255f/255f, 1f) },
            { CardType.Epic, new Color(166f/255f, 50f/255f,  243/255f, 1f) },
            { CardType.Legendary, new Color(255f/255f, 199f/255f, 0f/255f, 1f)},
            { CardType.Common, new Color(192f/255f, 209f/255f, 239f/255f, 1f) }
            
            //Old
            // { CardType.Rare, new Color(1f/255f, 163f/255f, 254f/255f, 1f) },
            // { CardType.Epic, new Color(160f/255f, 98f/255f, 255f/255f, 1f) },
            // { CardType.Legendary, new Color(255f/255f, 200f/255f, 0f/255f, 1f)},
            // { CardType.Common, new Color(189f/255f, 189f/255f, 189f/255f, 1f) }
        };

        public static Color GetColorForType(CardType type)
        {
            if (_cardColors.TryGetValue(type, out Color color))
            {
                return color;
            }
            
            return Color.white;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    [CreateAssetMenu(fileName = "CardAppearancePopupSettings", menuName = "Settings/CardAppearancePopupSettings")]
    public class CardAppearancePopupSettings : ScriptableObject
    {
        public List<CardGridSettings> _cardGridSettings;
        public CardGridSettings GetSettingsForAmount(int amount) => 
            _cardGridSettings.FirstOrDefault(x => x.CardAmountMin <= amount  && x.CardAmountMax  >= amount);
    }
}
using _Game.UI._CardsGeneral._Cards.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Summoning.Scripts
{
    public class CardSummoningView : MonoBehaviour
    {
        [SerializeField] private Image _cardColorIcon;
        [SerializeField] private TMP_Text _typeLabel;
        [SerializeField] private TMP_Text _summoningLabel;

        public CardType CardType;

        public void SetColor(Color color)
        {
            _typeLabel.color = color;
            _summoningLabel.color = color;
            _cardColorIcon.color = color;
        }

        public void SetTypeName(string typeName)
        {
            _typeLabel.text = typeName;
        }

        public void SetValue(string value)
        {
            _summoningLabel.text = value;
        }
    }
}
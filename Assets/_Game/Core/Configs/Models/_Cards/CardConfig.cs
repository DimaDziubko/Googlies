using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.Utils._Static;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core.Configs.Models._Cards
{
    [CreateAssetMenu(fileName = "CardConfig", menuName = "Configs/Card")]
    public class CardConfig : ScriptableObject
    {
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        public int Id;

        [ValueDropdown("GetCardTypes")]
        [OnValueChanged("UpdateColorBasedOnType")]
        public CardType Type;

        [ReadOnly]
        [ColorPalette] 
        public Color ColorIdentifier;

        public Sprite Icon;
        
        public string Name;
        [MultiLineProperty(5)] 
        public string Derscription;
        public Material MaterialIdentifier;

        public float DropChance;
        public Boost[] Boosts;

        public int GetUpgradeCount(int level)
        {
            switch (level)
            {
                case 1:
                    return 2;
                case 2:
                    return 3;
                default:
                    return 4;
            }   
        }

        [Button]
        private void SetMaterialColor()
        {
            MaterialIdentifier.SetColor(BaseColor, ColorIdentifier);
        }
        
        private void UpdateColorBasedOnType() => ColorIdentifier = GetColorForType(Type);

        private Color GetColorForType(CardType type) => CardColorMapper.GetColorForType(Type);

        private CardType[] GetCardTypes() => (CardType[])System.Enum.GetValues(typeof(CardType));
    }
}
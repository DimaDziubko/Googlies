using System;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Utils;
using _Game.Utils._Static;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardModel
    {
        public event Action NewMarkChanged;
        public Card Card => _card;
        public Sprite Icon => _config.Icon;
        public float ProgressValue => Mathf.Clamp01((float)_card.Count / GetUpgradeCount());
        public bool IsReadyForUpgrade => Card.Count >= GetUpgradeCount();
        public CardType Type => _config.Type;
        public bool IsHighest { get; private set; }

        private bool _isNew;
        public bool IsNew => _isNew;
        public string Name => _config.Name;
        public string CardDescription => _config.Derscription;
        public Boost[] Boosts => _config.Boosts;

        public int Id => _card.Id;

        private CardConfig _config;
        private Card _card;

        public CardModel(CardConfig config, Card card)
        {
            _config = config;
            _card = card;
        }

        public Color GetColor() => _config.ColorIdentifier;

        public int GetUpgradeCount() => _config.GetUpgradeCount(_card.Level);

        public Color GetBarColor()
        {
            if (IsReadyForUpgrade) return Constants.ColorTheme.THEMED_PROGRESS_BAR_FULL;
            return CardColorMapper.GetColorForType(CardType.Common);
        }
        
        public string GetBarColorName()
        {
            if (IsReadyForUpgrade) return "FullProgressBar";
            return "ProgressBar";
        }

        public Material GetMaterial() => _config.MaterialIdentifier;
        public void SetNew(bool isNew)
        {
            _isNew = isNew;
            NewMarkChanged?.Invoke();
        }
        public void SetHighest(bool isHighest) => IsHighest = isHighest;

        public void Upgrade()
        {
            int upgradeCount = GetUpgradeCount();
            _card.RemoveCard(upgradeCount);
            _card.LevelUp();
        }
        public void IncreaseCount() => _card.AddCard(1);

        public CardModel CreateClone() => new(_config, _card);
    }
}
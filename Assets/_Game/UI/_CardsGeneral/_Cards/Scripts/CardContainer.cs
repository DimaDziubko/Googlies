using System;
using System.Collections.Generic;
using System.Linq;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardContainer : IDisposable
    {
         private readonly Dictionary<int, CardModel> _cards = new();
        private CardType _highestTypeInContainer = CardType.Rare;

        public event Action<CardModel> CardAdded;
        public event Action<CardModel> CardRemoved;

        public void Initialize(IEnumerable<CardModel> models)
        {
            foreach (var model in models)
            {
                _cards.TryAdd(model.Id, model);
                UpdateHighestType(model.Type);
            }
        }
        
        public void Add(CardModel model)
        {
            if (!_cards.TryGetValue(model.Id, out var existingModel))
            {
                if (model.Type > _highestTypeInContainer)
                {
                    model.SetHighest(true);
                    _highestTypeInContainer = model.Type;
                    UpdateHighestType(model.Type);
                }

                _cards[model.Id] = model;
            }
            else
            {
                existingModel.IncreaseCount();
            }
            
            CardAdded?.Invoke(model);
        }

        public void Remove(int id)
        {
            if (_cards.Remove(id, out var model))
            {
                CardRemoved?.Invoke(model);
            }
        }

        public CardModel GetById(int id) => 
            _cards.GetValueOrDefault(id);

        public IReadOnlyCollection<CardModel> GetAll()
        {
            return _cards.Values;
        }

        public IEnumerable<CardModel> GetByType(CardType type) => 
            _cards.Values.Where(m => m.Type == type);

        public bool Contains(int id) => 
            _cards.ContainsKey(id);

        void IDisposable.Dispose() => 
            Clear();

        private void Clear() => 
            _cards.Clear();
        
        private void UpdateHighestType(CardType type)
        {
            if (type > _highestTypeInContainer)
            {
                _highestTypeInContainer = type;

                foreach (var model in _cards.Values)
                {
                    model.SetHighest(model.Type >= _highestTypeInContainer);
                }
            }
        }
    }
}
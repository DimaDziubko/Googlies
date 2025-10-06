using System;
using System.Collections.Generic;
using _Game.Core.UserState._State;
using _Game.Gameplay._RewardProcessing;
using _Game.Utils;
using Zenject;

namespace _Game.Gameplay._ItemProvider
{
    public class ItemProvider :  
        IItemProvider, 
        IInitializable, 
        IDisposable
    {
        private Dictionary<int, ItemLocal> _localItems = new();
        
        void IInitializable.Initialize()
        {
            _localItems = new()
            {
                {
                    Constants.ItemId.GEMS,
                    new MoneyItemLocal()
                    {
                        Id = Constants.ItemId.GEMS,
                        IconKey = "Gem_0",
                        CurrencyType = CurrencyType.Gems,
                    }
                },
                
                {
                    Constants.ItemId.COINS,
                    new MoneyItemLocal()
                    {
                        Id = Constants.ItemId.COINS,
                        IconKey = "Coin_0",
                        CurrencyType = CurrencyType.Coins,
                    }
                },
                
                {
                    Constants.ItemId.SKILL_POTION,
                    new MoneyItemLocal()
                    {
                        Id = Constants.ItemId.SKILL_POTION,
                        IconKey = "SkillPotion_0",
                        CurrencyType = CurrencyType.SkillPotion,
                    }
                },
                
                {
                    Constants.ItemId.GREEN_KEY,
                    new KeyItemLocal()
                    {
                        Id = Constants.ItemId.GREEN_KEY,
                        IconKey = "GreenKey",
                        DungeonType = DungeonType.ZombieRush
                    }
                },
                
                {
                    Constants.ItemId.YELLOW_KEY,
                    new KeyItemLocal()
                    {
                        Id = Constants.ItemId.YELLOW_KEY,
                        IconKey = "YellowKey",
                        DungeonType = DungeonType.Legends
                    }
                },
                
                {
                    Constants.ItemId.RED_KEY,
                    new KeyItemLocal()
                    {
                        Id = Constants.ItemId.RED_KEY,
                        IconKey = "RedKey",
                        DungeonType = DungeonType.Bosses
                    }
                },
                
                {
                    Constants.ItemId.CARD,
                    new CardItemLocal()
                    {
                        Id = Constants.ItemId.CARD,
                        IconKey = "CardsIcon",
                    }
                },

            };
        }

        public ItemLocal GetItem(int id)
        {
            if (_localItems.TryGetValue(id, out var localItem))
            {
                return localItem;
            }

            return MoneyItemLocal.Default();
        }

        void IDisposable.Dispose()
        {
            _localItems.Clear();
        }
    }
}
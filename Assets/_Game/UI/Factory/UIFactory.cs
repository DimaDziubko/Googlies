using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._Shop.Scripts._DecorAndUtils;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Skills.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI.Factory
{
    public interface IUIFactory
    {
        Plug GetShopItemPlug(Transform parent);
        CardItemView GetCard(Transform parent);
        CardView GetCardView(Transform parent);
        TutorialPointerView GetTutorialPointer(RectTransform parent);
        T GetShopItem<T>(int id, Transform parent) where T : ShopItemView;
        SkillItemView GetSkillItemView(Transform containerTransform);
        
        void Reclaim(Plug item);
        void Reclaim(ShopItemView itemView);
        void Reclaim(CardItemView cardItemView);
        void Reclaim(TutorialPointerView cardItemView);
        void Reclaim(SkillItemView skillItemView);
        void Reclaim(CardView cardView);
    }

    [CreateAssetMenu(fileName = "UI Factory", menuName = "Factories/UI")]
    public class UIFactory : GameObjectFactory, IUIFactory
    {
        [SerializeField] private List<ShopItemView> _shopItemViews;
        private Dictionary<int, ShopItemView> _shopItemPrefabs;

        [SerializeField, Required] private Plug _shopItemPlug;

        [SerializeField, Required] private CardItemView cardItemView;
        [SerializeField, Required] private SkillItemView _skillItemView;

        [SerializeField, Required] private TutorialPointerView _tutorialPointer;
        [SerializeField, Required] private CardView _cardView;

        public void Initialize()
        {
            _shopItemPrefabs = new Dictionary<int, ShopItemView>();
            foreach (var prefab in _shopItemViews)
            {
                int id = prefab.Id;
                if (!_shopItemPrefabs.ContainsKey(id))
                {
                    _shopItemPrefabs.Add(id, prefab);
                }
                else
                {
                    Debug.LogWarning($"Duplicate id {id} found in _shopItemPrefabs.");
                }
            }
        }
        
        public T GetShopItem<T>(int id, Transform parent) where T : ShopItemView
        {
            if (_shopItemPrefabs.TryGetValue(id, out var prefab))
            {
                var instance = CreateGameObjectInstance(prefab, parent) as T;
                if (instance != null)
                    instance.OriginFactory = this;
                return instance;
            }
            else
            {
                Debug.LogError($"Prefab for type {id} not found.");
                return null;
            }
        }

        public Plug GetShopItemPlug(Transform parent)
        {
            var instance = CreateGameObjectInstance(_shopItemPlug, parent);
            if(instance != null)
                instance.OriginFactory = this;
            return instance;
        }

        public CardItemView GetCard(Transform parent)
        {
            var instance = CreateGameObjectInstance(cardItemView, parent);
            if(instance != null)
                instance.OriginFactory = this;
            return instance;
        }
        
        public CardView GetCardView(Transform parent)
        {
            var instance = CreateGameObjectInstance(_cardView, parent);
            if(instance != null)
                instance.OriginFactory = this;
            return instance;
        }
        
        public SkillItemView GetSkillItemView(Transform parent)
        {
            var instance = CreateGameObjectInstance(_skillItemView, parent);
            if(instance != null)
                instance.OriginFactory = this;
            return instance;
        }

        public TutorialPointerView GetTutorialPointer(RectTransform parent)
        {
            var instance = CreateGameObjectInstance(_tutorialPointer, parent);
            if (instance != null)
            {
                instance.OriginFactory = this;
                instance.Parent = parent;
            }
            return instance;
        }

        public void Reclaim(ShopItemView itemView) => 
            Destroy(itemView.gameObject);

        public void Reclaim(Plug item) => 
            Destroy(item.gameObject);

        public void Reclaim(CardItemView cardItemView) => 
            Destroy(cardItemView.gameObject);
        
        public void Reclaim(CardView cardView) => 
            Destroy(cardView.gameObject);

        public void Reclaim(TutorialPointerView tutorialPointer) => 
            Destroy(tutorialPointer.gameObject);

        public void Reclaim(SkillItemView skillItemView) => 
            Destroy(skillItemView.gameObject);
    }
}
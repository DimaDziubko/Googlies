using System.Collections.Generic;
using System.Linq;
using _Game.Core.Services._Camera;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._MainMenu.State;
using _Game.UI._ParticleAttractorSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._MainMenu.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class MainMenuScreen : MonoBehaviour
    {
        public TutorialStep UpgradeTutorialStep => _upgradesTutorialStep;
        public TutorialStep CardsTutorialStep => _cardsTutorialStep;
        public TutorialStep SkillsTutorialStep => _skillsTutorialStep;
        
        public MenuButton DungeonButton => _dungeonButton;
        public MenuButton UpgradesButton => _upgradeButton;
        public MenuButton BattleButton => _battleButton;
        public MenuButton CardsButton => _cardsButton;
        public MenuButton ShopButton => _shopButton;
        
        public AttractorWrapper SkillAttractorWrapper => _skillAttractorWrapper;


        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private RectTransform _canvasRectTransform;

        [SerializeField, Required] private MenuButton _dungeonButton;
        [SerializeField, Required] private MenuButton _upgradeButton;
        [SerializeField, Required] private MenuButton _battleButton;
        [SerializeField, Required] private MenuButton _cardsButton;
        [SerializeField, Required] private MenuButton _shopButton;

        [SerializeField, Required] public TutorialStep _upgradesTutorialStep;
        [SerializeField, Required] public TutorialStep _cardsTutorialStep;
        [SerializeField, Required] public TutorialStep _skillsTutorialStep;
        
        [SerializeField, Required] private AttractorWrapper _skillAttractorWrapper;


        private LocalStateMachine _menuStateMachine;
        
        
        [SerializeField] private Vector2 _highlightedBtnScale = new(1f, 1.2f);

        private MainMenu _mainMenu;

        [SerializeField] private Vector2 _normalButtonSize;
        
        [ShowInInspector, ReadOnly]
        private MenuButtonType _highlightedType;

        public void Construct(
            IWorldCameraService cameraService,
            MainMenu mainMenu
            )
        {
            _canvas.enabled = false;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _mainMenu = mainMenu;
            _mainMenu.Screen = this;
            
            InitButtonsLayout();
        }
        
        public void Show()
        {
            if(!gameObject.activeInHierarchy) gameObject.SetActive(true);
            _mainMenu.OnScreenOpened();
            _canvas.enabled = true;
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
            _mainMenu.OnScreenClosed();
        }

        [Button]
        private void InitButtonsLayout()
        {
            float canvasWidth = _canvasRectTransform.rect.width;
            float normalWidth = canvasWidth / (GetAllButtons().Count() - 1 + _highlightedBtnScale.x);

            _normalButtonSize = new Vector2(normalWidth, _normalButtonSize.y);

            foreach (var button in GetAllButtons())
            {
                button.SetSize(_normalButtonSize);
                button.SetHighlighted(false);
            }
            
            RebuildLayout();
        }
        

        [Button]
        private void RebuildLayout()
        {
            float x = -_canvasRectTransform.rect.width / 2f;

            foreach (var button in GetAllButtons())
            {
                var rect = button.RectTransform;
                float width = rect.sizeDelta.x;
                rect.anchoredPosition = new Vector2(x + width / 2f, rect.anchoredPosition.y);
                x += width;
            }
        }

        public void SetButtonHighlighted(MenuButtonType buttonType, bool isHighlighted)
        {
            foreach (var button in GetAllButtons())
            {
                bool isTarget = button.Type == buttonType;
                if (isTarget && isHighlighted)
                {
                    Vector2 highlightedSize = new Vector2(
                        _normalButtonSize.x * _highlightedBtnScale.x,
                        _normalButtonSize.y * _highlightedBtnScale.y
                    );
                    button.SetSize(highlightedSize);
                    button.SetHighlighted(true);
                }
                else
                {
                    button.SetSize(_normalButtonSize);
                    button.SetHighlighted(false);
                }
            }

            _highlightedType = isHighlighted ? buttonType : default;
            RebuildLayout();
        }

        
        private IEnumerable<MenuButton> GetAllButtons()
        {
            yield return _dungeonButton;
            yield return _upgradeButton;
            yield return _battleButton;
            yield return _cardsButton;
            yield return _shopButton;
        }
    }
}

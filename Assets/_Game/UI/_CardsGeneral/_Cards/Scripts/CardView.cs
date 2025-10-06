using System;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI.Factory;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private Image[] _colorIdentifiers;
        [SerializeField] private GameObject _newNotification;
        [SerializeField] private Image _coloredRippleImage;
        [SerializeField] private CardViewAppearanceAnimation _appearanceAnimation;
        [SerializeField] private CanvasGroup _canvasGroup;

        //For dynamic resize
        [SerializeField, Required] private RectTransform _cardTransform;
        [SerializeField, Required] private RectTransform _newTransform;
        [SerializeField, Required] private RectTransform _identifierTransform;

        [SerializeField, Required] private float _newXSizeRatio;
        [SerializeField, Required] private float _newYSizeRatio;
        [SerializeField, Required] private float _identifierYSizeRatio;

        [Space]
        [SerializeField] private Sprite _oneBoostSprite;
        [SerializeField] private Sprite _twoBoostsSprite;
        [Space]
        [SerializeField] private Image[] _boostImages;


        public void SetIcon(Sprite icon) => _icon.sprite = icon;
        public void SetLevel(string level) => _levelLabel.text = level;
        public IUIFactory OriginFactory { get; set; }

        public void SetColor(Color color, Material material)
        {
            foreach (var image in _colorIdentifiers)
                image.material = material;
            //_colorIdentifiers.
            _coloredRippleImage.material = material;

            _appearanceAnimation.SetColor(color);
        }

        public void SetActive(bool isActive)
        {
            Resize();
            gameObject.SetActive(isActive);
        }

        public void Show()
        {
            Resize();
            _canvasGroup.alpha = 1;
        }

        [Button]
        public void Resize()
        {
            var size = _cardTransform.sizeDelta;
            var identifierSize = new Vector2(_identifierTransform.sizeDelta.x, size.y * _identifierYSizeRatio);

            _identifierTransform.sizeDelta = identifierSize;
        }

        public void Hide() => _canvasGroup.alpha = 0;
        public void SetNew(bool isActive) => _newNotification.SetActive(isActive);
        public void SetActiveRipple(bool isActive) => _coloredRippleImage.enabled = isActive;

        public void PlaySimpleAppearanceAnimation()
        {
            _appearanceAnimation.PlaySimple();
        }

        public void PlayRippleAppearanceAnimation(Action callBack)
        {
            _appearanceAnimation.PlayRipple(callBack);
        }

        public void Recycle()
        {
            OriginFactory.Reclaim(this);
        }

        internal void SetBoostView(Sprite boost)
        {
            _boostImages[0].sprite = boost;
            _boostImages[1].enabled = false;

            _colorIdentifiers[0].sprite = _oneBoostSprite;
        }

        internal void SetBoostView(Sprite boost1, Sprite boost2)
        {
            _boostImages[0].sprite = boost1;
            _boostImages[1].sprite = boost2;

            _colorIdentifiers[0].sprite = _twoBoostsSprite;
        }
    }
}
using _Game.UI.Factory;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardItemView : MonoBehaviour
    {
        public event UnityAction CardClicked
        {
            add => _cardBtn.onClick.AddListener(value);
            remove => _cardBtn.onClick.RemoveListener(value);
        }

        [SerializeField, Required] private Transform _transform;

        [SerializeField, Required] private Slider _progressBar;
        [SerializeField, Required] private TMP_Text _progressLabel;
        [SerializeField, Required] private Button _cardBtn;
        [SerializeField, Required] private GameObject _upgradeNitifier;
        [SerializeField, Required] private Animation _animation;
        [SerializeField, Required] private CardView _cardView;
        [SerializeField, Required] private Image _barFillImage;

        [SerializeField, Required] private ImageColorBinder _barColorBinder;
        public IUIFactory OriginFactory { get; set; }

        public Transform Transform => _transform;
        
        public CardView CardView => _cardView;

        public void SetProgressValue(float value)
        {
            _progressBar.value = value;
        }

        public void SetProgress(string progress)
        {
            _progressLabel.text = progress;
        }

        public void SetActiveUpgradeNotifier(bool isActive)
        {
            _upgradeNitifier.SetActive(isActive);
            if (isActive) _animation.Play();
            else
            {
                _animation.Stop();
            }
        }

        public void SetBarColor(Color color)
        {
            _barFillImage.color = color;
        }
        
        public void SetBarColor(string color) => 
            _barColorBinder.SetColorByName(color);

        public void Release() => 
            OriginFactory.Reclaim(this);
    }
}

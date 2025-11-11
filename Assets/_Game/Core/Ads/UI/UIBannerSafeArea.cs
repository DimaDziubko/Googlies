using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core.Ads.UI
{
    public class UIBannerSafeArea : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        private Vector2 _originalAnchorMin;
        private Vector2 _originalAnchorMax;

        private BannerSafeAreaService _safeAreaService;
        private SignalBus _signalBus;
        private bool _isSubscribed = false;

        private void OnValidate()
        {
            if (!_rectTransform)
                _rectTransform = GetComponent<RectTransform>();
        }

        private void Awake()
        {
            _originalAnchorMin = _rectTransform.anchorMin;
            _originalAnchorMax = _rectTransform.anchorMax;

            var context = ProjectContext.Instance;
            if (context != null)
            {
                _safeAreaService = context.Container.TryResolve<BannerSafeAreaService>();
                _signalBus = context.Container.TryResolve<SignalBus>();
            }
        }

        private void Start()
        {
            if (_safeAreaService != null)
            {
                ApplyBannerOffset(_safeAreaService.BottomOffset);
            }

            if (_signalBus != null)
            {
                _signalBus.Subscribe<BannerSafeAreaChangedSignal>(OnBannerChanged);
                _isSubscribed = true;
            }
        }

        private void OnDestroy()
        {
            if (_isSubscribed && _signalBus != null)
            {
                _signalBus.Unsubscribe<BannerSafeAreaChangedSignal>(OnBannerChanged);
            }
        }

        private void OnBannerChanged(BannerSafeAreaChangedSignal signal)
        {
            ApplyBannerOffset(signal.BottomOffset);
        }

        private void ApplyBannerOffset(float bannerHeightInPixels)
        {
            // Вычисляем процент экрана, который занимает баннер
            float bannerHeightPercent = bannerHeightInPixels / Screen.height;

            // Изменяем нижний anchor (поднимаем нижнюю границу UI)
            Vector2 newAnchorMin = _originalAnchorMin;
            newAnchorMin.y = _originalAnchorMin.y + bannerHeightPercent;

            // Применяем новые anchors
            _rectTransform.anchorMin = newAnchorMin;
            _rectTransform.anchorMax = _originalAnchorMax;

            Debug.Log($"[UIBannerSafeArea] {gameObject.name} - Anchors updated: min.y={newAnchorMin.y} (banner: {bannerHeightInPixels}px = {bannerHeightPercent * 100:F2}%)");
        }

        [Button("Force Update")]
        private void ForceUpdate()
        {
            if (_safeAreaService != null)
            {
                ApplyBannerOffset(_safeAreaService.BottomOffset);
            }
        }

        [Button("Reset Anchors")]
        private void ResetAnchors()
        {
            if (_rectTransform != null)
            {
                _rectTransform.anchorMin = _originalAnchorMin;
                _rectTransform.anchorMax = _originalAnchorMax;
            }
        }
    }

    //Canvas(Screen Space - Overlay)
    //└── SafeAreaPanel(Anchors: Min(0,0) Max(1,1), Offsets: все 0)
    //    └── UIBannerSafeArea ← ДОБАВЬ ЭТОТ КОМПОНЕНТ
    //        ├── TopPanel
    //        ├── GameHUD
    //        ├── BottomNavigation
    //        └── Popups
}

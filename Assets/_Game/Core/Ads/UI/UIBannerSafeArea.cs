using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core.Ads.UI
{
    public class UIBannerSafeArea : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Vector2 _originalAnchorMin;
        private Vector2 _originalAnchorMax;
        private Vector2 _originalOffsetMin;
        private Vector2 _originalOffsetMax;

        private BannerSafeAreaService _safeAreaService;
        private SignalBus _signalBus;
        private bool _isSubscribed = false;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            // Сохраняем оригинальные значения
            _originalAnchorMin = _rectTransform.anchorMin;
            _originalAnchorMax = _rectTransform.anchorMax;
            _originalOffsetMin = _rectTransform.offsetMin;
            _originalOffsetMax = _rectTransform.offsetMax;

            // Резолвим сервисы из контейнера
            var context = ProjectContext.Instance;
            if (context != null)
            {
                _safeAreaService = context.Container.TryResolve<BannerSafeAreaService>();
                _signalBus = context.Container.TryResolve<SignalBus>();
            }
        }

        private void Start()
        {
            // Сразу применяем текущий offset баннера
            if (_safeAreaService != null)
            {
                ApplyBannerOffset(_safeAreaService.BottomOffset);
            }

            // Подписываемся на изменения
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
            // НАХУЙ anchors! Используем offsetMin/offsetMax - они работают в Canvas Units

            // Получаем Canvas и его scale
            Canvas canvas = GetComponentInParent<Canvas>();
            float canvasScale = canvas != null ? canvas.scaleFactor : 1f;

            // Конвертируем пиксели баннера в Canvas Units
            float bannerHeightInCanvasUnits = bannerHeightInPixels / canvasScale;

            // Двигаем нижний край вверх (offsetMin.y)
            Vector2 newOffsetMin = _originalOffsetMin;
            newOffsetMin.y = _originalOffsetMin.y + bannerHeightInCanvasUnits;

            _rectTransform.offsetMin = newOffsetMin;
            _rectTransform.offsetMax = _originalOffsetMax;

            Debug.Log($"[UIBannerSafeArea] {gameObject.name}");
            Debug.Log($"  Canvas Scale: {canvasScale}");
            Debug.Log($"  Banner Pixels: {bannerHeightInPixels}px");
            Debug.Log($"  Banner Canvas Units: {bannerHeightInCanvasUnits}");
            Debug.Log($"  OffsetMin: {newOffsetMin} (original: {_originalOffsetMin})");
        }

        [Button("Force Update")]
        private void ForceUpdate()
        {
            if (_safeAreaService != null)
            {
                ApplyBannerOffset(_safeAreaService.BottomOffset);
            }
        }

        [Button("Reset")]
        private void ResetToOriginal()
        {
            if (_rectTransform != null)
            {
                _rectTransform.offsetMin = _originalOffsetMin;
                _rectTransform.offsetMax = _originalOffsetMax;
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

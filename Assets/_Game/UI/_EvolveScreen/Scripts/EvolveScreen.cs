using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay.Common;
using _Game.UI._Shop.Scripts._AmountView;
using _Game.UI._TimelineInfoPresenter;
using _Game.UI._TimelineInfoScreen.Scripts;
using _Game.UI._TravelScreen.Scripts;
using _Game.UI.Common.Scripts;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._EvolveScreen.Scripts
{
    public class EvolveScreen : MonoBehaviour
    {
        private const float POSITION_LEFT = -90f;
        private const float SCROLL_COEFFICIENT = 0.825f;

        public event UnityAction CloseClicked
        {
            add => _closeButton.onClick.AddListener(value);
            remove => _closeButton.onClick.RemoveListener(value);
        }

        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private TMP_Text _timelineLaybel;
        [SerializeField, Required] private TMP_Text _difficultyLaybel;
        [SerializeField, Required] private TransactionButton _evolveButton;
        [SerializeField, Required] private AmountView _rewardView;
        [SerializeField, Required] private Button _closeButton;

        [Space]
        [SerializeField, Required] private ScrollRect _scrollRect;
        [SerializeField, Required] private AgeInfoListView _ageInfoListView;
        [SerializeField, Required] private TimelineProgressBar _progressBar;
        [SerializeField] private AudioClip _evolveSFX;

        [SerializeField] private float _animationDelay = 1.0f;
        [SerializeField] private float _scrollAnimationDuration = 3f;
        [SerializeField] private float _rippleAnimationDuration = 0.2f;

        [Header("Switch to TRAVEL")]
        [SerializeField, Required] private TravelScreen _travelScreen;
        [SerializeField, Required] private CanvasGroup _evolutionPanelGroup;
        [SerializeField, Required] private GameObject _evolutionPanelObject;



        private readonly List<TimelineInfoItemPresenter> _presenters = new();
        private UniTaskCompletionSource<bool> _taskCompletion;

        private IEvolveScreenPresenter _presenter;
        private ITimelineInfoPresenter _timelineInfoPresenter;
        private IAgeNavigator _ageNavigator;
        private IAdsService _adsService;
        private IMyLogger _logger;
        private IConfigRepository _config;
        private IAudioService _audioService;
        private IFeatureUnlockSystem _featureUnlockSystem;
        private IUserContainer _userContainer;
        private Sequence _animation;
        private Sequence _subAnimation;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private ITimelineConfigRepository TimelineConfigRepository => _config.TimelineConfigRepository;

        public void Construct(
            IWorldCameraService cameraService,
            IEvolveScreenPresenter evolveScreenPresenter,
            IConfigRepository config,
            IAudioService audioService,
            ITimelineInfoPresenter timelineInfoPresenter,
            IMyLogger logger,
            IAdsService adsService,
            IAgeNavigator ageNavigator,
            IFeatureUnlockSystem featureUnlockSystem,
            ITravelScreenPresenter travelScreenPresenter,
            IUserContainer userContainer
            )
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _presenter = evolveScreenPresenter;
            _config = config;
            _timelineInfoPresenter = timelineInfoPresenter;
            _logger = logger;
            _adsService = adsService;
            _ageNavigator = ageNavigator;
            _audioService = audioService;
            _featureUnlockSystem = featureUnlockSystem;
            _userContainer = userContainer;

            _travelScreen.Construct(
                travelScreenPresenter,
                config,
                featureUnlockSystem
                );
            _canvas.enabled = false;
            _evolutionPanelGroup.alpha = 0f;
        }

        public void Show()
        {
            if (TimelineState.AgeId == TimelineConfigRepository.LastAgeIdx())
            {
                _travelScreen.Show();
                _canvas.enabled = true;
                return;
            }
            _evolutionPanelGroup.alpha = 1f;

            Unsubscribe();
            Subscribe();
            _presenter.OnScreenOpen();
            InitTimeline().Forget();
            OnStateChanged();

            _canvas.enabled = true;
        }

        private void OnStateChanged()
        {
            _timelineLaybel.text = _presenter.GetTimelineNumber();

            if (_featureUnlockSystem.IsFeatureUnlocked(Feature.Skills))
            {
                _rewardView.SetActive(true);
                _rewardView.SetIcon(_config.IconConfigRepository.GetCurrencyIconFor(CurrencyType.SkillPotion));
                _rewardView.SetAmount(_config.SkillConfigRepository.RewardPerEvolve.Amount.ToCompactFormat());
            }
            else
            {
                _rewardView.SetActive(false);
            }

            OnButtonStateChanged();
        }

        private void OnButtonStateChanged()
        {
            float price = _presenter.GetEvolutionPrice();
            _evolveButton.SetPrice(price.ToCompactFormat());
            _evolveButton.SetCurrencyIcon(_presenter.GetCurrencyIcon());
            _evolveButton.SetMoneyPanelActive(price > 0);
            _evolveButton.SetInteractable(_presenter.CanEvolve());
        }

        private async UniTask InitTimeline()
        {
            await UniTask.Yield();

            _difficultyLaybel.text = _timelineInfoPresenter.GetDifficulty();
            _timelineLaybel.text = _timelineInfoPresenter.GetTimelineText();

            foreach (var item in _timelineInfoPresenter.Items)
            {
                AgeInfoView view = _ageInfoListView.SpawnElement();
                TimelineInfoItemPresenter presenter = new TimelineInfoItemPresenter(item, view);
                presenter.Initialize();
                _presenters.Add(presenter);
            }

            _progressBar.SetActive(true);
            UpdateSlider(_timelineInfoPresenter.CurrentAge, _presenters.Count);

            await UniTask.WaitUntil(() => _scrollRect.content.rect.height > 100);
            Debug.Log($"✅ Content ready: {_scrollRect.content.rect.height}");

            await UniTask.DelayFrame(1);

            var currentMarker = _presenters[_timelineInfoPresenter.CurrentAge].View.MarkerRect;
            Debug.Log($"🎯 Current age: {_timelineInfoPresenter.CurrentAge}, Marker: {currentMarker.name}, pos: {currentMarker.position}");

            _scrollRect.SnapScrollToTarget(currentMarker);
        }

        private async void InitSlider()
        {
            await UniTask.Yield();

            if (_presenters.Count < 2)
            {
                _progressBar.SetWidth(0);
                _progressBar.SetActive(false);
                return;
            }

            _progressBar.SetActive(true);

            float distanceBetweenCenters = _presenters[1].View.Transform.anchoredPosition.x - _presenters[0].View.Transform.anchoredPosition.x;
            float totalWidth = (_presenters.Count + 1) * Mathf.Abs(distanceBetweenCenters);

            _progressBar.SetWidth(totalWidth);
            _progressBar.SetAnchoredPosition(POSITION_LEFT);
        }

        private float _scrollCoefficient = SCROLL_COEFFICIENT;

        private async void AdjustScrollPosition(int currentItem, int items)
        {
            await UniTask.Yield();
            currentItem += 1;

            Debug.Log($"[AdjustScrollPosition] currentItem: {currentItem}, items: {items}");

            float scrollPercentage = ((float)currentItem / (items - 1)) * _scrollCoefficient;

            if (currentItem >= items - 1)
            {
                scrollPercentage = 1f;
            }

            Debug.Log($"[AdjustScrollPosition] scrollPercentage calculated: {scrollPercentage}");
            Debug.Log($"[AdjustScrollPosition] horizontalNormalizedPosition before: {_scrollRect.horizontalNormalizedPosition}");

            _scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(scrollPercentage);

            Debug.Log($"[AdjustScrollPosition] horizontalNormalizedPosition after: {_scrollRect.horizontalNormalizedPosition}");
        }

        private void ScrollToTarget(RectTransform target)
        {
            if (_scrollRect == null || target == null)
            {
                Debug.LogWarning("[ScrollToTargetInstant] ScrollRect or target is null!");
                return;
            }

            var content = _scrollRect.content;
            if (content == null)
            {
                Debug.LogWarning("[ScrollToTargetInstant] ScrollRect has no content!");
                return;
            }

            // Получаем локальную позицию цели относительно контента
            Vector3 targetLocalPos = content.InverseTransformPoint(target.position);

            // Общая прокручиваемая ширина
            float totalScrollable = content.rect.width - _scrollRect.viewport.rect.width;

            if (totalScrollable <= 0f)
                return; // контент не скроллится

            // Центр цели в координатах контента
            float targetCenterX = -targetLocalPos.x - (target.rect.width * 0.5f);

            // Конвертируем в нормализованное значение (0..1)
            float normalized = Mathf.InverseLerp(-totalScrollable * 0.5f, totalScrollable * 0.5f, targetCenterX);
            normalized = Mathf.Clamp01(normalized);

            // Мгновенно ставим позицию скролла
            _scrollRect.horizontalNormalizedPosition = normalized;

            Debug.Log($"[ScrollToTargetInstant] targetX: {targetCenterX:F2}, normalized: {normalized:F3}");
        }

        private void PlayEvolveAnimation()
        {
            _progressBar.Cleanup();

            _animation?.Kill();
            _subAnimation?.Kill();

            PlayEvolveSound();

            _animation = DOTween.Sequence().AppendInterval(_animationDelay);

            _animation.OnComplete(() =>
            {
                _subAnimation = DOTween.Sequence();

                int nextAge = _ageNavigator.CurrentIdx + 1;

                _presenters[nextAge].SetLocked(false);

                int totalAges = _ageNavigator.GetTotalAgesCount();

                if (nextAge >= totalAges)
                {
                    _logger.LogWarning("Attempting to access out of range age. Animation aborted.");
                    return;
                }

                float nextViewportAndBarValue = (float)nextAge / (totalAges - 1);

                _subAnimation.Append(_scrollRect.DOHorizontalNormalizedPos(nextViewportAndBarValue, _scrollAnimationDuration));
                _subAnimation.Join(_progressBar.PlayValueAnimation(nextViewportAndBarValue, _scrollAnimationDuration));

                _subAnimation.AppendCallback(() =>
                {
                    _presenters[nextAge].PlayRippleAnimation(_rippleAnimationDuration);
                });

                _subAnimation.OnComplete(() =>
                {
                    _ageNavigator.MoveToNextAge();
                    if (_adsService.IsAdReady(AdType.Interstitial)) _adsService.ShowInterstitialVideo(Placement.Evolution);
                });
            });
        }

        public async UniTask<bool> ShowScreenWithTransitionAnimation()
        {
            await UniTask.Yield();

            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            PlayEvolveAnimation();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void UpdateSlider(int currentAge, int ages)
        {
            _progressBar.AdjustScrollPositionToAge(++currentAge);
        }

        private void PlayEvolveSound()
        {
            if (_audioService != null && _evolveSFX != null)
            {
                _audioService.PlayOneShot(_evolveSFX);
            }
        }

        public void OnTimelineStateChanged()
        {
            Cleanup();
            InitTimeline().Forget();
        }

        private void Subscribe()
        {
            _evolveButton.ButtonClicked += _presenter.OnEvolveClicked;
            _evolveButton.InactiveClicked += _presenter.OnInactiveEvolveClicked;
            _presenter.StateChanged += OnStateChanged;
            _presenter.ButtonStateChanged += OnButtonStateChanged;
        }

        private void Unsubscribe()
        {
            _evolveButton.ButtonClicked -= _presenter.OnEvolveClicked;
            _evolveButton.InactiveClicked -= _presenter.OnInactiveEvolveClicked;
            _presenter.StateChanged -= OnStateChanged;
            _presenter.ButtonStateChanged -= OnButtonStateChanged;
        }

        private void Cleanup()
        {
            _animation?.Kill();
            _animation = null;

            _subAnimation?.Kill();
            _subAnimation = null;

            _progressBar.Cleanup();

            foreach (var presenter in _presenters)
            {
                presenter.Dispose();
                _ageInfoListView.DestroyElement(presenter.View);
            }

            _presenters.Clear();
        }

        public void OnExit()
        {
            _audioService.PlayButtonSound();
            _taskCompletion?.TrySetResult(true);
            Unsubscribe();
            Cleanup();
        }

        public void Dispose()
        {
            OnExit();
            _presenter.OnScreenDisposed();
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            _presenter.OnScreenActiveChanged(isActive);
        }
    }
}
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
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._EvolveScreen.Scripts
{
    public class EvolveScreen : MonoBehaviour
    {
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

            _travelScreen.Construct(travelScreenPresenter, config, featureUnlockSystem, this);
            _canvas.enabled = false;

            TimelineState.NextAgeOpened += TimelineStateChanged;
        }

        private void TimelineStateChanged()
        {
            Show();
        }

        public void Show()
        {
            _evolutionPanelGroup.alpha = 0f;
            _travelScreen.Hide();

            if (TimelineState.AgeId == TimelineConfigRepository.LastAgeIdx())
            {
                _travelScreen.Show();
                _canvas.enabled = true;
                return;
            }

            _evolutionPanelGroup.alpha = 1f;
            _canvas.enabled = true;

            Unsubscribe();
            Subscribe();
            _presenter.OnScreenOpen();
            InitTimeline().Forget();
            OnStateChanged();
        }

        private void OnStateChanged()
        {
            _timelineLaybel.text = _presenter.GetTimelineNumber();
            UpdateRewardView();
            OnButtonStateChanged();
        }

        private void UpdateRewardView()
        {
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

            SpawnAgeInfoViews();
            _progressBar.SetActive(true);
            UpdateSlider(_timelineInfoPresenter.CurrentAge);

            await WaitForScrollContentReady();
            ScrollToCurrentAge();
        }

        private void SpawnAgeInfoViews()
        {
            foreach (var item in _timelineInfoPresenter.Items)
            {
                AgeInfoView view = _ageInfoListView.SpawnElement();
                TimelineInfoItemPresenter presenter = new TimelineInfoItemPresenter(item, view);
                presenter.Initialize();
                _presenters.Add(presenter);
            }
        }

        private async UniTask WaitForScrollContentReady()
        {
            await UniTask.WaitUntil(() => _scrollRect.content.rect.height > 100);
            await UniTask.DelayFrame(1);
            Debug.Log($"✅ Content ready: {_scrollRect.content.rect.height}");
        }

        private void ScrollToCurrentAge()
        {
            int currentAge = _timelineInfoPresenter.CurrentAge;
            float scrollPosition = CalculateMiddleScrollPosition(currentAge);
            _scrollRect.horizontalNormalizedPosition = scrollPosition;
            Debug.Log($"🎯 Scrolled to age {currentAge}, position: {scrollPosition:F2}");
        }

        private void PlayEvolveAnimation()
        {
            _evolveButton.gameObject.SetActive(false);
            _closeButton.interactable = false;
            _progressBar.Cleanup();
            _animation?.Kill();
            _subAnimation?.Kill();
            PlayEvolveSound();

            _animation = DOTween.Sequence().AppendInterval(_animationDelay);
            _animation.OnComplete(() => AnimateToNextAge());
        }

        private void AnimateToNextAge()
        {
            _subAnimation = DOTween.Sequence();

            int nextAge = _ageNavigator.CurrentIdx + 1;
            int totalAges = _ageNavigator.GetTotalAgesCount();

            if (!ValidateNextAge(nextAge, totalAges))
                return;

            _presenters[nextAge].SetLocked(false);

            float targetScrollPosition = CalculateMiddleScrollPosition(nextAge);

            _subAnimation.Append(_scrollRect.DOHorizontalNormalizedPos(targetScrollPosition, _scrollAnimationDuration));
            _subAnimation.Join(_progressBar.PlayValueAnimation(nextAge + 1, _scrollAnimationDuration));
            _subAnimation.AppendCallback(() => _presenters[nextAge].PlayRippleAnimation(_rippleAnimationDuration));
            _subAnimation.OnComplete(() => OnAnimationComplete());
        }

        private bool ValidateNextAge(int nextAge, int totalAges)
        {
            if (nextAge >= totalAges)
            {
                _logger.LogWarning("Attempting to access out of range age. Animation aborted.");
                return false;
            }
            return true;
        }

        private void OnAnimationComplete()
        {
            _ageNavigator.MoveToNextAge();

            if (_adsService.IsAdReady(AdType.Interstitial))
                _adsService.ShowInterstitialVideo(Placement.Evolution);

            _evolveButton.SetInteractable(false);
            _evolveButton.gameObject.SetActive(true);
            _closeButton.interactable = true;
        }

        private float CalculateMiddleScrollPosition(int currentAge)
        {
            if (!ValidateScrollCalculation())
                return 0f;

            int count = _presenters.Count;
            (int a, int b) = GetMarkerIndices(currentAge, count);

            var (markerA, markerB) = GetMarkers(a, b);
            if (markerA == null || markerB == null)
                return 0f;

            Vector3 midpoint = CalculateMidpoint(markerA, markerB);
            float scrollPos = CalculateScrollPosition(midpoint);

            Debug.Log($"🎯 Calculated scroll position for age {currentAge}: {scrollPos:F3}");
            return scrollPos;
        }

        private bool ValidateScrollCalculation()
        {
            return _scrollRect != null && _presenters != null && _presenters.Count > 0;
        }

        private (int a, int b) GetMarkerIndices(int currentAge, int count)
        {
            int a = currentAge;
            int b = currentAge + 1;

            if (b >= count)
            {
                b = currentAge;
                a = Mathf.Max(0, currentAge - 1);
            }

            return (a, b);
        }

        private (RectTransform markerA, RectTransform markerB) GetMarkers(int indexA, int indexB)
        {
            var markerA = _presenters[indexA].View.MarkerRect;
            var markerB = _presenters[indexB].View.MarkerRect;

            if (!markerA || !markerB)
            {
                Debug.LogWarning($"[CalculateMiddleScrollPosition] Missing marker refs! A:{markerA} B:{markerB}");
                return (null, null);
            }

            return (markerA, markerB);
        }

        private Vector3 CalculateMidpoint(RectTransform markerA, RectTransform markerB)
        {
            var content = _scrollRect.content;
            Vector3 localA = content.InverseTransformPoint(markerA.position);
            Vector3 localB = content.InverseTransformPoint(markerB.position);
            return (localA + localB) * 0.5f;
        }

        private float CalculateScrollPosition(Vector3 midpoint)
        {
            var content = _scrollRect.content;
            var viewport = _scrollRect.viewport;

            float contentWidth = content.rect.width;
            float viewportWidth = viewport.rect.width;

            if (Mathf.Approximately(contentWidth, viewportWidth))
                return 0f;

            float targetCenterX = midpoint.x;
            float scrollPos = (targetCenterX - viewportWidth / 2f) / (contentWidth - viewportWidth);

            return Mathf.Clamp01(scrollPos);
        }

        private void UpdateSlider(int currentAge)
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
            _evolveButton.ButtonClicked += PlayEvolveAnimation;
            _evolveButton.InactiveClicked += _presenter.OnInactiveEvolveClicked;
            _presenter.StateChanged += OnStateChanged;
            _presenter.ButtonStateChanged += OnButtonStateChanged;
        }

        private void Unsubscribe()
        {
            _evolveButton.ButtonClicked -= PlayEvolveAnimation;
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
            TimelineState.NextAgeOpened -= TimelineStateChanged;
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
using System.Collections.Generic;
using _Game.Common;
using _Game.Core._Logger;
using _Game.Core.Ads;
using _Game.Core.Navigation.Age;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Gameplay.Common;
using _Game.UI._TimelineInfoPresenter;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._TimelineInfoScreen.Scripts
{
    public class TimelineInfoScreen : MonoBehaviour
    {

        [SerializeField, Required] private Canvas _canvas;

        [SerializeField] private ScrollRect _scrollRect;

        [SerializeField] private AgeInfoListView _ageInfoListView;

        [SerializeField] private TimelineProgressBar _progressBar;
        [SerializeField] private ThemedButton _exitBtn;
        [SerializeField] private AudioClip _evolveSFX;

        [SerializeField] private float _animationDelay = 1.0f;
        [SerializeField] private float _scrollAnimationDuration = 3f;
        [SerializeField] private float _rippleAnimationDuration = 0.2f;

        [SerializeField] private TMP_Text _difficultyText;
        [SerializeField] private TMP_Text _timelineText;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private readonly List<TimelineInfoItemPresenter> _presenters = new();

        private IAudioService _audioService;
        private ITimelineInfoPresenter _timelineInfoPresenter;
        private IAgeNavigator _ageNavigator;
        private IAdsService _adsService;
        private IMyLogger _logger;

        private bool _isShowForInfo;

        private Sequence _animation;
        private Sequence _subAnimation;

        public void Construct(
            IAudioService audioService,
            ITimelineInfoPresenter timelineInfoPresenter,
            IMyLogger logger,
            IWorldCameraService cameraService,
            IAdsService adsService,
            IAgeNavigator ageNavigator
            )
        {
            _audioService = audioService;
            _timelineInfoPresenter = timelineInfoPresenter;
            _logger = logger;
            //_exitBtn.interactable = false;
            _adsService = adsService;
            _ageNavigator = ageNavigator;

            InitInfoItems();
        }

        private void OnStateChanged()
        {
            Cleanup();
            InitInfoItems();
        }

        private void InitInfoItems()
        {
            _difficultyText.text = _timelineInfoPresenter.GetDifficulty();
            _timelineText.text = _timelineInfoPresenter.GetTimelineText();

            foreach (var item in _timelineInfoPresenter.Items)
            {
                AgeInfoView view = _ageInfoListView.SpawnElement();
                TimelineInfoItemPresenter presenter = new TimelineInfoItemPresenter(item, view);
                presenter.Initialize();
                _presenters.Add(presenter);
            }

            InitSlider();
            UpdateSlider(_timelineInfoPresenter.CurrentAge, _presenters.Count);
            AdjustScrollPosition(_timelineInfoPresenter.CurrentAge, _presenters.Count);

            Unsubscribe();
            Subscribe();
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

            float totalWidth = (_presenters.Count - 1) * Mathf.Abs(distanceBetweenCenters);

            _progressBar.SetWidth(totalWidth);
        }

        //public async UniTask<bool> ShowScreen()
        //{
        //    await UniTask.Yield();
        //    //_exitBtn.SetInteractable(true);
        //    _taskCompletion = new UniTaskCompletionSource<bool>();
        //    var result = await _taskCompletion.Task;

        //    return result;
        //}

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

        [Button]
        public void PlayFirstAgeAnimation()
        {
            _progressBar.Cleanup();

            _animation?.Kill();
            _subAnimation?.Kill();

            //_exitBtn.SetInteractable(false);

            _animation = DOTween.Sequence().AppendInterval(_animationDelay);
            _animation.OnComplete(() =>
            {
                _subAnimation = DOTween.Sequence();

                int age = 0;
                _subAnimation.AppendCallback(() =>
                {
                    _presenters[0].PlayRippleAnimation(_rippleAnimationDuration);
                });

                _subAnimation.OnComplete(() =>
                {
                    //_exitBtn.SetInteractable(true);
                });
            });
        }

        private void PlayEvolveAnimation()
        {
            _progressBar.Cleanup();

            _animation?.Kill();
            _subAnimation?.Kill();

            _exitBtn.SetInteractable(false);

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


                    _exitBtn.SetInteractable(true);

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
                    _exitBtn.SetInteractable(true);
                    if (_adsService.IsAdReady(AdType.Interstitial)) _adsService.ShowInterstitialVideo(Placement.Evolution);
                });
            });
        }

        private async void AdjustScrollPosition(int currentItem, int items)
        {
            await UniTask.Yield();

            float scrollPercentage = ((float)currentItem / (items - 1));
            _scrollRect.horizontalNormalizedPosition = scrollPercentage;
        }

        private void OnExit()
        {
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
            Unsubscribe();
            Cleanup();
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

        private void Subscribe()
        {
            _exitBtn.onClick.AddListener(OnExit);
            _timelineInfoPresenter.StateChanged += OnStateChanged;
        }

        private void Unsubscribe()
        {
            _exitBtn.onClick.RemoveAllListeners();
            _timelineInfoPresenter.StateChanged -= OnStateChanged;
        }

        private void UpdateSlider(int currentAge, int ages) =>
            _progressBar.UpdateValue(currentAge, ages);

        private void PlayEvolveSound()
        {
            if (_audioService != null && _evolveSFX != null)
            {
                _audioService.PlayOneShot(_evolveSFX);
            }
        }
    }
}

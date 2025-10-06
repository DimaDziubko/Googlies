using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#elif UNITY_ANDROID
using Google.Play.Review;
#endif

namespace _Game.UI.RateGame.Scripts
{
    public class RateGamePresenter : IDisposable
    {
        public event Action OnSetPP;

#if UNITY_ANDROID
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;
#endif
        private RateGameScreen _view;

        public RateGamePresenter(
            RateGameScreen rateGameScreen
            )
        {
            _view = rateGameScreen;

            Unsubscribe();
            Subscribe();
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _view.OnClose += Close;
            _view.OnRateGame += RateGame;
        }

        private void Unsubscribe()
        {
            _view.OnClose -= Close;
            _view.OnRateGame -= RateGame;
        }

        private async void RateGame()
        {
#if UNITY_IOS
            Device.RequestStoreReview();
#elif UNITY_ANDROID
            LaunchReviewAsync().Forget();
#endif
            await UniTask.Delay(1000);
            Close();
        }

#if UNITY_ANDROID
        private async UniTask LaunchReviewAsync()
        {
            if (_playReviewInfo == null)
            {
                await InitReviewAsync(true);
            }

            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            await UniTask.WaitUntil(() => launchFlowOperation.IsDone);

            _playReviewInfo = null;

            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                DirectlyOpen();
                return;
            }
            Close();
        }
        private async UniTask InitReviewAsync(bool force = false)
        {
            if (_reviewManager == null)
                _reviewManager = new ReviewManager();

            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            await UniTask.WaitUntil(() => requestFlowOperation.IsDone);

            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                if (force) DirectlyOpen();
                return;
            }

            _playReviewInfo = requestFlowOperation.GetResult();

            // if native window is showed
            if (_playReviewInfo != null)
            {
                OnSetPP?.Invoke();
            }
            else
            {
                DirectlyOpen();
            }
        }
#endif
        private void Close()
        {
            _view.Close();
        }
        private void DirectlyOpen()
        {
            OnSetPP?.Invoke();
            Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}");
        }
    }
}

using System;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._Shop.Scripts._AmountView;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._Hud._DailyTaskView
{
    public class DailyTaskView : MonoBehaviour
    {
        public event UnityAction OnButtonClick
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }

        [SerializeField] private ThemedButton _button;
        [SerializeField] private TMP_Text _dailyInfo;
        [SerializeField] private TMP_Text _progress;
        [SerializeField] private TMP_Text _taskLaybel;
        [SerializeField] private DailyTaskViewAnimator _animator;
        [SerializeField] private AmountView _rewardView;
        [SerializeField] private Slider _progressBar;

        [SerializeField] private TutorialStep _dailyTaskStep;

        public TutorialStep DailyTaskStep => _dailyTaskStep;

        public void Show() => _animator.PlayAppearAnimation(null);
        public AmountView RewardView => _rewardView;
        public void PlayNotification() => _animator.PlayNotificationAnimation();
        public void StopNotification() => _animator.StopNotificationAnimation();
        public void SetInteractable(bool isInteractable) => _button.SetInteractable(isInteractable);
        public void SetProgress(string progress) => _progress.text = progress;
        public void SetProgress(float progress) => _progressBar.value = progress;
        public void SetTaskLaybel(string task) => _taskLaybel.text = task;
        public void SetDailyInfo(string info) => _dailyInfo.text = info;

        public void Hide() => gameObject.SetActive(false);
        public void PlayHide(Action callback) => _animator.PlayDisappearAnimation(callback);
        public void PlayRefresh(Action callback) => _animator.PlayRefreshAnimation(callback);

        public void Cleanup() => _animator.Cleanup();
    }
}
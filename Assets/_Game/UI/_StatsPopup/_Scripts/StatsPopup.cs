using System.Collections.Generic;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._StatsPopup._Scripts
{
    public class StatsPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private TMP_Text _currentWarriorLabel;

        [SerializeField] private ThemedButton _previousStatsButton;
        [SerializeField] private ThemedButton _nextStatsButton;

        [SerializeField] private UnitInfoItem _playerUnitInfoItem;
        [SerializeField] private UnitInfoItem _enemyUnitInfoItem;

        [SerializeField] private Button[] _cancelButtons;

        [SerializeField] private PopupAppearanceAnimation _animation;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private readonly Dictionary<UnitInfoItem, Dictionary<StatType, StatInfoView>> _cachedStats = new(2);

        private IAudioService _audioService;
        private IStatsPopupPresenter _statsPopupPresenter;

        private UnitType _typeToShow;

        public void Construct(
            Camera uICamera,
            IAudioService audioService,
            IStatsPopupPresenter statsPopupPresenter)
        {
            _audioService = audioService;
            _canvas.worldCamera = uICamera;
            _statsPopupPresenter = statsPopupPresenter;
            Init();
        }

        private void Init()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }

            _previousStatsButton.onClick.AddListener(OnPreviousButtonClicked);
            _nextStatsButton.onClick.AddListener(OnNextButtonClicked);

            _canvas.enabled = false;
        }

        private void Cleanup()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.RemoveAllListeners();
            }

            _previousStatsButton.onClick.RemoveListener(OnPreviousButtonClicked);
            _nextStatsButton.onClick.RemoveListener(OnNextButtonClicked);
        }

        public async UniTask<bool> ShowStatsAndAwaitForExit(UnitType type)
        {
            _typeToShow = type;
            UpdateView(_typeToShow);
            UpdateButtons();

            _canvas.enabled = true;
            _animation.PlayShow();
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        private void UpdateButtons()
        {
            _previousStatsButton.gameObject.SetActive(_statsPopupPresenter.CanMovePrevious(_typeToShow));
            _nextStatsButton.gameObject.SetActive(_statsPopupPresenter.CanMoveNext(_typeToShow));
        }

        private void UpdateView(UnitType type)
        {
            _currentWarriorLabel.text = _statsPopupPresenter.GetNameFor(type);
            UpdateUnitView(_playerUnitInfoItem, type, Faction.Player);
            UpdateUnitView(_enemyUnitInfoItem, type, Faction.Enemy);
        }

        private void UpdateUnitView(UnitInfoItem unitInfoItem, UnitType type, Faction faction)
        {
            unitInfoItem.SetIcon(_statsPopupPresenter.GetIconFor(type, faction));
            unitInfoItem.SetTimelineText(_statsPopupPresenter.GetTimelineText());

            UpdateStat(unitInfoItem, type, StatType.Damage, faction);
            UpdateStat(unitInfoItem, type, StatType.Health, faction);
        }

        private void UpdateStat(UnitInfoItem unitInfoItem, UnitType type, StatType statType, Faction faction)
        {
            if (!_cachedStats.ContainsKey(unitInfoItem))
            {
                _cachedStats[unitInfoItem] = new Dictionary<StatType, StatInfoView>(2);
            }

            if (!_cachedStats[unitInfoItem].TryGetValue(statType, out var statView))
            {
                statView = unitInfoItem.StatInfoListView.SpawnElement();
                _cachedStats[unitInfoItem][statType] = statView;
                statView.SetIcon(_statsPopupPresenter.GetStatIcon(statType));
            }

            statView.SetValue(_statsPopupPresenter.GetStatValue(type, statType, faction));
        }

        private void OnNextButtonClicked()
        {
            bool found =
                _statsPopupPresenter.FindNextAvailable(_typeToShow, true, out _typeToShow);
            if (found)
            {
                UpdateView(_typeToShow);
                UpdateButtons();
                PlayButtonSound();
            }
        }

        private void OnPreviousButtonClicked()
        {
            bool found =
                _statsPopupPresenter.FindNextAvailable(_typeToShow, false, out _typeToShow);
            if (found)
            {
                UpdateView(_typeToShow);
                UpdateButtons();
                PlayButtonSound();
            }
        }

        private void OnCancelled()
        {
            Cleanup();
            PlayButtonSound();
            _animation.PlayHide(OnHideComplete);
        }
        
        
        private void OnHideComplete()
        {
            _canvas.enabled = false;
            _taskCompletion.TrySetResult(true);
        }

        private void PlayButtonSound() => _audioService.PlayButtonSound();
    }
}

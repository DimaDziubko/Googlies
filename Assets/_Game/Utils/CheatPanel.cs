using Sirenix.OdinInspector;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Utils
{
    public class CheatPanel : MonoBehaviour
    {
        public event UnityAction AllBattlesWonClicked
        {
            add => _allBattlesWonButton.onClick.AddListener(value);
            remove => _allBattlesWonButton.onClick.RemoveListener(value);
        }

        public event UnityAction NextTimelineClicked
        {
            add => _nextTimelineBtn.onClick.AddListener(value);
            remove => _nextTimelineBtn.onClick.RemoveListener(value);
        }

        public event UnityAction PreviousTimelineClicked
        {
            add => _previousTimelineBtn.onClick.AddListener(value);
            remove => _previousTimelineBtn.onClick.RemoveListener(value);
        }

        public event UnityAction NextAgeClicked
        {
            add => _nextAgeBtn.onClick.AddListener(value);
            remove => _nextAgeBtn.onClick.RemoveListener(value);
        }

        public event UnityAction PreviousAgeClicked
        {
            add => _previousAgeBtn.onClick.AddListener(value);
            remove => _previousAgeBtn.onClick.RemoveListener(value);
        }

        public event UnityAction NextBattleClicked
        {
            add => _nextBattleBtn.onClick.AddListener(value);
            remove => _nextBattleBtn.onClick.RemoveListener(value);
        }

        public event UnityAction PreviousBattleClicked
        {
            add => _previousBattleBtn.onClick.AddListener(value);
            remove => _previousBattleBtn.onClick.RemoveListener(value);
        }

        public event UnityAction CoinsButtonClicked
        {
            add => _coinsButton.onClick.AddListener(value);
            remove => _coinsButton.onClick.RemoveListener(value);
        }

        public event UnityAction GemsButtonClicked
        {
            add => _gemsButton.onClick.AddListener(value);
            remove => _gemsButton.onClick.RemoveListener(value);
        }

        public event UnityAction SkillPetPotionButtonClicked
        {
            add => _skillPetFeedButton.onClick.AddListener(value);
            remove => _skillPetFeedButton.onClick.RemoveListener(value);
        }
        
        public event UnityAction BPPointsButtonClicked
        {
            add => _bpButton.onClick.AddListener(value);
            remove => _bpButton.onClick.RemoveListener(value);
        }

        [SerializeField, Required] private TMP_Text _timelineLabel;
        [SerializeField, Required] private TMP_Text _ageLabel;
        [SerializeField, Required] private TMP_Text _battleLabel;
        
        [SerializeField, Required] private TMP_Text _balancyLabel;

        [SerializeField, Required] private ThemedButton _nextTimelineBtn;
        [SerializeField, Required] private ThemedButton _previousTimelineBtn;

        [SerializeField, Required] private ThemedButton _nextAgeBtn;
        [SerializeField, Required] private ThemedButton _previousAgeBtn;

        [SerializeField, Required] private ThemedButton _nextBattleBtn;
        [SerializeField, Required] private ThemedButton _previousBattleBtn;

        [SerializeField, Required] private ThemedButton _coinsButton;
        [SerializeField, Required] private ThemedButton _gemsButton;

        [SerializeField, Required] private ThemedButton _allBattlesWonButton;
        [SerializeField, Required] private ThemedButton _skillPetFeedButton;
        
        [SerializeField, Required] private ThemedButton _bpButton;

        public void SetTimeline(string timeline) => _timelineLabel.text = timeline;
        public void SetAge(string age) => _ageLabel.text = age;
        public void SetBattle(string battle) => _battleLabel.text = battle;
        public void SetBalancyInfo(string info) => _balancyLabel.text = info;

        public void SetNextTimelineBtnInteractable(bool isInteractable) =>
            _nextTimelineBtn.SetInteractable(isInteractable);

        public void SetPreviousTimelineBtnInteractable(bool isInteractable) =>
            _previousTimelineBtn.SetInteractable(isInteractable);

        public void SetNextAgeBtnInteractable(bool isInteractable) =>
            _nextAgeBtn.SetInteractable(isInteractable);

        public void SetPreviousAgeBtnInteractable(bool isInteractable) =>
            _previousAgeBtn.SetInteractable(isInteractable);

        public void SetNextBattleBtnInteractable(bool isInteractable) =>
            _nextBattleBtn.SetInteractable(isInteractable);

        public void SetPreviousBattleBtnInteractable(bool isInteractable) =>
            _previousBattleBtn.SetInteractable(isInteractable);

    }
}

using _Game.Core._GameMode;
using _Game.Core._Logger;
using _Game.LiveopsCore;
using _Game.LiveopsCore._Enums;
using _Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._StartBattleScreen.Scripts
{
    public class StartBattleScreen : MonoBehaviour
    {
        public event UnityAction StartClicked
        {
            add => _startBattleButton.onClick.AddListener(value);
            remove => _startBattleButton.onClick.RemoveListener(value);
        }

        public event UnityAction NextBattleClicked
        {
            add => _nextBattleButton.onClick.AddListener(value);
            remove => _nextBattleButton.onClick.RemoveListener(value);
        }

        public event UnityAction PreviousBattleClicked
        {
            add => _previousBattleButton.onClick.AddListener(value);
            remove => _previousBattleButton.onClick.RemoveListener(value);
        }

        public event UnityAction SettingsClicked
        {
            add => _settingsButton.onClick.AddListener(value);
            remove => _settingsButton.onClick.RemoveListener(value);
        }

        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _startBattleButton;
        [SerializeField] private Button _nextBattleButton;
        [SerializeField] private Button _previousBattleButton;
        [SerializeField] private Button _settingsButton;

        [SerializeField, Required] private GameEventPanel _leftGameEventPanel;
        [SerializeField, Required] private GameEventPanel _rightGameEventPanel;
        
        [SerializeField] private CheatPanel _cheatPanel;

        public CheatPanel CheatPanel => _cheatPanel;
        
        
        public GameEventPanel GetGameEventPanel(GameEventPanelType type)
        {
            return type switch
            {
                GameEventPanelType.Left => _leftGameEventPanel,
                GameEventPanelType.Right => _rightGameEventPanel,
                _ => _leftGameEventPanel
            };
        }
        
        private IMyLogger _logger;
        private IStartBattleScreenPresenter _presenter;


        public void Construct(
            Camera uICamera,
            IMyLogger logger,
            IStartBattleScreenPresenter presenter)
        {
            _canvas.enabled = false;
            _canvas.worldCamera = uICamera;
            _logger = logger;
            
            _presenter = presenter;
            _presenter.Screen = this;

            if (!GameModeSettings.I.IsCheatEnabled)
                _cheatPanel.gameObject.SetActive(false);
        }

        public void SetStartBattleBtnInteractable(bool isInteractable) =>
            _startBattleButton.interactable = isInteractable;

        public void SetNextBattleBtnInteractable(bool isInteractable) => 
            _nextBattleButton.interactable = isInteractable;

        public void SetNextBattleBtnActive(bool isActive) =>
            _nextBattleButton.gameObject.SetActive(isActive);

        public void SetPreviousBattleBtnInteractable(bool isInteractable) => 
            _previousBattleButton.interactable = isInteractable;

        public void SetPreviousBattleBtnActive(bool isActive) =>
            _previousBattleButton.gameObject.SetActive(isActive);

        public void Show()
        {
            _presenter.OnScreenOpened();
            _canvas.enabled = true;
        }

        public void Hide()
        {
            _presenter.OnScreenClosed();
            _canvas.enabled = false;
        }

        public void Dispose() => _presenter.OnScreenDispose();

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            _presenter.OnActiveChanged(isActive);
        }

        public void ClearEventPanels()
        {
            _leftGameEventPanel.Cleanup();
            _rightGameEventPanel.Cleanup();
        }
    }
}

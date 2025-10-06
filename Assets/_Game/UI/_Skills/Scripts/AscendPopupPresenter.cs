using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Utils.Extensions;

namespace _Game.UI._Skills.Scripts
{
    public class AscendPopupPresenter
    {
        private readonly SkillModel _model;
        private readonly AscendPopup _popup;
        private readonly IAudioService _audioService;
        private readonly IIconConfigRepository _config;
        private readonly IMyLogger _logger;

        private readonly Dictionary<Boost, PassiveBoostInfoView> _passiveBoostInfo = new();

        public AscendPopupPresenter(
            SkillModel model, 
            AscendPopup popup, 
            IMyLogger logger,
            IAudioService audioService,
            IIconConfigRepository config)
        {
            _model = model;
            _popup = popup;
            _logger = logger;
            _audioService = audioService;
            _config = config;
        }

        public void Initialize()
        {
            _model.Skill.OnAscensionLevelUp += OnAscend;
            _popup.OnAscendButtonClicked += _model.Ascend;
            _popup.OnCancelButtonClicked += Hide;
            
            _popup.SetAscendButtonInteractable(_model.IsAscendAvailable());
            _popup.SetActive(false);
        }

        public void Show()
        {
            UpdateBoostInfo();
            _popup.SetActive(true);
            _popup.SetAscendButtonInteractable(_model.IsAscendAvailable());
            _popup.PopupAppearanceAnimation.PlayShow();
        }

        private void Hide()
        {
            PlayButtonSound();
            _popup.PopupAppearanceAnimation.PlayHide(OnHideComplete);
        }

        private void OnHideComplete() => _popup.SetActive(false);

        public void Dispose()
        {
            _model.Skill.OnAscensionLevelUp -= OnAscend;
            _popup.OnAscendButtonClicked -= _model.Ascend;
            _popup.OnCancelButtonClicked -= Hide;
            _popup.PopupAppearanceAnimation.Cleanup();
            _passiveBoostInfo.Clear();
        }

        private void OnAscend(ActiveSkill skill)
        {
            UpdateBoostInfo();
            PlayUpgradeSound();
            _popup.SetAscendButtonInteractable(_model.IsAscendAvailable());
        }

        private void PlayButtonSound() => 
            _audioService.PlayButtonSound();
        
        private void PlayUpgradeSound() => 
            _audioService.PlayUpgradeSound();
        
        private void UpdateBoostInfo()
        {
            for (int i = 0; i < _model.Boosts.Length; i++)
            {
                var boost = _model.Boosts[i];
                
                if (!_passiveBoostInfo.TryGetValue(boost, out var view))
                {
                    view = _popup.PassiveBoostInfoListView.SpawnElement();
                    _passiveBoostInfo.Add(boost, view);
                }
                
                float currentPassiveValue = _model.GetCurrentPassiveFor(i);
                float nextPassiveValue = _model.GetNextPassiveFor(i);
                
                view.SetIcon(_config.ForBoostIcon(boost.Type));
                view.SetName(boost.Name);
                view.SetCurrentValue(currentPassiveValue.ToCompactFormat());
                view.SetNextValue(nextPassiveValue.ToCompactFormat());
                view.SetNextValueActive(!_model.IsAscendAvailable());
            }
        }
    }
}
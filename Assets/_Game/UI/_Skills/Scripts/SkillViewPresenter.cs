using System;
using _Game.Core.Services.UserContainer;
using Sirenix.OdinInspector;

namespace _Game.UI._Skills.Scripts
{
    public class SkillViewPresenter
    {
        [ShowInInspector, ReadOnly]
        private readonly SkillModel _model;
        [ShowInInspector, ReadOnly]
        private readonly SkillView _view;
        private readonly IUserContainer _userContainer;
        public SkillView View => _view;
        
        public SkillViewPresenter(
            SkillModel model,
            SkillView view,
            IUserContainer userContainer)
        {
            _model = model;
            _view = view;
            _userContainer = userContainer;
        }

        public bool IsNewSkill => _model.IsNew;

        public void Initialize()
        {
            _view.SetIcon(_model.GetIcon());
            _view.SetSplashImageActive(false);
            _view.SetAnimationBgActive(false);
        }

        public void Dispose() { }
        public void PlaySimpleAppearanceAnimation() => _view.PlaySimpleAnimation();
        public void PlayIlluminationAppearanceAnimation(Action callback = null) => _view.PlayIlluminationAnimation(callback);
    }
}
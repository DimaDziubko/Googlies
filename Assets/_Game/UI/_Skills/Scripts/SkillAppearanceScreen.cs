using System.Collections.Generic;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Skills.Scripts
{
    public class SkillAppearanceScreen : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private SkillView _singleView;
        [SerializeField] private SkillView[] _10Views;
        [SerializeField] private Button _exitButton;
        [SerializeField] private int _appearanceDelay = 1000;
        [SerializeField] private int _collectionAppearanceDelay = 200;
        [SerializeField] private int _illuminationAnimationDelay = 200;

        private IAudioService _audioService;
        private IUserContainer _userContainer;

        private UniTaskCompletionSource<bool> _taskCompletion;

        [ShowInInspector, ReadOnly] private readonly List<SkillViewPresenter> _presenters = new();

        public void Construct(
            Camera cameraServiceUICameraOverlay,
            IAudioService audioService,
            IUserContainer userContainer)
        {
            _canvas.worldCamera = cameraServiceUICameraOverlay;
            _audioService = audioService;
            _userContainer = userContainer;
            
            DisableViews();

            Init();

            _canvas.enabled = true;
        }
        
        [Button]
        private void DisableViews()
        {
            _singleView.SetActive(false);
            foreach (var view in _10Views)
            {
                view.SetActive(false);
            }
        }

        private void Init()
        {
            _exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        private void OnExitButtonClicked()
        {
            Cleanup();
            _taskCompletion.TrySetResult(true);
            PlayButtonSound();
        }

        public async UniTask<bool> ShowAnimationAndAwaitForExit(
            List<SkillModel> cardModelsForAnimation)
        {
            _exitButton.interactable = false;
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();

            await UniTask.Delay(_appearanceDelay);
            if (cardModelsForAnimation.Count == 1)
            {
                InitSkill(cardModelsForAnimation[0], _singleView);
            }
            else
            {
                InitSkills(cardModelsForAnimation);
            }

            await PlayCollectionAnimation(_presenters);

            _exitButton.interactable = true;
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void InitSkill(SkillModel model, SkillView view)
        {
            SkillViewPresenter viewPresenter = new SkillViewPresenter(model, view, _userContainer);
            viewPresenter.Initialize();
            _presenters.Add(viewPresenter);
        }

        private void InitSkills(List<SkillModel> models)
        {
            for (int i = 0; i < models.Count && i < _10Views.Length; i++)
            {
                InitSkill(models[i], _10Views[i]);
            }
        }

        private async UniTask PlayCollectionAnimation(List<SkillViewPresenter> presenters)
        {
            foreach (var presenter in presenters)
            {
                await PlaySingleAnimation(presenter);
                await UniTask.Delay(_collectionAppearanceDelay);
            }
        }

        private async UniTask PlaySingleAnimation(SkillViewPresenter viewPresenter)
        {
            PlayAppearanceSound();

            viewPresenter.View.SetActive(true);
            viewPresenter.View.SetAnimationBgActive(true);
            
            if (viewPresenter.IsNewSkill)
            {
                viewPresenter.PlayIlluminationAppearanceAnimation(PlayIlluminationSound);
                await UniTask.Delay(_illuminationAnimationDelay);
            }
            else
            {
                viewPresenter.PlaySimpleAppearanceAnimation();
            }

            await UniTask.Yield();
        }

        private void PlayAppearanceSound() => _audioService.PlayCardAppearanceSfx();
        private void PlayIlluminationSound() => _audioService.PlayCardRippleSfx();
        private void PlayButtonSound() => _audioService.PlayButtonSound();
        
        private void Cleanup()
        {
            _exitButton.onClick.RemoveAllListeners();
            foreach (var presenter in _presenters)
            {
                presenter.Dispose();
            }

            _presenters.Clear();
        }
    }
}
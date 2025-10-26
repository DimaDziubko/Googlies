using System.Collections.Generic;
using System.Linq;
using _Game.Core.Boosts;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI.Common.Scripts;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._BoostPopup
{
    public class BoostPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private BoostInfoContainer _boostInfoContainerListView;

        [SerializeField] private Button[] _cancelButtons;

        [SerializeField] private PopupAppearanceAnimation _animation;

        private UniTaskCompletionSource<bool> _taskCompletion;

        private List<BoostInfoContainer> _containers = new(2);

        private Dictionary<BoostModel, BoostDataPresenter> _presenters = new();

        private BoostDataPresenter.Factory _factory;


        private IAudioService _audioService;
        private IBoostPopupPresenter _popupPresenter;

        public void Construct(
            Camera uICameraOverlay,
            IAudioService audioService,
            IBoostPopupPresenter popupPresenter,
            BoostDataPresenter.Factory factory)
        {
            _canvas.worldCamera = uICameraOverlay;
            _audioService = audioService;
            _popupPresenter = popupPresenter;
            _factory = factory;
            Init();
        }

        private void Init()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }
        }

        public async UniTask<bool> Show(BoostSource mainSource, BoostSource subSource)
        {
            if (subSource != BoostSource.None)
            {
                InitBoostContainer(subSource);
            }

            if (mainSource != BoostSource.None)
            {
                InitBoostContainer(mainSource);
            }

            _animation.PlayShow(OnShowComplete);

            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        private void OnShowComplete() => _canvas.enabled = true;

        private void InitBoostContainer(BoostSource source)
        {
            if (source != BoostSource.Total)
            {
                var panel = _boostInfoContainerListView.SpawnElement();
                panel.SetName(source.ToName());
            }

            IEnumerable<BoostModel> boosts = _popupPresenter.GetBoosts(source);

            if (boosts != null)
            {
                var boostsToShow = boosts.Where(x =>
                    (x.Value > 1) ||
                    (x.Type == BoostType.AllUnitDamage || x.Type == BoostType.AllUnitHealth));

                foreach (var boost in boostsToShow)
                {
                    var view = _boostInfoContainerListView.BoostInfoListView.SpawnElement();
                    var presenter = _factory.Create(boost, view);
                    presenter.Initialize();
                    _presenters.Add(boost, presenter);
                }
            }

            _containers.Add(_boostInfoContainerListView);
        }

        private void OnCancelled()
        {
            DisableButtons();
            PlayButtonSound();
            _animation.PlayHide(OnHideComplete);
        }

        private void DisableButtons()
        {
            foreach (var button in _cancelButtons)
            {
                button.interactable = false;
            }
        }

        private void OnHideComplete()
        {
            _canvas.enabled = false;
            _taskCompletion.TrySetResult(true);
        }

        private void PlayButtonSound() =>
            _audioService.PlayButtonSound();
    }
}
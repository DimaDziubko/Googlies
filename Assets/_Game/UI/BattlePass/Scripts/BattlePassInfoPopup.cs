using System.Collections;
using System.Collections.Generic;
using _Game.Core.Services.Audio;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Game.UI.BattlePass.Scripts
{
    public class BattlePassInfoPopup : MonoBehaviour
    {
        private const int ELEMENTS_COUNT = 7;
        private const float DURATION = 0.25f;
        private const float PEAK_SCALE = 1.2f;
        
        [SerializeField, Required] private UIDocument _uiDocument;

        [ShowInInspector, ReadOnly]
        private readonly List<VisualElement> _elements = new(ELEMENTS_COUNT);
        
        private Button _exitBtn;
        
        private IAudioService _audioService;

        public void Initialize(IAudioService audioService)
        {
            _audioService = audioService;
            
            for (int i = 1; i <= ELEMENTS_COUNT; i++)
            {
                var element = _uiDocument.rootVisualElement.Q<VisualElement>($"BP-info-container-{i}");
                if(element != null)
                    _elements.Add(element);
            }
            
            _exitBtn = _uiDocument.rootVisualElement.Q<Button>($"BP-exit-btn-bg");

            if (_exitBtn != null)
            {
                _exitBtn.clicked += OnExitBtnClicked;
                _exitBtn.SetEnabled(false);
            }
        }

        public void ShowSequence()
        {
            foreach (var element in _elements)
            {
                element.transform.scale = Vector3.zero;
            }
            
            StartCoroutine(AnimateElements());
        }

        private IEnumerator AnimateElements()
        {
            yield return null;
            
            foreach (var element in _elements)
            {
                yield return AnimateElement(element);
            }

            _exitBtn.SetEnabled(true);
        }

        private IEnumerator AnimateElement(VisualElement element)
        {
            element.style.opacity = 1f;
            element.transform.scale = Vector3.one * 0.001f;

            bool completed = false;
            
            _ = DOTween.To(() => 0f, t => {
                    float progress = t * 2f;
                    float scale = progress <= 1f
                        ? Mathf.Lerp(0f, PEAK_SCALE, progress)
                        : Mathf.Lerp(PEAK_SCALE, 1f, progress - 1f);

                    element.transform.scale = Vector3.one * scale;
                }, 1f, DURATION)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => completed = true);
            
            yield return new WaitUntil(() => completed);
        }

        private void OnExitBtnClicked()
        {
            SetActive(false);
            _audioService.PlayButtonSound();
        }

        public void Dispose()
        {
            if (_exitBtn != null)
                _exitBtn.clicked -= OnExitBtnClicked;
        }

        public void SetActive(bool isActive)
        {
            if (isActive)
            {
                _uiDocument.rootVisualElement.style.display = DisplayStyle.Flex; 
                return;
            }
            
            _uiDocument.rootVisualElement.style.display = DisplayStyle.None;
        }

        public void SetDocumentActive(bool isActive)
        {
            _uiDocument.enabled = isActive;
        }
    }
}
using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._BoostPopup
{
    namespace _Game.UI._BoostPopup
    {
        public class FadeAnimation : MonoBehaviour
        {
            [SerializeField] private float _fadeDelay;

            [SerializeField] private Image[] _images;
            [SerializeField] private TMP_Text[] _labels;

            [SerializeField] private List<FadeData> _fadeDataList;

            private readonly List<Tween> _activeTweens = new();
            
            public void Play(Action onComplete = null)
            {
                Play(_fadeDataList.Count - 1, onComplete);
            }
            
            public void Play(int targetCycleIndex, Action onComplete = null)
            {
                Cleanup();

                if (!gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(true);
                }

                SetInitialAlpha();

                var delayedCall =
                    DOVirtual.DelayedCall(_fadeDelay, () => PlayFadeSequence(0, targetCycleIndex, onComplete));
                _activeTweens.Add(delayedCall);
            }

            private void SetInitialAlpha()
            {
                if (_fadeDataList.Count > 0)
                {
                    var initialAlpha = _fadeDataList[0].AlphaValue;

                    if (_images != null)
                    {
                        foreach (var image in _images)
                        {
                            if (image != null)
                            {
                                image.color = new Color(image.color.r, image.color.g, image.color.b, initialAlpha);
                            }
                        }
                    }

                    if (_labels != null)
                    {
                        foreach (var label in _labels)
                        {
                            if (label != null)
                            {
                                label.color = new Color(label.color.r, label.color.g, label.color.b, initialAlpha);
                            }
                        }
                    }
                }
            }

            private void PlayFadeSequence(int index, int targetCycleIndex, Action onComplete = null)
            {
                if (index >= _fadeDataList.Count) return;

                var fadeData = _fadeDataList[index];

                foreach (var image in _images)
                {
                    if (image != null)
                    {
                        var imageTween = image.DOFade(fadeData.AlphaValue, fadeData.Duration);
                        imageTween.SetUpdate(true);
                        _activeTweens.Add(imageTween);
                    }
                }

                foreach (var label in _labels)
                {
                    if (label != null)
                    {
                        var labelTween = label.DOFade(fadeData.AlphaValue, fadeData.Duration);
                        labelTween.SetUpdate(true);
                        _activeTweens.Add(labelTween);
                    }
                }

                if (index == targetCycleIndex)
                {
                    onComplete?.Invoke();
                }

                var nextFadeCall = DOVirtual.DelayedCall(fadeData.Duration,
                        () => PlayFadeSequence(index + 1, targetCycleIndex, onComplete))
                    .SetUpdate(true);
                _activeTweens.Add(nextFadeCall);
            }

            public void Cleanup()
            {
                foreach (var tween in _activeTweens)
                {
                    tween?.Kill();
                }

                _activeTweens.Clear();

                if (gameObject.activeInHierarchy)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
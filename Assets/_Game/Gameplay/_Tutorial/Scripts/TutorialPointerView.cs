using _Game.UI.Factory;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public class TutorialPointerView : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _pointerTransform;
        [SerializeField, Required] private RectTransform _arrowViewTransform;
        [SerializeField, Required] private Animation _animation;

        //Appearance animation data
        [SerializeField] private Vector3 _defaultPositionOutOfScreen = new(0, 1900, 0);
        [SerializeField] private Vector3 _startAppearanceScale = new(4, 4, 1);
        [SerializeField] private float _animationDuration = 0.5f;

        public IUIFactory OriginFactory { get; set; }
        public RectTransform Parent { get; set; }

        private Tween _transformTween;
        private Tween _scaleTween;

        private Vector3 _localPosition;


        public void Show(TutorialStepData tutorialData)
        {
            Vector3 pointerPosition = tutorialData.CalculateRequiredPointerPosition(Parent);
            Quaternion pointerRotation = tutorialData.CalculateRequiredRotation();
            
            if (tutorialData.NeedAppearanceAnimation)
            {
                Rotation = pointerRotation;
                ShowWithAppearanceAnimation(tutorialData);
                return;
            }

            Enable();
            Position = pointerPosition;
            Rotation = pointerRotation;
            Size = tutorialData.RequiredPointerSize;
            StartAnimation();
        }

        private void ShowWithAppearanceAnimation(TutorialStepData tutorialData)
        {
            _transformTween?.Kill();
            _scaleTween?.Kill();
            
            Size = tutorialData.RequiredPointerSize;
            
            Enable();

            Vector3 pointerPosition = tutorialData.CalculateRequiredPointerPosition(Parent);
            Quaternion pointerRotation = tutorialData.CalculateRequiredRotation();
            
            _pointerTransform.anchoredPosition = tutorialData.IsUnderneath ? 
                new Vector2(pointerPosition.x, -_defaultPositionOutOfScreen.y) :
                new Vector2(pointerPosition.x, _defaultPositionOutOfScreen.y);

            _pointerTransform.localScale = _startAppearanceScale;

            _transformTween = _pointerTransform.DOAnchorPosY(pointerPosition.y, _animationDuration)
                .SetEase(Ease.OutQuad);

            _scaleTween = _pointerTransform.DOScale(Vector3.one, _animationDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    Position = pointerPosition;
                    Rotation = pointerRotation;
                    StartAnimation();
                });
        }
        public void Hide()
        {
            _scaleTween?.Kill();
            _transformTween?.Kill();
            StopAnimation();
            Disable();
            OriginFactory.Reclaim(this);
        }

        private Vector3 Position
        {
            get => _pointerTransform.anchoredPosition;
            set => _pointerTransform.anchoredPosition = value;
        }

        private Quaternion Rotation
        {
            get => _pointerTransform.rotation;
            set => _pointerTransform.rotation = value;
        }

        private Vector3 Size
        {
            get => _arrowViewTransform.sizeDelta;
            set => _arrowViewTransform.sizeDelta = new Vector2(value.x, value.y);
        }

        private void StartAnimation()
        {
            _animation.Play();
        }

        private void StopAnimation()
        {
            _animation.Stop();
        }

        private void Enable()
        {
            gameObject.SetActive(true);
        }

        private void Disable()
        {
            gameObject.SetActive(false);
        }

        public void SetActive(bool isVisible)
        {
            if (isVisible)
            {
                Enable();
                StartAnimation();
                return;
            }

            Disable();
        }
    }
}

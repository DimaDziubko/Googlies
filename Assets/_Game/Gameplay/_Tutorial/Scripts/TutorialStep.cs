using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public class TutorialStep : MonoBehaviour, ITutorialStep
    {
        public event Action<ITutorialStep> Show;
        public event Action<ITutorialStep> Complete;
        public event Action<ITutorialStep> Cancel;

        [SerializeField] private int _step;
        [SerializeField] private int[] _affectedSteps;
        [SerializeField] private int[] _exclusiveSteps;
        [SerializeField] private Vector2 _requiredPointerSize;
        [SerializeField] private Vector2 _offset;
        [SerializeField, Required] private RectTransform _rootCanvasTransform;
        [SerializeField, Required] private RectTransform _tutorialObjectRectTransform;
        [SerializeField] private bool _isUnderneath;
        [SerializeField] private bool _needAppearanceAnimation;

        private TutorialStepData _data;

        private Coroutine _activeCoroutine;

        public void ShowStep(float delay = 0f)
        {
            if (_activeCoroutine != null)
            {
                StopCoroutine(_activeCoroutine);
                _activeCoroutine = null;
            }

            if (delay > 0f)
            {
                _activeCoroutine = StartCoroutine(ShowStepWithDelayCoroutine(delay));
            }
            else
            {
                Show?.Invoke(this);
            }
        }

        public void CompleteStep()
        {
            CancelActiveCoroutine();
            Complete?.Invoke(this);
        }

        public void CancelStep()
        {
            CancelActiveCoroutine();
            Cancel?.Invoke(this);
        }

        private void CancelActiveCoroutine()
        {
            if (_activeCoroutine != null)
            {
                StopCoroutine(_activeCoroutine);
                _activeCoroutine = null;
            }
        }

        public TutorialStepData GetTutorialStepData()
        {
            _data ??= CreateData();
            return _data;
        }

        private IEnumerator ShowStepWithDelayCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            Show?.Invoke(this);
            _activeCoroutine = null;
        }

        private TutorialStepData CreateData()
        {
            var data = new TutorialStepData()
            {
                Step = _step,
                AffectedSteps = _affectedSteps,
                ExclusiveSteps = _exclusiveSteps,
                RequiredPointerSize = _requiredPointerSize,
                NeedAppearanceAnimation = _needAppearanceAnimation,
                IsUnderneath = _isUnderneath,
                TutorialObjectRectTransform = _tutorialObjectRectTransform
            };
           
            return data;
        }
        private void OnDestroy()
        {
            CancelActiveCoroutine();
        }
    }
}
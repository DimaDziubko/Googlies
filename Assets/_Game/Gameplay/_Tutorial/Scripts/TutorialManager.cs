using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI.Factory;
using _Game.Utils;
using UnityUtils;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public class TutorialManager : ITutorialManager, IDisposable
    {
        private readonly TutorialPointersParent _pointersParent;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IUIFactory _uiFactory;
        private readonly IMyLogger _logger;

        private ITutorialStateReadonly TutorialState => _userContainer.State.TutorialState;

        private readonly Dictionary<int, TutorialPointerView> _activePointers = new();
        private readonly HashSet<int> _registeredSteps = new();

        public TutorialManager(
            TutorialPointersParent pointersParent,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IUIFactory uiFactory,
            IMyLogger logger)
        {
            _pointersParent = pointersParent;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _uiFactory = uiFactory;
            _logger = logger;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            TutorialState.StepsCompletedChanged += OnStepCompleted;
            _pointersParent.Disable();
        }

        void IDisposable.Dispose()
        {
            TutorialState.StepsCompletedChanged -= OnStepCompleted;
            _gameInitializer.OnPostInitialization -= Init;
            ClearAllPointers();
            _registeredSteps.Clear();
        }

        public bool Register(ITutorialStep tutorialStep)
        {
            if (tutorialStep == null) return false;
            var stepData = tutorialStep.GetTutorialStepData();

            int stepId = stepData.Step;

            if (_registeredSteps.Contains(stepId))
            {
                _logger.Log($"STEP {stepId} ALREADY REGISTERED, SKIPPING", DebugStatus.Warning);
                return false;
            }

            if (stepData.ExclusiveSteps != null && !stepData.ExclusiveSteps
                .All(exclusiveStep => TutorialState.CompletedSteps.Contains(exclusiveStep)))
            {
                return false;
            }

            if (TutorialState.CompletedSteps.Contains(stepId)) return false;

            _logger.Log($"REGISTER STEP {stepId}", DebugStatus.Warning);

            tutorialStep.Show += Show;
            tutorialStep.Complete += OnStepComplete;
            tutorialStep.Cancel += OnTutorialBroke;

            _registeredSteps.Add(stepId);

            return true;
        }

        public void UnRegister(ITutorialStep tutorialStep)
        {
            if (tutorialStep == null) return;
            _logger.Log($"UNREGISTER STEP {tutorialStep.GetTutorialStepData().Step}", DebugStatus.Warning);

            var stepData = tutorialStep.GetTutorialStepData();
            int stepId = stepData.Step;

            if (_registeredSteps.Contains(stepId))
            {
                _logger.Log($"UNREGISTER STEP {stepId}", DebugStatus.Warning);
                _registeredSteps.Remove(stepId);
            }

            tutorialStep.Show -= Show;
            tutorialStep.Complete -= OnStepComplete;
            tutorialStep.Cancel -= OnTutorialBroke;
        }

        private void Show(ITutorialStep tutorialStep)
        {
            if (_pointersParent.OrNull() == null)
            {
                _logger.Log("TutorialPointersParent doesn't exist", DebugStatus.Warning);
                return;
            }

            var tutorialData = tutorialStep.GetTutorialStepData();

            _logger.Log($"SHOW STEP {tutorialData.Step}", DebugStatus.Warning);

            if (_activePointers.ContainsKey(tutorialData.Step) ||
                TutorialState.CompletedSteps.Contains(tutorialData.Step)) return;

            _pointersParent.Enable();
            TutorialPointerView view = _uiFactory.GetTutorialPointer(_pointersParent.RectTransform);

            view.Show(tutorialData);

            _activePointers[tutorialData.Step] = view;
        }

        private void OnTutorialBroke(ITutorialStep tutorialStep)
        {
            _logger.Log($"TUTORIAL STEP BREAK {tutorialStep.GetTutorialStepData().Step}", DebugStatus.Warning);
            Break(tutorialStep.GetTutorialStepData().Step);
        }

        private void OnStepComplete(ITutorialStep tutorialStep)
        {
            UnRegister(tutorialStep);
            var tutorialData = tutorialStep.GetTutorialStepData();
            foreach (var step in tutorialData.AffectedSteps)
            {
                _logger.Log($"TUTORIAL STEP COMPLETE {step}", DebugStatus.Warning);

                if (!TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.START_TUTORIAL_KEY))
                    _userContainer.TutorialStateHandler.CompleteTutorialStep(Constants.TutorialSteps.START_TUTORIAL_KEY);

                _userContainer.TutorialStateHandler.CompleteTutorialStep(step);

                if (TutorialState.CompletedSteps.Count == Constants.TutorialSteps.STEPS_COUNT_WITH_START_KEY)
                {
                    _userContainer.TutorialStateHandler.CompleteTutorialStep(Constants.TutorialSteps.COMPLETE_TUTORIAL_KEY);
                }
            }
        }

        private void OnStepCompleted(int step) => Break(step);

        private void Break(int step)
        {
            if (_activePointers.TryGetValue(step, out var pointer) && pointer != null)
            {
                pointer.Hide();
                _activePointers.Remove(step);
            }

            if (_activePointers.Count == 0 &&
                _pointersParent != null &&
                _pointersParent.gameObject != null)
            {
                _pointersParent.Disable();
            }
        }

        private void ClearAllPointers()
        {
            foreach (var pointer in _activePointers.Values)
            {
                pointer.Hide();
            }

            _activePointers.Clear();
        }
    }
}

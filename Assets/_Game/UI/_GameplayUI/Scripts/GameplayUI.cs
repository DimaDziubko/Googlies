using System.Collections.Generic;
using _Game.Core.CustomKernel;
using _Game.Core.Services._Camera;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.UI._Skills.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._GameplayUI.Scripts
{
    public class GameplayUI : MonoBehaviour, IGameTickable
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private FoodPanel _foodPanel;
        [SerializeField] private UnitBuilderUI _unitBuilderUI;


        [SerializeField, Required] private GameObject _skillPanel;
        [SerializeField] private List<SkillSocket> _skillViews;

        public UnitBuilderUI UnitBuilderUI => _unitBuilderUI;
        public FoodPanel FoodPanel => _foodPanel;

        public IEnumerable<SkillSocket> SkillViews => _skillViews;

        public void Construct(IWorldCameraService cameraService)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            Hide();
        }

        public void Show() => _canvas.enabled = true;

        public void Hide() => _canvas.enabled = false;

        void IGameTickable.Tick(float deltaTime)
        {
            _foodPanel.Tick(deltaTime);

            foreach (var skillView in _skillViews)
            {
                skillView.SmoothProgressController.Tick(deltaTime);
            }
        }

        public void SetSkillPanelActive(bool isActive) =>
            _skillPanel.SetActive(isActive);
    }
}
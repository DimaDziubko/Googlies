using System;
using System.Linq;
using _Game.Core.Services._Camera;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._BoostPopup;
using _Game.UI._Shop._MiniShop.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public class UpgradesScreen : MonoBehaviour
    {
        [SerializeField, Required] private Canvas _canvas;

        [SerializeField, Required] private UnitUpgradeView[] _unitItems;
        [SerializeField, Required] private UpgradeItemView _foodProduction, _baseHealth;
        [SerializeField, Required] private QuickBoostInfoPanel _quickBoostInfoPanel;

        [SerializeField, Required] private TutorialStep _foodStep;

        public TutorialStep FoodStep => _foodStep;
        public QuickBoostInfoPanel QuickBoostInfoPanel => _quickBoostInfoPanel;
        
        public UpgradeItemView GetUpgradeItemView(UpgradeItemType type)
        {
            switch (type)
            {
                case UpgradeItemType.FoodProduction:
                    return _foodProduction;
                case UpgradeItemType.BaseHealth:
                    return _baseHealth;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public UnitUpgradeView GetUnitUpgrade(UnitType unitType) => 
            _unitItems.FirstOrDefault(x => x.Type == unitType);
        
        private IMiniShopProvider _miniShopProvider;
        private IUpgradesScreenPresenter _presenter;


        public void Construct(
            IWorldCameraService cameraService,
            IUpgradesScreenPresenter upgradesScreenPresenter)
        {
            _canvas.enabled = false;
            _canvas.worldCamera = cameraService.UICameraOverlay;
            _presenter = upgradesScreenPresenter;
            _presenter.Screen = this;
        }

        public void Show()
        {
            _canvas.enabled = true;
            _presenter.OnUpgradesScreenOpened();
        }
        
        public void Hide()
        {
            _canvas.enabled = false;
            _presenter.OnUpgradesScreenClosed();
        }

        public void Dispose() => 
            _presenter.OnUpgradesScreenDisposed();

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
            _presenter.OnScreenActiveChanged(isActive);
        }
    }
}
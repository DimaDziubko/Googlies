using System.Collections.Generic;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Core.UserState._Handler._Upgrade
{
    public class UpgradeStateHandler : IUpgradeStateHandler
    {
        private readonly IUserContainer _userContainer;

        public UpgradeStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public void UpgradeItem(UpgradeItemType type)
        {
            _userContainer.State.TimelineState.UpgradeItemLevelUp(type);
            _userContainer.RequestSaveGame(true);
        }

        public void ChangeCardSummoningLevel(int newLevel)
        {
            _userContainer.State.CardsCollectionState.ChangeCardSummoningLevel(newLevel);
            _userContainer.RequestSaveGame();
        }
        
        public void OpenUnit(UnitType type)
        {
            _userContainer.State.TimelineState.OpenUnit(type);
            _userContainer.RequestSaveGame();
        }

        public void AddSkills(List<int> skillIds)
        {
            _userContainer.State.SkillCollectionState.AddSkills(skillIds);
            _userContainer.RequestSaveGame();
        }

        public void AddCard(Card newCard)
        {
            _userContainer.State.CardsCollectionState.AddCard(newCard);
            _userContainer.RequestSaveGame();
        }

        public void RemoveSkills()
        {
            _userContainer.State.SkillCollectionState.RemoveSkills();
            _userContainer.RequestSaveGame();
        }
        
        public void AddSummoningProgressCount(int i)
        {
            _userContainer.State.CardsCollectionState.ChangeCardSummoningProgressCount(i);
            _userContainer.RequestSaveGame();
        }
    }
}
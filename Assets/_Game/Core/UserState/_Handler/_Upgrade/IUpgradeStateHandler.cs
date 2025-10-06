using System.Collections.Generic;
using _Game.Core.UserState._State;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Core.UserState._Handler._Upgrade
{
    public interface IUpgradeStateHandler
    {
        void UpgradeItem(UpgradeItemType type);
        void ChangeCardSummoningLevel(int newLevel);
        void OpenUnit(UnitType modelType);
        void AddSkills(List<int> skillIds);
        void RemoveSkills();
        void AddCard(Card newCard);
        void AddSummoningProgressCount(int i);
    }
}
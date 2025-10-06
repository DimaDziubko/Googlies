using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Core.UserState._State
{
    public class UpgradeItem
    {
        public UpgradeItemType Type;
        public int Level;

        public void Reset()
        {
            Level = 0;
        }
    }
}
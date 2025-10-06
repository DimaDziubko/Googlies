using _Game.LiveopsCore._Enums;

namespace _Game.LiveopsCore
{
    public class GameEventShowcaseSavegamne
    {
        public ShowcaseCondition ShowcaseCondition;
        public int ShowOrder;
        public bool IsShown;

        public void SetShown(bool isShown)
        {
            IsShown = isShown;
        }
    }
}
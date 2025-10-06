using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class IceDebuff :IDebuff
    {
        public DebuffType DebuffType => DebuffType.Ice;
        
        private UnitBase _unit;
        private readonly IceDebuffView _view;

        public IceDebuff(IceDebuffView view)
        {
            _view = view;
        }

        public void Assign(UnitBase unitBase)
        {
            _unit = unitBase;
            unitBase.SetFrozen(true);
        }

        public void Remove()
        {
            _view.Recycle();
            _unit.SetFrozen(false);
        }
    }
}
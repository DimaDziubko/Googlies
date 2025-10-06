using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class ShapeShifterAggro : UnitAggro 
    {
        [SerializeField, Required] private ShapeShifter _shapeShifter;
        public override void OnAggro()
        {
            _shapeShifter.ShiftShape(false);
        }
    }
}
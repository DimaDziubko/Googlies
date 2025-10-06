using _Game.Gameplay._Units.Scripts;

namespace _Game.Utils.Extensions
{
    public static class TargetExtensions
    {
        public static bool IsValid(this ITarget target) =>
            target != null && 
            target.IsActive && 
            target.IsDead() == false;
    }
}
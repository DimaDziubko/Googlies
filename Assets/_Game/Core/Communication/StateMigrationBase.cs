using _Game.Core.UserState._State;
using Assets._Game.Core.Communication;

namespace _Game.Core.Communication
{
    public abstract class StateMigrationBase : IStateMigration
    {
        public abstract string TargetVersion { get; }

        public abstract void Migrate(ref UserAccountState state);
    }
}
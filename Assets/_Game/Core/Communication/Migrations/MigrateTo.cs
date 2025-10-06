using _Game.Core.Communication;
using _Game.Core.UserState._State;

public class MigrateTo : StateMigrationBase
{
    public override string TargetVersion => "";

    public override void Migrate(ref UserAccountState state)
    {
        
    }
}

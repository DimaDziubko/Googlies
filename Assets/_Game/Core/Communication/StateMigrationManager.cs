using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.UserState._State;
using Assets._Game.Core.Communication;

namespace _Game.Core.Communication
{
    public class StateMigrationManager
    {
        private readonly List<IStateMigration> _migrations = new List<IStateMigration>();

        public StateMigrationManager()
        {
            //_migrations.Add(new MigrationTo140());
            //_migrations.Add(new MigrationTo170());
        }

        public void Migrate(ref UserAccountState state)
        {
            var orderedMigrations = _migrations.OrderBy(m => m.TargetVersion,
                Comparer<string>.Create(CompareVersions)).ToList();

            foreach (var migration in orderedMigrations)
            {
                if (CompareVersions(state.Version, migration.TargetVersion) < 0)
                {
                    migration.Migrate(ref state);
                    state.Version = migration.TargetVersion;
                }
            }
        }

        private int CompareVersions(string version1, string version2)
        {
            var v1 = version1.Split('.').Select(int.Parse).ToArray();
            var v2 = version2.Split('.').Select(int.Parse).ToArray();

            for (int i = 0; i < Math.Min(v1.Length, v2.Length); i++)
            {
                int result = v1[i].CompareTo(v2[i]);
                if (result != 0)
                {
                    return result;
                }
            }
            return v1.Length.CompareTo(v2.Length);
        }
    }
}
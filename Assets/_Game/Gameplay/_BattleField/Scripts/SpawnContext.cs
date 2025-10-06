using System;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public enum GameMode
    {
        Battle,
        ZombieRush
    }

    public struct SpawnContext : IComparable
    {
        public Faction Faction { get; }
        public GameMode GameMode { get; }

        public SpawnContext(
            Faction faction,
            GameMode gameMode)
        {
            Faction = faction;
            GameMode = gameMode;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SpawnContext other)) return false;
            return Faction == other.Faction && GameMode == other.GameMode;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Faction, GameMode);
        }

        public override string ToString()
        {
            return $"{Faction}_{GameMode}";
        }

        public int CompareTo(object obj)
        {
            if (obj is not SpawnContext other)
                throw new ArgumentException($"Object must be of type {nameof(SpawnContext)}");

            int factionComparison = Faction.CompareTo(other.Faction);
            if (factionComparison != 0)
                return factionComparison;

            return GameMode.CompareTo(other.GameMode);
        }
    }
}
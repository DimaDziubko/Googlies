namespace _Game.Core.UserState._State
{
    public interface ISettingsSaveGameReadonly
    {
        public bool IsDamageTextOn { get; }
        public bool IsAmbienceOn { get; }
        public bool IsSfxOn { get; }
    }
    public class SettingsSaveGame : ISettingsSaveGameReadonly
    {
        public bool IsDamageTextOn;
        public bool IsAmbienceOn;
        public bool IsSfxOn;

        bool ISettingsSaveGameReadonly.IsDamageTextOn => IsDamageTextOn;
        bool ISettingsSaveGameReadonly.IsAmbienceOn => IsAmbienceOn;
        bool ISettingsSaveGameReadonly.IsSfxOn => IsSfxOn;

        public void SetDamageTextActive(bool isActive) => IsDamageTextOn = isActive;

        public void SetSfxActive(bool isActive) => IsSfxOn = isActive;

        public void SetAmbienceActive(bool isActive) => IsAmbienceOn = isActive;
    }
}
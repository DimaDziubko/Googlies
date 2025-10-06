namespace _Game.LiveopsCore._GameEventCurrencyManagement
{
    public interface IUnitDropChanceCalculator
    {
        bool ShouldDrop(UnitLootDropSettingsWithDropChance settings);
    }
}
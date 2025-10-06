namespace _Game.Gameplay._PickUp._PickUpFactory
{
    public interface IPickUpFactory
    {
        PickUp GetPickUp();
        void Reclaim(PowerUpType type, PickUpBase pickUpBase);
    }
}
    
    
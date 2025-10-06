using UnityEngine;

namespace _Game.Gameplay.Common
{
    public abstract class GameBehaviour : MonoBehaviour
    {
        public virtual bool GameUpdate(float deltaTime) => true;
        public virtual bool LateGameUpdate(float deltaTime) => true;
        public abstract void Recycle();
        public virtual void SetPaused(in bool isPaused) {}
        public virtual void SetSpeedFactor(float speedFactor) { }
        public virtual void Reset() { }
    }
}
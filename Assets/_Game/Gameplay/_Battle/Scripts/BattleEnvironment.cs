using _Game.UI._Environment.Factory;
using UnityEngine;

namespace _Game.Gameplay._Battle.Scripts
{
    public class BattleEnvironment : MonoBehaviour
    {
        public void Show() => gameObject.SetActive(true);

        public void Hide() => gameObject.SetActive(false);
        public IEnvironmentFactory OriginFactory { get; set; }

        public void Recycle()
        {
            OriginFactory.Reclaim(this);
        }
    }
}
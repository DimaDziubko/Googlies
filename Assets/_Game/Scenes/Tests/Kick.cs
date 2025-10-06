using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class Kick : MonoBehaviour
    {
        [Button]
        public void Delete()
        {
            Destroy(this.gameObject, 3);
        }
    }
}

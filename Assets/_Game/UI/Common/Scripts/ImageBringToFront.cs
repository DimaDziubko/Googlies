using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Image))]
    internal class ImageBringToFront : MonoBehaviour
    {
        [SerializeField] private int renderQueueBoost = 100;

        private Image image;
        private Material imageMaterial;

        void Awake()
        {
            image = GetComponent<Image>();
            CreateMaterialAndBringToFront();
        }

        private void CreateMaterialAndBringToFront()
        {
            if (image == null)
            {
                Debug.LogError($"[{gameObject.name}] Image component not found!");
                return;
            }

            // Створюємо новий матеріал
            imageMaterial = new Material(image.material);
            image.material = imageMaterial;

            // Підвищуємо renderQueue (3000 = default UI, більше = малюється поверх)
            imageMaterial.renderQueue += renderQueueBoost;

            Debug.Log($"[{gameObject.name}] Material created. RenderQueue: {imageMaterial.renderQueue}");
        }

        void OnDestroy()
        {
            if (imageMaterial != null)
            {
                Destroy(imageMaterial);
            }
        }
    }
}
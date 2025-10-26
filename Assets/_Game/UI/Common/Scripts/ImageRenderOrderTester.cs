using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace _Game.UI.Common.Scripts
{
    internal class ImageRenderOrderTester : MonoBehaviour
    {
        [Title("Поточний стан")]
        [ReadOnly, ShowInInspector] private int currentSortingOrder;
        [ReadOnly, ShowInInspector] private int currentSiblingIndex;
        [ReadOnly, ShowInInspector] private int currentRenderQueue;

        [Title("Налаштування")]
        [SerializeField] private int sortingOrderStep = 10;
        [SerializeField] private int renderQueueStep = 100;

        private Canvas canvas;
        private Image image;
        private Material imageMaterial;

        void Start()
        {
            image = GetComponent<Image>();
            UpdateInfo();
        }

        void Update()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            currentSiblingIndex = transform.GetSiblingIndex();

            if (canvas != null)
                currentSortingOrder = canvas.sortingOrder;

            if (imageMaterial != null)
                currentRenderQueue = imageMaterial.renderQueue;
        }

        // ========== ВАРІАНТ 1: SetSiblingIndex (реальна зміна ієрархії) ==========

        [Button("▲ Sibling вгору", ButtonSizes.Large), ButtonGroup("Sibling")]
        private void SiblingUp()
        {
            int current = transform.GetSiblingIndex();
            transform.SetSiblingIndex(current + 1);
            Debug.Log($"[{gameObject.name}] SetSiblingIndex: {current} -> {current + 1}");
        }

        [Button("▼ Sibling вниз", ButtonSizes.Large), ButtonGroup("Sibling")]
        private void SiblingDown()
        {
            int current = transform.GetSiblingIndex();
            if (current > 0)
                transform.SetSiblingIndex(current - 1);
            Debug.Log($"[{gameObject.name}] SetSiblingIndex: {current} -> {current - 1}");
        }

        [Button("⬆ На верх", ButtonSizes.Medium), ButtonGroup("Sibling")]
        private void SiblingToTop()
        {
            transform.SetAsLastSibling();
            Debug.Log($"[{gameObject.name}] SetAsLastSibling");
        }

        [Button("⬇ На низ", ButtonSizes.Medium), ButtonGroup("Sibling")]
        private void SiblingToBottom()
        {
            transform.SetAsFirstSibling();
            Debug.Log($"[{gameObject.name}] SetAsFirstSibling");
        }

        // ========== ВАРІАНТ 2: Canvas.overrideSorting (не чіпає ієрархію) ==========

        [Button("⚙ Додати Canvas", ButtonSizes.Large), ButtonGroup("Canvas")]
        [ShowIf("@canvas == null")]
        private void AddCanvas()
        {
            canvas = gameObject.GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = 0;

            // Додаємо GraphicRaycaster якщо треба
            if (GetComponent<GraphicRaycaster>() == null)
                gameObject.AddComponent<GraphicRaycaster>();

            Debug.Log($"[{gameObject.name}] Canvas added with overrideSorting = true");
        }

        [Button("▲ Sorting Order +", ButtonSizes.Large), ButtonGroup("Canvas")]
        [ShowIf("@canvas != null")]
        private void IncreaseSortingOrder()
        {
            if (canvas == null) return;
            canvas.sortingOrder += sortingOrderStep;
            Debug.Log($"[{gameObject.name}] Sorting Order: {canvas.sortingOrder}");
        }

        [Button("▼ Sorting Order -", ButtonSizes.Large), ButtonGroup("Canvas")]
        [ShowIf("@canvas != null")]
        private void DecreaseSortingOrder()
        {
            if (canvas == null) return;
            canvas.sortingOrder -= sortingOrderStep;
            Debug.Log($"[{gameObject.name}] Sorting Order: {canvas.sortingOrder}");
        }

        [Button("🔄 Reset Sorting Order", ButtonSizes.Medium), ButtonGroup("Canvas")]
        [ShowIf("@canvas != null")]
        private void ResetSortingOrder()
        {
            if (canvas == null) return;
            canvas.sortingOrder = 0;
            Debug.Log($"[{gameObject.name}] Sorting Order reset to 0");
        }

        [Button("🗑 Видалити Canvas", ButtonSizes.Medium), ButtonGroup("Canvas")]
        [ShowIf("@canvas != null")]
        private void RemoveCanvas()
        {
            if (canvas != null)
            {
                DestroyImmediate(canvas);
                canvas = null;
            }
            var raycaster = GetComponent<GraphicRaycaster>();
            if (raycaster != null)
                DestroyImmediate(raycaster);

            Debug.Log($"[{gameObject.name}] Canvas removed");
        }

        // ========== ВАРІАНТ 3: Material.renderQueue (через Material) ==========

        [Button("🎨 Створити Material", ButtonSizes.Large), ButtonGroup("Material")]
        [ShowIf("@imageMaterial == null")]
        private void CreateMaterial()
        {
            if (image == null)
                image = GetComponent<Image>();

            if (image == null)
            {
                Debug.LogError("Image component not found!");
                return;
            }

            imageMaterial = new Material(image.material);
            image.material = imageMaterial;
            Debug.Log($"[{gameObject.name}] Material created. RenderQueue: {imageMaterial.renderQueue}");
        }

        [Button("▲ RenderQueue +", ButtonSizes.Large), ButtonGroup("Material")]
        [ShowIf("@imageMaterial != null")]
        private void IncreaseRenderQueue()
        {
            if (imageMaterial == null) return;
            imageMaterial.renderQueue += renderQueueStep;
            Debug.Log($"[{gameObject.name}] RenderQueue: {imageMaterial.renderQueue}");
        }

        [Button("▼ RenderQueue -", ButtonSizes.Large), ButtonGroup("Material")]
        [ShowIf("@imageMaterial != null")]
        private void DecreaseRenderQueue()
        {
            if (imageMaterial == null) return;
            imageMaterial.renderQueue -= renderQueueStep;
            Debug.Log($"[{gameObject.name}] RenderQueue: {imageMaterial.renderQueue}");
        }

        [Button("🔄 Reset RenderQueue (3000)", ButtonSizes.Medium), ButtonGroup("Material")]
        [ShowIf("@imageMaterial != null")]
        private void ResetRenderQueue()
        {
            if (imageMaterial == null) return;
            imageMaterial.renderQueue = 3000; // Default UI queue
            Debug.Log($"[{gameObject.name}] RenderQueue reset to 3000");
        }

        [Button("🗑 Видалити Material", ButtonSizes.Medium), ButtonGroup("Material")]
        [ShowIf("@imageMaterial != null")]
        private void RemoveMaterial()
        {
            if (image != null && imageMaterial != null)
            {
                image.material = null;
                Destroy(imageMaterial);
                imageMaterial = null;
            }
            Debug.Log($"[{gameObject.name}] Material removed");
        }

        // ========== ДОПОМІЖНІ МЕТОДИ ==========

        [Button("📊 Показати всю інфу", ButtonSizes.Large), GUIColor(0.3f, 0.8f, 1f)]
        private void ShowAllInfo()
        {
            Debug.Log($"===== {gameObject.name} =====");
            Debug.Log($"Sibling Index: {transform.GetSiblingIndex()}");
            Debug.Log($"Canvas: {(canvas != null ? $"YES (sortingOrder: {canvas.sortingOrder})" : "NO")}");
            Debug.Log($"Material: {(imageMaterial != null ? $"YES (renderQueue: {imageMaterial.renderQueue})" : "NO")}");
            Debug.Log($"====================");
        }
    }
}
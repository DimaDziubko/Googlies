using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts._DecorAndUtils
{
    public class DynamicGridLayout : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _parentRectTransform;
        [SerializeField, Required] private GridLayoutGroup _gridLayoutGroup;
        [SerializeField] private int _numberOfColumns = 3;
        [SerializeField] private float _fixedSpacing = 30f;
        [SerializeField] private float _aspectRatio = 4f / 5f;
        
        public void SetColumns(int columns)
        {
            _numberOfColumns = columns;
        }

        public void SetSpacing(int spacing)
        {
            _fixedSpacing = spacing;
        }
        
        [Button]
        public void AdjustSpacing()
        {
            float parentWidth = _parentRectTransform.rect.width;
            float cellWidth = _gridLayoutGroup.cellSize.x;
            float totalCellWidth = cellWidth * _numberOfColumns;

            float availableSpace = parentWidth - totalCellWidth;
            float spacing = availableSpace / (_numberOfColumns - 1);

            _gridLayoutGroup.spacing = new Vector2(spacing, spacing);
        }
        
        [Button]
        public void AdjustCellSize()
        {
            float parentWidth = _parentRectTransform.rect.width;
            float totalSpacing = _fixedSpacing * (_numberOfColumns - 1);
            float availableWidth = parentWidth - totalSpacing;
            float cellWidth = availableWidth / _numberOfColumns;
            float cellHeight = cellWidth / (_aspectRatio);

            _gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
            _gridLayoutGroup.spacing = new Vector2(_fixedSpacing, _fixedSpacing);
        }

        public void AdjustViewport(int cardsCount)
        {
            int rows = Mathf.CeilToInt((float)cardsCount / _numberOfColumns);
            float cellHeight = _gridLayoutGroup.cellSize.y;
            float spacingHeight = _gridLayoutGroup.spacing.y;
            
            float totalHeight = (rows * cellHeight) + (rows * spacingHeight);
            
            _parentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
        }
    }
}

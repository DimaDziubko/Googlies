using UnityEngine;
using UnityUtils;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public class TutorialStepData
    {
        public int Step;
        public int[] AffectedSteps;
        public int[] ExclusiveSteps;
        public Vector2 RequiredPointerSize;
        public bool NeedAppearanceAnimation;
        public bool IsUnderneath;
        public RectTransform TutorialObjectRectTransform;

        public Vector3 CalculateRequiredPointerPosition(RectTransform rectTransform)
        {
            if (TutorialObjectRectTransform.OrNull() == null)
                return Vector3.zero;
            
            Vector3 worldPosition = TutorialObjectRectTransform.TransformPoint(TutorialObjectRectTransform.rect.center);
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, worldPosition, null, out var canvasPosition);

            Vector3 requiredPointerPosition = new Vector3(
                canvasPosition.x,
                canvasPosition.y,
                0);

            return requiredPointerPosition;
        }

        public Quaternion CalculateRequiredRotation()
        {
            Quaternion requiredPointerRotation = IsUnderneath ? Quaternion.Euler(0, 0, 180) : Quaternion.Euler(0, 0, 0);
            return requiredPointerRotation;
        }
    }
}
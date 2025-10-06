using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace Assets._Game.Utils.Bezier
{
    [ExecuteAlways]
public class BezierPoint : MonoBehaviour
{
    [SerializeField] private Transform _middle;
    [SerializeField] private Transform _firstHandler;
    [SerializeField] private Transform _secondHandler;

    private Vector3 _previousFirstHandlerPosition;
    private Vector3 _previousSecondHandlerPosition;

    public Vector3 MiddlePosition
    {
        get => _middle.transform.position;
    }

    public Vector3 FirstHandlerPosition
    {
        get => _firstHandler.transform.position;
        set
        {
            _firstHandler.transform.position = value;
            _previousFirstHandlerPosition = value;
            
#if UNITY_EDITOR
            // Оновлення в інспекторі при руханні
            EditorUtility.SetDirty(_firstHandler);
#endif
        }
    }

    public Vector3 SecondHandlerPosition
    {
        get => _secondHandler.transform.position;
        set
        {
            _secondHandler.transform.position = value;
            _previousSecondHandlerPosition = value;

#if UNITY_EDITOR
            EditorUtility.SetDirty(_secondHandler);
#endif
        }
    }

    private void MirrorHandler(Transform handler, Transform mirrorHandler)
    {
        Vector3 mirroredPosition = MiddlePosition + (MiddlePosition - handler.position);
        mirrorHandler.position = mirroredPosition;

#if UNITY_EDITOR
        EditorUtility.SetDirty(mirrorHandler);
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        // Перевірка, чи гра не відтворюється і не відбувається зміна режиму відтворення
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            // Перевірка, чи відбулася зміна позиції хендлера
            if (_previousFirstHandlerPosition != FirstHandlerPosition)
            {
                // Якщо так, то дзеркально оновлюємо другий хендлер
                MirrorHandler(_firstHandler, _secondHandler);
                _previousFirstHandlerPosition = FirstHandlerPosition;
            }
            if (_previousSecondHandlerPosition != SecondHandlerPosition)
            {
                // Якщо так, то дзеркально оновлюємо другий хендлер
                MirrorHandler(_secondHandler, _firstHandler);
                _previousSecondHandlerPosition = SecondHandlerPosition;
            }
        }
#endif
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawGizmoSphere(MiddlePosition, Color.yellow);
        DrawGizmoSphere(FirstHandlerPosition, Color.yellow);
        DrawGizmoSphere(SecondHandlerPosition, Color.yellow);
        
        Gizmos.color = Color.black;
        Gizmos.DrawLine(FirstHandlerPosition, SecondHandlerPosition);
    }

    private void DrawGizmoSphere(Vector3 position, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(position, 0.1f); // 0.1f - радіус, отже, діаметр буде 0.2f
    }
#endif
}
}
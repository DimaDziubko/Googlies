using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class LockInspector {
    static readonly MethodInfo flipLocked;
    static readonly PropertyInfo constrainProportions;
    const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

    static LockInspector() {
        // Cache static MethodInfo and PropertyInfo for performance
#if UNITY_2023_2_OR_NEWER
        var editorLockTrackerType = typeof(EditorGUIUtility).Assembly.GetType("UnityEditor.EditorGUIUtility+EditorLockTracker");
        flipLocked = editorLockTrackerType.GetMethod("FlipLocked", bindingFlags);
#endif
        constrainProportions = typeof(Transform).GetProperty("constrainProportionsScale", bindingFlags);
    }

    [MenuItem("Edit/Toggle Inspector Lock %l")]
    public static void Lock() {
#if UNITY_2023_2_OR_NEWER
    var inspectorWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");

    foreach (var inspectorWindow in Resources.FindObjectsOfTypeAll(inspectorWindowType)) {
        var lockTracker = inspectorWindowType.GetField("m_LockTracker", bindingFlags)
            ?.GetValue(inspectorWindow);
        flipLocked?.Invoke(lockTracker, new object[] { });
    }
#else
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
#endif
        foreach (var activeEditor in ActiveEditorTracker.sharedTracker.activeEditors) {
            var target = activeEditor.target as Transform;
            if (target == null) continue;

            var currentValue = (bool) constrainProportions.GetValue(target, null);
            constrainProportions.SetValue(target, !currentValue, null);
        }

        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
}

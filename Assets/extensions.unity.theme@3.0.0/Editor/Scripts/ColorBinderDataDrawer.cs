using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Unity.Theme.Binders;
using UnityEditor;
using UnityEngine;

namespace Unity.Theme.Editor
{
    # region Without Odin
    // [CustomPropertyDrawer(typeof(Binders.ColorBinderData), true)]
    // public class ColorBinderDataDrawer : PropertyDrawer
    // {
    //     const string templateGuid = "7bc7f57ecc1dcb54ebd343051d02f17b";
    //
    //     public override VisualElement CreatePropertyGUI(SerializedProperty property)
    //     {
    //         TemplateContainer root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(templateGuid)).Instantiate();
    //         Toggle toggleOverrideAlpha = root.Query<Toggle>("toggleOverrideAlpha").First();
    //         DropdownField dropdownColor = root.Query<DropdownField>("dropdownColor").First();
    //         Button btnOpenConfig = root.Query<Button>("btnOpenConfig").First();
    //         Slider sliderAlpha = root.Query<Slider>("sliderAlpha").First();
    //         VisualElement colorFill = root.Query<VisualElement>("colorFill").Last();
    //
    //         SerializedProperty colorGuid = property.FindPropertyRelative("colorGuid");
    //         SerializedProperty overrideAlpha = property.FindPropertyRelative("overrideAlpha");
    //         SerializedProperty alpha = property.FindPropertyRelative("alpha");
    //
    //         dropdownColor.choices = Theme.Instance?.ColorNames?.ToList() ?? new List<string>() { "error" };
    //         dropdownColor.value = Theme.Instance?.GetColorName(colorGuid.stringValue);
    //         toggleOverrideAlpha.value = overrideAlpha.boolValue;
    //         sliderAlpha.visible = overrideAlpha.boolValue;
    //         sliderAlpha.value = alpha.floatValue;
    //
    //         UpdateColorFill(colorFill, colorGuid.stringValue, overrideAlpha.boolValue ? alpha.floatValue : 1f);
    //
    //         dropdownColor.SubscribeOnValueChanged(root, evt =>
    //         {
    //             var guid = Theme.Instance?.GetColorByName(evt.newValue)?.Guid;
    //             colorGuid.stringValue = guid;
    //             UpdateColorFill(colorFill, colorGuid.stringValue, overrideAlpha.boolValue ? alpha.floatValue : 1f);
    //             colorGuid.serializedObject.ApplyModifiedProperties();
    //         });
    //
    //         toggleOverrideAlpha.SubscribeOnValueChanged(root, evt =>
    //         {
    //             overrideAlpha.boolValue = evt.newValue;
    //             sliderAlpha.visible = evt.newValue;
    //             UpdateColorFill(colorFill, colorGuid.stringValue, overrideAlpha.boolValue ? alpha.floatValue : 1f);
    //             overrideAlpha.serializedObject.ApplyModifiedProperties();
    //         });
    //
    //         sliderAlpha.SubscribeOnValueChanged(root, evt =>
    //         {
    //             alpha.floatValue = evt.newValue;
    //             UpdateColorFill(colorFill, colorGuid.stringValue, overrideAlpha.boolValue ? alpha.floatValue : 1f);
    //             alpha.serializedObject.ApplyModifiedProperties();
    //         });
    //
    //         Action onOpenConfigClicked = () => ThemeWindowEditor.ShowWindow();
    //         btnOpenConfig.clicked += onOpenConfigClicked;
    //         root.RegisterCallback<DetachFromPanelEvent>(evt => btnOpenConfig.clicked -= onOpenConfigClicked);
    //
    //         colorFill.BringToFront();
    //
    //         return root;
    //     }
    //     void UpdateColorFill(VisualElement colorFill, string colorGuid, float alpha)
    //     {
    //         Color color = Theme.Instance?.GetColorByGuid(colorGuid)?.Color ?? Theme.DefaultColor;
    //         color.a = alpha;
    //         colorFill.style.unityBackgroundImageTintColor = new StyleColor(color);
    //     }
    // }
    
    #endregion
    public class ColorBinderDataDrawer : OdinValueDrawer<ColorBinderData>
    {
         protected override void DrawPropertyLayout(GUIContent label)
        {
            ColorBinderData value = this.ValueEntry.SmartValue;
            
            SirenixEditorGUI.Title("Color Binding", "", TextAlignment.Left, true);
            
            if (Theme.Instance != null)
            {
                List<string> colorNames = Theme.Instance.ColorNames.ToList();
                
                List<string> colorGuids = Theme.Instance.ColorGuids.ToList();
                
                int selectedIndex = colorGuids.IndexOf(value.colorGuid);
                
                if (selectedIndex == -1)
                {
                    selectedIndex = 0;
                    value.colorGuid = colorGuids[selectedIndex];
                }
                
                EditorGUI.BeginChangeCheck();
                selectedIndex = SirenixEditorFields.Dropdown("Color", selectedIndex, colorNames.ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    value.colorGuid = colorGuids[selectedIndex];
                    value.OnColorChanged();
                }
            }

            EditorGUI.BeginChangeCheck();
            value.overrideAlpha = EditorGUILayout.Toggle("Override Alpha", value.overrideAlpha);
            if (EditorGUI.EndChangeCheck())
            {
                value.OnAlphaChanged();
            }

            if (value.overrideAlpha)
            {
                EditorGUI.BeginChangeCheck();
                value.alpha = EditorGUILayout.Slider("Alpha", value.alpha, 0f, 1f);
                if (EditorGUI.EndChangeCheck())
                {
                    value.OnAlphaChanged();
                }
            }
            
            if (Theme.Instance != null)
            {
                Color color = Theme.Instance.GetColorByGuid(value.colorGuid)?.Color ?? Theme.DefaultColor;
                color.a = value.overrideAlpha ? value.alpha : 1f;
                
                SirenixEditorGUI.DrawSolidRect(EditorGUILayout.GetControlRect(false, 20), color);
            }
            
            this.ValueEntry.SmartValue = value;
        }
    }
}
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Unity.Theme.Binders
{
    #region WithoutOdin

    // [Serializable]
    // public class ColorBinderData
    // {
    //     [SerializeField, HideInInspector] public string colorGuid;
    //     [SerializeField, HideInInspector] public bool overrideAlpha;
    //     [SerializeField, HideInInspector] public float alpha = 1f;
    //
    //     public bool IsConnected => Theme.Instance?.GetColorByGuid(colorGuid) != null;
    //     public string ColorName => Theme.Instance?.GetColorName(colorGuid);
    //     public void ResetColor() => colorGuid = null;
    // } 

    #endregion
    
    [Serializable]
    public class ColorBinderData
    {
        public event Action ColorChanged;
        
        [ValueDropdown("GetColorOptions"), OnValueChanged("OnColorChanged")]
        public string colorGuid;

        [LabelText("Override Alpha"), OnValueChanged("OnAlphaChanged")]
        public bool overrideAlpha;

        [ShowIf("overrideAlpha"), OnValueChanged("OnAlphaChanged")]
        [Range(0f, 1f)]
        public float alpha = 1f;

        [ShowInInspector, ReadOnly, LabelText("Color Name")]
        public string ColorName => Theme.Instance?.GetColorName(colorGuid);

        public bool IsConnected => Theme.Instance?.GetColorByGuid(colorGuid) != null;
        
        public void OnColorChanged()
        {
            UpdateColor();
            ColorChanged?.Invoke();
        }
        
        public void OnAlphaChanged()
        {
            UpdateColor();
            ColorChanged?.Invoke();
        }

        private void UpdateColor()
        {
            Color color = Theme.Instance?.GetColorByGuid(colorGuid)?.Color ?? Theme.DefaultColor;
            color.a = overrideAlpha ? alpha : 1f;
        }
        
        private IEnumerable<string> GetColorOptions()
        {
            return Theme.Instance?.ColorNames ?? new List<string> { "None" };
        }

        public void ResetColor()
        {
            colorGuid = null;
        }
    }
}
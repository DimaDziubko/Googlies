using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Unity.Theme.Binders
{
    [AddComponentMenu("Theme/Button State Color Binder")]
    public class ButtonColorBinder : BaseColorBinder
    {
        [SerializeField] private Image _image;
        [SerializeField] private ThemedButton _button;
        
        [SerializeField] private ColorBinderData _nonInteractableColorData = new ColorBinderData();

        protected override IEnumerable<Object> ColorTargets { get { yield return _image; } }

        protected override void Awake()
        {
            if (_image == null) _image = GetComponent<Image>();
            if (_button == null) _button = GetComponent<ThemedButton>();
            
            base.Awake();
            
            _nonInteractableColorData = _nonInteractableColorData ?? new ColorBinderData();
            if (string.IsNullOrEmpty(_nonInteractableColorData.colorGuid))
                _nonInteractableColorData.colorGuid = Theme.Instance?.GetColorFirst().Guid;

            if (!_nonInteractableColorData.IsConnected)
            {
                if (Theme.Instance?.debugLevel <= DebugLevel.Error)
                    Debug.LogError($"Non-interactable color with GUID='{_nonInteractableColorData.colorGuid}' not found in database at <b>{GameObjectPath()}</b>", gameObject);
            }
        }

        protected override void TrySetColor(ThemeData theme)
        {
            if (theme == null)
            {
                if (Theme.Instance?.debugLevel <= DebugLevel.Error)
                    Debug.LogError($"Current theme is null at <b>{GameObjectPath()}</b>", gameObject);
                return;
            }
            
            ColorBinderData activeColorData = _button.interactable ? data : _nonInteractableColorData;
            
            var colorData = theme.GetColorByGuid(activeColorData.colorGuid);
            if (colorData == null)
            {
                if (Theme.Instance?.debugLevel <= DebugLevel.Error)
                    Debug.LogError($"Color with GUID='{activeColorData.colorGuid}' not found in database at <b>{GameObjectPath()}</b>", gameObject);
            }
            else
            {
                var targetColor = GetTargetColor(colorData);
                var currentColor = GetColor();
                if (targetColor == currentColor)
                    return; // skip if color is the same

                if (Theme.Instance?.debugLevel <= DebugLevel.Log)
                    Debug.Log($"SetColor: '<b>{activeColorData.ColorName}</b>' {targetColor.ToHexRGBA()} at <b>{GameObjectPath()}</b>", gameObject);
                SetColor(targetColor);
                SetDirty();
            }
        }
        
        protected override Color GetTargetColor(ColorData colorData)
        {
            ColorBinderData activeColorData = _button.interactable ? data : _nonInteractableColorData;
            
            var result = colorData.Color;
            
            if (activeColorData.overrideAlpha) 
                result.a = activeColorData.alpha;

            return result;
        }
        
        protected override void SetColor(Color color)
        {
            if (_image == null)
            {
                if (Theme.Instance?.debugLevel <= DebugLevel.Error)
                    Debug.LogError($"Image not found at <b>{GameObjectPath()}</b>", gameObject);
                return;
            }
            _image.color = color;
            
            Debug.Log("<color=green>Set color</color>");
        }

        protected override Color? GetColor()
        {
            return _image?.color;
        }

        private void  OnInteractableChanged(bool isInteractable)
        {
            if (isInteractable)
            {
                SetColor(GetTargetColor(Theme.Instance?.GetColorByGuid(data.colorGuid)));
            }
            else
            {
                SetColor(GetTargetColor(Theme.Instance?.GetColorByGuid(_nonInteractableColorData.colorGuid)));
            }
        }

        protected override void Enable()
        {
            base.Enable();
            
            _nonInteractableColorData.ColorChanged += OnColorChanged;
            
            if (_button != null)
            {
                _button.InteractionChanged += OnInteractableChanged;
                OnInteractableChanged(_button.interactable);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            _nonInteractableColorData.ColorChanged -= OnColorChanged;
            
            if (_button != null)
            {
                _button.InteractionChanged += OnInteractableChanged;
            }
        }

        protected override void OnColorChanged()
        {
            base.OnColorChanged();
            OnInteractableChanged(_button.interactable);
        }

        [Button]
        public void SetInteractable()
        {
            _button.SetInteractable(!_button.interactable);
        }
    }
}
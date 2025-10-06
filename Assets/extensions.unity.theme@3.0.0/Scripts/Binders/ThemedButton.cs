using System;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Unity.Theme.Binders
{
    public class ThemedButton : Button
    {
        public event Action<bool> InteractionChanged;
        
        [Button]
        public void SetInteractable(bool isInteractable)
        {
            if (interactable != isInteractable)
            {
                interactable = isInteractable;
                InteractionChanged?.Invoke(isInteractable);
            }
        }
    }
}
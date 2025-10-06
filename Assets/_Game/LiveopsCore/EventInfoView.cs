using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _Game.LiveopsCore
{
    public class EventInfoView : MonoBehaviour
    {
        [SerializeField, Required] private TMP_Text _infoLabel;

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void SetInfo(string infoText)
        {
            _infoLabel.text = infoText;
        }
    }
}
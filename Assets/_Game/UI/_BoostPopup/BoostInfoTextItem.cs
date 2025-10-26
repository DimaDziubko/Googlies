using TMPro;
using UnityEngine;

namespace _Game.UI._BoostPopup
{
    public class BoostInfoTextItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameLabel;

        public void SetName(string name) =>
            _nameLabel.text = name;
    }
}
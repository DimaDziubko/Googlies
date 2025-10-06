using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Hud
{
    public class WaveInfoPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _allWavesSprite;
        [SerializeField] private Sprite _finalWaveSprite;
        [SerializeField] private WaveInfoPopupAnimation _animation;

        public void ShowWave(int wave, int wavesCount)
        {
            if (wave < wavesCount)
            {
                _label.text = $"Wave {wave}";
                _image.sprite = _allWavesSprite;
            }
            else
            {
                _label.text = $"Final wave";
                _image.sprite = _finalWaveSprite;
            }

            _animation.PlayAnimation();
        }

        public void HideWave() => _animation.StopAnimation();
    }
}

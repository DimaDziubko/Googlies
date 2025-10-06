using System.Collections;
using System.Collections.Generic;
using _Game.UI._ParticleAttractorSystem;
using _Game.Utils.Extensions;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Currencies
{
    public class CurrencyView : MonoBehaviour
    {
        [SerializeField, Required] private RectTransform _iconTransform;
        [SerializeField, Required] private TMP_Text _moneyText;
        [SerializeField, Required] private RectTransform _moneyTextTransform;
        [SerializeField, Required] private Image _iconHolder;
        [SerializeField, Required] private AttractorWrapper _attractorWrapper;

        [SerializeField] private float animationDuration = 1.0f;
        [SerializeField] private float _delay = 1f;

        [SerializeField] private Color spendColor;
        [SerializeField] private Color earnColor;


        private Coroutine _animationCoroutine;

        private readonly List<Sequence> _animationSequences = new();

        public RectTransform IconTransform => _iconTransform;
        public Vector2 Position => _iconTransform.position;
        public AttractorWrapper Attractor => _attractorWrapper;
        
        public void SetActive(bool isActive) => gameObject.SetActive(isActive);

        public void SetIcon(Sprite icon)
        {
            _iconHolder.sprite = icon;
        }

        public void Cleanup()
        {
            foreach (var sequence in _animationSequences)
            {
                sequence.Kill();
            }
            
            _animationSequences.Clear();
        }

        public void SetupCurrency(string money)
        {
            StopAnimations();
            _moneyText.text = money;
        }

        public void ChangeCurrency(string money)
        {
            StopAnimations();
            _moneyText.text = money;
            BounceAnimation();
        }

        public void AddCurrency(double startMoney, double range, int threshold = 0)
        {
            StopAnimations();
            _animationCoroutine = StartCoroutine(DelayedAddMoneyAnimation(startMoney, range, threshold));
        }

        public void RemoveCurrency(string money)
        {
            StopAnimations();

            _moneyText.text = money;

            BounceAnimation();
            ColorAnimation(spendColor);
        }

        private void ColorAnimation(Color color, float interval = 0.5f)
        {
            Sequence sequence = DOTween.Sequence();
            sequence
                .AppendCallback(() => _animationSequences.Add(sequence))
                .Append(_moneyText.DOColor(color, 0.1f))
                .AppendInterval(interval)
                .Append(_moneyText.DOColor(Color.white, 0.3f))
                .OnComplete(() => _animationSequences.Remove(sequence));
        }

        private void BounceAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence
                .AppendCallback(() => _animationSequences.Add(sequence))
                .Append(_moneyTextTransform.DOScale(new Vector3(1.1f, 1.1f, 1.0f), 0.2f))
                .Append(_moneyTextTransform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.4f))
                .OnComplete(() => _animationSequences.Remove(sequence));
        }

        private void StopAnimations()
        {
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }

            foreach (var sequence in _animationSequences)
            {
                sequence.Kill();
            }

            _animationSequences.Clear();
        }

        private IEnumerator AddMoneyAnimation(double startMoney, double range, int threshold = 0)
        {
            float progress = 0;

            while (progress <= 1)
            {
                yield return null;
                progress = Mathf.Min(1, progress + Time.deltaTime / animationDuration);

                double currentMoney = startMoney + range * progress;
                _moneyText.text = currentMoney.ToCompactFormat(threshold);
            }

            _moneyText.text = (startMoney + range).ToCompactFormat();
        }

        private IEnumerator DelayedAddMoneyAnimation(double startMoney, double range, int threshold)
        {
            if (_delay > 0)
            {
                yield return new WaitForSeconds(_delay);
            }
            
            _animationCoroutine = StartCoroutine(AddMoneyAnimation(startMoney, range, threshold));
            BounceAnimation();
            ColorAnimation(earnColor, animationDuration - 0.3f);
        }
    }
}
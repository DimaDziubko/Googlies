using UnityEngine;
using UnityEngine.UIElements;

namespace _Game.Utils.MyUILibrary
{
    class VerticalProgressBar : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<VerticalProgressBar, UxmlTraits>
        {
        }


        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlFloatAttributeDescription lowValue = new() { name = "low-value", defaultValue = 0 };
            UxmlFloatAttributeDescription highValue = new() { name = "high-value", defaultValue = 100 };
            UxmlFloatAttributeDescription value = new() { name = "value", defaultValue = 0.5f };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var progressBar = ve as VerticalProgressBar;
                progressBar.LowValue = lowValue.GetValueFromBag(bag, cc);
                progressBar.HighValue = highValue.GetValueFromBag(bag, cc);
                progressBar.Value = value.GetValueFromBag(bag, cc);
            }
        }

        private float _lowValue;
        private float _highValue = 100f;
        private float _value;

        public float LowValue
        {
            get => _lowValue;
            set
            {
                _lowValue = value;
                UpdateProgress();
            }
        }

        public float HighValue
        {
            get => _highValue;
            set
            {
                _highValue = value;
                UpdateProgress();
            }
        }

        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                UpdateProgress();
            }
        }

        private readonly VisualElement m_Background;
        private readonly VisualElement m_Progress;

        public VerticalProgressBar()
        {
            this.AddToClassList("unity-vertical-progress-bar");

            var barContainer = new VisualElement();
            barContainer.AddToClassList("unity-vertical-progress-bar__container");

            m_Background = new VisualElement();
            m_Background.AddToClassList("unity-vertical-progress-bar__background");
            barContainer.Add(m_Background);

            m_Progress = new VisualElement();
            m_Progress.AddToClassList("unity-vertical-progress-bar__progress");
            m_Background.Add(m_Progress);

            Add(barContainer);

            style.width = 40f;
            style.height = 200f;
            
            this.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void UpdateProgress()
        {
            float progressHeight = CalculateProgressHeight(Value);
            m_Progress.style.height = progressHeight;
        }

        private float CalculateProgressHeight(float value)
        {
            float totalHeight = m_Background.resolvedStyle.height;
            return Mathf.Clamp(value / HighValue * totalHeight, 0f, totalHeight);
        }
        
        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            UpdateProgress();
        }
    }
}
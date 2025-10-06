using UnityEngine;
using UnityEngine.UIElements;

// namespace _Game.Utils.MyUILibrary
// {
//     public class VerticalProgressBarTest : BindableElement, INotifyValueChanged<float>
// {
//     // Стандартні імена класів для стилів
//     public static readonly string ussClassName = "unity-vertical-progress-bar";
//     public static readonly string containerUssClassName = ussClassName + "__container";
//     public static readonly string progressUssClassName = ussClassName + "__progress";
//     public static readonly string backgroundUssClassName = ussClassName + "__background";
//     
//     private readonly VisualElement m_Background;
//     private readonly VisualElement m_Progress;
//     private float m_LowValue;
//     private float m_HighValue = 100f;
//     private float m_Value;
//     private const float k_MinVisibleProgress = 1f;
//
//     // Властивість для встановлення та отримання прогресу
//     public float value
//     {
//         get => this.m_Value;
//         set
//         {
//             if (Mathf.Approximately(this.m_Value, value)) return;
//             SetValueWithoutNotify(value);
//             this.SendEvent(ChangeEvent<float>.GetPooled(this.m_Value, value));
//         }
//     }
//
//     // Підготовка і відображення прогрес-бару
//     public VerticalProgressBar()
//     {
//         this.AddToClassList(ussClassName);
//
//         VisualElement container = new VisualElement();
//         container.AddToClassList(containerUssClassName);
//
//         // Фон прогрес-бару
//         m_Background = new VisualElement();
//         m_Background.AddToClassList(backgroundUssClassName);
//         container.Add(m_Background);
//
//         // Заповнювальна частина
//         m_Progress = new VisualElement();
//         m_Progress.AddToClassList(progressUssClassName);
//         m_Background.Add(m_Progress);
//
//         this.hierarchy.Add(container);
//
//         // Обробка зміни розмірів
//         this.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
//     }
//
//     // Ініціалізація висоти прогресу
//     private void OnGeometryChanged(GeometryChangedEvent e)
//     {
//         SetProgress(this.value);
//     }
//
//     // Встановлення прогресу
//     public void SetValueWithoutNotify(float newValue)
//     {
//         this.m_Value = Mathf.Clamp(newValue, m_LowValue, m_HighValue);
//         SetProgress(this.value);
//     }
//
//     private void SetProgress(float p)
//     {
//         float progressHeight = CalculateProgressHeight(p);
//         m_Progress.style.height = progressHeight;
//     }
//
//     private float CalculateProgressHeight(float p)
//     {
//         if (m_Background == null || m_Progress == null) return 0f;
//
//         IResolvedStyle layout = m_Background.resolvedStyle;
//         float height = layout.height;
//         return Mathf.Max(1f, height * (p / m_HighValue));
//     }
//     
//     public new class UxmlTraits : BindableElement.UxmlTraits
//     {
//         private UxmlFloatAttributeDescription m_LowValue;
//         private UxmlFloatAttributeDescription m_HighValue;
//         private UxmlFloatAttributeDescription m_Value;
//
//         public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
//         {
//             base.Init(ve, bag, cc);
//             VerticalProgressBar progressBar = ve as VerticalProgressBar;
//
//             progressBar.m_LowValue = this.m_LowValue.GetValueFromBag(bag, cc);
//             progressBar.m_HighValue = this.m_HighValue.GetValueFromBag(bag, cc);
//             progressBar.value = this.m_Value.GetValueFromBag(bag, cc);
//         }
//
//         public UxmlTraits()
//         {
//             this.m_LowValue = new UxmlFloatAttributeDescription { name = "low-value", defaultValue = 0f };
//             this.m_HighValue = new UxmlFloatAttributeDescription { name = "high-value", defaultValue = 100f };
//             this.m_Value = new UxmlFloatAttributeDescription { name = "value", defaultValue = 0f };
//         }
//     }
// }
// }
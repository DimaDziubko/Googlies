using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnbiasedTimeSample : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _systemTimeText;
    [SerializeField] private TextMeshProUGUI _realTimeText;
    [SerializeField] private TextMeshProUGUI _offsetTimeText;
    [SerializeField] private TextMeshProUGUI _isCheatedText;
    [SerializeField] private TextMeshProUGUI _usingDeviceTimeText;
    [SerializeField] private TextMeshProUGUI _upTimeText;
    [Space]
    [SerializeField] private Button _button;

    private void OnEnable()
    {
        _button.onClick.AddListener(TestTime);
    }

    void Awake()
    {
        // Делаем этот объект неуничтожимым
        DontDestroyOnLoad(gameObject);

        // Инициализируем
        UnbiasedTimeWrapper.Init();
        UnbiasedTimeWrapper.UpdateFromNtp();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // UnbiasedTimeWrapper.OnApplicationPause(pauseStatus);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        // UnbiasedTimeWrapper.OnApplicationPause(!hasFocus);
    }

    void TestTime()
    {
        UnbiasedTimeWrapper.UpdateFromNtp();
        _systemTimeText.text = ($"System Time: {DateTime.Now}");
        _realTimeText.text = ($"Real Time UTC: {UnbiasedTimeWrapper.GetUnbiasedUtcNow()}");
        _isCheatedText.text = ($"Is Cheated: {UnbiasedTimeWrapper.IsTimeCheated()}");
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(TestTime);
    }
}

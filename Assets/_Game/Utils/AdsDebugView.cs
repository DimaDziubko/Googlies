using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AdsDebugView : MonoBehaviour
{
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _showButton;
    [SerializeField] private TMP_Text _status;
    
    [SerializeField] private Button _interLoadButton;
    [SerializeField] private Button _interShowButton;
    [SerializeField] private TMP_Text _interStatus;

    [SerializeField] private Button _showMediationDebugger;
    
    public event UnityAction OnLoadClicked
    {
        add => _loadButton.onClick.AddListener(value);
        remove => _loadButton.onClick.RemoveListener(value);
    }

    public event UnityAction OnShowClicked
    {
        add => _showButton.onClick.AddListener(value);
        remove => _showButton.onClick.RemoveListener(value);
    }

    public event UnityAction OnInterLoadClicked
    {
        add => _interLoadButton.onClick.AddListener(value);
        remove => _interLoadButton.onClick.RemoveListener(value);
    }

    public event UnityAction OnInterShowClicked
    {
        add => _interShowButton.onClick.AddListener(value);
        remove => _interShowButton.onClick.RemoveListener(value);
    }
    
    public event UnityAction OnShowMediationDebuggerClicked
    {
        add => _showMediationDebugger.onClick.AddListener(value);
        remove => _showMediationDebugger.onClick.RemoveListener(value);
    }
    
    public void SetReadyToShow(bool isReady)
    {
        _showButton.interactable = isReady;
    }
    
    public void SetReadyToLoad(bool isReady)
    {
        _showButton.interactable = isReady;
    }

    public void SetStatus(string status)
    {
        _status.text = status;
    }
    
    
    public void SetInterReadyToShow(bool isReady)
    {
        _interShowButton.interactable = isReady;
    }
    
    public void SetInterReadyToLoad(bool isReady)
    {
        _interShowButton.interactable = isReady;
    }

    public void SetInterStatus(string status)
    {
        _interStatus.text = status;
    }
    
}



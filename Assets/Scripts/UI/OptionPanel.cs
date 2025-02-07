using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour
{
    [SerializeField] private Button _buttonExit;
    public Action onExitClick;
    
    void Start()
    {
        _buttonExit.onClick.AddListener(OnClickExit);
    }

    private void OnClickExit()
    {
        onExitClick?.Invoke();
    }
}

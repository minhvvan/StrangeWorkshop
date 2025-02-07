using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _textPressButton;
    [SerializeField] private float textBlinkInterval = 0.8f;
    
    private bool _canProceed;

    private void Awake()
    {
        _canProceed = false;
        _textPressButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        WaitForInitialization().Forget();
    }
    
    private async UniTaskVoid WaitForInitialization()
    {
        await UniTask.WaitUntil(() => GameManager.Instance.IsInitialized);
        _canProceed = true;
        _textPressButton.gameObject.SetActive(true);
        StartBlinkingText().Forget();
    }
    
    private async UniTaskVoid StartBlinkingText()
    {
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(textBlinkInterval));
            if (_textPressButton != null)
            {
                _textPressButton.enabled = !_textPressButton.enabled;
            }
        }
    }
    
    private void Update()
    {
        if (!_canProceed) return;

        if (Input.anyKeyDown)
        {
            ProceedToNextScene();
        }
    }
    
    private void ProceedToNextScene()
    {
        _canProceed = false;
        _textPressButton.gameObject.SetActive(false);

        GameManager.Instance.LoadMainMenu();
    }
}

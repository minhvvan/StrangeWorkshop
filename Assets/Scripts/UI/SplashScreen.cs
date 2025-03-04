using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _textPressButton;
    [SerializeField] private float _textBlinkInterval = 0.8f;

    [SerializeField] private ParticleSystem _particle;
    private float _currentMaxSize = 8f;
    private float _currentMinSize = 1f;
    private float _targetMaxSize = 80f;
    private float _targetMinSize = 10f;
    private float _sizeIncreaseSpeed = 10f; // 초당 증가할 크기
    
    private bool _canProceed;
    private bool _buttonPressed;

    private void Awake()
    {
        _canProceed = false;
        _buttonPressed = false;
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
        while (!_buttonPressed)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_textBlinkInterval));
            if (_textPressButton != null)
            {
                _textPressButton.enabled = !_textPressButton.enabled;
            }
        }

        _textPressButton.enabled = false;
    }
    
    private void Update()
    {
        if (!_canProceed) return;

        if (Input.anyKeyDown)
        {
            _buttonPressed = true;
            ProceedToNextScene();
        }
    }
    
    private async void ProceedToNextScene()
    {
        await StartIncreasingSize();
        
        _canProceed = false;
        if(_textPressButton != null) _textPressButton.enabled = false;

        GameManager.Instance.LoadMainMenu();
    }
    
    public async UniTask StartIncreasingSize()
    {
        while (_currentMaxSize < _targetMaxSize)
        {
            _currentMaxSize += _sizeIncreaseSpeed * Time.deltaTime;
            _currentMinSize += _sizeIncreaseSpeed * Time.deltaTime;
            _currentMaxSize = Mathf.Min(_currentMaxSize, _targetMaxSize);
            _currentMinSize = Mathf.Min(_currentMinSize, _targetMinSize);

            // ParticleSystem의 크기 범위 업데이트
            var main = _particle.main;
            var sizeOverLifetime = new ParticleSystem.MinMaxCurve(_currentMaxSize, _currentMinSize);
            sizeOverLifetime.mode = ParticleSystemCurveMode.TwoConstants;
            main.startSize = sizeOverLifetime;

            await UniTask.Yield();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : Singleton<BGMManager>
{ 
    [SerializeField] private BGMDataSO _bgmData;
    [SerializeField] private float _defaultFadeDuration = 1.0f;
    
    private AudioSource _musicSource;
    
    private bool _isFading;
    private string _previousScene;
    
    protected override void Awake()
    {
        base.Awake();
        _previousScene = SceneManager.GetActiveScene().name;
        _musicSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool bKeepTime = _previousScene == "StartUpScene" && scene.name == "MainMenuScene";

        if (_bgmData.TryGetAudioClip(scene.name, out AudioClip clip))
        {
            TransitionMusic(clip, bKeepTime).Forget();
        }
        
        _previousScene = scene.name;
    }
    
    private async UniTask TransitionMusic(AudioClip newClip, bool keepTime)
    {
        if (_isFading || newClip == null) return;

        _isFading = true;
        float startVolume = _musicSource.volume;
        float currentTime = keepTime ? _musicSource.time : 0f;

        // 두 번째 AudioSource 생성하여 크로스페이드
        var tempSource = gameObject.AddComponent<AudioSource>();
        tempSource.clip = newClip;
        tempSource.time = currentTime;
        tempSource.volume = 0f;
        tempSource.loop = true;
        tempSource.Play();

        // 크로스페이드
        float elapsed = 0;
        while (elapsed < _defaultFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _defaultFadeDuration;
            _musicSource.volume = Mathf.Lerp(startVolume, 0, t);
            tempSource.volume = Mathf.Lerp(0, startVolume, t);
            await UniTask.Yield();
        }

        // 이전 음악 정리
        _musicSource.Stop();
        _musicSource.clip = newClip;
        _musicSource.time = tempSource.time;
        _musicSource.volume = startVolume;
        _musicSource.Play();

        Destroy(tempSource);
        _isFading = false;
    }

    public void SetVolume(float volume) => _musicSource.volume = Mathf.Clamp01(volume);
}

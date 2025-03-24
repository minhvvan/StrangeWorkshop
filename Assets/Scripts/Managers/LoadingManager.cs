using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using static System.TimeSpan;

/// <summary>
/// Loading Scene 관리 매니저
/// 챕터로 진입 전 데이터 로딩 및 스토리 연출 등을 담당
/// </summary>

public class LoadingManager : Singleton<LoadingManager>
{
    [Header("Managers")]
    [Inject] private EventManager _eventManager;
    
    [Header("Handles")]
    private AsyncOperation _nextSceneLoad;

    [Header("Data")]
    private ChapterDataSO _currentChapterData;
    private bool _isStoryEnd;
    
    public async void LoadChapter(ChapterDataSO chapterDataSo)
    {
        _isStoryEnd = false;
        _currentChapterData = chapterDataSo;
        
        //Loading씬으로 이동
        await SceneManager.LoadSceneAsync("Scenes/LoadingScene");
        GameManager.Instance.RequestChangeGameState(GameState.Loading);

        // 그 다음 다음 씬 로딩을 시작
        _nextSceneLoad = SceneManager.LoadSceneAsync(_currentChapterData.sceneName);
        if (_nextSceneLoad != null) _nextSceneLoad.allowSceneActivation = false;

        await UniTask.WhenAll(
            WaitForSceneLoad(),
            PlayStory()
        );

        MoveToGameScene();
    }

    private async UniTask PlayStory()
    {
        if (!_currentChapterData || !_currentChapterData.openingStory) _isStoryEnd = true;

        var ktd = FindObjectOfType<KoreanTyperDemo_Cursor>();
        ktd?.TypingText(_currentChapterData.openingStory.description);

        await UniTask.WaitUntil(() => _isStoryEnd);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _isStoryEnd = true;
        }
    }

    private void MoveToGameScene()
    {   
        _nextSceneLoad.allowSceneActivation = true;
        GameManager.Instance.PlayChapterSequence();
    }
    
    private async UniTask WaitForSceneLoad(IProgress<float> progress = null)
    {
        // 이미 시작된 씬 로딩의 완료를 기다림
        while (_nextSceneLoad.progress < 0.9f)
        {
            progress?.Report(_nextSceneLoad.progress);
            await UniTask.Yield();
        }
    }
}

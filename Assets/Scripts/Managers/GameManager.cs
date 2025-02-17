using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class GameManager : Singleton<GameManager>
{
    [Header("Data")]
    private GameState _currentGameState;
    private ChapterDataSO _currentChapter;
    private ChapterListSO _chapterList;

    public ChapterDataSO CurrentChapterData => _currentChapter;

    [Header("Event")]
    private GameStateEventSO _gameStateEvent;

    public bool IsInitialized { get; private set; }
    private const string START_SCENE_NAME = "StartUpScene";

    private async void Start()
    {
        //로딩 화면을 보여주고 필요한 데이터 로드
        await Initialize();
    }

    public async UniTask Initialize()
    {
        //Load ChapterData
        _chapterList = await DataManager.Instance.LoadDataAsync<ChapterListSO>(Addresses.Data.Chapter.CHAPTER_LIST);
        _currentChapter = _chapterList.GetFirstChapter();
        
        //Load Events
        await UniTask.WaitUntil(() => EventManager.Instance.IsInitialized);
        _gameStateEvent = EventManager.Instance.GetEvent<GameStateEventSO>(Addresses.Events.Game.STATE_CHANGED);

        IsInitialized = true;
    }

    public void LoadMainMenu()
    {
        ChangeGameState(GameState.MainMenu);
        SceneManager.LoadScene("Scenes/MainMenuScene");
    }

    public void RequestChangeGameState(GameState newState)
    {
        if (!PreChangeGameState(newState)) return;
        ChangeGameState(newState);
        PostChangeGameState(newState);
    }

    private bool PreChangeGameState(GameState newState)
    {
        //TODO: Check Validation
        return true;
    }

    private void ChangeGameState(GameState newState)
    {
        //Test
        _currentGameState = newState;
        _gameStateEvent.Raise(_currentGameState);
    }
    
    private void PostChangeGameState(GameState newState)
    {
        //게임 상태 변경 후 추가 작업
        if (_currentGameState == GameState.GameOver)
        {
            GameOver();
        }
    }

    public void StartGame()
    {
        if (_currentGameState != GameState.MainMenu) return;
        SceneManager.LoadScene("ChapterSelectScene");
    }

    public void ClearChapter()
    {
        if (_currentGameState != GameState.InGame) return;
        _currentChapter = _chapterList.GetNextChapter(_currentChapter);
        LoadingManager.Instance.LoadChapter(_currentChapter);
    }

    public void PauseGame()
    {
        if (_currentGameState == GameState.InGame)
        {
            RequestChangeGameState(GameState.Paused);
        }
    }

    public void ResumeGame()
    {
        if (_currentGameState == GameState.Paused)
        {
            RequestChangeGameState(GameState.InGame);
        }
    }

    private void GameOver()
    {
        //TODO: GameOver -> 재시도 여부 묻는 UI + 챕터 선택 UI 추가 후 변경 예정(UIManager에서 처리)
        LoadMainMenu();
    }
}

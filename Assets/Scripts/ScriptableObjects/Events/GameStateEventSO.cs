using System;
using UnityEngine;

public enum GameState
{
    None,
    MainMenu,       // 메인 메뉴
    ChapterSelect,  // 챕터 선택 화면
    Loading,        // 로딩
    InGame,         // 게임 플레이 중
    Paused,         // 일시정지
    GameOver,       // 게임 오버
    GameClear,      // 게임 클리어
    Max
}

[CreateAssetMenu(fileName = "Events", menuName = "SO/Event/GameState")]
public class GameStateEventSO : BaseEventSO
{
    private event Action<GameState> _onEventRaised;
    
    public void Raise(GameState state) => _onEventRaised?.Invoke(state);
    public void AddListener(Action<GameState> listener) => _onEventRaised += listener;
    public void RemoveListener(Action<GameState> listener) => _onEventRaised -= listener;
}
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public static class GameBootstrapper
{
    private static bool isInitialized = false;
    private const string START_SCENE_NAME = "StartUpScene";

#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    private static void EditorInitialize()
    {
        // 스크립트 리컴파일/도메인 리로드 시에도 초기화 상태 리셋
        isInitialized = false;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        var currentScene = SceneManager.GetActiveScene().name;

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (!isInitialized && currentScene != START_SCENE_NAME)
            {
                Debug.Log($"Initializing game from scene: {currentScene}");
                _ = Initialize();
            }
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            isInitialized = false;
        }
    }

    private static async UniTask Initialize()
    {
        await UniTask.WaitUntil(() => EventManager.Instance.IsInitialized);
        await UniTask.WaitUntil(() => RecipeManager.Instance.IsInitialized);
        await UniTask.WaitUntil(() => GameManager.Instance.IsInitialized);

        isInitialized = true;
    }
#endif
}
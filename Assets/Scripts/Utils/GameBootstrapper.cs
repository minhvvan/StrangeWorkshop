using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Managers;
using Unity.VisualScripting;
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

    public static bool IsInitialized => isInitialized;

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
                Initialize();
            }
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            CleanupManagers();
            isInitialized = false;
        }
    }

    private static async void Initialize()
    {
        await Task.WhenAll(
            UIManager.Instance.Initialize().AsTask(),
            EventManager.Instance.InitializeEvents().AsTask(),
            RecipeManager.Instance.Initialize().AsTask(),
            GameManager.Instance.Initialize().AsTask(),
            VFXManager.Instance.Initialize().AsTask(),
            SFXManager.Instance.Initialize().AsTask()
        );

        EventManager.Instance.AddComponent<DontDestroyOnLoad>();
        RecipeManager.Instance.AddComponent<DontDestroyOnLoad>();
        GameManager.Instance.AddComponent<DontDestroyOnLoad>();
        LoadingManager.Instance.AddComponent<DontDestroyOnLoad>();
        UIManager.Instance.AddComponent<DontDestroyOnLoad>();
        VFXManager.Instance.AddComponent<DontDestroyOnLoad>();
        SFXManager.Instance.AddComponent<DontDestroyOnLoad>();

        isInitialized = true;
    }
    
    private static void CleanupManagers()
    {
        if (EventManager.Instance) Object.Destroy(EventManager.Instance.gameObject);
        if (RecipeManager.Instance) Object.Destroy(RecipeManager.Instance.gameObject);
        if (GameManager.Instance) Object.Destroy(GameManager.Instance.gameObject);
        if (LoadingManager.Instance) Object.Destroy(LoadingManager.Instance.gameObject);
        if (UIManager.Instance) Object.Destroy(UIManager.Instance.gameObject);
        if (VFXManager.Instance) Object.Destroy(VFXManager.Instance.gameObject);
        if (SFXManager.Instance) Object.Destroy(SFXManager.Instance.gameObject);
    }
#endif
}
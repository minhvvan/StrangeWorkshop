using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// [InitializeOnLoad]
public static class FirstSceneLoader
{
    private const string StartSceneName = "StartUpScene"; // 시작 씬 이름

    static FirstSceneLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            if (EditorSceneManager.GetActiveScene().name != StartSceneName)
            {
                Debug.Log($"Switching to start scene: {StartSceneName}");
                SceneManager.LoadScene(StartSceneName);
            }
        }
    }
}
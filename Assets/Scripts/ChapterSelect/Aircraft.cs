using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Aircraft : MonoBehaviour
{
    [SerializeField] private Animation _startAnim;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            gameObject.SetActive(false);
        }
    }

    public async Task OnClickedGameStart()
    {
        gameObject.SetActive(true);
        _animator.Play("MainMenu_Start");
    
        // 애니메이션이 끝날 때까지 대기
        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            await Task.Yield();
        }
    }
}

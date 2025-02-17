using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSpaceShip : MonoBehaviour
{
    private Animator _animator;
    private AudioSource _audio;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenuScene")
        {
            gameObject.SetActive(false);
        }
    }

    public async UniTask OnClickedGameStart()
    {
        gameObject.SetActive(true);
        _audio.Play();
        _animator.Play("MainMenu_Start");
    
        // 애니메이션이 끝날 때까지 대기
        while (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            await UniTask.Yield();
        }
    }
}

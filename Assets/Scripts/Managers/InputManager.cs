using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Vector2 = System.Numerics.Vector2;

public class InputManager: Singleton<InputManager>
{
     private IControllable _currentController;
     private IControllable _playerController;
     private Stack<IControllable> _controlStack = new Stack<IControllable>();
     private InputData _currentInputData = new InputData();

     public bool IsAcceptingInput { get; set; }

     protected override void Awake()
     {
          base.Awake();
          
          SceneManager.sceneLoaded += OnSceneLoaded;
     }

     private void Update()
     {
          if (!IsAcceptingInput) return;
          
          // 입력 수집
          _currentInputData.moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
          _currentInputData.interactPressed = Input.GetButtonDown("Interact");
          _currentInputData.interactAlternatePressed = Input.GetButtonDown("InteractAlternate");
          _currentInputData.dashPressed = Input.GetButtonDown("Dash");
        
          // 현재 컨트롤러에 입력 전달
          if (_currentController != null)
          {
               _currentController.ProcessInput(_currentInputData);
          }
     }

     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
     {
          _playerController = FindObjectOfType<CharacterInputHandler>();
          _currentController = _playerController;
          _controlStack.Push(_currentController);
     }

     public void SwitchControl(IControllable newController)
     {
          if (_currentController != null)
          {
               _currentController.OnControlEnd();
          }
        
          _controlStack.Push(newController);
          _currentController = newController;
          _currentController.OnControlStart();
     }

     public void ReturnToPreviousControl()
     {
          if (_controlStack.Count <= 1)
          {
               Debug.LogWarning("Control stack is empty");
               return;
          }
          
          if (_currentController != null)
          {
               _currentController.OnControlEnd();
               _controlStack.Pop();
          }
        
          _currentController = _controlStack.Peek();
          _currentController.OnControlStart();
     }

     public void ReturnToPlayerControl()
     {
          while (_controlStack.Count > 0)
          {
               IControllable controller = _controlStack.Pop();
               if (controller != null && controller != _playerController)
               {
                    controller.OnControlEnd();
               }
          }
        
          _controlStack.Push(_playerController);
          _currentController = _playerController;
          _currentController.OnControlStart();
     }
}

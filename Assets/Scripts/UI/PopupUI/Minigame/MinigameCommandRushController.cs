using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MinigameCommandRushController: MinigameBaseController
{
    //Game Logic
    enum CommandRushItem
    {
        LEFT_ARROW,
        RIGHT_ARROW,
        UP_ARROW,
        DOWN_ARROW,
        // A_BUTTON,
        // B_BUTTON,
    }

    [SerializeField] SerializedDictionary<CommandRushItem, GameObject> commandRushItems;
    List<CommandRushItem> _commandRushSequence;
    int _currentCommandIndex = 0;
    CancellationTokenSource _cts;

    //UI Logic
    [Header("UI")]
    [SerializeField] RectTransform commandRushItemParent;
    [SerializeField] GameObject commandRushItemSelector;

    [Header("UI Effect")]
    [SerializeField] RectTransform commonKeyImage;
    [SerializeField] RectTransform commonSuccessGroup;

    [SerializeField] RectTransform commonFailGroup;
  

    public void Initialize(int rushCount){
        _commandRushSequence = new List<CommandRushItem>();
        for (int i = 0; i < rushCount; i++)
        {
            int randomIndex = Random.Range(0, commandRushItems.Count);
            CommandRushItem randomCommand = (CommandRushItem)randomIndex;
            _commandRushSequence.Add(randomCommand);
        }
        ShowCommandRushItem();
        _currentCommandIndex = 0;
        commandRushItemSelector.SetActive(true);
        //_ = MoveToSelectorPosition(_currentCommandIndex);
    }


    void ShowCommandRushItem(){
        foreach (Transform child in commandRushItemParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < _commandRushSequence.Count; i++)
        {
            GameObject commandRushItem = Instantiate(commandRushItems[_commandRushSequence[i]], commandRushItemParent);
            commandRushItem.transform.localScale = Vector3.one;
        }
    }

    void HideCommandRushItem(){
        foreach (Transform child in commandRushItemParent)
        {
            Destroy(child.gameObject);
        }
        commandRushItemSelector.SetActive(false);
    }

    void CheckCommandRushItem(CommandRushItem commandRushItem){
        if (_currentCommandIndex >= _commandRushSequence.Count)
        {
            Debug.Log("All commands completed");
            return;
        }

        if (commandRushItem == _commandRushSequence[_currentCommandIndex])
        {
            //Push Effect
            GameObject commandRushItemObject = commandRushItemParent.GetChild(_currentCommandIndex).gameObject;
            commandRushItemObject.transform.DOPunchScale(Vector3.one * 1.2f, 0.2f).OnComplete(() =>
            {
                commandRushItemObject.transform.DOScale(Vector3.one, 0);
            });

            _currentCommandIndex++;
            //성공
            if (_currentCommandIndex == _commandRushSequence.Count)
            {
                ChangeState(MinigameState.Success);
            }
            else
            {
                _ = MoveToSelectorPosition(_currentCommandIndex);
            }
        }
        else
        {
            //실패
            Debug.Log("Fail");
            ChangeState(MinigameState.Fail);
        }
    }


    async UniTask MoveToSelectorPosition(int index)
    {
        commandRushItemSelector.SetActive(true);

        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
        _cts = new CancellationTokenSource();
        List<Transform> children = commandRushItemParent.GetComponentsInChildren<Transform>().ToList();

        if (index < 0 || index >= children.Count - 1)
        {
            Debug.LogError("Index out of range");
            return;
        }

        Transform targetChild = children[index + 1];
        Vector3 targetPosition = targetChild.position;
        targetPosition.z = commandRushItemSelector.transform.position.z;

        while (Vector3.Distance(commandRushItemSelector.transform.position, targetPosition) > 0.01f)
        {
            commandRushItemSelector.transform.position = Vector3.Lerp(commandRushItemSelector.transform.position, targetPosition, Time.deltaTime * 10f);
            await UniTask.Delay(10, cancellationToken: _cts.Token);
            if (_cts.Token.IsCancellationRequested)
            {
                break;
            }            
        }
    }

    void Update()
    {
        if(minigameState == MinigameState.Idle)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ChangeState(MinigameState.Playing);
            }
        }

        if(minigameState == MinigameState.Playing)
        {   
            if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                CheckCommandRushItem(CommandRushItem.LEFT_ARROW);
            }
            else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                CheckCommandRushItem(CommandRushItem.RIGHT_ARROW);
            }
            else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                CheckCommandRushItem(CommandRushItem.UP_ARROW);
            }
            else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                CheckCommandRushItem(CommandRushItem.DOWN_ARROW);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OnFail?.Invoke();
            Hide();
        }
    }

    void Hide()
    {   
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => {
            Destroy(gameObject);
        });
    }

    protected override void ChangeState(MinigameState state)
    {
        if(state == minigameState) return;       

        //exit
        switch(minigameState)
        {
            case MinigameState.Idle:
                break;
            case MinigameState.Playing:
                break;
            case MinigameState.Success:    
                commonSuccessGroup.gameObject.SetActive(false);
                
                break;
            case MinigameState.Fail:                
                commonFailGroup.gameObject.SetActive(false);
                break;
        }

        //enter
        switch(state)
        {
            case MinigameState.Idle:
                IdleMinigame();
                break;
            case MinigameState.Playing:
                StartMinigame();
                break;
            case MinigameState.Success:
                SuccessMiniGame();
                break;
            case MinigameState.Fail:
                FailMiniGame();
                break;
        }

        minigameState = state;
    }

    void IdleMinigame()
    {
    }

    void StartMinigame()
    {
        commonKeyImage.DOLocalMoveY(-80, 0.5f).OnComplete(() => {
            Initialize(4);
        });
    }

    void SuccessMiniGame()
    {
        commonKeyImage.DOKill();
        //commonKey Fadeout
        commonKeyImage.GetComponent<Image>().DOFade(0, 0.2f).OnComplete(() => {
            commonKeyImage.gameObject.SetActive(false);
        });

        
        commonSuccessGroup.gameObject.SetActive(true);
        commonSuccessGroup.transform.DOPunchScale(Vector3.one * 1.1f, 0.1f).OnComplete(() => commonSuccessGroup.transform.DOScale(Vector3.one, 0.2f));
        
        DOVirtual.DelayedCall(0.3f, () =>
        {
            HideCommandRushItem();
            DOVirtual.DelayedCall(0.3f, () => {
                Hide();
            });
        });

        OnSuccess?.Invoke();
    }

    void FailMiniGame()
    {
        commonKeyImage.DOKill();
        commonKeyImage.GetComponent<Image>().DOFade(0, 0.2f).OnComplete(() => {
            commonKeyImage.gameObject.SetActive(false);
        });
        commonFailGroup.gameObject.SetActive(true);
        commonFailGroup.transform.DOScale(0, 0).OnComplete(() => commonFailGroup.transform.DOScale(Vector3.one, 0.3f));

        HideCommandRushItem();            
        DOVirtual.DelayedCall(0.3f, () => {
            Hide();
        });
        OnFail?.Invoke();
    }
}

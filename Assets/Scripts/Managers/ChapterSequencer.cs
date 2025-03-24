using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using Unity.VisualScripting;
using UnityEngine;

public class ChapterSequencer
{
    private List<SequenceCommandSO> commands;
    private CommandCompleteEventSO _commandCompleteEventSO;
    
    private int currentCommandIndex = 0;
    private SequenceCommandSO currentCommand;
    
    public async UniTask Initialize(List<SequenceCommandSO> sequenceCommands)
    {
        currentCommandIndex = 0;
        commands = sequenceCommands;
        
        if (!_commandCompleteEventSO.IsUnityNull()) return;
        
        // 이벤트 등록
        _commandCompleteEventSO = await DataManager.Instance.LoadDataAsync<CommandCompleteEventSO>(Addresses.Events.Game.COMMAND_COMPLETE);
        _commandCompleteEventSO.AddListener(OnCommandCompleted);
    }

    public void StartSequence()
    {
        ExecuteNextCommand();
    }

    private void OnCommandCompleted()
    {
        Debug.Log("OnCommandCompleted");
        ExecuteNextCommand();
    }

    private void ExecuteNextCommand()
    {
        //모든 시퀀스 완료
        if (currentCommandIndex >= commands.Count)
        {
            Debug.Log("All commands completed");
            return;
        }
        
        currentCommand = commands[currentCommandIndex++];

        if (!currentCommand.IsInitialized) currentCommand.Initialize();
        currentCommand.Execute(_commandCompleteEventSO);
    }
}

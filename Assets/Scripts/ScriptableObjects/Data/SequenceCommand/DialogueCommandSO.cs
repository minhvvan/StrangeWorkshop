using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueCommandSO", menuName = "SO/Chapter/Dialogue")]
public class DialogueCommandSO:SequenceCommandSO
{
    [Serializable]
    public class DialogueData
    {
        public string Talker;
        public List<string> Dialogue;
    }
    
    [SerializeField] private List<DialogueData> _dialogueData;
    private CommandCompleteEventSO _commandComplete;
    
    public override void Initialize()
    {
        IsInitialized = true;
    }

    public override void Execute(CommandCompleteEventSO completeEventSo)
    {
        _commandComplete = completeEventSo;
        
        var dialogueUI = UIManager.Instance.GetUI<DialogueUI>(UIType.DialogueUI);
        dialogueUI.SetDialogueData(_dialogueData);
        dialogueUI.ShowUI();

        dialogueUI.OnDialogueEnded -= OnDialogueEnd;
        dialogueUI.OnDialogueEnded += OnDialogueEnd;
    }

    private void OnDialogueEnd()
    {
        var dialogueUI = UIManager.Instance.GetUI<DialogueUI>(UIType.DialogueUI);
        dialogueUI.HideUI();
        _commandComplete.Raise();
    }
}

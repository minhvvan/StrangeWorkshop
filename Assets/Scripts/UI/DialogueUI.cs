using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueCommandSO;

public class DialogueUI : MonoBehaviour, IGameUI
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image characterImage;
    
    private List<DialogueData> _dialogueData = new List<DialogueData>();
    private int _cursor = 0;
    private int _index = 0;
    private int _currentMaxDialogue = 0;
    
    public Action OnDialogueEnded;
    
    public void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }

    public void Initialize()
    {
    }

    public void CleanUp()
    {
    }

    public void SetDialogueData(List<DialogueData> dialogueData)
    {
        _index = 0;
        _cursor = 0;
        _dialogueData = dialogueData;
        SetDialogue();
    }

    private void SetDialogue()
    {
        if (_index >= _dialogueData.Count)
        {
            OnDialogueEnded?.Invoke();
            return;
        }
        
        _currentMaxDialogue = _dialogueData[_index].Dialogue.Count;
        nameText.text = _dialogueData[_index].Talker;
        dialogueText.text = _dialogueData[_index].Dialogue[_cursor];
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_cursor < _dialogueData.Count - 1)
            {
                _cursor++;
            }
            else
            {
                _cursor = 0;
                _index++;
            }

            SetDialogue();
        }
    }
}

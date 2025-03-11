using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    private Quest quest;
    private RectTransform _root;
    
    [SerializeField] private TMP_Text _questText;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private GameObject _checkmark;

    [SerializeField] private float _padding;
    private float alpha;
    
    public void Init(Quest quest)
    {
        this.quest = quest;
        _root = GetComponent<RectTransform>();
        alpha = _backgroundImage.color.a;
        // Debug.Log(quest.GetQuestDescription());
        UpdateUI();
    }

    public void UpdateUI()
    {
        _questText.text = quest.GetQuestDescription();
        _root.sizeDelta = new Vector2(_root.sizeDelta.x, _questText.preferredHeight + _padding);
        
        if (quest.GetQuestStatus() == QuestStatus.Completed)
        {
            Color color = Color.green;
            color.a = alpha;
            _backgroundImage.color = color;
            
            _checkmark.SetActive(true);
        }
        else if (quest.GetQuestStatus() == QuestStatus.Failed)
        {
            Color color = Color.red;
            color.a = alpha;
            _backgroundImage.color = color;
            
            _backgroundImage.color = color;
        }
    }
}

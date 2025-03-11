using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class QuestUIController : MonoBehaviour, IGameUI
{
    private RectTransform _root;

    [SerializeField] private QuestUI _quest1;
    [SerializeField] private QuestUI _quest2;

    private Dictionary<int, QuestUI> _questUIs = new Dictionary<int, QuestUI>();

    public void ShowUI()
    {
        UIAnimationUtility.SlideInLeft(_root);
    }

    public void HideUI()
    {
        Vector2 originalPos = _root.anchoredPosition;
        UIAnimationUtility.SlideOutLeft(_root, callback: () => { _root.anchoredPosition = originalPos; });
    }

    async public void Initialize()
    {
        _root = GetComponent<RectTransform>();
        await UniTask.WaitUntil(() => QuestManager.Instance.IsInitialized);
        SetQuests();
    }

    public void CleanUp()
    {
    }

    public void UpdateQuestProgress(Quest quest)
    {
        _questUIs[quest.id].UpdateUI();
    }

    private void SetQuests()
    {
        Quest quest1 = QuestManager.Instance.availableQuests[0];
        Quest quest2 = QuestManager.Instance.availableQuests[1];
        _questUIs[quest1.id] = _quest1;
        _questUIs[quest2.id] = _quest2;
        _quest1.Init(quest1);
        _quest2.Init(quest2);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "OpenUICommandSO", menuName = "SO/Chapter/OpenUI")]
public class OpenUICommandSO: SequenceCommandSO
{
    [SerializeField] private UIType uiType;
    
    public override void Initialize()
    {
    }

    public override void Execute(CommandCompleteEventSO completeEventSo)
    {
        var ui = UIManager.Instance.GetUI<IGameUI>(uiType);
        ui.ShowUI();
        completeEventSo.Raise();
    }
}

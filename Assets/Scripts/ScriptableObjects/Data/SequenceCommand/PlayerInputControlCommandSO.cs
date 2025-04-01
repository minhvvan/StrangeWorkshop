using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInputControlCommandSO", menuName = "SO/Chapter/PlayerInputControl")]
public class PlayerInputControlCommandSO: SequenceCommandSO
{
    [SerializeField] private bool flag;
    
    public override void Initialize()
    {
    }

    public override void Execute(CommandCompleteEventSO completeEventSo)
    {
        InputManager.Instance.IsAcceptingInput = flag;
        completeEventSo.Raise();
    }
}

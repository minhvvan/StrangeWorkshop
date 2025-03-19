using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "CameraMoveCommandSO", menuName = "SO/Chapter/CameraMove")]
public class CameraMoveCommandSO: SequenceCommandSO
{
    [SerializeField] private PlayableAsset _target;
    
    private CommandCompleteEventSO _completeEvent;
    
    public override void Initialize()
    {
    }

    public override void Execute(CommandCompleteEventSO completeEventSo)
    {
        _completeEvent = completeEventSo;
        
        var director = FindObjectOfType<PlayableDirector>();
        director.playableAsset = _target;
        director.stopped += OnPlayableDirectorStopped;
        
        director.Play();
    }

    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        director.stopped -= OnPlayableDirectorStopped;
        _completeEvent?.Raise();
    }
}
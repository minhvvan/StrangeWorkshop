using Cysharp.Threading.Tasks;

public interface IInteractKits
{
    UniTask Initialize();
    void Excute(SampleCharacterController player = null, int? cost = null);
}
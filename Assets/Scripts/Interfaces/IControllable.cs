
public interface IControllable
{
    public void OnControlStart();
    public void OnControlEnd();
    public void ProcessInput(InputData input);
}
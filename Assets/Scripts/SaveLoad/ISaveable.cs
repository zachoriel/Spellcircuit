public interface ISaveable
{
    public object CaptureState();
    public void RestoreState(string state);
}
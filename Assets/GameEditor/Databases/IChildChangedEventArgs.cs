namespace GameEditor.Databases
{
    public interface IChildChangedEventArgs
    {
        object Snapshot { get; }
        object SnapshotValue { get; }
        string PreviousChildName { get; }
    }
}
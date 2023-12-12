namespace GameEditor.Databases
{
    public interface IValueChangedEventArgs
    {
        object Snapshot { get; }
        object SnapshotValue { get; }
        string SnapshotGetRawJsonValue();
    }
}
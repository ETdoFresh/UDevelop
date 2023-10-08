using ETdoFresh.UnityPackages.EventBusSystem;

public class CommandEvent : EventBusEvent
{
    public string Command { get; set; }
}
namespace UDevelop.UI.Events
{
    public class UIPopupEvent : UIEvent
    {
        public enum Type { Hide, Show }

        public string PopupName { get; set; }
        public Type PopupType { get; set; } = Type.Show;
    }
}
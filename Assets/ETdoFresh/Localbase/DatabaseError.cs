namespace ETdoFresh.Localbase
{
    public class DatabaseError
    {
        public int Code { get; private set; }
        public string Message { get; private set; }
        public string Details { get; private set; }

        internal DatabaseError(int code, string message, string details)
        {
            Code = code;
            Message = message;
            Details = details;
        }

        internal DatabaseError(int code, string message) : this(code, message, null) { }
    }
}
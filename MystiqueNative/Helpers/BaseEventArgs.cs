namespace MystiqueNative.Helpers
{
    public class BaseEventArgs : System.EventArgs
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class BaseUpdateArgs : BaseEventArgs
    {
        public int EditedId { get; set; }
    }

    public class BaseListEventArgs : BaseEventArgs
    {
        public bool IsEmpty { get; set; }
        public int ResultCount { get; set; }
    }
}
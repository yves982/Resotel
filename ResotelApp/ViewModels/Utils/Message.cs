namespace ResotelApp.ViewModels.Utils
{
    class Message<T>
    {
        public EMessage MessageType { get; set; }

        public T Data { get; set; }

        public Message() { }

        public Message(EMessage message, T data = default(T))
        {
            MessageType = message;
            Data = data;
        }
    }
}

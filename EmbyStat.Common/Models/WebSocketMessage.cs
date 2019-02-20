namespace EmbyStat.Common.Models
{
    public class WebSocketMessage<T>
    {
        public string MessageType { get; set; }
        public T Data { get; set; }
    }
}

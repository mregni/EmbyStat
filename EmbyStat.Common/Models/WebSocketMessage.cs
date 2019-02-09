using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Models
{
    public class WebSocketMessage<T>
    {
        public string MessageType { get; set; }
        public T Data { get; set; }
    }
}

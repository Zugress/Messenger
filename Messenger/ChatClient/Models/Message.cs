using System;

namespace ChatClient.Models
{
    public class Message
    {
        public string Sender { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Time { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"[{Time:HH:mm:ss}] {Sender}: {Text}";
        }
    }
}
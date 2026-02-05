using System;

namespace ChatClient.Models
{
    public class Message
    {
        public string Sender { get; set; } = "";
        public string Text { get; set; } = "";
        public DateTime Time { get; set; } = DateTime.Now;

        public string ToNetworkString()
        {
            return string.Format("{0}|{1}|{2:yyyy-MM-dd HH:mm:ss}", Sender, Time, Text);
        }

        public static Message FromNetworkString(string networkString)
        {
            var parts = networkString.Split(new char[] { '|' }, 3);
            var message = new Message();

            if (parts.Length > 0) message.Sender = parts[0];
            if (parts.Length > 1) message.Text = parts[1];
            if (parts.Length > 2 && DateTime.TryParse(parts[2], out DateTime time))
                message.Time = time;

            return message;
        }

        public override string ToString()
        {
            return string.Format("[{0:HH:mm:ss}] {1}: {2}", Time, Sender, Text);
        }
    }
}
namespace ChatClient.Network
{
    public class Client
    {
        public bool IsConnected { get; private set; }

        public void Connect(string ip, int port, string username)
        {
            // заглушка
            IsConnected = true;
        }

        public void Disconnect()
        {
            IsConnected = false;
        }
    }
}
namespace ChatServer.Network
{
    public class ChatServer
    {
        public bool IsRunning { get; private set; }

        public void Start(int port)
        {
            // заглушка
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Network
{
    public class ServerClient
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly Server _server;
        private readonly byte[] _buffer = new byte[4096];

        public string Username { get; private set; } = "";

        public ServerClient(TcpClient tcpClient, Server server)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
            _server = server;

            Task.Run(() => ReceiveMessagesAsync());
        }

        public async Task SendAsync(string data)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data + "\n");
                await _stream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch
            {
                Disconnect();
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            try
            {
                while (_tcpClient.Connected)
                {
                    int bytesRead = await _stream.ReadAsync(_buffer, 0, _buffer.Length);
                    if (bytesRead == 0) break;

                    string data = Encoding.UTF8.GetString(_buffer, 0, bytesRead).Trim();
                    await ProcessReceivedData(data);
                }
            }
            catch
            {
                
            }
            finally
            {
                Disconnect();
            }
        }


        private async Task ProcessReceivedData(string data)
        {
            var parts = data.Split(new char[] { '|' }, 2);
            if (parts.Length < 2) return;

            string command = parts[0];
            string content = parts[1];

            switch (command)
            {
                case "JOIN":
                    Username = content;
                    await _server.BroadcastMessageAsync("Server",
                        string.Format("{0} connected", Username));
                    break;

                case "MESSAGE":
                    if (!string.IsNullOrEmpty(Username))
                    {
                        await _server.BroadcastMessageAsync(Username, content);
                    }
                    break;
            }
        }


        public void Disconnect()
        {
            try
            {
                _stream.Close();
                _tcpClient.Close();
                _server.RemoveClient(this);
            }
            catch { }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Network
{
    public class Server
    {
        private TcpListener _listener;
        private List<ServerClient> _clients = new List<ServerClient>();
        private bool _isRunning;

        public bool IsRunning { get { return _isRunning; } }

        public event Action<string> LogMessage;

        public async Task StartAsync(int port)
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, port);
                _listener.Start();
                _isRunning = true;

                if (LogMessage != null)
                    LogMessage(string.Format("Server started on port {0}", port));

                while (_isRunning)
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    var client = new ServerClient(tcpClient, this);
                    _clients.Add(client);
                }
            }
            catch (Exception ex)
            {
                if (LogMessage != null)
                    LogMessage(string.Format("Server error: {0}", ex.Message));
                Stop();
            }
        }

        public void Stop()
        {
            _isRunning = false;

            foreach (var client in _clients)
            {
                client.Disconnect();
            }
            _clients.Clear();

            if (_listener != null)
                _listener.Stop();

            if (LogMessage != null)
                LogMessage("Server stopped");
        }

        public async Task BroadcastMessageAsync(string sender, string messageText)
        {
            var formattedMessage = string.Format("MESSAGE|{0}|{1}|{2:yyyy-MM-dd HH:mm:ss}",
                sender, messageText, DateTime.Now);

            var tasks = new List<Task>();

            foreach (var client in _clients)
            {
                tasks.Add(client.SendAsync(formattedMessage));
            }

            await Task.WhenAll(tasks);

            if (LogMessage != null)
                LogMessage(string.Format("[{0:HH:mm:ss}] {1}: {2}", DateTime.Now, sender, messageText));
        }
        public async Task NotifyUserChangeAsync(string username, bool isConnected)
        {
            await SendUserListToAllAsync();

            string action = isConnected ? "подключился" : "отключился";
            if (LogMessage != null)
                LogMessage(string.Format("[{0:HH:mm:ss}] {1} {2}. Всего пользователей: {3}",
                    DateTime.Now, username, action, _clients.Count));
        }

        public List<string> GetUserList()
        {
            var users = new List<string>();
            foreach (var client in _clients)
            {
                if (!string.IsNullOrEmpty(client.Username))
                {
                    users.Add(client.Username);
                }
            }
            return users;
        }

        public void RemoveClient(ServerClient client)
        {
            _clients.Remove(client);
        }
        public async Task SendUserListToAllAsync()
        {
            var users = GetUserList();

            if (users.Count == 0)
                return; // Был баг с отправкой пустого списка, из за чего ломалось отображения пользователей

            var userListString = string.Join(",", users);
            var message = string.Format("USERLIST|{0}", userListString);

            var tasks = new List<Task>();
            foreach (var client in _clients)
            {
                tasks.Add(client.SendAsync(message));
            }

            await Task.WhenAll(tasks);
        }

    }
}
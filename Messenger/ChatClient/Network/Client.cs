using ChatClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Network
{
    public class Client
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        public string Username { get; private set; } = "";
        public bool IsConnected
        {
            get
            {
                return _tcpClient != null && _tcpClient.Connected;
            }
        }

        public event Action<Message> MessageReceived;
        public event Action<string> LogMessage;
        public event Action<List<string>> UsersListUpdated;

        public async Task ConnectAsync(string ip, int port, string username)
        {
            try
            {
                Username = username;
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(ip, port);
                _stream = _tcpClient.GetStream();

                if (LogMessage != null)
                    LogMessage(string.Format("Connected to server as {0}", username));

                await SendAsync(string.Format("JOIN|{0}", username));

                Task.Run(() => ReceiveMessagesAsync());
            }
            catch (Exception ex)
            {
                if (LogMessage != null)
                    LogMessage(string.Format("Connection failed: {0}", ex.Message));
                Disconnect();
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_stream != null)
                    _stream.Close();
                if (_tcpClient != null)
                    _tcpClient.Close();

                if (LogMessage != null)
                    LogMessage("Disconnected");
            }
            catch { }
        }

        public async Task SendMessageAsync(string text)
        {
            if (!IsConnected || _stream == null) return;

            try
            {
                await SendAsync(string.Format("MESSAGE|{0}", text));
            }
            catch (Exception ex)
            {
                if (LogMessage != null)
                    LogMessage(string.Format("Send failed: {0}", ex.Message));
                Disconnect();
            }
        }

        private async Task SendAsync(string data)
        {
            if (_stream == null) return;

            byte[] buffer = Encoding.UTF8.GetBytes(data + "\n");
            await _stream.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task ReceiveMessagesAsync()
        {
            if (_stream == null) return;

            byte[] buffer = new byte[4096];

            try
            {
                while (IsConnected)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    ProcessReceivedData(data);
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

        private void ProcessReceivedData(string data)
        {
            var parts = data.Split(new char[] { '|' }, 2);
            if (parts.Length < 2) return;

            string command = parts[0];
            string content = parts[1];

            switch (command)
            {
                case "MESSAGE":
                    var message = Message.FromNetworkString(content);
                    if (MessageReceived != null)
                        MessageReceived(message);
                    break;

                case "USERLIST":
                    if (string.IsNullOrEmpty(content))
                        return;

                    var users = content.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                      .ToList();

                    if (UsersListUpdated != null)
                        UsersListUpdated(users);
                    break;

                case "ERROR":
                    if (LogMessage != null)
                        LogMessage(string.Format("Server error: {0}", content));
                    break;
            }
        }
    }
}
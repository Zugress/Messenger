using ChatClient.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }

        private ChatClient.Network.Client _client = new ChatClient.Network.Client();
        private System.Windows.Forms.TextBox _txtChat;
        private System.Windows.Forms.TextBox _txtMessage;
        private System.Windows.Forms.TextBox _txtIp;
        private System.Windows.Forms.TextBox _txtPort;
        private System.Windows.Forms.TextBox _txtUsername;
        private System.Windows.Forms.Button _btnConnect;
        private System.Windows.Forms.Button _btnDisconnect;
        private System.Windows.Forms.Button _btnSend;

        private void InitializeCustomComponents()
        {
            this.Text = "Мессенджер - Клиент";
            this.Size = new System.Drawing.Size(800, 600);

            // Создаем элементы в правильном порядке
            _txtChat = new System.Windows.Forms.TextBox
            {
                Multiline = true,
                ScrollBars = System.Windows.Forms.ScrollBars.Vertical,
                Dock = System.Windows.Forms.DockStyle.Fill,
                ReadOnly = true,
                Location = new System.Drawing.Point(0, 100), // Отступ сверху для панели
                Size = new System.Drawing.Size(600, 450)     // Размер с учетом других элементов
            };

            // Панель ввода сообщения (ВНИЗУ)
            var inputPanel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Bottom,
                Height = 50
            };

            _txtMessage = new System.Windows.Forms.TextBox
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Multiline = true
            };

            _btnSend = new System.Windows.Forms.Button
            {
                Text = "Отправить",
                Dock = System.Windows.Forms.DockStyle.Right,
                Width = 100,
                Enabled = false
            };

            inputPanel.Controls.Add(_btnSend);
            inputPanel.Controls.Add(_txtMessage);

            // Панель подключения (СВЕРХУ) - делаем ее последней
            var connectPanel = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Top,
                Height = 100
            };

            // Добавляем элементы на панель подключения
            var lblIp = new System.Windows.Forms.Label
            {
                Text = "IP:",
                Location = new System.Drawing.Point(10, 12),
                AutoSize = true
            };

            _txtIp = new System.Windows.Forms.TextBox
            {
                Text = "127.0.0.1",
                Location = new System.Drawing.Point(40, 10),
                Width = 100
            };

            var lblPort = new System.Windows.Forms.Label
            {
                Text = "Port:",
                Location = new System.Drawing.Point(150, 12),
                AutoSize = true
            };

            _txtPort = new System.Windows.Forms.TextBox
            {
                Text = "8888",
                Location = new System.Drawing.Point(190, 10),
                Width = 60
            };

            var lblUsername = new System.Windows.Forms.Label
            {
                Text = "Имя:",
                Location = new System.Drawing.Point(260, 12),
                AutoSize = true
            };

            _txtUsername = new System.Windows.Forms.TextBox
            {
                Text = "User1",
                Location = new System.Drawing.Point(300, 10),
                Width = 100
            };

            _btnConnect = new System.Windows.Forms.Button
            {
                Text = "Подключиться",
                Location = new System.Drawing.Point(410, 10),
                Width = 100
            };

            _btnDisconnect = new System.Windows.Forms.Button
            {
                Text = "Отключиться",
                Location = new System.Drawing.Point(520, 10),
                Width = 100,
                Enabled = false
            };

            connectPanel.Controls.Add(lblIp);
            connectPanel.Controls.Add(_txtIp);
            connectPanel.Controls.Add(lblPort);
            connectPanel.Controls.Add(_txtPort);
            connectPanel.Controls.Add(lblUsername);
            connectPanel.Controls.Add(_txtUsername);
            connectPanel.Controls.Add(_btnConnect);
            connectPanel.Controls.Add(_btnDisconnect);

            this.Controls.Add(_txtChat);
            this.Controls.Add(inputPanel);
            this.Controls.Add(connectPanel);

            _client.MessageReceived += OnMessageReceived;
            _client.LogMessage += OnLogMessage;

            _btnConnect.Click += BtnConnect_Click;
            _btnDisconnect.Click += BtnDisconnect_Click;
            _btnSend.Click += BtnSend_Click;
            _txtMessage.KeyDown += TxtMessage_KeyDown;
        }

        private void OnMessageReceived(ChatClient.Models.Message message)
        {
            if (_txtChat.InvokeRequired)
            {
                _txtChat.Invoke(new Action(() => _txtChat.AppendText(message.ToString() + Environment.NewLine)));
            }
            else
            {
                _txtChat.AppendText(message.ToString() + Environment.NewLine);
            }
        }

        private void OnLogMessage(string log)
        {
            if (_txtChat.InvokeRequired)
            {
                _txtChat.Invoke(new Action(() => _txtChat.AppendText("[System] " + log + Environment.NewLine)));
            }
            else
            {
                _txtChat.AppendText("[System] " + log + Environment.NewLine);
            }
        }

        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                int port = int.Parse(_txtPort.Text);
                await _client.ConnectAsync(_txtIp.Text, port, _txtUsername.Text);
                _btnConnect.Enabled = false;
                _btnDisconnect.Enabled = true;
                _btnSend.Enabled = true;
            }
            catch (Exception ex)
            {
                _txtChat.AppendText("[Error] " + ex.Message + Environment.NewLine);
            }
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            _client.Disconnect();
            _btnConnect.Enabled = true;
            _btnDisconnect.Enabled = false;
            _btnSend.Enabled = false;
        }

        private async void BtnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_txtMessage.Text))
            {
                await _client.SendMessageAsync(_txtMessage.Text);
                _txtMessage.Clear();
            }
        }

        private void TxtMessage_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter && !e.Shift && _btnSend.Enabled)
            {
                e.SuppressKeyPress = true;
                _btnSend.PerformClick();
            }
        }
    }

}

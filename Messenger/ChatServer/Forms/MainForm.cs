using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatServer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
        }
        private ChatServer.Network.Server _server = new ChatServer.Network.Server();

        private void InitializeCustomComponents()
        {
            this.Text = "Мессенджер - Сервер";

            var txtLog = new System.Windows.Forms.TextBox
            {
                Multiline = true,
                ScrollBars = System.Windows.Forms.ScrollBars.Vertical,
                Dock = System.Windows.Forms.DockStyle.Fill,
                ReadOnly = true
            };

            var btnStart = new System.Windows.Forms.Button
            {
                Text = "Запустить сервер",
                Dock = System.Windows.Forms.DockStyle.Top,
                Height = 40
            };
            var btnStop = new System.Windows.Forms.Button
            {
                Text = "Остановить сервер",
                Dock = System.Windows.Forms.DockStyle.Top,
                Height = 40,
                Enabled = false
            };

            this.Controls.Add(txtLog);
            this.Controls.Add(btnStop);
            this.Controls.Add(btnStart);

            _server.LogMessage += (message) =>
            {
                if (txtLog.InvokeRequired)
                {
                    txtLog.Invoke(new Action(() => txtLog.AppendText(message + Environment.NewLine)));
                }
                else
                {
                    txtLog.AppendText(message + Environment.NewLine);
                }
            };

            btnStart.Click += async (s, e) =>
            {
                try
                {
                    await _server.StartAsync(8888);
                    btnStart.Enabled = false;
                    btnStop.Enabled = true;
                }
                catch (Exception ex)
                {
                    txtLog.AppendText("[Error] " + ex.Message + Environment.NewLine);
                }
            };

            btnStop.Click += (s, e) =>
            {
                _server.Stop();
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            };
        }
    }

}

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
        private void InitializeCustomComponents()
        {
            this.Text = "Мессенджер - Клиент";

            var txtChat = new System.Windows.Forms.TextBox();
            txtChat.Multiline = true;
            txtChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtChat.Dock = System.Windows.Forms.DockStyle.Fill;
            txtChat.ReadOnly = true;

            var txtMessage = new System.Windows.Forms.TextBox();
            txtMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            txtMessage.Height = 50;

            var btnSend = new System.Windows.Forms.Button();
            btnSend.Text = "Отправить";
            btnSend.Dock = System.Windows.Forms.DockStyle.Right;
            btnSend.Width = 100;

            this.Controls.Add(txtChat);
            this.Controls.Add(txtMessage);
            this.Controls.Add(btnSend);

            btnSend.Click += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(txtMessage.Text))
                {
                    txtChat.AppendText($"[{DateTime.Now:HH:mm:ss}] Я: {txtMessage.Text}\r\n");
                    txtMessage.Clear();
                }
            };
        }
    }

}

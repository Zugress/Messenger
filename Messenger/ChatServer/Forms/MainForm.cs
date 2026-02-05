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
        private void InitializeCustomComponents()
        {
            this.Text = "Мессенджер - Сервер";

            var txtLog = new System.Windows.Forms.TextBox();
            txtLog.Multiline = true;
            txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            txtLog.ReadOnly = true;

            var btnStart = new System.Windows.Forms.Button();
            btnStart.Text = "Запустить сервер";
            btnStart.Dock = System.Windows.Forms.DockStyle.Top;
            btnStart.Height = 40;

            this.Controls.Add(txtLog);
            this.Controls.Add(btnStart);

            btnStart.Click += (sender, e) =>
            {
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] Сервер запущен\r\n");
                btnStart.Enabled = false;
            };
        }
    }

}

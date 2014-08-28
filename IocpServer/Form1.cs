using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IocpServer 
{
    public partial class MainForm : Form
    {
        public delegate void SetListBoxCallBack(string str);
        public SetListBoxCallBack setlistboxcallback;
        public delegate void SetTextBoxCallBack(string str);
        public SetTextBoxCallBack setTextBoxCallBack;
        public void SetListBox(string str)
        {
            infoList.Items.Insert(0, str);
            infoList.SelectedIndex = 0;
        }

        public void settext(string str)
        {
            txtMess.Text += str + "\r\n--------------------------\r\n";
        }

        private IoServer iocp = new IoServer(10, 1024);

        public MainForm()
        {
            InitializeComponent();
            setlistboxcallback = new SetListBoxCallBack(SetListBox);
            setTextBoxCallBack = new SetTextBoxCallBack(settext);
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            iocp.Start(9900);
            iocp.mainForm = this;
            startBtn.Enabled = false;
            stopBtn.Enabled = true;
            SetListBox("监听开启...");
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            iocp.Stop();
            startBtn.Enabled = true;
            stopBtn.Enabled = false;
            SetListBox("监听停止...");
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            if (stopBtn.Enabled)
                iocp.Stop();
            this.Close();
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            infoList.Items.Clear();
        }

    }
}

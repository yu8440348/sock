using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace IocpClient
{
    public partial class Form1 : Form
    {
        public Socket clientSk = null;
        SocketAsyncEventArgs ConnectSAE = new SocketAsyncEventArgs();
        SocketAsyncEventArgs SendSAE = new SocketAsyncEventArgs();
        SocketAsyncEventArgs RecieveSAE = new SocketAsyncEventArgs();
        private byte[] _sendBuf = new byte[11240];
        public Form1()
        {
            InitializeComponent();
            SetTextboxcallback = new SetTextbox(SetText);
            clientSk = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ConnectSAE.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9900);
            ConnectSAE.Completed += new EventHandler<SocketAsyncEventArgs>(ConnectSAE_Completed);
            this.SendSAE.SetBuffer(0, _sendBuf.Length);
            clientSk.ConnectAsync(ConnectSAE);

        }
        public delegate void SetTextbox(string str);
        public SetTextbox SetTextboxcallback;

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes("数据\r\n");
            int len = data.Length;
            //this._sendBuf = new byte[len];
            Array.Copy(data, 0, this._sendBuf, 0, data.Length);
            this.SendSAE.SetBuffer(0, len);
            //先调用异步接收，再调用异步发送。让你体验到异步明显非一般的感觉。
            clientSk.ReceiveAsync(RecieveSAE);
            clientSk.SendAsync(SendSAE);
        }


        private void ConnectSAE_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket clientSk = sender as Socket;
            if (e.SocketError == SocketError.Success && e.ConnectSocket!=null)
            {
                byte[] buffer = new byte[2048];
                SendSAE.Completed += new EventHandler<SocketAsyncEventArgs>(SendSAE_Completed);
                SendSAE.SetBuffer(this._sendBuf, 0, _sendBuf.Length);
                RecieveSAE.SetBuffer(buffer, 0, buffer.Length);
                RecieveSAE.Completed += new EventHandler<SocketAsyncEventArgs>(RecieveSAE_Completed);

            }
        }

        private void RecieveSAE_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket sk = sender as Socket;
            byte[] data = e.Buffer;  //注意这里，如何取关联到套接字的发送接受的缓冲区中的值。
            string msg = System.Text.Encoding.UTF8.GetString(data);
            this.Invoke(this.SetTextboxcallback, "接收消息: " + msg + "\r\n");

            //sk.DisconnectAsync();//你看看 该怎么做呢？
        }
        private void SendSAE_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket sk = sender as Socket;
            if (e.SocketError == SocketError.Success)
            {
                this.Invoke(this.SetTextboxcallback, "发送成功!\r\n");
            }
        }


        public void SetText(string str)
        {
            this.textBox1.Text +=str+ "--------------------------\r\n";
        }
    }
}

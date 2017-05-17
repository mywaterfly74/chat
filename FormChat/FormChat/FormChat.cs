using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormChat
{
    public partial class FormChat : Form
    {
        static TcpClient client;
        public static NetworkStream stream;
        public static void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        string message = builder.ToString();
                        listBoxChat.Items.Add(message);
                    }));
                }
                catch
                {
                    MessageBox.Show("Соединение с сервером потеряно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //соединение было прервано
                    Disconnect();
                    Close();
                }
            }
        }

        static void SendMessage(string newMessage)
        {
            try
            {
                string message = newMessage;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
            catch
            {
                MessageBox.Show("Соединение с сервером потеряно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //соединение было прервано
                Disconnect();
            }
        }
        public FormChat(string userName, TcpClient newClient)
        {
            client = newClient;
            stream = client.GetStream(); // получаем поток

            string message = userName;
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);

            // запускаем новый поток для получения данных
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();
            //старт потока            
            InitializeComponent();
        }

        public void Send()
        {
            if (textBoxMessage.Text != "")
            {
                SendMessage(textBoxMessage.Text);
                listBoxChat.Items.Add("Вы: " + textBoxMessage.Text);
                textBoxMessage.Clear();
            }
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            Send();
        }

        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void textBoxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Send();
            }
        }
    }
}

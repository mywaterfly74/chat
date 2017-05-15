using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace FormChat
{
    public partial class FormLogin : Form
    {
        public static TcpClient client;
        public static string userName;
        public static string host;
        public static int port;

        public FormLogin()
        {

            InitializeComponent();
        }

 
        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (textBoxPort.Text == "" || textBoxIP.Text == "" || textBoxNick.Text == "")
            {
                MessageBox.Show("Необходимо заполнить все поля", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                client = new TcpClient();
                userName = Convert.ToString(textBoxNick.Text);
                host = Convert.ToString(textBoxIP.Text);
                port = int.Parse(textBoxPort.Text);

                try
                {
                    client.Connect(host, port);
                    //подключение клиента
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Невозможно установить соединение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
                Hide();                                
                FormChat chat = new FormChat(userName, client);
                chat.Show();
            }
        }

        private void textBoxPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number))
            {
                e.Handled = true;
            }
        }
    }
}

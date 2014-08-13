using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WebSocket4Net;

namespace Admin_Window
{
    /*static class Program
    {
        static WebSocket webSocket;
        static Login login;
        static Form1 form;
        public static void Initialize_WebSocket()
        {
            System.IO.StreamReader reader = new System.IO.StreamReader("settings.txt");

            string server_Address = "";
            string port = "";
            while (!reader.EndOfStream)
            {
                string temp = reader.ReadLine();
                string variable = temp.Substring(0,temp.IndexOf(":"));
                string value =temp.Substring(temp.IndexOf(":")+1);

                if (variable == "websocket_Server_Address")
                    server_Address = value;
                if(variable == "port")
                    port = value;
            }
            webSocket = new WebSocket("ws://" + server_Address + ":" + port + "/");
            webSocket.Opened += new EventHandler(on_Connection_Opened);
            webSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(on_Message_Received);
            webSocket.Open();
        }

        private static void on_Connection_Opened(Object sender, EventArgs e)
        {
            MessageBox.Show("Connection got established with the server");
        }

        private static void on_Message_Received(Object sender, MessageReceivedEventArgs e)
        {
            if(e.Message == "Message|^|Welcome client")
            {
                return;
            }

            string type = e.Message.Substring(0, e.Message.IndexOf("|^|"));

            if (type == "Message")
            {
                //this.InitializeComponent();
                MessageBox.Show(e.Message.Substring(e.Message.IndexOf("|^|") + 3));
            }
            else if (type == "Loggin_Accepted")
            {
                MessageBox.Show("Login Accepted");
                login.Hide();
                form.Show();
            }
        }

        static void on_Login_Ok_Clicked(string password)
        {

            if (password == "203ECAD3-5F01-4AB8-93AE-4857389C43C9")
            {
                MessageBox.Show("Welcome Developer");
            }
            else
            {
                if (webSocket.State == WebSocketState.Open)
                    webSocket.Send("Admin_Logging_In|^|" + password);
                else
                    MessageBox.Show("Server is not running");
            }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main()
        {
            Initialize_WebSocket();
            form = new Form1();
            form.Hide();
            login = new Login();
            login.ok += new Login.ok_Clicked_Delegate(on_Login_Ok_Clicked);
            login.Show();
            Application.Run();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
    */

    class Temp_Application_Context : ApplicationContext
    {
        //form_Control_Panel form;
        //public Temp_Application_Context()
        //{
        //    form = new form_Control_Panel();
        //    form.Show();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
       //}
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Temp_Application_Context context = new Temp_Application_Context();
            //Application.Run(new Form1());
            //try
            //{
                Application.Run(new form_Control_Panel());
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //    System.Diagnostics.StackTrace temp = new System.Diagnostics.StackTrace(e);

            //    MessageBox.Show(temp.ToString());
            //}

        }
    }
}

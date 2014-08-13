using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

using MySql_CSharp_Console;
using Common_Classes;

using System.IO;

using SuperSocket.ClientEngine;
using WebSocket4Net;

// This is for system tray coordinates
using System.Runtime.InteropServices;

namespace Reminder
{
    public partial class Form_Reminder : Form
    {
        public static string company_Name;
        public static string username;
        DBConnect db_Connect;
        
        public static System.Drawing.Image white_Border = System.Drawing.Image.FromFile("white.jpg");
        public static System.Drawing.Image red_Border = System.Drawing.Image.FromFile("red.jpg");
        public static System.Drawing.Image green_Border = System.Drawing.Image.FromFile("green.jpg");
        public static System.Drawing.Image pink_Border = System.Drawing.Image.FromFile("pink.jpg");
        public static System.Drawing.Image i_Forgot_Jpg = System.Drawing.Image.FromFile("I_Forgot.jpg");

        public static string application_Dir = System.IO.Directory.GetCurrentDirectory();

        My_PictureBox clicked_My_PictureBox;
        Job_Display_Form job_Display_Form;
        List<My_PictureBox> picture_Boxes;

        WebSocket webSocket;
        string token;

        NotifyIcon notify_Icon;
        Useful_Functions.SimpleAES crypting_Object;
        System.Threading.ManualResetEvent reset_Event;
        System.Threading.ManualResetEvent login_Reset_Event;

        private string database_Server;
        private string database_Name;
        private string database_Uid;
        private string database_Password;

        public string ftp_Server;
        public string ftp_Username;
        public string ftp_Password;

        static string doc_To_Html_Server_Address = "";
        static string doc_To_Html_Server_Port = "";
        static string doc_To_Html_Server_Max_Connection = "";
        static string doc_To_Html_Server_Max_Command_Length = "";

        static string resume_View_Server_Address = "";
        static string resume_View_Server_Port = "";
        static string resume_View_Server_Max_Connection = "";
        static string resume_View_Server_Max_Command_Length = "";

        static string reminder_Server_Address = "";
        static string reminder_Server_Port = "";
        static string reminder_Server_Max_Connection = "";
        static string reminder_Server_Max_Command_Length = "";

        List<System.Threading.Thread> threads;

        bool exit_Requested;

        public Form_Reminder()
        {
            InitializeComponent();

            crypting_Object = new Useful_Functions.SimpleAES();
            load_Values_From_Registry();
            
            reset_Event = new System.Threading.ManualResetEvent(false);
            login_Reset_Event = new System.Threading.ManualResetEvent(false);
            db_Connect = new DBConnect(database_Server,database_Name,database_Uid,database_Password);

            wipe_Out_The_User_Name_In_The_Registry();
            LoginWindow login_Window = new LoginWindow();
            bool cancel_Pressed = false;
            Company_Info temp_Company_Info = null;
            login_Window.button_Ok_Login.Click += new EventHandler((sender_0, e_0) =>
            {
                if (login_Window.textBox_Username_Login.Text == "")
                {
                    MessageBox.Show(login_Window, "Username can't be Empty", "Suggestion");
                    return;
                }
                else if (login_Window.textBox_Password_Login.Text == "")
                {
                    MessageBox.Show(login_Window, "Password can't be Empty");
                    return;
                }
                string encrypted_General_password = crypting_Object.EncryptToString(login_Window.textBox_Password_Login.Text);
                if (!db_Connect.Check_User_Account(login_Window.textBox_Username_Login.Text, encrypted_General_password, company_Name))
                    MessageBox.Show(login_Window, "Username/Password is wrong");
                else
                {
                    temp_Company_Info = db_Connect.get_Company_Info_For_This_Company_Name(company_Name);
                    username = login_Window.textBox_Username_Login.Text;
                    put_This_User_Name_In_The_Registry(username);
                    login_Window.Hide();
                }
                login_Reset_Event.Set();
            });
            login_Window.button_Cancel_Login.Click += new EventHandler((sender_0, e_0) => { 
                login_Window.Hide();
                cancel_Pressed = true;
                login_Reset_Event.Set();
            });
            login_Window.FormClosed += new FormClosedEventHandler((sender_0, e_0) => {
                login_Window.Hide();
                cancel_Pressed = true;
                login_Reset_Event.Set();
            });
            login_Window.ShowDialog();
            login_Reset_Event.WaitOne();

            if (cancel_Pressed)
            {
                Environment.Exit(0);
            }

            if(temp_Company_Info != null)
                if(temp_Company_Info.message_Image_Path != "")
                {
                    Welcome_Window temp_Welcome_Window = new Welcome_Window(temp_Company_Info.message_Image_Path, temp_Company_Info.message);
                    temp_Welcome_Window.ShowDialog();
                }

            Initialize_WebSocket();
            reset_Event.WaitOne();

            this.MouseClick += new MouseEventHandler(form_Reminder_MouseClick);
            
            clicked_My_PictureBox = null;
            picture_Boxes = new List<My_PictureBox>();
            align_Picture_Boxes();

            this.Load += new EventHandler(Form_Reminder_Load);
            this.Resize += new EventHandler((sender,e) => {
                if (WindowState == FormWindowState.Minimized)
                    this.Hide();
            });
            this.MinimizeBox = true;
            this.MaximizeBox = false;

            job_Display_Form = new Job_Display_Form();
            job_Display_Form.Hide();

            threads = new List<System.Threading.Thread>();
            exit_Requested = false;
        }

        public void load_Values_From_Registry()
        {
            Microsoft.Win32.RegistryKey temp = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey temp_0 = temp.OpenSubKey("SOFTWARE", true);
            if (!temp_0.GetSubKeyNames().Contains("Resume View"))
            {
#if Console
                Console.WriteLine("There is no reg entry for the key value \"Resume View\"");
#endif
                return;
            }

            Microsoft.Win32.RegistryKey resume_View_Key = temp_0.OpenSubKey("Resume View", true);

            database_Server = (string)(resume_View_Key.GetValue("database_Server"));
            database_Server = crypting_Object.DecryptString(database_Server);
            database_Name = (string)(resume_View_Key.GetValue("database_Name"));
            database_Name = crypting_Object.DecryptString(database_Name);
            database_Uid = (string)(resume_View_Key.GetValue("database_Uid"));
            database_Uid = crypting_Object.DecryptString(database_Uid);
            database_Password = (string)(resume_View_Key.GetValue("database_Password"));
            database_Password = crypting_Object.DecryptString(database_Password);

            ftp_Server = (string)(resume_View_Key.GetValue("ftp_Server"));
            ftp_Server = crypting_Object.DecryptString(ftp_Server);
            ftp_Username = (string)(resume_View_Key.GetValue("ftp_Username"));
            ftp_Username = crypting_Object.DecryptString(ftp_Username);
            ftp_Password = (string)(resume_View_Key.GetValue("ftp_Password"));
            ftp_Password = crypting_Object.DecryptString(ftp_Password);

            doc_To_Html_Server_Address = (string)(resume_View_Key.GetValue("doc_To_Html_Server_Address"));
            doc_To_Html_Server_Address = crypting_Object.DecryptString(doc_To_Html_Server_Address);
            doc_To_Html_Server_Port = (string)(resume_View_Key.GetValue("doc_To_Html_Server_Port"));
            doc_To_Html_Server_Port = crypting_Object.DecryptString(doc_To_Html_Server_Port);
            doc_To_Html_Server_Max_Connection = (string)(resume_View_Key.GetValue("doc_To_Html_Server_Max_Connection"));
            doc_To_Html_Server_Max_Connection = crypting_Object.DecryptString(doc_To_Html_Server_Max_Connection);
            doc_To_Html_Server_Max_Command_Length = (string)(resume_View_Key.GetValue("doc_To_Html_Server_Max_Command_Length"));
            doc_To_Html_Server_Max_Command_Length = crypting_Object.DecryptString(doc_To_Html_Server_Max_Command_Length);

            resume_View_Server_Address = (string)(resume_View_Key.GetValue("resume_View_Server_Address"));
            resume_View_Server_Address = crypting_Object.DecryptString(resume_View_Server_Address);
            resume_View_Server_Port = (string)(resume_View_Key.GetValue("resume_View_Server_Port"));
            resume_View_Server_Port = crypting_Object.DecryptString(resume_View_Server_Port);
            resume_View_Server_Max_Connection = (string)(resume_View_Key.GetValue("resume_View_Server_Max_Connection"));
            resume_View_Server_Max_Connection = crypting_Object.DecryptString(resume_View_Server_Max_Connection);
            resume_View_Server_Max_Command_Length = (string)(resume_View_Key.GetValue("resume_View_Server_Max_Command_Length"));
            resume_View_Server_Max_Command_Length = crypting_Object.DecryptString(resume_View_Server_Max_Command_Length);

            reminder_Server_Address = (string)(resume_View_Key.GetValue("reminder_Server_Address"));
            reminder_Server_Address = crypting_Object.DecryptString(reminder_Server_Address);
            reminder_Server_Port = (string)(resume_View_Key.GetValue("reminder_Server_Port"));
            reminder_Server_Port = crypting_Object.DecryptString(reminder_Server_Port);
            reminder_Server_Max_Connection = (string)(resume_View_Key.GetValue("reminder_Server_Max_Connection"));
            reminder_Server_Max_Connection = crypting_Object.DecryptString(reminder_Server_Max_Connection);
            reminder_Server_Max_Command_Length = (string)(resume_View_Key.GetValue("reminder_Server_Max_Command_Length"));
            reminder_Server_Max_Command_Length = crypting_Object.DecryptString(reminder_Server_Max_Command_Length);

            company_Name = (string)(resume_View_Key.GetValue("company_Name"));
            company_Name = crypting_Object.DecryptString(company_Name);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_NOCLOSE = 0x200;

                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_NOCLOSE;
                return cp;
            }
        }

        void wipe_Out_The_User_Name_In_The_Registry()
        {
            Microsoft.Win32.RegistryKey temp = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey temp_0 = temp.OpenSubKey("SOFTWARE", true);
            if (!temp_0.GetSubKeyNames().Contains("Resume View"))
                temp_0.CreateSubKey("Resume View");

            Microsoft.Win32.RegistryKey resume_View_Key = temp_0.OpenSubKey("Resume View", true);
            resume_View_Key.SetValue("username", "");
        }

        void put_This_User_Name_In_The_Registry(string username)
        {
            Microsoft.Win32.RegistryKey temp = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey temp_0 = temp.OpenSubKey("SOFTWARE", true);
            if (!temp_0.GetSubKeyNames().Contains("Resume View"))
                temp_0.CreateSubKey("Resume View");

            Microsoft.Win32.RegistryKey resume_View_Key = temp_0.OpenSubKey("Resume View", true);
            resume_View_Key.SetValue("username", crypting_Object.EncryptToString(username));
        }

        void Form_Reminder_Load(object sender, EventArgs e)
        {
            initialize_Notify_Icon();
            notify_Icon.Visible = true;
            this.Hide();
        }

        public void initialize_Notify_Icon()
        {
            notify_Icon = new NotifyIcon();
            notify_Icon.Icon = new Icon("rv_Logo.ico",16,16);
            ContextMenu notify_Icon_Context_Menu = new System.Windows.Forms.ContextMenu();
            notify_Icon_Context_Menu.MenuItems.Add("Show Main Window", (sender, evt) => { 
                this.WindowState = FormWindowState.Maximized;
                this.Show();
            });
            notify_Icon_Context_Menu.MenuItems.Add("Exit",make_The_Logout_And_Exit);
            notify_Icon.ContextMenu = notify_Icon_Context_Menu;
            notify_Icon.Text = username;
        }

        void make_The_Logout_And_Exit(Object sender,EventArgs evt)
        {
            exit_Requested = true;
            wipe_Out_The_User_Name_In_The_Registry();
            webSocket.Close();
        }

        public void Initialize_WebSocket()
        {
            webSocket = new WebSocket("ws://" + reminder_Server_Address + ":" + reminder_Server_Port + "/");
            webSocket.Opened += new EventHandler(on_Connection_Opened);
            webSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(on_Message_Received);
            webSocket.Closed += new EventHandler(on_WebSocket_Closed);
            webSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(webSocket_Error);
            webSocket.Open();
        }

        void webSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            MessageBox.Show("Connection can't be made, Because Server is not running:" + e.Exception.Message);
            //MessageBox.Show("Connection can't be made, Because Server is not running");
            Application.Exit();
        }

        private void on_Connection_Opened(Object sender, EventArgs e)
        {
            reset_Event.Set();
            webSocket.Send("Get_Token|^|"+username+"|^|"+company_Name);
        }

        void on_WebSocket_Closed(object sender, EventArgs e)
        {
            if(!exit_Requested)
                MessageBox.Show("Connection got cut,Server closed the connection. Aborting");
            for (int i = 0; i < threads.Count; ++i)
                threads[i].Abort();
            Application.Exit();
        }

        private void on_Message_Received(Object sender, MessageReceivedEventArgs e)
        {
            string[] collection = e.Message.Split(new string[] {"|^|"},StringSplitOptions.None);
            string type = collection[0];

            if (type == "Identify_Yourself")
            {
                webSocket.Send("Get_Token|^|" + username + "|^|" + company_Name);
            }
            else if (type == "Refresh")
            {
                align_Picture_Boxes();
                return;
            }
            else if (type == "Show")
            {
                string subject = collection[1];
                string body = collection[2];
                string string_Datetime = collection[3];
                DateTime temp_Date = new DateTime(1, 1, 1);
                DateTime.TryParseExact(string_Datetime, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out temp_Date);
                string status = collection[4];
                Image back_Ground_Image = null;
                if (status == "Start")
                    back_Ground_Image = Image.FromFile("green.jpg");
                else if (status == "Middle")
                    back_Ground_Image = Image.FromFile("pink.jpg");
                else if (status == "End")
                    back_Ground_Image = Image.FromFile("red.jpg");
                else
                    back_Ground_Image = Image.FromFile("white.jpg");

                System.Threading.Thread temp_Thread = new System.Threading.Thread(() =>
                {
                    show_Notification(subject, body, temp_Date,back_Ground_Image);
                });
                temp_Thread.IsBackground = true;
                threads.Add(temp_Thread);
                temp_Thread.Start();
            }
            else if (type == "Message")
            {
                string content = collection[1];
                MessageBox.Show(content);
            }
            else if (type == "Logout")
            {
                string content = collection[1];
                MessageBox.Show(content);
            }
        }

        void show_Notification(string subject,string body,DateTime temp_Date,Image given_Border)
        {
            form_Notification temp_Form = new form_Notification(subject, body, temp_Date);
            //notification_Forms.Add(temp_Form);
            temp_Form.BackgroundImage = given_Border;
            temp_Form.button_Dismiss.Click += new EventHandler((sender_0, evt_0) =>
            {
                form_Notification temp_Form_Notification = (form_Notification)(((Button)(sender_0)).Parent);
                temp_Form_Notification.Close();
            });

            temp_Form.Load += new EventHandler((sender_Object, evt) =>
            {
                Form temp_Form_1 = (Form)(sender_Object);
                int x = Screen.PrimaryScreen.WorkingArea.Width - temp_Form_1.Width;
                int y = Screen.PrimaryScreen.WorkingArea.Height - temp_Form_1.Height;
                temp_Form_1.Location = new Point(x, y);
            });
            temp_Form.ShowDialog();
        }

        #region Gives the rectangle of the system tray => Code was taken from the below page
        //http://stackoverflow.com/questions/7294878/position-form-above-the-clicked-notify-icon

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, IntPtr windowTitle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


        public static IntPtr GetTrayHandle()
        {
            IntPtr taskBarHandle = FindWindow("Shell_TrayWnd", null);
            if (!taskBarHandle.Equals(IntPtr.Zero))
            {
                return FindWindowEx(taskBarHandle, IntPtr.Zero, "TrayNotifyWnd", IntPtr.Zero);
            }
            return IntPtr.Zero;
        }

        public static Rectangle GetTrayRectangle()
        {
            RECT rect;
            GetWindowRect(GetTrayHandle(), out rect);
            return new Rectangle(new Point(rect.left, rect.top), new Size((rect.right - rect.left) + 1, (rect.bottom - rect.top) + 1));
        }

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public override string ToString()
            {
                return "(" + left + ", " + top + ") --> (" + right + ", " + bottom + ")";
            }
        }
        #endregion

        void form_Reminder_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
                return;
            ContextMenu temp_ContextMenu = new ContextMenu();
            MenuItem add_Reminder_MenuItem = new MenuItem("Add Reminder", add_Reminder_MenuItem_Click);
            temp_ContextMenu.MenuItems.Add(add_Reminder_MenuItem);
            Object temp = GetChildAtPoint(e.Location);
            if (temp != null)
                if (temp.ToString().Contains("My_PictureBox"))
                {
                    clicked_My_PictureBox = (My_PictureBox)(temp);
                    MenuItem delete_Reminder_MenuItem = new MenuItem("Delete Reminder", delete_Reminder_MenuItem_Click);
                    temp_ContextMenu.MenuItems.Add(delete_Reminder_MenuItem);
                }
            MenuItem settings_MenuItem = new MenuItem("Settings", settings_Click);
            temp_ContextMenu.MenuItems.Add(settings_MenuItem);
            temp_ContextMenu.Show(this, e.Location);
        }

        void settings_Click(object sender, EventArgs e)
        {
            User_Detail temp_User_Detail = db_Connect.get_User_Details_For_This_User(username,company_Name);
            Form_Settings temp_Form_Settings = new Form_Settings("Gmail", "", "",crypting_Object.DecryptString(temp_User_Detail.encrypted_General_Password));
            temp_Form_Settings.button_Cancel_Settings_Form.Click +=new EventHandler((sender_1,evt_1)=>{
                Form_Settings temp_Form = (Form_Settings)((Button)(sender_1)).Parent;
                temp_Form.Hide();
            });
            temp_Form_Settings.button_Ok_Settings_Form.Click += new EventHandler((sender_2, evt_2) =>
            {
                Form_Settings temp_Form = (Form_Settings)((Button)(sender_2)).Parent;
                db_Connect.update_Settings_For_This_User(username,company_Name,
                    temp_Form.textBox_Mail_Id.Text,
                    crypting_Object.EncryptToString(temp_Form.textBox_Mail_Password.Text),
                    crypting_Object.EncryptToString(temp_Form.textBox_General_Password.Text));
                temp_Form.Hide();
            });
            temp_Form_Settings.ShowDialog();
        }

        void add_Reminder_MenuItem_Click(object sender, EventArgs e)
        {
            if(db_Connect.get_User_Details_For_This_User(username,company_Name).mail_Id == "")
            {
                MessageBox.Show("Please set the Mail Id First");
                settings_Click(null, EventArgs.Empty);
                return;
            }
            if(db_Connect.get_User_Details_For_This_User(username,company_Name).encrypted_Mail_Password == "")
            {
                MessageBox.Show("Please set the Mail Password First");
                settings_Click(null, EventArgs.Empty);
                return;
            }

            Reminder_Creator_Form job_Form = new Reminder_Creator_Form("Add",database_Server,database_Name,database_Uid,database_Password);
            string[] users = db_Connect.get_Users_Details(company_Name).Keys.ToArray();
            job_Form.checkedListBox_Receivers_Reminder_Creator_Form.Items.AddRange(users);
            job_Form.button_Add_Or_Update_Reminder_Creator_Form.Click += new EventHandler(button_Add_Or_Update_Reminder_Creator_Form_Click);
            job_Form.ShowDialog();
        }

        void delete_Reminder_MenuItem_Click(object sender, EventArgs e)
        {
            if (clicked_My_PictureBox != null)
            {
                clicked_My_PictureBox.Scale(new SizeF((float)2,(float)2));
            }
            if (MessageBox.Show("Are you sure that you want to delete this reminder", "Caption", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (clicked_My_PictureBox != null)
                    db_Connect.delete_This_Reminder(clicked_My_PictureBox.job.id);
                align_Picture_Boxes();
            }
            else
            {
                clicked_My_PictureBox.Scale(new SizeF((float)0.5, (float)0.5));
            }
        }

        void button_Add_Or_Update_Reminder_Creator_Form_Click(object sender, EventArgs e)
        {
            Reminder_Creator_Form reminder_Creator_Form = (Reminder_Creator_Form)(((Button)(sender)).Parent);
            string error_Message="";
            if (!reminder_Creator_Form.check_Form(ref error_Message))
            {
                MessageBox.Show(error_Message);
                return;
            }
            //List<string> temp = reminder_Creator_Form.checkedListBox_Receivers_Reminder_Creator_Form.CheckedItems.Select(x => x.ToString()).ToList();

            string server_Side_File_Name = "remind_"+Useful_Functions.Useful_Functions.get_Guid_String()+reminder_Creator_Form.image_Path.Substring(reminder_Creator_Form.image_Path.LastIndexOf("."));
            string image_Path = upload_The_File_Write_It_In_Database(server_Side_File_Name, reminder_Creator_Form.image_Path);
            if (image_Path == "")
            {
                MessageBox.Show("Error in image uploading,Reminder can't be saved");
                return;
            }

            reminder_Creator_Form.Hide();
            Job.Time_Gap_Type time_Gap_Of_Minutes_Or_Hours = Job.Time_Gap_Type.Hour;
            if (reminder_Creator_Form.radioButton_Hour_Reminder_Creator_Form.Checked)
                time_Gap_Of_Minutes_Or_Hours = Job.Time_Gap_Type.Hour;
            else if (reminder_Creator_Form.radioButton_Minute_Reminder_Creator_Form.Checked)
                time_Gap_Of_Minutes_Or_Hours = Job.Time_Gap_Type.Minute;

            List<Job.Reminder_Type> given_Reminder_Types = new List<Job.Reminder_Type>();
            if(reminder_Creator_Form.checkBox_Sms_Reminder_Creator_Form.Checked)
                given_Reminder_Types.Add(Job.Reminder_Type.SMS);
            if(reminder_Creator_Form.checkBox_Mail_Remainder_Creator_Form.Checked)
                given_Reminder_Types.Add(Job.Reminder_Type.Mail);
            if(reminder_Creator_Form.checkBox_Computer_Reminder_Creator_Form.Checked)
                given_Reminder_Types.Add(Job.Reminder_Type.Computer);

            List<string> job_Doiers_Names = reminder_Creator_Form.checkedListBox_Receivers_Reminder_Creator_Form.CheckedItems.Cast<string>().ToList();
            List<string> job_Doiers_Mail_Ids = (from f in job_Doiers_Names select db_Connect.get_Mail_Id_For_This_Username(f,company_Name)).ToList();
            List<string> job_Doiers_Mobile_Nos = (from f in job_Doiers_Names select db_Connect.get_Phone_No_For_This_Username(f,company_Name)).ToList();

            TimeZoneInfo temp_Time_Zone_Info = TimeZoneInfo.Local;
            Job temp_Job = new Job(image_Path,
                 company_Name,
                 job_Doiers_Names,
                 job_Doiers_Mail_Ids,
                 job_Doiers_Mobile_Nos,
                 username,
                 reminder_Creator_Form.textBox_Subject_Reminder_Creator_Form.Text,
                 reminder_Creator_Form.textBox_Body_Reminder_Creator_Form.Text,
                 reminder_Creator_Form.textBox_Notes_Reminder_Creator_Form.Text,
                 Convert.ToInt32(reminder_Creator_Form.numericUpDown_Reminder_Creator_Form.Value),
                 time_Gap_Of_Minutes_Or_Hours,
                 given_Reminder_Types,
                 DateTime.Now.ToUniversalTime(),
                 reminder_Creator_Form.dateTimePicker_Start_Reminder_Creator_Form.Value.ToUniversalTime(),
                 reminder_Creator_Form.dateTimePicker_DeadLine_Reminder_Creator_Form.Value.ToUniversalTime(),
                 "remind_"+Useful_Functions.Useful_Functions.get_Guid_String(),
                 new DateTime(1,1,1),
                 temp_Time_Zone_Info);
                 db_Connect.save_This_Reminder_In_Database(temp_Job);
                 align_Picture_Boxes();
        }

        string upload_The_File_Write_It_In_Database(string server_Side_File_Name, string local_File_Path) // Returns the web address of the image
        {
            string web_Base_Address = db_Connect.get_WebPage_Base_Address_For_This_Company(company_Name);
            string uri = String.Format("ftp://"+web_Base_Address+"Photos/{1}", ftp_Server, server_Side_File_Name);
            try
            {
                WebClient temp_WebClient = new WebClient();
                temp_WebClient.Credentials = new NetworkCredential(ftp_Username, ftp_Password);
                temp_WebClient.UploadFile(new Uri(uri), local_File_Path);
            }
            catch (Exception excp)
            {
                MessageBox.Show("There is a problem in network, File uploading getting cancelled. Try Later");
                return "";
            }
            return uri.Replace("ftp:","http:");
        }

        void align_Picture_Boxes()
        {
            List<Job> temp_Jobs = db_Connect.get_All_Reminders_Created_By_This_Person(username, company_Name);
            DateTime current_Time = DateTime.Now;

            foreach (My_PictureBox temp_My_PictureBox in picture_Boxes)
                this.Controls.Remove(temp_My_PictureBox);
            picture_Boxes.Clear();

            List<Job> beyond_The_Limit_Jobs = new List<Job>();
            List<Job> end_Jobs = new List<Job>();
            List<Job> middle_Jobs = new List<Job>();
            List<Job> start_Jobs = new List<Job>();
            for (int i = 0; i < temp_Jobs.Count; ++i)
            {
                if (temp_Jobs[i].get_Status(current_Time) == Job.Status.Beyond_The_Limit)
                    beyond_The_Limit_Jobs.Add(temp_Jobs[i]);
                if (temp_Jobs[i].get_Status(current_Time) == Job.Status.End)
                    end_Jobs.Add(temp_Jobs[i]);
                if (temp_Jobs[i].get_Status(current_Time) == Job.Status.Middle)
                    middle_Jobs.Add(temp_Jobs[i]);
                if (temp_Jobs[i].get_Status(current_Time) == Job.Status.Start)
                    start_Jobs.Add(temp_Jobs[i]);
            }

            List<Job> sorted_Jobs = new List<Job>();
            sorted_Jobs.AddRange(beyond_The_Limit_Jobs);
            sorted_Jobs.AddRange(end_Jobs);
            sorted_Jobs.AddRange(middle_Jobs);
            sorted_Jobs.AddRange(start_Jobs);

            int column_Count = 8;
            for (int i = 0; i < sorted_Jobs.Count; ++i)
            {
                My_PictureBox temp_My_PictureBox = new My_PictureBox(sorted_Jobs[i]);
                int x = ((i % column_Count) * (temp_My_PictureBox.width) )+ ((i%column_Count) * 20)+20;
                int y =  ((i / column_Count) * temp_My_PictureBox.height)+((i/column_Count)*12)+10;
                temp_My_PictureBox.Location = new Point(x,y);
                temp_My_PictureBox.MouseHover += new EventHandler(mouse_Hovered);
                temp_My_PictureBox.MouseLeave +=new EventHandler((sender,e)=>
                {
                    job_Display_Form.Hide();
                });
                picture_Boxes.Add(temp_My_PictureBox);
                this.Controls.Add(temp_My_PictureBox);
            }

            clicked_My_PictureBox = null;
        }

        void mouse_Hovered(Object sender,EventArgs e)
        {
            MessageBox.Show("AAA");
            //job_Display_Form.update(sorted_Jobs[i]);
            job_Display_Form.Show();
        }
    }


}

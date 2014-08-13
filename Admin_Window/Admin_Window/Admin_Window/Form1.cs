using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WebSocket4Net;
using Microsoft.Office.Interop.Word;

using MySql_CSharp_Console;
using MySql.Data.MySqlClient;

using Common_Classes;

using Useful_Functions;

namespace Admin_Window
{
    public partial class form_Control_Panel : Form
    {
        static WebSocket webSocket;
        //form_Control_Panel form;
        Login login;
        Change_Password change_Password_Window;
        internal static System.Collections.Concurrent.ConcurrentDictionary<string, User_Detail> users_Details; //username,password

        MySqlConnection mySqlConnection;
        MySqlDataAdapter mySqlDataAdapter;
        MySqlCommandBuilder mySqlCommandBuilder;
        DataTable dataTable;
        BindingSource bindingSource;

        private DBConnect database_Connection;

        string mysql_Server;
        string mysql_Database_Name;
        string mysql_Database_Username;
        string mysql_Database_Password;
        string mysql_Database_Connection_String;

        string data_Query_String;
        string pattern_String;
        int alias_Count;

        static string company_Name;
        string token;

        static public string ftp_Server;
        static public string ftp_Username;
        static public string ftp_Password;

        static string upload_Folder_Path;
        static long continue_Upload;
        static System.Threading.Thread file_Upload_Thread;
        static DateTime upload_Start_Time;
        static readonly Object locker = new object();

        Thread refreshing_Thread;
        bool thread_Stop;
        string view;
        bool update_Users_Details;
        bool qualified_Count_Status;
        internal static System.Collections.Concurrent.ConcurrentDictionary<string, Interview_And_Candidates_Count_Detail> Interviews_And_Candidates_Count_Details = null;   //Dict of <Interviewname,filtyered_Persons_count>
        bool filtered_Counts_Refreshed_Status;
        bool remaining_SMS_Count_Changed;

        double remaining_SMS_Count;

        User_Display_Form list_View_Users_User_Display_Form;

        Useful_Functions.SimpleAES crypting_Object;

        string web_Base_Address;

        private void bind(string query)
        {
            mySqlDataAdapter = new MySqlDataAdapter(query, mySqlConnection);
            mySqlCommandBuilder = new MySqlCommandBuilder(mySqlDataAdapter);

            //mySqlDataAdapter.UpdateCommand = mySqlCommandBuilder.GetUpdateCommand();
            //mySqlDataAdapter.DeleteCommand = mySqlCommandBuilder.GetDeleteCommand();
            //mySqlDataAdapter.InsertCommand = mySqlCommandBuilder.GetInsertCommand();

            dataTable = new DataTable();
            mySqlDataAdapter.Fill(dataTable);

            bindingSource = new BindingSource();
            bindingSource.DataSource = dataTable;

            SetControlPropertyThreadSafe(dataGridView1, "DataSource", bindingSource);
        }

        private void dataGridView_Cell_Got_Clicked(object o, DataGridViewCellMouseEventArgs e)
        {
            Record temp = new Record();

            temp.textBox_Name_Record.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            temp.textBox_PhoneNo_Record.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            temp.textBox_MailId_Record.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            temp.textBox_Uri_Record.Text = dataGridView1.Rows[e.RowIndex].Cells["local_Url"].Value.ToString();
            temp.main_Uri = dataGridView1.Rows[e.RowIndex].Cells["local_Url"].Value.ToString();
            
            temp.button_Select_Record.Click += new EventHandler(button_Select_Record_Clicked);
            temp.button_History_Record.Click += new EventHandler(button_History_Record_Clicked);

            foreach (Interview_And_Candidates_Count_Detail interview_Detail in Interviews_And_Candidates_Count_Details.Values)
                if (interview_Detail.date > DateTime.Now)
                    temp.comboBox_Interview_Record.Items.Add(interview_Detail.interview_Name);

            if (dataGridView1.Rows[e.RowIndex].Cells["call_Result"].Value.ToString() == "True")
            {
                temp.textBox_Qualified_Status_Record.Text = "Qualified";
                temp.textBox_Qualified_Status_Record.BackColor = System.Drawing.Color.LightGreen;
            }
            else if (dataGridView1.Rows[e.RowIndex].Cells["call_Result"].Value.ToString() == "False")
            {
                temp.textBox_Qualified_Status_Record.Text = "Not Qualified";
                temp.textBox_Qualified_Status_Record.BackColor = System.Drawing.Color.LightPink;
            }

            if (dataGridView1.Rows[e.RowIndex].Cells["attended_Status"].Value.ToString() == "True")
            {
                temp.textBox_Attended_Status_Record.Text = "Attended";
                temp.textBox_Attended_Status_Record.BackColor = System.Drawing.Color.LightGreen;
            }
            else if (dataGridView1.Rows[e.RowIndex].Cells["attended_Status"].Value.ToString() == "False")
            {
                temp.textBox_Attended_Status_Record.Text = "Didn't Attend";
                temp.textBox_Attended_Status_Record.BackColor = System.Drawing.Color.LightPink;
            }

            if (dataGridView1.Rows[e.RowIndex].Cells["selected_Status"].Value.ToString() == "True")
            {
                temp.textBox_Selected_Status_Record.Text = "Selected";
                temp.textBox_Selected_Status_Record.BackColor = System.Drawing.Color.LightGreen;
            }
            else if (dataGridView1.Rows[e.RowIndex].Cells["selected_Status"].Value.ToString() == "False")
            {
                temp.textBox_Selected_Status_Record.Text = "Got Rejected";
                temp.textBox_Selected_Status_Record.BackColor = System.Drawing.Color.LightPink;
            }

            if (dataGridView1.Rows[e.RowIndex].Cells["joined_Status"].Value.ToString() == "True")
            {
                temp.textBox_Joined_Status_Record.Text = "Joined";
                temp.textBox_Joined_Status_Record.BackColor = System.Drawing.Color.LightGreen;
            }
            else if (dataGridView1.Rows[e.RowIndex].Cells["joined_Status"].Value.ToString() == "False")
            {
                temp.textBox_Joined_Status_Record.Text = "Didn't Join";
                temp.textBox_Joined_Status_Record.BackColor = System.Drawing.Color.LightPink;
            }

            temp.Show();
        }

        private void button_Select_Record_Clicked(object sender, EventArgs e)
        {
            Button temp_Select_Button = (Button)(sender);
            Record temp_Record_Form = (Record)(temp_Select_Button.Parent);
            string interview_Name =  temp_Record_Form.comboBox_Interview_Record.SelectedItem.ToString();
            //format is Admin_Add_This_Url_In_This_Interview|^|name|^|phoneNo|^|mailId|^|interview_Name|^|time_String|^|Url;
            string name = temp_Record_Form.textBox_Name_Record.Text;
            string phoneNo = temp_Record_Form.textBox_PhoneNo_Record.Text;
            string mailId = temp_Record_Form.textBox_MailId_Record.Text;
            webSocket.Send("Admin_Add_This_Url_In_This_Interview|^|"+name+"|^|"+phoneNo+"|^|"+mailId+"|^|"+interview_Name+"|^|"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +"|^|"+temp_Record_Form.main_Uri+"|^|"+company_Name);
            temp_Record_Form.Close();
        }

        private void button_History_Record_Clicked(object sender, EventArgs e)
        {
            Button temp_Select_Button = (Button)(sender);
            Record temp_Record_Form = (Record)(temp_Select_Button.Parent);
            webSocket.Send("Admin_Get_History_For_This_Url|^|"+temp_Record_Form.main_Uri+"|^|"+company_Name);
        }

        /*private void bind_Null_Table()
        {
            string query = "SELECT * FROM rv_0";

            mySqlDataAdapter = new MySqlDataAdapter(query, mySqlConnection);
            mySqlCommandBuilder = new MySqlCommandBuilder(mySqlDataAdapter);

            //mySqlDataAdapter.UpdateCommand = mySqlCommandBuilder.GetUpdateCommand();
            //mySqlDataAdapter.DeleteCommand = mySqlCommandBuilder.GetDeleteCommand();
            //mySqlDataAdapter.InsertCommand = mySqlCommandBuilder.GetInsertCommand();

            dataTable = new DataTable();
            mySqlDataAdapter.Fill(dataTable);

            bindingSource = new BindingSource();
            bindingSource.DataSource = dataTable;

            //SetControlPropertyThreadSafe(dataGridView1, "DataSource", bindingSource);
            //dataGridView1.DataSource = bindingSource;
        }
         * */

        
        void update_UI_With_Data()
        {
            while (!thread_Stop)
            {
                if (view == "login_View")
                {
                    change_Into_Log_In_View();
                    view = "";
                }
                else if (view == "logout_View")
                {
                    change_Into_Log_Out_View();
                    view = "";
                }
                else if (view == "initial View")
                {
                    change_Into_Log_Out_View();
                    SetControlPropertyThreadSafe(button_Login, "Enabled", false);
                    SetControlPropertyThreadSafe(button_Connect_Server, "Enabled", true);
                    SetControlPropertyThreadSafe(button_Connect_Server, "BackgroundImage", null);
                    users_Details.Clear();
                    refresh_ListView_Users();
                    refresh_ComboBox_Coordinator();
                    Interviews_And_Candidates_Count_Details.Clear();
                    refresh_ListView_Candidates();

                    view = "";
                }


                if (update_Users_Details)
                {
                    refresh_ListView_Users();
                    refresh_ComboBox_Coordinator();
                    update_Users_Details = false;
                }

                if (qualified_Count_Status)
                    qualified_Count_Status = false;

                if (filtered_Counts_Refreshed_Status)
                {
                    clear_Interview_Related_Fields();
                    refresh_ListView_Candidates();
                    filtered_Counts_Refreshed_Status = false;
                }

                if (remaining_SMS_Count_Changed)
                {
                    SetControlPropertyThreadSafe(label_Remaining_SMS_Count, "Text", remaining_SMS_Count.ToString());
                    remaining_SMS_Count_Changed = false;
                }

                Thread.Sleep(1000);
            }
        }

        private delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);

        public static void SetControlPropertyThreadSafe(Control control, string propertyName, object propertyValue)
        {
            if (control.InvokeRequired)
                control.Invoke(new SetControlPropertyThreadSafeDelegate(SetControlPropertyThreadSafe), new object[] { control, propertyName, propertyValue });
            else
                control.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.SetProperty, null, control, new object[] { propertyValue });
        }

        private void load_Mysql_Settings()
        {
            Microsoft.Win32.RegistryKey temp = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey temp_0 = temp.OpenSubKey("SOFTWARE", true);
            if (!temp_0.GetSubKeyNames().Contains("Resume View"))
            {
                Console.WriteLine("There is no reg entry for the key value \"Resume View\"");
                return;
            }

            Microsoft.Win32.RegistryKey resume_View_Key = temp_0.OpenSubKey("Resume View", true);

            mysql_Server = (string)(resume_View_Key.GetValue("database_Server"));
            mysql_Server = crypting_Object.DecryptString(mysql_Server);
            mysql_Database_Name = (string)(resume_View_Key.GetValue("database_Name"));
            mysql_Database_Name = crypting_Object.DecryptString(mysql_Database_Name);
            mysql_Database_Username = (string)(resume_View_Key.GetValue("database_Uid"));
            mysql_Database_Username = crypting_Object.DecryptString(mysql_Database_Username);
            mysql_Database_Password = (string)(resume_View_Key.GetValue("database_Password"));
            mysql_Database_Password = crypting_Object.DecryptString(mysql_Database_Password);
            company_Name = (string)(resume_View_Key.GetValue("company_Name"));
        }

        public void load_Parameters_From_Registry()
        {
            Microsoft.Win32.RegistryKey temp = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey temp_0 = temp.OpenSubKey("SOFTWARE", true);
            if (!temp_0.GetSubKeyNames().Contains("Resume View"))
            {
                Console.WriteLine("There is no reg entry for the key value \"Resume View\"");
                return;
            }

            Microsoft.Win32.RegistryKey resume_View_Key = temp_0.OpenSubKey("Resume View", true);

            company_Name = (string)(resume_View_Key.GetValue("company_Name"));
        }

        public form_Control_Panel()
        {
            InitializeComponent();
            crypting_Object = new Useful_Functions.SimpleAES();
            database_Connection = new DBConnect();

            this.FormClosed += new FormClosedEventHandler(form_Control_Panel_FormClosed);

            refreshing_Thread = new Thread(new ThreadStart(this.update_UI_With_Data));
            view = "";
            update_Users_Details = false;
            qualified_Count_Status = false;
            filtered_Counts_Refreshed_Status = false;
            remaining_SMS_Count_Changed = false;
            thread_Stop = false;

            load_Mysql_Settings();
            load_Parameters_From_Registry();
            mySqlConnection = null;

            mysql_Database_Connection_String = ("SERVER=" + mysql_Server + ";" + "DATABASE=" + mysql_Database_Name + ";" + "UID=" + mysql_Database_Username + ";" + "PASSWORD=" + mysql_Database_Password + ";");
            mySqlConnection = new MySqlConnection(mysql_Database_Connection_String);
            try
            {
                mySqlConnection.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show("Mysql Server is not running, Start it \nThen start this Application .\nExiting");
                Environment.Exit(0);
            }

            refreshing_Thread.Start();
            change_Password_Window = null;
            data_Query_String = "";
            pattern_String = "";
            alias_Count = 0;
            Interviews_And_Candidates_Count_Details = new System.Collections.Concurrent.ConcurrentDictionary<string, Interview_And_Candidates_Count_Detail>();
            users_Details = new System.Collections.Concurrent.ConcurrentDictionary<string, User_Detail>();

            file_Upload_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(upload_The_Local_Resumes));
            file_Upload_Thread.IsBackground = true;
            upload_Folder_Path = "";
            continue_Upload = 0;
            initialize_FTP_Settings();
            // the below are related with bind function
            dataGridView1.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(dataGridView_Cell_Got_Clicked);
            listView_Users_Details.MouseUp += new MouseEventHandler(listView_Users_Details_MouseUp);
            listView_Users_Details.ItemMouseHover += new ListViewItemMouseHoverEventHandler(listView_Users_Details_ItemMouseHover);
            //listView_Users_Details.MouseMove += new MouseEventHandler(listView_Users_Details_MouseMove);
            listView_Users_Details.LostFocus += new EventHandler(listView_Users_Details_LostFocus);
            listView_Users_Details.MouseLeave += new EventHandler(listView_Users_Details_MouseLeave);
            list_View_Users_User_Display_Form = new User_Display_Form();
        }

        void listView_Users_Details_MouseLeave(object sender, EventArgs e)
        {
            list_View_Users_User_Display_Form.Hide();
        }

        void listView_Users_Details_LostFocus(object sender, EventArgs e)
        {
            list_View_Users_User_Display_Form.Hide();
        }

        /*void listView_Users_Details_MouseMove(object sender, MouseEventArgs e)
        {
            mouse_Position_ListView_Users_Details = e.Location;
            ListViewItem listViewItem_Under_Mouse = listView_Users_Details.HitTest(e.Location).Item;
            if(listViewItem_Under_Mouse == null)
                if (list_View_Users_User_Display_Form != null)
                    list_View_Users_User_Display_Form.Hide();
        }*/

        void listView_Users_Details_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            string username = e.Item.SubItems[0].Text;
            //User_Detail user_Detail = database_Connection.get_User_Details_For_This_User(username, company_Name);
            User_Detail user_Detail = null;
            if (users_Details.ContainsKey(username))
                user_Detail = users_Details[username];

            list_View_Users_User_Display_Form.update(user_Detail);
            Point a = new Point(Control.MousePosition.X + 10, Control.MousePosition.Y + 10);
            list_View_Users_User_Display_Form.Location = a;
            list_View_Users_User_Display_Form.Show();
        }

        void listView_Users_Details_MouseUp(object sender, MouseEventArgs e)
        {
            //list_View_Users_User_Display_Form.Hide();
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ListViewItem clicked_Item = listView_Users_Details.HitTest(e.Location).Item;
                ContextMenu temp_ContextMenu = new ContextMenu();
                MenuItem add_User_Menu_Item = new MenuItem("Add Account", add_User);
                temp_ContextMenu.MenuItems.Add(add_User_Menu_Item);
                if (clicked_Item != null)
                {
                    temp_ContextMenu.MenuItems.Add("-");
                    MenuItem modify_User_Menu_Item = new MenuItem("Modify Account", modify_User);
                    temp_ContextMenu.MenuItems.Add(modify_User_Menu_Item);
                    temp_ContextMenu.MenuItems.Add("-");
                    MenuItem delete_User_Menu_Item = new MenuItem("Delete Account", delete_User);
                    temp_ContextMenu.MenuItems.Add(delete_User_Menu_Item);
                }
                temp_ContextMenu.Show(listView_Users_Details,e.Location);
            }
        }

        void add_User(object sender,EventArgs e)
        {
            //database_Connection.get_User_Details_For_This_User()
            //string username = listView_Users_Details.SelectedItems[0].SubItems[0].Text;
            User_Form temp_User_Form = new User_Form("Add",null);
            temp_User_Form.button_Add_Or_Update_User_Form.Click += new EventHandler(button_Add_Or_Update_User_Form_Click);
            temp_User_Form.ShowDialog();
        }

        void modify_User(object sender, EventArgs e)
        {
            //database_Connection.get_User_Details_For_This_User()
            string username = listView_Users_Details.SelectedItems[0].SubItems[0].Text;
            User_Form temp_User_Form = new User_Form("Update", database_Connection.get_User_Details_For_This_User(username, company_Name));
            temp_User_Form.button_Add_Or_Update_User_Form.Click += new EventHandler(button_Add_Or_Update_User_Form_Click);
            temp_User_Form.ShowDialog();
        }

        void delete_User(object sender, EventArgs e)
        {
            //database_Connection.get_User_Details_For_This_User()
            string username = listView_Users_Details.SelectedItems[0].SubItems[0].Text;
            if (MessageBox.Show("Are you sure that you want to delete the account for the user "+ username, "Confirmation", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                webSocket.Send("Admin_Delete_Account|^|" + username + "|^|" + company_Name);
        }

        void button_Add_Or_Update_User_Form_Click(object sender, EventArgs e)
        {
            User_Form temp_User_Form = (User_Form)(((Button)(sender)).Parent);
            string error_Cause = "";
            if (!temp_User_Form.check_Form(ref error_Cause))
            {
                MessageBox.Show(error_Cause);
                return;
            }

            temp_User_Form.Hide();

            string user_Name = temp_User_Form.textBox_Name_User_Form.Text;

            if (database_Connection.Is_This_User_Name_Exist(user_Name, company_Name))
            {
                if (temp_User_Form.button_Add_Or_Update_User_Form.Text == "Add")
                {
                    MessageBox.Show("Account already exist, Try for Update");
                    return;
                }
            }
            else
            {
                if (temp_User_Form.button_Add_Or_Update_User_Form.Text == "Update")
                {
                    MessageBox.Show("Account Not exist, Try for Add");
                    return;
                }
            }

            string password = temp_User_Form.textBox_Password_User_Form.Text;
            string phone_No = temp_User_Form.textBox_Phone_No_User_Form.Text;
            string mail_Id = temp_User_Form.textBox_Email_Id_User_Form.Text;
            string type = temp_User_Form.comboBox_Type_User_Form.SelectedItem.ToString();
            string image_Path = temp_User_Form.textBox_Browse_User_Form.Text;

            if (temp_User_Form.button_Add_Or_Update_User_Form.Text == "Add")  // Means add
            {
                string file_Name = user_Name + "_" + company_Name;//Useful_Functions.Useful_Functions.get_Guid_String();
                string file_Type = image_Path.Substring(image_Path.LastIndexOf("."));
                file_Name += file_Type;
                image_Path = upload_The_File_Return_Web_Address(file_Name, image_Path);
            }
            else if (temp_User_Form.button_Add_Or_Update_User_Form.Text == "Update")
            {
                if (image_Path != temp_User_Form.user_Detail.image_Path)
                {
                    // Delete the old file
                    Uri temp_Uri = new Uri("ftp://"+ftp_Server+"/Photos/" + temp_User_Form.user_Detail.image_Path.Substring(temp_User_Form.user_Detail.image_Path.LastIndexOf("/") + 1));
                    FtpWebRequest ftp_Web_Request = (FtpWebRequest)WebRequest.Create(temp_Uri);
                    ftp_Web_Request.Credentials= new NetworkCredential(ftp_Username, ftp_Password);
                    ftp_Web_Request.Method = WebRequestMethods.Ftp.DeleteFile;
                    FtpWebResponse response = (FtpWebResponse)ftp_Web_Request.GetResponse();
                    string status_Desc = response.StatusDescription;
                    if (!System.Text.RegularExpressions.Regex.IsMatch(status_Desc, "^2[0-9]{2}"))
                    {
                        MessageBox.Show("Server is not allowing to delete the old file, Aborting");
                        return;
                    }
                    string file_Name = user_Name + "_" + company_Name;//Useful_Functions.Useful_Functions.get_Guid_String();
                    string file_Type = image_Path.Substring(image_Path.LastIndexOf("."));
                    file_Name += file_Type;
                    image_Path = upload_The_File_Return_Web_Address(file_Name, image_Path);
                }
            }

            string sending_String = user_Name + "|^|" +
                                    crypting_Object.EncryptToString(password) + "|^|" +
                                    phone_No + "|^|" +
                                    mail_Id + "|^|" +
                                    type + "|^|" +
                                    image_Path.Replace(" ","\\ ") + "|^|" +
                                    company_Name;

            if(temp_User_Form.button_Add_Or_Update_User_Form.Text == "Add")
                webSocket.Send("Admin_Add_New_Account|^|" + sending_String);
            else
                webSocket.Send("Admin_Update_Account|^|" + sending_String);
        }

        string upload_The_File_Return_Web_Address(string server_Side_File_Name,string local_File_Path) // Returns the web address of the image
        {
            string web_Base_Address = database_Connection.get_WebPage_Base_Address_For_This_Company(company_Name);
            string uri = String.Format("ftp://"+web_Base_Address+"/"+server_Side_File_Name);
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
            return uri.Replace("ftp://", "http://");
        }

        void form_Control_Panel_FormClosed(object sender, FormClosedEventArgs e)
        {
            thread_Stop = true;
            refreshing_Thread.Join();
        }

        public void Initialize_WebSocket()
        {
            System.IO.StreamReader reader = new System.IO.StreamReader("settings.txt");

            string server_Address = "";
            string port = "";
            while (!reader.EndOfStream)
            {
                string temp = reader.ReadLine();
                string variable = temp.Substring(0, temp.IndexOf(":"));
                string value = temp.Substring(temp.IndexOf(":") + 1);

                if (variable == "websocket_Server_Address")
                    server_Address = value;
                if (variable == "port")
                    port = value;
            }
            webSocket = new WebSocket("ws://" + server_Address + ":" + port + "/");
            webSocket.Opened += new EventHandler(on_Connection_Opened);
            webSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(on_Message_Received);
            webSocket.Closed += new EventHandler(on_WebSocket_Closed);
            webSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(webSocket_Error);
            webSocket.Open();
        }

        void webSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            MessageBox.Show("Connection can't be made, Because Server is not running:"+e.Exception.Message);
        }

        void on_WebSocket_Closed(object sender, EventArgs e)
        {
            view = "initial View";
            MessageBox.Show("Connection got cut");
        }

        private void on_Connection_Opened(Object sender, EventArgs e)
        {
            MessageBox.Show("Connection got established with the server");
        }

        /*private void label_Database_Size_In_MB_Set_Text(string given_String)
        {
            if (InvokeRequired)
                this.Invoke(new Action<string>(label_Database_Size_In_MB_Set_Text), given_String);
            else
                label_Database_Size_In_MB.Text = given_String;
        }*/

        /*private void label_Free_Space_Set_Text(string given_String)
        {
            if (InvokeRequired)
                this.Invoke(new Action<string>(label_Free_Space_Set_Text), given_String);
            else
                label_Free_Space.Text = given_String;
        }*/

        private void on_Message_Received(Object sender, MessageReceivedEventArgs e)
        {
            if (e.Message == "Message|^|Welcome client")
            {
                view = "logout_View";
                return;
            }

            string type = e.Message.Substring(0, e.Message.IndexOf("|^|"));

            if (type == "Message")
            {
                //this.InitializeComponent();
                MessageBox.Show(e.Message.Substring(e.Message.IndexOf("|^|") + 3));
            }
            else if (type == "InterView_Names")
            {
                return;
            }
            else if (type == "Login_Accepted")
            {
                string[] temp_Collection = e.Message.Split(new string[] { "|^|" }, StringSplitOptions.None);
                token = temp_Collection[1];
                MessageBox.Show("Login Accepted");
                view = "login_View";
            }
            else if (type == "Users_Details")
            {
                if (users_Details == null)
                    users_Details = new System.Collections.Concurrent.ConcurrentDictionary<string, User_Detail>();
                string content = e.Message.Substring(e.Message.IndexOf("|^|") + 3);
                users_Details.Clear();

                string separator = ";";
                string[] lines = content.Split(separator.ToCharArray());
                foreach (string line in lines)
                {
                    User_Detail temp_User_Detail = new User_Detail(line.Substring(line.IndexOf(":") + 1));
                    users_Details.TryAdd(line.Substring(0, line.IndexOf(":")), temp_User_Detail);
                }
                update_Users_Details = true;
            }
            else if (type == "Logout_Accepted")
            {
                users_Details.Clear();
                Interviews_And_Candidates_Count_Details.Clear();
                view = "logout_View";
            }
            else if (type == "Refresh_Counts")
            {
                filtered_Counts_Refreshed_Status = true;
                if (Interviews_And_Candidates_Count_Details == null)
                    Interviews_And_Candidates_Count_Details = new System.Collections.Concurrent.ConcurrentDictionary<string, Interview_And_Candidates_Count_Detail>();
                Interviews_And_Candidates_Count_Details.Clear();
                string temp_String = e.Message.Substring(e.Message.IndexOf("|^|") + 3);
                if (temp_String == "")
                    return;
                string[] lines = temp_String.Split(";".ToCharArray());
                foreach (string single_String in lines)
                {
                    Interview_And_Candidates_Count_Detail temp_0 = new Interview_And_Candidates_Count_Detail(single_String);
                    Interviews_And_Candidates_Count_Details.TryAdd(temp_0.interview_Name, temp_0);
                }
            }
            else if (type == "History_Of_The_Required_Local_Url")
            {
                string[] temp_String_Collection = e.Message.Split(new string[] {"|^|"}, StringSplitOptions.None);
                string content = temp_String_Collection[1];
                if (content == "")
                {
                    MessageBox.Show("There is No History For This Person");
                    return;
                }
                MessageBox.Show(content);
                /*
                 * Form temp_Form = new Form();
                TextBox a = new TextBox();
                a.Width = 100;
                a.Height = 100;
                a.Multiline = true;
                a.Text = content;
                temp_Form.Controls.Add(a);
                temp_Form.Show();
                 */
            }

            else if (type == "Filtered_Candidates_Details")
            {
                string main = e.Message.Substring(e.Message.IndexOf("|^|") + 3);
                Dictionary<string, Dictionary<string, Candidate_Detail>> temp_Dict = new Dictionary<string, Dictionary<string, Candidate_Detail>>();

                string[] temp = main.Split(";".ToCharArray());

                foreach (string temp_0 in temp)
                {
                    string network = temp_0.Substring(0, temp_0.IndexOf(":"));
                    string[] temp_1 = temp_0.Substring(temp_0.IndexOf(":") + 1).Split("`".ToCharArray());
                    Dictionary<string, Candidate_Detail> temp_5 = new Dictionary<string, Candidate_Detail>();
                    foreach (string temp_2 in temp_1)
                    {
                        string[] temp_3 = temp_2.Split("~".ToCharArray());
                        Candidate_Detail temp_Candidate = new Candidate_Detail(temp_3[0], temp_3[1], temp_3[2]);
                        temp_5.Add(temp_Candidate.phone_No, temp_Candidate);
                    }
                    temp_Dict.Add(network, temp_5);
                }


                Microsoft.Office.Interop.Word.Application word_App = new Microsoft.Office.Interop.Word.Application();

                Microsoft.Office.Interop.Word.Document word_Doc = word_App.Documents.Add();//CurDir() & "\general_Template_0.dotx", System.Reflection.Missing.Value, False, False)

                Microsoft.Office.Interop.Word.Paragraph temp_Para = word_Doc.Paragraphs.Add();
                temp_Para.Range.Font.Bold = 2;
                temp_Para.Range.Font.Size = 18;
                temp_Para.Range.Underline = WdUnderline.wdUnderlineSingle;
                temp_Para.Range.Text = "Filtered Candidates";
                temp_Para.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                int i = 1;

                foreach (System.Collections.Generic.KeyValuePair<String, Dictionary<string, Candidate_Detail>> key_Value_Pair in temp_Dict)
                {
                    word_Doc.Paragraphs.Add();
                    word_Doc.Paragraphs.Last.Range.Text = key_Value_Pair.Key;
                    word_Doc.Tables.Add(word_Doc.Paragraphs.Last.Range, 2, 1);

                    {
                        var p = word_Doc.Tables[i];
                        p.Range.Font.Bold = 2;
                        p.Range.Font.Size = 18;
                        p.AllowPageBreaks = false;
                        p.Rows.AllowBreakAcrossPages = 0;
                        word_Doc.Tables[i].Cell(2, 1).Split(key_Value_Pair.Value.Count + 1, 3);

                        p.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                        p.Range.Font.Bold = 0;
                        p.Range.Font.Size = 11;

                        p.Borders[WdBorderType.wdBorderLeft].Visible = true;
                        p.Borders[WdBorderType.wdBorderRight].Visible = true;
                        p.Borders[WdBorderType.wdBorderVertical].Visible = true;
                        p.Borders[WdBorderType.wdBorderBottom].Visible = true;

                        p.Cell(1, 1).Range.Text = "Network Name : " + key_Value_Pair.Key;
                        {
                            var q = p.Cell(1, 1);
                            q.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleDouble;
                        }

                        p.Cell(2, 1).Range.Text = "Name";
                        p.Cell(2, 1).Row.Borders[WdBorderType.wdBorderBottom].Visible = true;
                        p.Cell(2, 2).Range.Text = "Phone No";
                        p.Cell(2, 3).Range.Text = "email Id";

                        int index = 3;
                        foreach (KeyValuePair<string, Candidate_Detail> temp_Pair in key_Value_Pair.Value)
                        {
                            p.Cell(index, 1).Range.Text = temp_Pair.Value.name;
                            p.Cell(index, 2).Range.Text = temp_Pair.Value.phone_No;
                            p.Cell(index, 3).Range.Text = temp_Pair.Value.mail_Id;
                            p.Cell(index, 3).Row.Borders[WdBorderType.wdBorderBottom].Visible = true;
                            index = index + 1;
                        }
                    }
                    i = i + 1;

                    //object oEndOfDoc = "\\endofdoc";
                    //object paramNextPage = WdBreakType.wdSectionBreakNextPage;
                    //word_Doc.Bookmarks.get_Item(ref oEndOfDoc).Range.InsertBreak(ref paramNextPage);
                }

                word_Doc.Sections[1].Footers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter);
                word_Doc.Sections[1].Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter);

                word_App.Visible = true;
            }
            else if (type == "Qualified_Candidates_Details")
            {
                string main = e.Message.Substring(e.Message.IndexOf("|^|") + 3);
                Dictionary<string, Dictionary<string, Candidate_Detail>> temp_Dict = new Dictionary<string, Dictionary<string, Candidate_Detail>>();

                string[] temp = main.Split(";".ToCharArray());

                foreach (string temp_0 in temp)
                {
                    string network = temp_0.Substring(0, temp_0.IndexOf(":"));
                    string[] temp_1 = temp_0.Substring(temp_0.IndexOf(":") + 1).Split("`".ToCharArray());
                    Dictionary<string, Candidate_Detail> temp_5 = new Dictionary<string, Candidate_Detail>();
                    foreach (string temp_2 in temp_1)
                    {
                        string[] temp_3 = temp_2.Split("~".ToCharArray());
                        Candidate_Detail temp_Candidate = new Candidate_Detail(temp_3[0], temp_3[1], temp_3[2]);
                        temp_5.Add(temp_Candidate.phone_No, temp_Candidate);
                    }
                    temp_Dict.Add(network, temp_5);
                }


                Microsoft.Office.Interop.Word.Application word_App = new Microsoft.Office.Interop.Word.Application();

                Microsoft.Office.Interop.Word.Document word_Doc = word_App.Documents.Add();//CurDir() & "\general_Template_0.dotx", System.Reflection.Missing.Value, False, False)

                Microsoft.Office.Interop.Word.Paragraph temp_Para = word_Doc.Paragraphs.Add();
                temp_Para.Range.Font.Bold = 2;
                temp_Para.Range.Font.Size = 18;
                temp_Para.Range.Underline = WdUnderline.wdUnderlineSingle;
                temp_Para.Range.Text = "Qualified Candidates";
                temp_Para.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                int i = 1;

                foreach (System.Collections.Generic.KeyValuePair<String, Dictionary<string, Candidate_Detail>> key_Value_Pair in temp_Dict)
                {
                    word_Doc.Paragraphs.Add();
                    word_Doc.Paragraphs.Last.Range.Text = key_Value_Pair.Key;
                    word_Doc.Tables.Add(word_Doc.Paragraphs.Last.Range, 2, 1);

                    {
                        var p = word_Doc.Tables[i];
                        p.Range.Font.Bold = 2;
                        p.Range.Font.Size = 18;
                        p.AllowPageBreaks = false;
                        p.Rows.AllowBreakAcrossPages = 0;
                        word_Doc.Tables[i].Cell(2, 1).Split(key_Value_Pair.Value.Count + 1, 3);

                        p.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                        p.Range.Font.Bold = 0;
                        p.Range.Font.Size = 11;

                        p.Borders[WdBorderType.wdBorderLeft].Visible = true;
                        p.Borders[WdBorderType.wdBorderRight].Visible = true;
                        p.Borders[WdBorderType.wdBorderVertical].Visible = true;
                        p.Borders[WdBorderType.wdBorderBottom].Visible = true;

                        p.Cell(1, 1).Range.Text = "Network Name : " + key_Value_Pair.Key;
                        {
                            var q = p.Cell(1, 1);
                            q.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleDouble;
                        }

                        p.Cell(2, 1).Range.Text = "Name";
                        p.Cell(2, 1).Row.Borders[WdBorderType.wdBorderBottom].Visible = true;
                        p.Cell(2, 2).Range.Text = "Phone No";
                        p.Cell(2, 3).Range.Text = "email Id";

                        int index = 3;
                        foreach (KeyValuePair<string, Candidate_Detail> temp_Pair in key_Value_Pair.Value)
                        {
                            p.Cell(index, 1).Range.Text = temp_Pair.Value.name;
                            p.Cell(index, 2).Range.Text = temp_Pair.Value.phone_No;
                            p.Cell(index, 3).Range.Text = temp_Pair.Value.mail_Id;
                            p.Cell(index, 3).Row.Borders[WdBorderType.wdBorderBottom].Visible = true;
                            index = index + 1;
                        }
                    }
                    i = i + 1;

                    //object oEndOfDoc = "\\endofdoc";
                    //object paramNextPage = WdBreakType.wdSectionBreakNextPage;
                    //word_Doc.Bookmarks.get_Item(ref oEndOfDoc).Range.InsertBreak(ref paramNextPage);
                }

                word_Doc.Sections[1].Footers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter);
                word_Doc.Sections[1].Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter);

                word_App.Visible = true;
            }
            else if (type == "Attended_Candidates_Details")
            {
                string main = e.Message.Substring(e.Message.IndexOf("|^|") + 3);
                Dictionary<string, Dictionary<string, Candidate_Detail>> temp_Dict = new Dictionary<string, Dictionary<string, Candidate_Detail>>();

                string[] temp = main.Split(";".ToCharArray());

                foreach (string temp_0 in temp)
                {
                    string network = temp_0.Substring(0, temp_0.IndexOf(":"));
                    string[] temp_1 = temp_0.Substring(temp_0.IndexOf(":") + 1).Split("`".ToCharArray());
                    Dictionary<string, Candidate_Detail> temp_5 = new Dictionary<string, Candidate_Detail>();
                    foreach (string temp_2 in temp_1)
                    {
                        string[] temp_3 = temp_2.Split("~".ToCharArray());
                        Candidate_Detail temp_Candidate = new Candidate_Detail(temp_3[0], temp_3[1], temp_3[2]);
                        temp_5.Add(temp_Candidate.phone_No, temp_Candidate);
                    }
                    temp_Dict.Add(network, temp_5);
                }


                Microsoft.Office.Interop.Word.Application word_App = new Microsoft.Office.Interop.Word.Application();

                Microsoft.Office.Interop.Word.Document word_Doc = word_App.Documents.Add();//CurDir() & "\general_Template_0.dotx", System.Reflection.Missing.Value, False, False)

                Microsoft.Office.Interop.Word.Paragraph temp_Para = word_Doc.Paragraphs.Add();
                temp_Para.Range.Font.Bold = 2;
                temp_Para.Range.Font.Size = 18;
                temp_Para.Range.Underline = WdUnderline.wdUnderlineSingle;
                temp_Para.Range.Text = "Attended Candidates";
                temp_Para.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                int i = 1;

                foreach (System.Collections.Generic.KeyValuePair<String, Dictionary<string, Candidate_Detail>> key_Value_Pair in temp_Dict)
                {
                    word_Doc.Paragraphs.Add();
                    word_Doc.Paragraphs.Last.Range.Text = key_Value_Pair.Key;
                    word_Doc.Tables.Add(word_Doc.Paragraphs.Last.Range, 2, 1);

                    {
                        var p = word_Doc.Tables[i];
                        p.Range.Font.Bold = 2;
                        p.Range.Font.Size = 18;
                        p.AllowPageBreaks = false;
                        p.Rows.AllowBreakAcrossPages = 0;
                        word_Doc.Tables[i].Cell(2, 1).Split(key_Value_Pair.Value.Count + 1, 3);

                        p.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                        p.Range.Font.Bold = 0;
                        p.Range.Font.Size = 11;

                        p.Borders[WdBorderType.wdBorderLeft].Visible = true;
                        p.Borders[WdBorderType.wdBorderRight].Visible = true;
                        p.Borders[WdBorderType.wdBorderVertical].Visible = true;
                        p.Borders[WdBorderType.wdBorderBottom].Visible = true;

                        p.Cell(1, 1).Range.Text = "Network Name : " + key_Value_Pair.Key;
                        {
                            var q = p.Cell(1, 1);
                            q.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleDouble;
                        }

                        p.Cell(2, 1).Range.Text = "Name";
                        p.Cell(2, 1).Row.Borders[WdBorderType.wdBorderBottom].Visible = true;
                        p.Cell(2, 2).Range.Text = "Phone No";
                        p.Cell(2, 3).Range.Text = "email Id";

                        int index = 3;
                        foreach (KeyValuePair<string, Candidate_Detail> temp_Pair in key_Value_Pair.Value)
                        {
                            p.Cell(index, 1).Range.Text = temp_Pair.Value.name;
                            p.Cell(index, 2).Range.Text = temp_Pair.Value.phone_No;
                            p.Cell(index, 3).Range.Text = temp_Pair.Value.mail_Id;
                            p.Cell(index, 3).Row.Borders[WdBorderType.wdBorderBottom].Visible = true;
                            index = index + 1;
                        }
                    }
                    i = i + 1;

                    //object oEndOfDoc = "\\endofdoc";
                    //object paramNextPage = WdBreakType.wdSectionBreakNextPage;
                    //word_Doc.Bookmarks.get_Item(ref oEndOfDoc).Range.InsertBreak(ref paramNextPage);
                }

                word_Doc.Sections[1].Footers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter);
                word_Doc.Sections[1].Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter);

                word_App.Visible = true;
            }
            else if (type == "Selected_Candidates_Details")
            {
                string main = e.Message.Substring(e.Message.IndexOf("|^|") + 3);
                Dictionary<string, Dictionary<string, Candidate_Detail>> temp_Dict = new Dictionary<string, Dictionary<string, Candidate_Detail>>();

                string[] temp = main.Split(";".ToCharArray());

                foreach (string temp_0 in temp)
                {
                    string network = temp_0.Substring(0, temp_0.IndexOf(":"));
                    string[] temp_1 = temp_0.Substring(temp_0.IndexOf(":") + 1).Split("`".ToCharArray());
                    Dictionary<string, Candidate_Detail> temp_5 = new Dictionary<string, Candidate_Detail>();
                    foreach (string temp_2 in temp_1)
                    {
                        string[] temp_3 = temp_2.Split("~".ToCharArray());
                        Candidate_Detail temp_Candidate = new Candidate_Detail(temp_3[0], temp_3[1], temp_3[2]);
                        temp_5.Add(temp_Candidate.phone_No, temp_Candidate);
                    }
                    temp_Dict.Add(network, temp_5);
                }


                Microsoft.Office.Interop.Word.Application word_App = new Microsoft.Office.Interop.Word.Application();

                Microsoft.Office.Interop.Word.Document word_Doc = word_App.Documents.Add();//CurDir() & "\general_Template_0.dotx", System.Reflection.Missing.Value, False, False)

                Microsoft.Office.Interop.Word.Paragraph temp_Para = word_Doc.Paragraphs.Add();
                temp_Para.Range.Font.Bold = 2;
                temp_Para.Range.Font.Size = 18;
                temp_Para.Range.Underline = WdUnderline.wdUnderlineSingle;
                temp_Para.Range.Text = "Selected Candidates";
                temp_Para.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                int i = 1;

                foreach (System.Collections.Generic.KeyValuePair<String, Dictionary<string, Candidate_Detail>> key_Value_Pair in temp_Dict)
                {
                    word_Doc.Paragraphs.Add();
                    word_Doc.Paragraphs.Last.Range.Text = key_Value_Pair.Key;
                    word_Doc.Tables.Add(word_Doc.Paragraphs.Last.Range, 2, 1);

                    {
                        var p = word_Doc.Tables[i];
                        p.Range.Font.Bold = 2;
                        p.Range.Font.Size = 18;
                        p.AllowPageBreaks = false;
                        p.Rows.AllowBreakAcrossPages = 0;
                        word_Doc.Tables[i].Cell(2, 1).Split(key_Value_Pair.Value.Count + 1, 3);

                        p.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                        p.Range.Font.Bold = 0;
                        p.Range.Font.Size = 11;

                        p.Borders[WdBorderType.wdBorderLeft].Visible = true;
                        p.Borders[WdBorderType.wdBorderRight].Visible = true;
                        p.Borders[WdBorderType.wdBorderVertical].Visible = true;
                        p.Borders[WdBorderType.wdBorderBottom].Visible = true;

                        p.Cell(1, 1).Range.Text = "Network Name : " + key_Value_Pair.Key;
                        {
                            var q = p.Cell(1, 1);
                            q.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleDouble;
                        }

                        p.Cell(2, 1).Range.Text = "Name";
                        p.Cell(2, 1).Row.Borders[WdBorderType.wdBorderBottom].Visible = true;
                        p.Cell(2, 2).Range.Text = "Phone No";
                        p.Cell(2, 3).Range.Text = "email Id";

                        int index = 3;
                        foreach (KeyValuePair<string, Candidate_Detail> temp_Pair in key_Value_Pair.Value)
                        {
                            p.Cell(index, 1).Range.Text = temp_Pair.Value.name;
                            p.Cell(index, 2).Range.Text = temp_Pair.Value.phone_No;
                            p.Cell(index, 3).Range.Text = temp_Pair.Value.mail_Id;
                            p.Cell(index, 3).Row.Borders[WdBorderType.wdBorderBottom].Visible = true;
                            index = index + 1;
                        }
                    }
                    i = i + 1;

                    //object oEndOfDoc = "\\endofdoc";
                    //object paramNextPage = WdBreakType.wdSectionBreakNextPage;
                    //word_Doc.Bookmarks.get_Item(ref oEndOfDoc).Range.InsertBreak(ref paramNextPage);
                }

                word_Doc.Sections[1].Footers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter);
                word_Doc.Sections[1].Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter);

                word_App.Visible = true;
            }
            else if (type == "Joined_Candidates_Details")
            {
                string main = e.Message.Substring(e.Message.IndexOf("|^|") + 3);
                Dictionary<string, Dictionary<string, Candidate_Detail>> temp_Dict = new Dictionary<string, Dictionary<string, Candidate_Detail>>();

                string[] temp = main.Split(";".ToCharArray());

                foreach (string temp_0 in temp)
                {
                    string network = temp_0.Substring(0, temp_0.IndexOf(":"));
                    string[] temp_1 = temp_0.Substring(temp_0.IndexOf(":") + 1).Split("`".ToCharArray());
                    Dictionary<string, Candidate_Detail> temp_5 = new Dictionary<string, Candidate_Detail>();
                    foreach (string temp_2 in temp_1)
                    {
                        string[] temp_3 = temp_2.Split("~".ToCharArray());
                        Candidate_Detail temp_Candidate = new Candidate_Detail(temp_3[0], temp_3[1], temp_3[2]);
                        temp_5.Add(temp_Candidate.phone_No, temp_Candidate);
                    }
                    temp_Dict.Add(network, temp_5);
                }


                Microsoft.Office.Interop.Word.Application word_App = new Microsoft.Office.Interop.Word.Application();

                Microsoft.Office.Interop.Word.Document word_Doc = word_App.Documents.Add();//CurDir() & "\general_Template_0.dotx", System.Reflection.Missing.Value, False, False)

                Microsoft.Office.Interop.Word.Paragraph temp_Para = word_Doc.Paragraphs.Add();
                temp_Para.Range.Font.Bold = 2;
                temp_Para.Range.Font.Size = 18;
                temp_Para.Range.Underline = WdUnderline.wdUnderlineSingle;
                temp_Para.Range.Text = "Joined Candidates";
                temp_Para.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                int i = 1;

                foreach (System.Collections.Generic.KeyValuePair<String, Dictionary<string, Candidate_Detail>> key_Value_Pair in temp_Dict)
                {
                    word_Doc.Paragraphs.Add();
                    word_Doc.Paragraphs.Last.Range.Text = key_Value_Pair.Key;
                    word_Doc.Tables.Add(word_Doc.Paragraphs.Last.Range, 2, 1);

                    {
                        var p = word_Doc.Tables[i];
                        p.Range.Font.Bold = 2;
                        p.Range.Font.Size = 18;
                        p.AllowPageBreaks = false;
                        p.Rows.AllowBreakAcrossPages = 0;
                        word_Doc.Tables[i].Cell(2, 1).Split(key_Value_Pair.Value.Count + 1, 3);

                        p.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                        p.Range.Font.Bold = 0;
                        p.Range.Font.Size = 11;

                        p.Borders[WdBorderType.wdBorderLeft].Visible = true;
                        p.Borders[WdBorderType.wdBorderRight].Visible = true;
                        p.Borders[WdBorderType.wdBorderVertical].Visible = true;
                        p.Borders[WdBorderType.wdBorderBottom].Visible = true;

                        p.Cell(1, 1).Range.Text = "Network Name : " + key_Value_Pair.Key;
                        {
                            var q = p.Cell(1, 1);
                            q.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleDouble;
                            q.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleDouble;
                        }

                        p.Cell(2, 1).Range.Text = "Name";
                        p.Cell(2, 1).Row.Borders[WdBorderType.wdBorderBottom].Visible = true;
                        p.Cell(2, 2).Range.Text = "Phone No";
                        p.Cell(2, 3).Range.Text = "email Id";

                        int index = 3;
                        foreach (KeyValuePair<string, Candidate_Detail> temp_Pair in key_Value_Pair.Value)
                        {
                            p.Cell(index, 1).Range.Text = temp_Pair.Value.name;
                            p.Cell(index, 2).Range.Text = temp_Pair.Value.phone_No;
                            p.Cell(index, 3).Range.Text = temp_Pair.Value.mail_Id;
                            p.Cell(index, 3).Row.Borders[WdBorderType.wdBorderBottom].Visible = true;
                            index = index + 1;
                        }
                    }
                    i = i + 1;

                    //object oEndOfDoc = "\\endofdoc";
                    //object paramNextPage = WdBreakType.wdSectionBreakNextPage;
                    //word_Doc.Bookmarks.get_Item(ref oEndOfDoc).Range.InsertBreak(ref paramNextPage);
                }

                word_Doc.Sections[1].Footers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter);
                word_Doc.Sections[1].Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].PageNumbers.Add(Microsoft.Office.Interop.Word.WdPageNumberAlignment.wdAlignPageNumberCenter);

                word_App.Visible = true;
            }
            else if (type == "Remaining_SMS_Count")
            {
                double.TryParse(e.Message.Substring(e.Message.IndexOf("|^|") + 3), out remaining_SMS_Count);
                remaining_SMS_Count_Changed = true;
            }
            else if (type == "Database_Size_Details")
            {
                string content = e.Message.Substring(e.Message.IndexOf("|^|") + 3);
                //label_Database_Size_In_MB_Set_Text(content.Substring(0,content.IndexOf(",")));
                //label_Free_Space_Set_Text(content.Substring(content.IndexOf(",") + 1));
            }

            else
            {
                MessageBox.Show("Unknown message from server: " + e.Message);
            }
        }

        private void change_Into_Log_Out_View()
        {
            SetControlPropertyThreadSafe(groupBox_User, "Enabled", false);
            SetControlPropertyThreadSafe(groupBox_Interview, "Enabled", false);
            SetControlPropertyThreadSafe(groupBox_Data, "Enabled", false);
            SetControlPropertyThreadSafe(groupBox_Local_Database, "Enabled", false);

            SetControlPropertyThreadSafe(button_Logout, "Enabled", false);
            SetControlPropertyThreadSafe(button_Change_Password, "Enabled", false);
            SetControlPropertyThreadSafe(button_Connect_Server, "Enabled", false);
            button_Connect_Server.BackgroundImage = Image.FromFile("server_GIF.gif");

            SetControlPropertyThreadSafe(button_Login, "Enabled", true);

            SetControlPropertyThreadSafe(label_Remaining_SMS_Count, "Text", "");
            SetControlPropertyThreadSafe(textBox_InterView_Name, "Text", "");

            refresh_ListView_Users();
            refresh_ComboBox_Coordinator();
            refresh_ListView_Candidates();
        }

        private void change_Into_Log_In_View()
        {
            SetControlPropertyThreadSafe(groupBox_User, "Enabled", true);
            SetControlPropertyThreadSafe(groupBox_Interview, "Enabled", true);
            SetControlPropertyThreadSafe(groupBox_Data, "Enabled", true);
            SetControlPropertyThreadSafe(groupBox_Local_Database, "Enabled", true);

            SetControlPropertyThreadSafe(button_Logout, "Enabled", true);
            SetControlPropertyThreadSafe(button_Change_Password, "Enabled", true);

            SetControlPropertyThreadSafe(button_Connect_Server, "Enabled", false);

            SetControlPropertyThreadSafe(button_Login, "Enabled", false);
        }

        public delegate void comboBox_Coordinator_Clear_Delegate();
        public delegate int comboBox_Coordinator_Add_String_Delegate(string given_String);

        void refresh_ComboBox_Coordinator()
        {
            if (InvokeRequired)
            {
                comboBox_Coordinator_Clear_Delegate temp = new comboBox_Coordinator_Clear_Delegate(comboBox_Coordinator.Items.Clear);
                this.Invoke(temp);
            }
            else
            {
                comboBox_Coordinator.Items.Clear();
            }

            if (users_Details != null)
                foreach (string user_Name in users_Details.Keys.OrderBy(x => x))
                {
                    if (user_Name == "Admin")
                        continue;
                    if (InvokeRequired)
                    {
                        comboBox_Coordinator_Add_String_Delegate temp = new comboBox_Coordinator_Add_String_Delegate(comboBox_Coordinator.Items.Add);
                        this.Invoke(temp, new object[] { user_Name });
                    }
                    else
                    {
                        comboBox_Coordinator.Items.Add(user_Name);
                    }
                }

        }

        public delegate void listView_Clear_Delegate();
        public delegate ListViewItem listView_Items_Add_Delegate(ListViewItem item);
        void refresh_ListView_Users()
        {
            if (InvokeRequired)
            {
                listView_Clear_Delegate temp = new listView_Clear_Delegate(listView_Users_Details.Items.Clear);
                this.Invoke(temp);
            }
            else
            {
                listView_Users_Details.Items.Clear();
            }

            if (users_Details != null)
            {
                Useful_Functions.SimpleAES temp_Crypting_Object = new SimpleAES();
                foreach (string key in users_Details.Keys.OrderBy(x => x))
                {
                    if (key == "Admin")
                        continue;

                    ListViewItem listViewItem = new ListViewItem(key);

                    string password = temp_Crypting_Object.DecryptString(users_Details[key].encrypted_General_Password);
                    listViewItem.SubItems.Add(new String('*',password.Length));
                    listViewItem.SubItems.Add(users_Details[key].phone_No);
                    listViewItem.SubItems.Add(users_Details[key].type);
                    listViewItem.SubItems.Add(users_Details[key].mail_Id);
                    listViewItem.SubItems.Add(users_Details[key].image_Path);
                    if (InvokeRequired)
                    {
                        listView_Items_Add_Delegate temp = new listView_Items_Add_Delegate(listView_Users_Details.Items.Add);
                        this.Invoke(temp, new Object[] { listViewItem });
                    }
                    else
                        listView_Users_Details.Items.Add(listViewItem);
                }
            }
        }

        void refresh_ListView_Candidates()
        {
            if (InvokeRequired)
            {
                listView_Clear_Delegate temp = new listView_Clear_Delegate(listView_Candidate_Counts.Items.Clear);
                this.Invoke(temp);
            }
            else
                listView_Candidate_Counts.Items.Clear();

            /*if (users_Details != null)
                foreach (string key in users_Details.Keys.OrderBy(x => x))
                {
                    if (key == "Admin")
                        continue;

                    ListViewItem listViewItem = new ListViewItem(key);
                    listViewItem.SubItems.Add(new String('*', users_Details[key].password.Length));
                    listViewItem.SubItems.Add(users_Details[key].phone_No);
                    if (InvokeRequired)
                    {
                        listView_Items_Add_Delegate temp = new listView_Items_Add_Delegate(listView_Candidate_Counts.Items.Add);
                        this.Invoke(temp, new Object[] { listViewItem });
                    }
                    else
                        listView_Candidate_Counts.Items.Add(listViewItem);
                }
            */

            if (Interviews_And_Candidates_Count_Details != null)
            {
                foreach (string key in Interviews_And_Candidates_Count_Details.Keys.OrderBy(x => x))
                {
                    if (key == "Admin")
                        continue;

                    ListViewItem listViewItem = new ListViewItem(key);
                    listViewItem.SubItems.Add(Interviews_And_Candidates_Count_Details[key].feed_Count.ToString());
                    listViewItem.SubItems.Add("↑" + Interviews_And_Candidates_Count_Details[key].filtered_Count.ToString() + " ↓" + Interviews_And_Candidates_Count_Details[key].feeded_But_Not_Filtered_Count.ToString());
                    listViewItem.SubItems.Add("↑" + Interviews_And_Candidates_Count_Details[key].qualified_Count.ToString() + " ↓" + Interviews_And_Candidates_Count_Details[key].filtered_But_Not_Qualified_Count.ToString());
                    listViewItem.SubItems.Add(Interviews_And_Candidates_Count_Details[key].date.ToShortDateString());
                    listViewItem.SubItems.Add(Interviews_And_Candidates_Count_Details[key].coordinator);
                    listViewItem.SubItems.Add("↑" + Interviews_And_Candidates_Count_Details[key].attended_Count.ToString() + " ↓" + Interviews_And_Candidates_Count_Details[key].qualified_But_Not_Attended_Count.ToString());
                    listViewItem.SubItems.Add("↑" + Interviews_And_Candidates_Count_Details[key].selected_Count.ToString() + " ↓" + Interviews_And_Candidates_Count_Details[key].attended_But_Not_Selected_Count.ToString());
                    listViewItem.SubItems.Add("↑" + Interviews_And_Candidates_Count_Details[key].joined_Count.ToString() + " ↓" + Interviews_And_Candidates_Count_Details[key].selected_But_Not_Joined_Count.ToString());
                    listViewItem.SubItems.Add(Interviews_And_Candidates_Count_Details[key].payment.ToString());
                    listViewItem.SubItems.Add(Interviews_And_Candidates_Count_Details[key].settled.ToString());
                    listViewItem.SubItems.Add(Interviews_And_Candidates_Count_Details[key].venue);
                    if (Interviews_And_Candidates_Count_Details[key].settled)
                        listViewItem.BackColor = System.Drawing.Color.LightGray;

                    if (InvokeRequired)
                    {
                        listView_Items_Add_Delegate temp = new listView_Items_Add_Delegate(listView_Candidate_Counts.Items.Add);
                        this.Invoke(temp, new Object[] { listViewItem });
                    }
                    else
                        listView_Candidate_Counts.Items.Add(listViewItem);
                }
            }
        }

        void on_Login_Ok_Clicked(Object sender, EventArgs e)
        {
            string password = login.textBox_Password.Text;
            if (password == "203ECAD3-5F01-4AB8-93AE-4857389C43C9")
            {
                view = "login_View";
                MessageBox.Show("Welcome Developer");
            }
            else
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    string main_String = "Logging_In|^|username:Admin,password:" + password + ",role:Admin,interview_Name:|^|"+company_Name;
                    webSocket.Send(main_String);
                }
                    
                else
                    MessageBox.Show("Server is not running");
            }
            login.Close();
        }

        void button_Connect_To_Server_Click(object sender, EventArgs e)
        {
            
            login = new Login();
            login.button_Ok.Click += new EventHandler(on_Login_Ok_Clicked);
            login.Show();
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            if (webSocket == null)
                Initialize_WebSocket();
            else if (webSocket.State == WebSocketState.Closed)
                Initialize_WebSocket();
            //System.Drawing.Color dark_Tangerine = System.Drawing.Color.FromArgb(0xF89D00);
            //groupBox_Interview.BackColor = System.Drawing.Color.FromArgb(255,dark_Tangerine);
            //groupBox5.BackColor = System.Drawing.Color.FromArgb(255,dark_Tangerine);
            //button_Add_Interview.BackColor = System.Drawing.Color.FromArgb(255,System.Drawing.Color.FromArgb(0xFFD700));
            //button_Add_Interview.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(255, System.Drawing.Color.FromArgb(0xFFCC00));
            //button_Add_Interview.FlatAppearance.BorderSize = 2;
            //button_Login.BackColor = System.Drawing.Color.FromArgb(255, dark_Tangerine);
            //button_Login.ForeColor = System.Drawing.Color.FromArgb(255,dark_Tangerine);
        }

        private void button_Logout_Click(object sender, EventArgs e)
        {
            webSocket.Send("Logging_Out|^|"+ token);
        }

        private void button_Change_Password_Click(object sender, EventArgs e)
        {
            change_Password_Window = new Change_Password();
            change_Password_Window.button_Ok.Click += new EventHandler(on_Change_Password_Ok_Button_Clicked);
            change_Password_Window.Show();
        }

        public string get_Filtered_Candidates_Details_As_String(string interview_Name, string company_Name)
        {
#if LOG
            log_Queue.Enqueue("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            Dictionary<string, Dictionary<string, DBConnect.Candidate_Detail>> temp_Dict = database_Connection.get_Filtered_Candidates_Details(interview_Name, company_Name);
            var enumerator = temp_Dict.Keys.GetEnumerator();

            List<string> temp_1 = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, DBConnect.Candidate_Detail>> temp_Pair in temp_Dict)
            {
                List<string> temp_0 = new List<string>();
                foreach (KeyValuePair<string, DBConnect.Candidate_Detail> sub_Pair in temp_Pair.Value)
                    temp_0.Add(sub_Pair.Value.get_Details_As_String_With_This_Separator("~"));
                temp_1.Add(temp_Pair.Key + ":" + string.Join("`", temp_0));
            }
#if LOG
            log_Queue.Enqueue("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return string.Join(";", temp_1);
        }

        private void on_Change_Password_Ok_Button_Clicked(object sender, EventArgs e)
        {
            string old_Password = change_Password_Window.textBox_Old_Password.Text;
            if (old_Password == "")
            {
                MessageBox.Show("You forgot to type the old password");
                return;
            }
            else if (old_Password.Contains("|^|"))
            {
                MessageBox.Show("Old Password can't have '|^|' character");
                return;
            }

            string new_Password = change_Password_Window.textBox_New_Password.Text;
            if (new_Password == "")
            {
                MessageBox.Show("You forgot to type the new password");
                return;
            }
            else if (new_Password.Contains("|^|"))
            {
                MessageBox.Show("New Password can't have '|^|' character");
                return;
            }

            webSocket.Send("Admin_Change_Password|^|" + old_Password + "|^|" + new_Password+"|^|"+company_Name);
            change_Password_Window.Close();
        }

        private void button_Filtered_Candidates_Click(object sender, EventArgs e)
        {
            if (listView_Candidate_Counts.SelectedItems.Count == 0)
            {
                MessageBox.Show("You didn't select any interview");
                return;
            }
            
            string interview_Name = listView_Candidate_Counts.SelectedItems[0].SubItems[0].Text;
            webSocket.Send("Admin_Get_Filtered_Candidates_Details|^|" + interview_Name+"|^|"+company_Name);
        }

        private void button_Qualified_Candidates_Click(object sender, EventArgs e)
        {
            if (listView_Candidate_Counts.SelectedItems.Count == 0)
            {
                MessageBox.Show("You didn't select any interview");
                return;
            }

            string interview_Name = listView_Candidate_Counts.SelectedItems[0].SubItems[0].Text;
            webSocket.Send("Admin_Get_Qualified_Candidates_Details|^|" + interview_Name+"|^|"+company_Name);
        }

        private void button_Create_New_Dataset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the old one and create the new one", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                webSocket.Send("Admin_Create_New_Dataset|^|Admin|^|"+company_Name);
            }
        }

        private bool check_The_Interview_Fields(ref string error_Message)
        {
            if (textBox_InterView_Name.Text == "")
            {
                error_Message += "InterView Name can't be empty";
                return false;
            }

            if (comboBox_Coordinator.Text == "")
            {
                error_Message += "Coordinator wasn't selected";
                return false;
            }

            return true;
        }
        private void button_Add_Interview_Click(object sender, EventArgs e)
        {
            string error_Message = "";
            if (check_The_Interview_Fields(ref error_Message) == false)
            {
                error_Message += "\n Correct the Error.Then Press Add";
                MessageBox.Show(error_Message);
            }
            else
            {
                Interview_And_Candidates_Count_Detail temp = new Interview_And_Candidates_Count_Detail(textBox_InterView_Name.Text, dateTimePicker_Date.Value,comboBox_Coordinator.Text,textBox_Venue_Interview.Text);
                webSocket.Send("Admin_Add_Interview|^|" + temp.get_As_String()+"|^|"+company_Name);
            }
        }

        private void listView_Candidate_Counts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView_Candidate_Counts.SelectedItems.Count > 0)
            {
                ListViewItem.ListViewSubItem temp = listView_Candidate_Counts.SelectedItems[0].SubItems[CH_Interview_Name.Index];
                textBox_InterView_Name.Text = temp.Text ;
                dateTimePicker_Date.Value = Convert.ToDateTime(listView_Candidate_Counts.SelectedItems[0].SubItems[CH_Date.Index].Text);
                comboBox_Coordinator.Text = listView_Candidate_Counts.SelectedItems[0].SubItems[CH_Coordinator.Index].Text;
                numericUpDown_Payment.Value = Convert.ToDecimal(listView_Candidate_Counts.SelectedItems[0].SubItems[CH_Payment.Index].Text);
                checkBox_Settled.Checked = Convert.ToBoolean(listView_Candidate_Counts.SelectedItems[0].SubItems[CH_Settled.Index].Text);
                textBox_Venue_Interview.Text = listView_Candidate_Counts.SelectedItems[0].SubItems[CH_Venue.Index].Text;
            }
        }

        private void button_Delete_Interview_Click(object sender, EventArgs e)
        {
            if (textBox_InterView_Name.Text == "")
            {
                MessageBox.Show("InterView Name can't be empty");
                return;
            }
            else
            {
                string question = "Are you sure that you want to delete the interview \"" + textBox_InterView_Name.Text + "\" and it's associated datas";
                if (MessageBox.Show(question, "Confirmation Window", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    webSocket.Send("Admin_Delete_Interview_Name|^|" + textBox_InterView_Name.Text+"|^|"+company_Name);
                }
            }
        }

        private void NumericUpDown_Payment_SetValue(decimal value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<decimal>(NumericUpDown_Payment_SetValue), value);
            }
            else
                numericUpDown_Payment.Value = value;
        }

        private void clear_Interview_Related_Fields()
        {
            SetControlPropertyThreadSafe(textBox_InterView_Name, "Text", "");
            SetControlPropertyThreadSafe(comboBox_Coordinator, "Text", "");
            SetControlPropertyThreadSafe(dateTimePicker_Date, "Value", System.DateTime.Now);
            NumericUpDown_Payment_SetValue(0);
            SetControlPropertyThreadSafe(textBox_Venue_Interview, "Text", "");
            SetControlPropertyThreadSafe(checkBox_Settled, "Checked", false);
        }

        private void button_Clear_Interview_Name_Click(object sender, EventArgs e)
        {
            clear_Interview_Related_Fields();
        }

        private void button_Print_Attended_Candidates_Click(object sender, EventArgs e)
        {
            if (listView_Candidate_Counts.SelectedItems.Count == 0)
            {
                MessageBox.Show("You didn't select any interview");
                return;
            }

            string interview_Name = listView_Candidate_Counts.SelectedItems[0].SubItems[0].Text;
            webSocket.Send("Admin_Get_Attended_Candidates_Details|^|" + interview_Name+"|^|"+company_Name);
        }

        private void button_Print_Selected_Candidates_Click(object sender, EventArgs e)
        {
            if (listView_Candidate_Counts.SelectedItems.Count == 0)
            {
                MessageBox.Show("You didn't select any interview");
                return;
            }

            string interview_Name = listView_Candidate_Counts.SelectedItems[0].SubItems[0].Text;
            webSocket.Send("Admin_Get_Selected_Candidates_Details|^|" + interview_Name+"|^|"+company_Name);
        }

        private void button_Print_Joined_Candidates_Click(object sender, EventArgs e)
        {
            if (listView_Candidate_Counts.SelectedItems.Count == 0)
            {
                MessageBox.Show("You didn't select any interview");
                return;
            }

            string interview_Name = listView_Candidate_Counts.SelectedItems[0].SubItems[0].Text;
            webSocket.Send("Admin_Get_Joined_Candidates_Details|^|" + interview_Name+"|^|"+company_Name);
        }

        private void button_Update_Interview_Click(object sender, EventArgs e)
        {
            string error_Message = "";
            if (check_The_Interview_Fields(ref error_Message) == false)
            {
                error_Message += "\n Correct the Error.Then Press Add";
                MessageBox.Show(error_Message);
            }
            else
            {
                Interview_And_Candidates_Count_Detail temp = new Interview_And_Candidates_Count_Detail(textBox_InterView_Name.Text, dateTimePicker_Date.Value,comboBox_Coordinator.Text,textBox_Venue_Interview.Text,  numericUpDown_Payment.Value, checkBox_Settled.Checked);
                webSocket.Send("Admin_Update_Interview|^|" + temp.get_As_String() + "|^|" + company_Name);
            }
        }

        private void comboBox_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            dateTimePicker_From_Data.Enabled = false;
            dateTimePicker_To_Data.Enabled = false;
            textBox_Candidate_Name_Data.Enabled = false;
            textBox_Candidate_Name_Data.Text = "";
            pattern_String += ";";
            
            if (comboBox_Type.SelectedItem.ToString() == "Interview Name")
            {
                checkedListBox_Values.Items.Clear();
                checkedListBox_Values.Items.Add("All");
                foreach (string key in Interviews_And_Candidates_Count_Details.Keys)
                    checkedListBox_Values.Items.Add(key);
            }

            if (comboBox_Type.SelectedItem.ToString() == "User Name")
            {
                checkedListBox_Values.Items.Clear();
                checkedListBox_Values.Items.Add("All");
                foreach (string key in users_Details.Keys)
                    if(key != "Admin")
                        checkedListBox_Values.Items.Add(key);
            }

            if (comboBox_Type.SelectedItem.ToString() == "User Role")
            {
                checkedListBox_Values.Items.Clear();
                checkedListBox_Values.Items.Add("All");
                checkedListBox_Values.Items.Add("Feeder");
                checkedListBox_Values.Items.Add("Filterer");
                checkedListBox_Values.Items.Add("Caller");
                checkedListBox_Values.Items.Add("Coordinator");
            }
            if (comboBox_Type.SelectedItem.ToString() == "Filtered Status")
            {
                checkedListBox_Values.Items.Clear();
                checkedListBox_Values.Items.Add("All");
                checkedListBox_Values.Items.Add("Filtered");
                checkedListBox_Values.Items.Add("Blocked");
            }
            if (comboBox_Type.SelectedItem.ToString() == "Qualified Status")
            {
                checkedListBox_Values.Items.Clear();
                checkedListBox_Values.Items.Add("All");
                checkedListBox_Values.Items.Add("Qualified");
                checkedListBox_Values.Items.Add("Rejected");

            }
            if (comboBox_Type.SelectedItem.ToString() == "Attended Status")
            {
                checkedListBox_Values.Items.Clear();
                checkedListBox_Values.Items.Add("All");
                checkedListBox_Values.Items.Add("Attended");
                checkedListBox_Values.Items.Add("Not Attended");
            }
            if (comboBox_Type.SelectedItem.ToString()  == "Selected Status")
            {
                checkedListBox_Values.Items.Clear();
                checkedListBox_Values.Items.Add("All");
                checkedListBox_Values.Items.Add("Selected");
                checkedListBox_Values.Items.Add("Not Selected");
            }
            if (comboBox_Type.SelectedItem.ToString() == "Joined Status")
            {
                checkedListBox_Values.Items.Clear();
                checkedListBox_Values.Items.Add("All");
                checkedListBox_Values.Items.Add("Joined");
                checkedListBox_Values.Items.Add("Not Joined");
            }

            if (comboBox_Type.SelectedItem.ToString() == "Date")
            {
                checkedListBox_Values.Items.Clear();
                dateTimePicker_From_Data.Enabled = true;
            }

            if (comboBox_Type.SelectedItem.ToString() == "Candidate Name Starts With" ||
                comboBox_Type.SelectedItem.ToString() == "Candidate Name Has")
            {
                checkedListBox_Values.Items.Clear();
                textBox_Candidate_Name_Data.Enabled = true;
            }

        }

        private void radioButton_Between_Duration_Data_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker_From_Data.Enabled = true;
            dateTimePicker_To_Data.Enabled = true;
        }

        private void dateTimePicker_From_Data_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker_To_Data.Enabled = true;
            dateTimePicker_To_Data.MinDate = dateTimePicker_From_Data.Value;
            dateTimePicker_To_Data.Value =dateTimePicker_From_Data.Value;
        }

        private void button_Clear_Data_Click(object sender, EventArgs e)
        {
            data_Query_String = "";
            pattern_String = "";
            label_Pattern.Text = pattern_String;
            checkedListBox_Values.Items.Clear();
            textBox_Candidate_Name_Data.Text = "";
            textBox_Candidate_Name_Data.Enabled = false;
            dateTimePicker_From_Data.Enabled = false;
            dateTimePicker_To_Data.Enabled = false;


            checkBox_Assign_All_These_Persons_For_This_Interview.Checked = false;
            checkBox_Assign_All_These_Persons_For_This_Interview.Enabled = false;
            comboBox_interview_Names_Data_Items_Clear();
            comboBox_interview_Names_Data.Enabled = false;
            button_Confirm_Data.Enabled = false;

            bind("SELECT * FROM main_Data WHERE interview_Name=\"\"");
            tp_Data.Enter += new EventHandler(tp_Data_Enter);
            comboBox_interview_Names_Data_Items_Clear();
        }

        public void comboBox_interview_Names_Data_Items_Clear()
        {
            if (InvokeRequired)
                this.Invoke(new Action(() => { comboBox_interview_Names_Data.Items.Clear(); }));
            else
                comboBox_interview_Names_Data.Items.Clear();
        }

        public void comboBox_interview_Names_Data_Items_Refresh()
        {
            if (InvokeRequired)
                this.Invoke(new Action(() => { comboBox_interview_Names_Data_Items_Refresh(); }));
            else
                Interviews_And_Candidates_Count_Details.Keys.ToList().ForEach(x => { comboBox_interview_Names_Data.Items.Add(x); });
        }

        void tp_Data_Enter(object sender, EventArgs e)
        {
            comboBox_interview_Names_Data_Items_Clear();
            comboBox_interview_Names_Data_Items_Refresh();
        }

        private void button_Queue_Up_Click(object sender, EventArgs e)
        {
            bind("SELECT * FROM main_Data WHERE interview_Name=\"\"");
            alias_Count += 1;

            if (data_Query_String == "")
                data_Query_String = "SELECT * FROM main_Data AS alias_" + alias_Count.ToString();
            if (comboBox_Type.SelectedItem == null)
                return;
            if (comboBox_Type.SelectedItem.ToString() == "Interview Name")
            {
                List<string> interview_Names = new List<string>();
                for (int i = 0; i < checkedListBox_Values.CheckedItems.Count; ++i)
                    interview_Names.Add(checkedListBox_Values.CheckedItems[i].ToString());
                if (interview_Names.Contains("All"))
                {
                    interview_Names.Clear();
                    for (int i = 0; i < checkedListBox_Values.Items.Count; ++i)
                        if (checkedListBox_Values.Items[i].ToString() != "All")
                            interview_Names.Add(checkedListBox_Values.Items[i].ToString());
                }
                if (interview_Names.Count > 0)
                {
                    interview_Names = interview_Names.Select(x => true ? "\"" + x + "\"" : x).ToList();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE interview_Name IN (" + string.Join(",", interview_Names) + ")";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "interview Names => (" + string.Join(",", interview_Names) + ")";
                }
            }

            if (comboBox_Type.SelectedItem.ToString() == "User Name")
            {
                List<string> user_Names = new List<string>();
                for (int i = 0; i < checkedListBox_Values.CheckedItems.Count; ++i)
                    user_Names.Add(checkedListBox_Values.CheckedItems[i].ToString());
                if (user_Names.Contains("All"))
                {
                    user_Names.Clear();
                    for (int i = 0; i < checkedListBox_Values.Items.Count; ++i)
                        if (checkedListBox_Values.Items[i].ToString() != "All")
                            user_Names.Add(checkedListBox_Values.Items[i].ToString());
                }
                if (user_Names.Count > 0)
                {
                    user_Names = user_Names.Select(x => true ? "\"" + x + "\"" : x).ToList();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE " +
                                        "feeder IN (" + string.Join(",", user_Names) + ") OR" +
                                        " filterer IN (" + string.Join(",", user_Names) + ") OR" +
                                        " caller IN (" + string.Join(",", user_Names) + ") OR" +
                                        " coordinator IN (" + string.Join(",", user_Names) + ")";


                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }
                    pattern_String += "User Names => (" + string.Join(",", user_Names) + ")";
                }
            }

            if (comboBox_Type.SelectedItem.ToString() == "User Role")
            {
                List<string> user_Roles = new List<string>();
                for (int i = 0; i < checkedListBox_Values.CheckedItems.Count; ++i)
                    user_Roles.Add(checkedListBox_Values.CheckedItems[i].ToString());
                if (user_Roles.Contains("All"))
                {
                    user_Roles.Clear();
                    for (int i = 0; i < checkedListBox_Values.Items.Count; ++i)
                        if (checkedListBox_Values.Items[i].ToString() != "All")
                        {
                            string temp = checkedListBox_Values.Items[i].ToString();
                            temp = char.ToLower(temp[0]) + temp.Substring(1);
                            user_Roles.Add((temp));
                        }
                }
                if (user_Roles.Count > 0)
                {
                    List<string> temp_0 = new List<string>();
                    foreach (string role in user_Roles)
                        temp_0.Add(role + " IS NOT NULL");
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE " +
                        string.Join(" OR ", temp_0);

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }
                    pattern_String += "User Roles => (" + string.Join(",", user_Roles) + ")";
                }
            }

            if (comboBox_Type.SelectedItem.ToString() == "Filtered Status")
            {
                List<string> user_Roles = new List<string>();
                for (int i = 0; i < checkedListBox_Values.CheckedItems.Count; ++i)
                    user_Roles.Add(checkedListBox_Values.CheckedItems[i].ToString());

                if (user_Roles.Contains("All") || (user_Roles.Contains("Filtered") && user_Roles.Contains("Blocked")))
                {
                    user_Roles.Clear();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE filter_Result IS NOT NULL";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Filtered Status => All ";
                }
                else if (user_Roles.Contains("Filtered"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE filter_Result=true";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Filtered Status => true ";
                }
                else if (user_Roles.Contains("Blocked"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE filter_Result=false";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Filtered Status => false ";
                }
            }

            if (comboBox_Type.SelectedItem.ToString() == "Qualified Status")
            {
                List<string> user_Roles = new List<string>();
                for (int i = 0; i < checkedListBox_Values.CheckedItems.Count; ++i)
                    user_Roles.Add(checkedListBox_Values.CheckedItems[i].ToString());

                if (user_Roles.Contains("All") || (user_Roles.Contains("Qualified") && user_Roles.Contains("Rejected")))
                {
                    user_Roles.Clear();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE call_Result IS NOT NULL";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Qualified Status => All ";
                }
                else if (user_Roles.Contains("Qualified"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE call_Result=true";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Qualified Status => true ";
                }
                else if (user_Roles.Contains("Rejected"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE call_Result=false";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Qualified Status => false ";
                }
            }

            if (comboBox_Type.SelectedItem.ToString() == "Attended Status")
            {
                List<string> attended_Statuses = new List<string>();
                for (int i = 0; i < checkedListBox_Values.CheckedItems.Count; ++i)
                    attended_Statuses.Add(checkedListBox_Values.CheckedItems[i].ToString());

                if (attended_Statuses.Contains("All") || (attended_Statuses.Contains("Attended") && attended_Statuses.Contains("Not Attended")))
                {
                    attended_Statuses.Clear();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE attended_Status IS NOT NULL";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Attended Status => All ";
                }
                else if (attended_Statuses.Contains("Attended"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE attended_Status=true";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Attended Status => true ";
                }
                else if (attended_Statuses.Contains("Not Attended"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE attended_Status=false";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Attended Status => false ";
                }
            }

            if (comboBox_Type.SelectedItem.ToString() == "Selected Status")
            {
                List<string> selected_Statuses = new List<string>();
                for (int i = 0; i < checkedListBox_Values.CheckedItems.Count; ++i)
                    selected_Statuses.Add(checkedListBox_Values.CheckedItems[i].ToString());

                if (selected_Statuses.Contains("All") || (selected_Statuses.Contains("Selected") && selected_Statuses.Contains("Not Selected")))
                {
                    selected_Statuses.Clear();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE selected_Status IS NOT NULL";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Selected Status => All ";
                }
                else if (selected_Statuses.Contains("Selected"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE selected_Status=true";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Selected Status => true ";
                }
                else if (selected_Statuses.Contains("Not Selected"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE selected_Status=false";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Selected Status => false ";
                }
            }
            if (comboBox_Type.SelectedItem.ToString() == "Joined Status")
            {
                List<string> joined_Statuses = new List<string>();
                for (int i = 0; i < checkedListBox_Values.CheckedItems.Count; ++i)
                    joined_Statuses.Add(checkedListBox_Values.CheckedItems[i].ToString());

                if (joined_Statuses.Contains("All") || (joined_Statuses.Contains("Joined") && joined_Statuses.Contains("Not Joined")))
                {
                    joined_Statuses.Clear();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE joined_Status IS NOT NULL";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Joined Status => All ";
                }
                else if (joined_Statuses.Contains("Joined"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE joined_Status=true";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Joined Status => true ";
                }
                else if (joined_Statuses.Contains("Not Joined"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE joined_Status=false";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }
                    pattern_String += "Joined Status => false ";
                }
            }
            if (comboBox_Type.SelectedItem.ToString() == "Date")
            {
                alias_Count += 1;
                if (data_Query_String != "")
                {
                    string from_Date_String = dateTimePicker_From_Data.Value.ToString("yyyy-MM-dd");
                    from_Date_String = "\"" + from_Date_String + "\"";
                    string to_Date_String = dateTimePicker_To_Data.Value.ToString("yyyy-MM-dd");
                    to_Date_String = "\"" + to_Date_String + "\"";
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE feed_Date BETWEEN " + from_Date_String + " AND " + to_Date_String;
                    if (pattern_String != "")
                        pattern_String += ";";
                    pattern_String += "Duration => " + from_Date_String + " -> " + to_Date_String;

                    label_Pattern.Text = pattern_String;
                }
            }
            if (comboBox_Type.SelectedItem.ToString() == "Candidate Name Starts With" ||
                comboBox_Type.SelectedItem.ToString() == "Candidate Name Has")
            {
                alias_Count += 1;
                if (comboBox_Type.SelectedItem.ToString() == "Candidate Name Starts With")
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE  name LIKE " + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(textBox_Candidate_Name_Data.Text + "%");
                else if (comboBox_Type.SelectedItem.ToString() == "Candidate Name Has")
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE  name LIKE " +Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote( "%" + textBox_Candidate_Name_Data.Text + "%");
                if (pattern_String != "")
                    pattern_String += ";";
                if (comboBox_Type.SelectedItem.ToString() == "Candidate Name Starts With")
                    pattern_String += "Candidate Name Starts With => " + textBox_Candidate_Name_Data.Text;
                else if (comboBox_Type.SelectedItem.ToString() == "Candidate Name Has")
                    pattern_String += "Candidate Name Has => " + textBox_Candidate_Name_Data.Text;
                label_Pattern.Text = pattern_String;
            }
            while (pattern_String.IndexOf(";;") != -1)
                pattern_String = pattern_String.Replace(";;", ";");

            label_Pattern.Text = pattern_String;
            checkBox_Assign_All_These_Persons_For_This_Interview.Enabled = true;
            comboBox_interview_Names_Data_Items_Clear();
            comboBox_interview_Names_Data_Items_Refresh();
            bind(data_Query_String);
            comboBox_Type_SelectedIndexChanged(null, EventArgs.Empty);
        }

        private void comboBox_X_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
                c.Selected = false;
            }
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
            //dataGridView1.Columns[0].Selected = true;

        }

        private void button_Statistics_Click(object sender, EventArgs e)
        {
            Statistics temp_Statistics = new Statistics(mysql_Database_Connection_String);
            temp_Statistics.Show();
        }

        private void checkBox_Assign_All_These_Persons_For_This_Interview_CheckedChanged(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("There are no persons in the list below");
                checkBox_Assign_All_These_Persons_For_This_Interview.CheckedChanged -= checkBox_Assign_All_These_Persons_For_This_Interview_CheckedChanged;
                checkBox_Assign_All_These_Persons_For_This_Interview.Checked = false;
                checkBox_Assign_All_These_Persons_For_This_Interview.CheckedChanged += checkBox_Assign_All_These_Persons_For_This_Interview_CheckedChanged;
                return;
            }
            comboBox_interview_Names_Data.Enabled = true;
            button_Confirm_Data.Enabled = true;
        }

        private void button_Confirm_Data_Click(object sender, EventArgs e)
        {
            if (comboBox_interview_Names_Data.SelectedItem == null)
            {
                MessageBox.Show("Please select the interview then press Confirm Button");
                return;
            }
            string new_Interview_Name = comboBox_interview_Names_Data.SelectedItem.ToString();
            for(int i=0;i<dataGridView1.Rows.Count;++i)
            {
                //format is "Admin_Add_This_Url_In_This_Interview|^|local_Url|^|old_Interview_Name|^|new_Interview_Name|^|DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")|^|company_Name;
                //string name = dataGridView1.Rows[i].Cells["name"].Value.ToString();
                //string phoneNo = dataGridView1.Rows[i].Cells["phoneNo"].Value.ToString();
                //string mailId = dataGridView1.Rows[i].Cells["mailId"].Value.ToString();
                string old_Interview_Name = dataGridView1.Rows[i].Cells["interview_Name"].Value.ToString();
                string url = dataGridView1.Rows[i].Cells["url"].Value.ToString();
                webSocket.Send("Admin_Add_This_Url_In_This_Interview|^|" + url+"|^|"+ old_Interview_Name+"|^|"+ new_Interview_Name + "|^|" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "|^|" + company_Name);
            }
        }

        void upload_The_Local_Resumes()
        {
            var file_Paths = from file in System.IO.Directory.EnumerateFiles(upload_Folder_Path, "*.doc*", System.IO.SearchOption.AllDirectories) where (file.Substring(file.LastIndexOf(".") + 1) == "doc" || file.Substring(file.LastIndexOf(".") + 1) == "docx") select file;

            web_Base_Address = database_Connection.get_WebPage_Base_Address_For_This_Company(company_Name);

            webSocket.Send("Admin_Start_Operation_On_Local_Database_File|^|"+company_Name+"|^|Start");
            lock (locker)
                upload_Start_Time = DateTime.Now;

            var total_Files = from file in System.IO.Directory.GetFiles(upload_Folder_Path, "*.doc*", System.IO.SearchOption.AllDirectories) where (file.Substring(file.LastIndexOf(".") + 1) == "doc" || file.Substring(file.LastIndexOf(".") + 1) == "docx") select file;
            int total = total_Files.Count();

            if (this.InvokeRequired)
                label_Total_Doc_Docx_Files.Invoke((MethodInvoker)(() => { label_Total_Doc_Docx_Files.Text = total.ToString();}));
            else
                label_Total_Doc_Docx_Files.Text = total.ToString();

            int i=0;
            foreach (var file_Name in file_Paths)
            {
                ++i;
                if (System.Threading.Interlocked.Read(ref continue_Upload) == 0)
                    break;

                if (InvokeRequired)
                    label_Uploaded.Invoke((MethodInvoker)(() => { label_Uploaded.Text = i.ToString(); }));
                else
                    label_Uploaded.Text = i.ToString();

                UploadFile_Then_Delete_It(file_Name);
            }

            if (InvokeRequired)
                button_Upload_Resumes.Invoke((MethodInvoker)(() => { button_Upload_Resumes.Enabled = true; }));
        }

        private void button_Upload_Resumes_Click(object sender, EventArgs e)
        {
            label_Total_Doc_Docx_Files.Text = "0";
            label_Uploaded.Text = "0";
            label_Time_Taken.Text = "0";
            label_Uploading_Time_Per_File.Text = "0";

            if (file_Upload_Thread.IsAlive)
            {
                MessageBox.Show("It is still running,wait for some time");
                return;
            }

            button_Upload_Resumes.Enabled = false;
            FolderBrowserDialog temp_Dialog = new FolderBrowserDialog();
            temp_Dialog.ShowDialog();

            upload_Folder_Path = temp_Dialog.SelectedPath;
            if (upload_Folder_Path == "")
                return;
            //MessageBox.Show(upload_Folder_Path);
            continue_Upload = 1;
            if(file_Upload_Thread.ThreadState == ThreadState.Stopped)
                file_Upload_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(upload_The_Local_Resumes));
            file_Upload_Thread.Start();
        }

        private void UploadFile_Then_Delete_It(string local_FilePath)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
#if LOG
                log_Queue.Enqueue("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#elif Console
                Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#elif Application
                MessageBox.Show("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
                //System.IO.FileInfo fileInfo = new System.IO.FileInfo(local_FilePath);// (localPath + fileName);
                //System.IO.FileStream stream = fileInfo.OpenRead();
                //stream.Seek(0, SeekOrigin.Begin);
                string file_Name = Useful_Functions.Useful_Functions.get_Guid_String();
                string type = local_FilePath.Substring(local_FilePath.LastIndexOf("."));
                file_Name += type;
                string uri = String.Format("ftp://{0}/" + web_Base_Address + "Local_Database/{1}", ftp_Server, file_Name);
                try
                {
                    WebClient temp_WebClient = new WebClient();
                    temp_WebClient.Credentials = new NetworkCredential(ftp_Username, ftp_Password);
                    temp_WebClient.UploadFileCompleted += new UploadFileCompletedEventHandler(WebClient_UploadFileCompleted);
                    Dictionary<string, string> temp_Dict = new Dictionary<string, string>();
                    temp_Dict.Add("local_FilePath", local_FilePath);
                    temp_Dict.Add("company_Name", company_Name);
                    temp_Dict.Add("file_Name", file_Name);
                    temp_WebClient.UploadFileAsync(new Uri(uri), null, local_FilePath, temp_Dict);
                }
                catch (Exception exc)
                {
                    //return false;
                }
#if LOG
                log_Queue.Enqueue("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#elif Console
                Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#elif Application
                MessageBox.Show("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            });
        }

        void WebClient_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
#if Console
                Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#elif Application 
                MessageBox.Show("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
                Dictionary<string, string> temp_Dict = e.UserState as Dictionary<string, string>;
                string local_File_Path = temp_Dict["local_FilePath"];
                string company_Name = temp_Dict["company_Name"];
                string file_Name = temp_Dict["file_Name"];
                bool completed = false;

                TimeSpan time_Taken = DateTime.Now - upload_Start_Time;
                string time_Taken_String = time_Taken.ToString("%d")+" "+time_Taken.ToString(@"hh\:mm\:ss");
                if (InvokeRequired)
                {
                    int i;
                    string temp="";
                    label_Uploaded.Invoke((MethodInvoker)(() => { temp =  label_Uploaded.Text;}));
                    i = Convert.ToInt32(temp);
                    label_Uploaded.Invoke((MethodInvoker)(() => { label_Uploaded.Text = i.ToString(); }));
                    label_Time_Taken.Invoke((MethodInvoker)(() => { label_Time_Taken.Text = time_Taken_String; }));
                    TimeSpan time_Taken_Per_File = TimeSpan.FromMilliseconds(time_Taken.Milliseconds / i);
                    string time_Taken_Per_File_String = time_Taken_Per_File.ToString("%d") + " " + time_Taken_Per_File.ToString(@"hh\:mm\:ss");
                    label_Uploading_Time_Per_File.Invoke((MethodInvoker)(() => { label_Uploading_Time_Per_File.Text = time_Taken_Per_File_String;}));
                }
                else
                {
                    int i;
                    string temp = "";
                    temp = label_Uploaded.Text;
                    i = Convert.ToInt32(temp);
                    label_Uploaded.Text = i.ToString();
                    label_Time_Taken.Text = time_Taken_String;
                    TimeSpan time_Taken_Per_File = TimeSpan.FromMilliseconds(time_Taken.Milliseconds / i);
                    string time_Taken_Per_File_String = time_Taken_Per_File.ToString("%d") + " " + time_Taken_Per_File.ToString(@"hh\:mm\:ss");
                    label_Uploading_Time_Per_File.Text = time_Taken_Per_File_String;
                }

                while (!completed)
                {
                    try
                    {
                        System.IO.File.Delete(local_File_Path);
                        completed = true;
                        webSocket.Send("Admin_Start_Operation_On_Local_Database_File|^|" + company_Name + "|^|" + file_Name);
                    }
                    catch (Exception exp)
                    { 
                        continue;
                    }
                }
#if Console
                Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#elif Application
                MessageBox.Show("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            });
        }

        private void initialize_FTP_Settings()
        {
#if LOG
            log_Queue.Enqueue("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            Microsoft.Win32.RegistryKey temp = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey temp_0 = temp.OpenSubKey("SOFTWARE", true);
            if (!temp_0.GetSubKeyNames().Contains("Resume View"))
            {
#if Console
                Console.WriteLine("There is no reg entry for the key value \"Resume View\"");
#elif Application
                MessageBox.Show("There is no reg entry for the key value \"Resume View\"");
#endif


                return;
            }

            Microsoft.Win32.RegistryKey resume_View_Key = temp_0.OpenSubKey("Resume View", true);

            ftp_Server = (string)(resume_View_Key.GetValue("ftp_Server"));
            ftp_Server = crypting_Object.DecryptString(ftp_Server);
            ftp_Username = (string)(resume_View_Key.GetValue("ftp_Username"));
            ftp_Username = crypting_Object.DecryptString(ftp_Username);
            ftp_Password = (string)(resume_View_Key.GetValue("ftp_Password"));
            ftp_Password = crypting_Object.DecryptString(ftp_Password);

#if LOG
            log_Queue.Enqueue("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#elif Application
                MessageBox.Show("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        private void button_Stop_Uploading_Click(object sender, EventArgs e)
        {
            System.Threading.Interlocked.Exchange(ref continue_Upload, 0);
            upload_Folder_Path = "";
            upload_Start_Time = new DateTime(1, 1, 1);

            label_Total_Doc_Docx_Files.Text = "0";
            label_Uploaded.Text = "0";
            label_Time_Taken.Text = "0";
            label_Uploading_Time_Per_File.Text = "0";
        }
    }

    public class Candidate_Detail
    {
        public string name { get; private set; }
        public string phone_No { get; private set; }
        public string mail_Id { get; private set; }
        public Candidate_Detail(string name, string phone_No, string mail_Id)
        {
            this.name = name;
            this.phone_No = phone_No;
            this.mail_Id = mail_Id;
        }
    }
}
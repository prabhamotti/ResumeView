using SuperWebSocket;
using SuperWebSocket.Config;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine;
using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using MySql_CSharp_Console;
using System.Collections.Generic;
using WebSocket4Net;

//using HtmlAgilityPack;
//using mshtml;

using Common_Classes;

namespace temp_Check
{
    public class RV_Client
    {
        private string username;
        public string get_Username() { return username; }
        private Server.Role role;
        public Server.Role get_Role() { return role; }
        private string interview_Name;
        public string get_Interview_Name() { return interview_Name; }
        private string company_Name;
        public string get_Company_Name() { return company_Name; }
        private WebSocketSession session;
        public WebSocketSession get_Session() { return session; }
        public System.DateTime Login_Time;
        public System.DateTime Logout_Time;

        public RV_Client(string username,Server.Role role, WebSocketSession session, string interview_Name, string company_Name)
        {
            this.username = username;
            this.role = role;
            this.session = session;
            this.interview_Name = interview_Name;
            this.company_Name = company_Name;
        }
    }

    public class Server : WebSocketServer
    {
        public enum Presence_Status
        {
            Exact_Presence,
            Partial_Presence,
            Absent
        }
        public enum Role
        {
            Feed,
            Filter,
            Call,
            Coordinate,
            Management,
            Admin
        }
        private DBConnect database_Connection;

        private System.Collections.Generic.List<string> base_Connections;
        private System.Collections.Generic.Dictionary<string, RV_Client> clients;  //id,User_Object
        //private System.Tuple<int, WebSocketSession> admin; // <token,session> Admin's token is either 0(if he Logged in) or -1(if he is not Logged in)
#if DEBUG
          private static System.IO.StreamWriter DEBUG_File;
#endif

        private int no_Of_Urls_Count = 5;
        //private int sms_Total_Count;
        //private int sms_Used_Count;

        //System.Collections.ObjectModel.Collection<string> interview_Names;

        Useful_Functions.SimpleAES crypting_Object;

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
        WebSocket doc_To_Html_WebSocket;

        static string resume_View_Server_Address = "";
        static string resume_View_Server_Port = "";
        static string resume_View_Server_Max_Connection = "";
        static string resume_View_Server_Max_Command_Length = "";

        static string reminder_Server_Address = "";
        static string reminder_Server_Port = "";
        static string reminder_Server_Max_Connection = "";
        static string reminder_Server_Max_Command_Length = "";
        WebSocket reminder_Server_WebSocket;

        //static WebSocket doc_To_Html_WebSocket;

        public Dictionary<string, List<Tuple<string, string>>> file_Link_Element_Class_Or_Id_Names; // "naukri",<"class","dwnlnk">
                                                              //////////////////////////////////////// <"id","dlnk">         
                                                              //////////////////////////////////////// "monster",<"class","d09wnlnk">
                                                              //////////////////////////////////////// <"id","dlnk21">       

#if LOG
		static System.Collections.Concurrent.ConcurrentQueue<string> DEBUG_Queue;
        static System.Threading.Thread DEBUG_Thread;  

        private static void DEBUG_Start()
        {
            DEBUG_File = new StreamWriter("DEBUG.txt",true);
            DEBUG_File.AutoFlush = true;
            string temp_String = "";
            while (true)
            { 
                while(DEBUG_Queue.Count > 10)
                    for (int i = 0; i < 10; ++i)
                        if(DEBUG_Queue.TryDequeue(out temp_String))
                            DEBUG_File.WriteLine(temp_String);
                System.Threading.Thread.Sleep(1000);
            }
            DEBUG_File.Close();
        }
#endif

        public Server()
        {
            crypting_Object = new Useful_Functions.SimpleAES();
            load_Values_From_Registry();
            initialize_WebSocket_For_Doc_To_Html_Server();
            clients = new System.Collections.Generic.Dictionary<string, RV_Client>();
            Initialize_Database_Connection();

            this.Setup(new RootConfig(), new ServerConfig
            {
                Port = Int32.Parse(resume_View_Server_Port),
                Ip = "Any",
                MaxConnectionNumber = Int32.Parse(resume_View_Server_Max_Connection),
                MaxCommandLength = 100000,
                Mode = SuperSocket.SocketBase.SocketMode.Async,
                Name = "Resumeview Server"
            }, SocketServerFactory.Instance);

            //interview_Names = new System.Collections.ObjectModel.Collection<string>();
            if (!database_Connection.check_For_Mysql_Connection())
            {
#if Console
                Console.WriteLine("It seems Mysql is not running. Make it to Run, then Restart this");
#endif

                Console.ReadLine();
                Environment.Exit(0);
            }

            assign_The_Function_For_Events();
            file_Link_Element_Class_Or_Id_Names = new Dictionary<string, List<Tuple<string, string>>>();
            load_File_Link_Element_Class_Or_Id_Names();

            base_Connections = new List<string>();
            initialize_WebSocket_For_Reminders_Server();
            //load_Interview_Names(company_Name);
        }
        
        private void initialize_WebSocket_For_Reminders_Server()
        {
            reminder_Server_WebSocket = new WebSocket("ws://" + reminder_Server_Address + ":" + reminder_Server_Port + "/");
            reminder_Server_WebSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(reminder_Server_WebSocket_Message_Received);
            reminder_Server_WebSocket.Opened += new EventHandler(reminder_Server_WebSocket_Opened);
            reminder_Server_WebSocket.Closed += new EventHandler(reminder_Server_WebSocket_Closed);
            reminder_Server_WebSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(reminder_Server_WebSocket_Error);
            reminder_Server_WebSocket.Open();
        }

        void reminder_Server_WebSocket_Opened(object sender, EventArgs e)
        {
            reminder_Server_WebSocket.Send("Resume_View_Server");
        }

        void reminder_Server_WebSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            reminder_Server_WebSocket.Open();
        }

        void reminder_Server_WebSocket_Closed(object sender, EventArgs e)
        {
            reminder_Server_WebSocket.Open();
        }

        void reminder_Server_WebSocket_Message_Received(object sender,MessageReceivedEventArgs e)
        {
            string[] collections = e.Message.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string type = collections[0];
            
            if (type == "Base_Connection_Disconnected")
            {
                string ip_Address = collections[1];
                clear_All_The_Connections_Related_To_This_Ip_Address(ip_Address);
            }
            else if (type == "New_Base_Connection")
            {
                string ip_Address = collections[1];
                //if(base_Connections.Contains(host))
                //    Console.WriteLine("Two base connections from the same host");
                base_Connections.Add(ip_Address);
            }
            /*else if (type == "New_Set_Of_Base_Connections")
            {
                base_Connections.Clear();
                //if(base_Connections.Contains(host))
                //    Console.WriteLine("Two base connections from the same host");
                for (int j = 1; j < collections.Length;++j )
                    base_Connections.Add(collections[j]);
            }*/
        }

        private void Initialize_Database_Connection()
        {
            database_Connection = new DBConnect(database_Server, database_Name, database_Uid, database_Password);
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
        }

        public void initialize_WebSocket_For_Doc_To_Html_Server()
        {
#if DEBUG
            Console.WriteLine();
#endif
            doc_To_Html_WebSocket = new WebSocket("ws://" + doc_To_Html_Server_Address + ":" + doc_To_Html_Server_Port + "/");
            doc_To_Html_WebSocket.Closed += new EventHandler(doc_To_Html_WebSocket_Closed);
            doc_To_Html_WebSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(doc_To_Html_WebSocket_Error);
        }

        void doc_To_Html_WebSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            doc_To_Html_WebSocket.Open();
        }

        void doc_To_Html_WebSocket_Closed(object sender, EventArgs e)
        {
            doc_To_Html_WebSocket.Open();
        }

        private void assign_The_Function_For_Events()
        {
            this.NewSessionConnected += new SessionEventHandler<WebSocketSession>(this.WebSocketServer_NewSessionConnected);
            this.NewDataReceived += new SessionEventHandler<WebSocketSession, byte[]>(this.WebSocketServer_NewDataReceived);
            this.NewMessageReceived += new SessionEventHandler<WebSocketSession, string>(this.WebSocketServer_NewMessageReceived);
            this.SessionClosed += new SessionEventHandler<WebSocketSession, SuperSocket.SocketBase.CloseReason>(this.WebSocketServer_SessionClosed);

            this.Admin_Change_Password_Event = new Admin_Change_Password_Delegate(this.Admin_Change_Password_Function);
            this.Admin_Get_Users_Details_Event = new Admin_Get_Users_Details_Delegate(this.Admin_Get_Users_Details_Function);
            this.Admin_Add_New_Account_Event = new Admin_Add_New_Account_Delegate(this.Admin_Add_New_Account_Function);
            this.Admin_Update_Account_Event = new Admin_Update_Account_Delegate(this.Admin_Update_Account_Function);
            this.Admin_Delete_Account_Event = new Admin_Delete_Account_Delegate(this.Admin_Delete_Account_Function);
            this.Admin_Get_Filtered_Candidates_Details_Event = new Admin_Get_Filtered_Candidates_Details_Delegate(this.Admin_Get_Filtered_Candidates_Details_Function);
            this.Admin_Get_Qualified_Candidates_Details_Event = new Admin_Get_Qualified_Candidates_Details_Delegate(this.Admin_Get_Qualified_Candidates_Details_Function);
            this.Admin_Get_Attended_Candidates_Details_Event = new Admin_Get_Attended_Candidates_Details_Delegate(this.Admin_Get_Attended_Candidates_Details_Function);
            this.Admin_Get_Selected_Candidates_Details_Event = new Admin_Get_Selected_Candidates_Details_Delegate(this.Admin_Get_Selected_Candidates_Details_Function);
            this.Admin_Get_Joined_Candidates_Details_Event = new Admin_Get_Joined_Candidates_Details_Delegate(this.Admin_Get_Joined_Candidates_Details_Function);
            this.Admin_Update_Interview_Event = new Admin_Update_Interview_Delegate(this.Admin_Update_Interview_Function);
            this.Admin_Add_This_Url_In_This_Interview_Event = new Admin_Add_This_Url_In_This_Interview_Delegate(this.Admin_Add_This_Url_In_This_Interview_Function);
            this.Admin_Get_History_For_This_Url_Event = new Admin_Get_History_For_This_Url_Delegate(this.Admin_Get_History_For_This_Url_Function);

            this.Admin_Create_New_Dataset_Event = new Admin_Create_New_Dataset_Delegate(this.Admin_Create_New_Dataset_Function);
            this.Admin_Add_Interview_Event = new Admin_Add_Interview_Delegate(this.Admin_Add_Interview_Function);
            this.Admin_Delete_Interview_Name_Event = new Admin_Delete_Interview_Name_Delegate(this.Admin_Delete_Interview_Name_Function);
            this.Admin_Start_Operation_On_Local_Database_File_Event = new Admin_Start_Operation_On_Local_Database_File_Delegate(this.Admin_Start_Operation_On_Local_Database_File_Function);

            this.Logging_In_Event = new Logging_In_Delegate(this.Logging_In_Function);
            this.Get_Interview_Names_Event = new Get_Interview_Names_Delegate(this.Get_Interview_Names_Function);
            this.Request_For_Logging_Out_Event = new Request_For_Logging_Out_Delegate(this.Request_For_Logging_Out_Function);
            this.Feed_Event = new Feed_Delegate(this.Feed_Function);
            this.Filter_Accept_Event = new Filter_Accept_Delegate(this.Filter_Accept_Function);
            this.Filter_Reject_Event = new Filter_Reject_Delegate(this.Filter_Reject_Function);
            //this.Url_Event = new Url_Delegate(this.Url_Function);
            this.Get_Urls_For_Filter_Event = new Get_Urls_For_Filter_Delegate(this.Get_Urls_For_Filter_Function);
            this.Get_Contents_For_Caller_Event = new Get_Contents_For_Caller_Delegate(this.Get_Contents_For_Caller_Function);
            this.Get_Contents_For_Coordinator_Event = new Get_Contents_For_Coordinator_Delegate(this.Get_Contents_For_Coordinator_Function);
            this.Get_Refresh_Event = new Get_Refresh_Delegate(this.Get_Refresh_Function);
            this.SMS_Event = new SMS_Delegate(this.SMS_Function);
            this.Caller_Accept_Event = new Caller_Accept_Delegate(this.Caller_Accept_Function);
            this.Caller_Reject_Event = new Caller_Reject_Delegate(this.Caller_Reject_Function);
            this.SMS_Format_Event = new SMS_Format_Delegate(this.SMS_Format_Function);
            this.Subject_Format_Event = new Subject_Format_Delegate(this.Subject_Format_Function);
            this.Mail_Format_Event = new Mail_Format_Delegate(this.Mail_Format_Function);
            this.Attended_Status_Event = new Attended_Status_Delegate(this.Attended_Status_Function);
            this.Selected_Status_Event = new Selected_Status_Delegate(this.Selected_Status_Function);
            this.Joined_Status_Event = new Joined_Status_Delegate(this.Joined_Status_Function);
        }

        private void load_File_Link_Element_Class_Or_Id_Names()
        {
            file_Link_Element_Class_Or_Id_Names = database_Connection.get_File_Link_Element_Class_Or_Id_Names();
        }

        //private void load_Interview_Names(string company_Name)
        //{
        //    interview_Names = database_Connection.get_Interview_Names(company_Name);
        //}

        public void start()
        {
            base.Start();
            doc_To_Html_WebSocket.Open();
        }

        static string surround_It_With_Single_Quote(string given)
        {
            return "'" + given + "'";
        }

        /*public static double get_No_Of_Days_For_Expiry()
        {
            try
            {
                // Setting
                // I have to use CreateSubKey 
                // (create or open it if already exits), 
                // 'cause OpenSubKey open a subKey as read-only
                Microsoft.Win32.RegistryKey temp = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
                Microsoft.Win32.RegistryKey temp_0 = temp.OpenSubKey("SOFTWARE", true);
                Microsoft.Win32.RegistryKey temp_1 = temp_0.OpenSubKey("Resume View");
                //temp_1.CreateSubKey("Date");
                // Save the value
                DateTime expiry_Date = new DateTime();
                if (temp_1.GetValue("Date") != null)
                {
                    Useful_Functions.SimpleAES temp_Crypting_Object = new Useful_Functions.SimpleAES();
                    string regval_String = (string)temp_1.GetValue("Date");
                    string decrypted_String = temp_Crypting_Object.DecryptString(regval_String);
                    DateTime.TryParse(decrypted_String, out expiry_Date);
                }

                Console.WriteLine("Connecting with the GMT time server,it may take time. If it takes more than 180 seconds(three minutes) Please restart the server");
                DateTime current_Date_From_Time_Server =Useful_Functions.Useful_Functions.GetNISTDate();
                Console.WriteLine("Got the reply from GMT server");
                if (current_Date_From_Time_Server != Useful_Functions.Useful_Functions.GetDummyDate())
                    if (expiry_Date.Date >= current_Date_From_Time_Server.Date)
                        return (expiry_Date.Date - current_Date_From_Time_Server.Date).TotalDays;
                    else
                        return -1;
                else
                    return -100;
            }
            catch (Exception e)
            {
                // AAAAAAAAAAARGH, an error!
                Console.WriteLine(e.Message);
                return -200;
            }
        }
        */

        public string get_Database_Size_Details()
        {
            return database_Connection.get_Database_Details_As_String();
        }

        public void download_This_File_Upload_It_Then_Delete_It(string fileLink, string cookies_String, string local_Saving_File_Path, string url, string company_Name, string interview_Name, string token)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
#if DEBUG
                Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

                WebClient wb = new WebClient();
                wb.Headers.Add(HttpRequestHeader.Cookie, cookies_String);
                //wb.DownloadDataCompleted += new DownloadDataCompletedEventHandler(file_Download_Completed);
                wb.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(file_Download_Completed);
                Dictionary<string, string> temp_Dict = new Dictionary<string, string>();
                temp_Dict.Add("token", token);
                temp_Dict.Add("fileLink", fileLink);
                temp_Dict.Add("cookies_String", cookies_String);
                temp_Dict.Add("local_Saving_File_Path", local_Saving_File_Path);
                temp_Dict.Add("url", url);
                temp_Dict.Add("company_Name", company_Name);
                temp_Dict.Add("interview_Name", interview_Name);
                wb.DownloadFileAsync(new Uri(fileLink), local_Saving_File_Path, temp_Dict);
#if DEBUG
                Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            });
        }

        public void file_Download_Completed(object sender, System.ComponentModel.AsyncCompletedEventArgs args)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
#if DEBUG
                Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
                WebClient wb = sender as WebClient;
                Dictionary<string, string> temp_Dict = args.UserState as Dictionary<string, string>;
                WebHeaderCollection collection = wb.ResponseHeaders;
                if (collection != null)
                {
                    string a = "doc";
                    if (collection["Content-Disposition"] != null)
                    {
                        a = collection["Content-Disposition"];
                        a = a.Substring(a.IndexOf(";") + 1);
                        a = a.Substring(a.LastIndexOf(".") + 1);
                    }
                    System.IO.File.Move(temp_Dict["local_Saving_File_Path"], temp_Dict["local_Saving_File_Path"] + "." + a);
                    temp_Dict["local_Saving_File_Path"] = temp_Dict["local_Saving_File_Path"] + "." + a;
                    string file_Name = temp_Dict["local_Saving_File_Path"];
                    if (temp_Dict["local_Saving_File_Path"].Contains("/"))
                        file_Name = temp_Dict["local_Saving_File_Path"].Substring(temp_Dict["local_Saving_File_Path"].LastIndexOf("/") + 1);

                    string company_Name = temp_Dict["company_Name"];
                    string interview_Name = temp_Dict["interview_Name"];
                    string url = temp_Dict["url"];

                    UploadFile_Then_Delete_It(temp_Dict["local_Saving_File_Path"], file_Name, url, interview_Name, company_Name);
                }
                else
                {
                    string local_Saving_File_Path = temp_Dict["local_Saving_File_Path"];
                    System.IO.File.Delete(local_Saving_File_Path);
                    if (clients.ContainsKey(temp_Dict["token"]))
                        clients[temp_Dict["token"]].get_Session().SendResponse("Verify|^|" + temp_Dict["url"]);
                    string company_Name = temp_Dict["company_Name"];
                    string interview_Name = temp_Dict["interview_Name"];
                    string url = temp_Dict["url"];
                    database_Connection.Delete_Entry_By_Referring_This_Url(url, interview_Name, company_Name);
                }
#if DEBUG
                Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            });
        }

        private void UploadFile_Then_Delete_It(string local_FilePath, string server_File_Path, string url, string interview_Name, string company_Name)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
#if DEBUG
                Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

                //System.IO.FileInfo fileInfo = new System.IO.FileInfo(local_FilePath);// (localPath + fileName);
                //System.IO.FileStream stream = fileInfo.OpenRead();
                //stream.Seek(0, SeekOrigin.Begin);
                string file_Name = local_FilePath;
                if (file_Name.Contains("/"))
                    file_Name = file_Name.Substring(file_Name.LastIndexOf("/") + 1);
                string uri = String.Format("ftp://{0}/{1}", ftp_Server, file_Name);

                try
                {
                    WebClient temp_WebClient = new WebClient();
                    temp_WebClient.Credentials = new NetworkCredential(ftp_Username, ftp_Password);
                    temp_WebClient.UploadFileCompleted += new UploadFileCompletedEventHandler(WebClient_UploadFileCompleted);
                    Dictionary<string, string> temp_Dict = new Dictionary<string, string>();
                    temp_Dict.Add("local_FilePath", local_FilePath);
                    temp_Dict.Add("url", url);
                    temp_Dict.Add("interview_Name", interview_Name);
                    temp_Dict.Add("company_Name", company_Name);
                    temp_Dict.Add("file_Name", file_Name);
                    temp_WebClient.UploadFileAsync(new Uri(uri), null, local_FilePath, temp_Dict);
                }
                catch (Exception exc)
                {
                    //return false;
                }
#if DEBUG
                Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            });
        }

        void WebClient_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
#if DEBUG
                Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

                Dictionary<string, string> temp_Dict = e.UserState as Dictionary<string, string>;
                string local_File_Path = temp_Dict["local_FilePath"];
                string url = temp_Dict["url"];
                string interview_Name = temp_Dict["interview_Name"];
                string company_Name = temp_Dict["company_Name"];
                string file_Name = temp_Dict["file_Name"];
                bool completed = false;
                while (!completed)
                {
                    try
                    {
                        System.IO.File.Delete(local_File_Path);
                        string webPage_Address = database_Connection.get_WebPage_Base_Address_For_This_Company(company_Name);
                        string web_Path = webPage_Address + file_Name;
                        database_Connection.update_File_Location(url, web_Path, interview_Name, company_Name);
                        completed = true;
                    }
                    catch (Exception exp)
                    {
                        continue;
                    }
                }
#if DEBUG
                Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            });
        }

        void Delete_This_File_In_The_Server_Side(string file_Name)
        {
            System.Threading.ThreadPool.QueueUserWorkItem((state) =>
            {
#if DEBUG
                Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
                string uri = String.Format("ftp://{0}/{1}", ftp_Server, file_Name);

                try
                {
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                    request.Credentials = new NetworkCredential(ftp_Username, ftp_Password);
                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    //Console.WriteLine("Delete status: {0}", response.StatusDescription);  
                    response.Close();
                }
                catch (Exception exc)
                {
                    //return false;
                }
#if DEBUG
                Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            });

        }

        public void WebSocketServer_NewSessionConnected(WebSocketSession session)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            //DEBUG_File.Write("connected : " + session.Host + System.Environment.NewLine);
            //MessageBox.Show("connected : " + session.Host + System.Environment.NewLine);
            //session.StartSession();
            //session.SendResponse("Message|^|Welcome client");
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public void WebSocketServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason close_Reason)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string ip_Address = session.RemoteEndPoint.Address.ToString();
            //MessageBox.Show("Host "+ host + " requested a disconnect");


            if (close_Reason == SuperSocket.SocketBase.CloseReason.ServerClosing)
                return;
            else
                clear_All_The_Connections_Related_To_This_Ip_Address(ip_Address);

#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Admin_Change_Password_Delegate(WebSocketSession session, string e);
        public event Admin_Change_Password_Delegate Admin_Change_Password_Event;
        public void Admin_Change_Password_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string old_Password = temp_Collection[1];
            string new_Password = temp_Collection[2];
            string company_Name = temp_Collection[3];

            User_Detail temp_User_Detail = database_Connection.get_User_Details_For_This_User("Admin",company_Name);
            string image_Path = temp_User_Detail.image_Path;

            if (database_Connection.Check_User_Account("Admin", old_Password, company_Name) == true)
            {
                database_Connection.Update_User_Info("Admin", new_Password, "0000000000", "mail_id","Employee",image_Path,company_Name);
                session.SendResponse("Message|^|Admin Password Changed Successfully");
            }
            else
                session.SendResponse("Message|^|Your old Password didnt match with the database record");
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            return;
        }

        public delegate void Admin_Get_Users_Details_Delegate(WebSocketSession session, string e);
        public event Admin_Get_Users_Details_Delegate Admin_Get_Users_Details_Event;
        public void Admin_Get_Users_Details_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string company_Name = temp_Collection[1];
            session.SendResponse("Users_Details|^|" + get_Users_Details_As_String(company_Name));
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        public delegate void Admin_Add_New_Account_Delegate(WebSocketSession session, string e);
        public event Admin_Add_New_Account_Delegate Admin_Add_New_Account_Event;
        public void Admin_Add_New_Account_Function(WebSocketSession session, string e) //format is Admin_Add_New_Account|^|username|^|password
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string username = temp_Collection[1];
            string password = temp_Collection[2];
            string phone_No = temp_Collection[3];
            string mail_Id = temp_Collection[4];
            string type = temp_Collection[5];
            string image_Path = temp_Collection[6];
            string company_Name = temp_Collection[7];
            
            if (database_Connection.Is_This_User_Name_Exist(username, company_Name))
                session.SendResponse("Message|^|User Name Already Exist");
            else
            {
                database_Connection.Add_User_Info(username, password, phone_No, type, company_Name,image_Path,mail_Id);
                session.SendResponse("Users_Details|^|" + get_Users_Details_As_String(company_Name));
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        public delegate void Admin_Update_Account_Delegate(WebSocketSession session, string e);
        public event Admin_Update_Account_Delegate Admin_Update_Account_Event;
        public void Admin_Update_Account_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string username = temp_Collection[1];
            string password = temp_Collection[2];
            string phone_No = temp_Collection[3];
            string mail_Id = temp_Collection[4];
            string type = temp_Collection[5];
            string image_Path = temp_Collection[6];
            string company_Name = temp_Collection[7];

            if (!database_Connection.Is_This_User_Name_Exist(username, company_Name))
                session.SendResponse("Message|^|There is no such username in the DataBase");
            else
            {
                database_Connection.Update_User_Info(username, password, phone_No, mail_Id,type,image_Path, company_Name);
                session.SendResponse("Users_Details|^|" + get_Users_Details_As_String(company_Name));
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        public delegate void Admin_Delete_Account_Delegate(WebSocketSession session, string e);
        public event Admin_Delete_Account_Delegate Admin_Delete_Account_Event;
        public void Admin_Delete_Account_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string username = temp_Collection[1];
            string company_Name = temp_Collection[2];

            if (!database_Connection.Is_This_User_Name_Exist(username, company_Name))
            {
                session.SendResponse("Message|^|There is no such username in the DataBase");
            }
            else
            {
                User_Detail temp_User_Detail = database_Connection.get_User_Details_For_This_User(username, company_Name);
                if (temp_User_Detail.image_Path != "")
                {
                    Uri temp_Uri = new Uri("ftp://" + ftp_Server + "/Photos/" + temp_User_Detail.image_Path.Substring(temp_User_Detail.image_Path.LastIndexOf("/") + 1));
                    FtpWebRequest ftp_Web_Request = (FtpWebRequest)WebRequest.Create(temp_Uri);
                    ftp_Web_Request.Credentials = new NetworkCredential(ftp_Username, ftp_Password);
                    ftp_Web_Request.Method = WebRequestMethods.Ftp.DeleteFile;
                    FtpWebResponse response = (FtpWebResponse)ftp_Web_Request.GetResponse();
                    string status_Desc = response.StatusDescription;
                }
                //if (!System.Text.RegularExpressions.Regex.IsMatch(status_Desc, "^2[0-9]{2}"))
                //{
                //    MessageBox.Show("Server is not allowing to delete the old file, Aborting");
                //    return;
                //}

                database_Connection.Delete_User_Info(username, company_Name);
                session.SendResponse("Users_Details|^|" + get_Users_Details_As_String(company_Name));
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        public delegate void Admin_Get_Filtered_Candidates_Details_Delegate(WebSocketSession session, string e);
        public event Admin_Get_Filtered_Candidates_Details_Delegate Admin_Get_Filtered_Candidates_Details_Event;
        public void Admin_Get_Filtered_Candidates_Details_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string interview_Name = temp_Collection[1];
            string company_Name = temp_Collection[2];

            string temp = get_Filtered_Candidates_Details_As_String(interview_Name, company_Name);
            if (temp == "")
                session.SendResponse("Message|^|There is no Filtered Candidate Now");
            else
                session.SendResponse("Filtered_Candidates_Details|^|" + temp);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        public delegate void Admin_Get_Qualified_Candidates_Details_Delegate(WebSocketSession session, string e);
        public event Admin_Get_Qualified_Candidates_Details_Delegate Admin_Get_Qualified_Candidates_Details_Event;
        public void Admin_Get_Qualified_Candidates_Details_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string interview_Name = temp_Collection[1];
            string company_Name = temp_Collection[2];
            string temp = get_Qualified_Candidates_Details_As_String(interview_Name, company_Name);
            if (temp == "")
                session.SendResponse("Message|^|There is no Qualified Candidate ni");
            else
                session.SendResponse("Qualified_Candidates_Details|^|" + temp);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        public delegate void Admin_Get_Attended_Candidates_Details_Delegate(WebSocketSession session, string e);
        public event Admin_Get_Attended_Candidates_Details_Delegate Admin_Get_Attended_Candidates_Details_Event;
        public void Admin_Get_Attended_Candidates_Details_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string interview_Name = temp_Collection[1];
            string company_Name = temp_Collection[2];
            string temp = get_Attended_Candidates_Details_As_String(interview_Name, company_Name);
            if (temp == "")
                session.SendResponse("Message|^|There is no Attended Candidate Now");
            else
                session.SendResponse("Attended_Candidates_Details|^|" + temp);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        public delegate void Admin_Get_Selected_Candidates_Details_Delegate(WebSocketSession session, string e);
        public event Admin_Get_Selected_Candidates_Details_Delegate Admin_Get_Selected_Candidates_Details_Event;
        public void Admin_Get_Selected_Candidates_Details_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string interview_Name = temp_Collection[1];
            string company_Name = temp_Collection[2];
            string temp = get_Selected_Candidates_Details_As_String(interview_Name, company_Name);
            if (temp == "")
                session.SendResponse("Message|^|There is no Selected Candidate Now");
            else
                session.SendResponse("Selected_Candidates_Details|^|" + temp);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        public delegate void Admin_Get_Joined_Candidates_Details_Delegate(WebSocketSession session, string e);
        public event Admin_Get_Joined_Candidates_Details_Delegate Admin_Get_Joined_Candidates_Details_Event;
        public void Admin_Get_Joined_Candidates_Details_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string interview_Name = temp_Collection[1];
            string company_Name = temp_Collection[2];
            string temp = get_Joined_Candidates_Details_As_String(interview_Name, company_Name);
            if (temp == "")
                session.SendResponse("Message|^|There is no Joinded Candidate Now");
            else
                session.SendResponse("Joined_Candidates_Details|^|" + temp);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        public delegate void Admin_Update_Interview_Delegate(WebSocketSession session, string e);
        public event Admin_Update_Interview_Delegate Admin_Update_Interview_Event;
        public void Admin_Update_Interview_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string interview_Detail = temp_Collection[1];
            string company_Name = temp_Collection[2];
            Interview_And_Candidates_Count_Detail temp = new Interview_And_Candidates_Count_Detail(interview_Detail);
            if (database_Connection.is_This_Interview_Name_Exist(temp.interview_Name, company_Name))
            {
                database_Connection.Update_Interview_Details(temp.interview_Name, temp.date, temp.coordinator, temp.venue, temp.payment, temp.settled, company_Name);
                session.SendResponse("Message|^|Interview Details got updated");
                refresh_The_Admin_With_Candidates_Counts(company_Name);
            }
            else
                session.SendResponse("Message|^|Interview name is not exist");
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Admin_Add_This_Url_In_This_Interview_Delegate(WebSocketSession session, string e);
        public event Admin_Add_This_Url_In_This_Interview_Delegate Admin_Add_This_Url_In_This_Interview_Event;
        public void Admin_Add_This_Url_In_This_Interview_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            //format is "Admin_Add_This_Url_In_This_Interview|^|url|^|old_Interview_Name|^|new_Interview_Name|^|DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")|^|company_Name;
            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string url = temp_Collection[1];
            string old_Interview_Name = temp_Collection[2];
            string new_Interview_Name = temp_Collection[3];

            string time_String = temp_Collection[4];
            DateTime datetime;
            DateTime.TryParse(time_String, out datetime);
            string company_Name = temp_Collection[5];

            if (!database_Connection.Is_This_Url_Exist_For_This_Interview(url, new_Interview_Name, company_Name))
            {
                database_Connection.supply_This_Urls_Details_To_This_New_Interview(url, old_Interview_Name, new_Interview_Name, company_Name, datetime);
                refresh_All_The_Callers_With_Data(company_Name);
                refresh_The_Admin_With_Candidates_Counts(company_Name);
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Admin_Get_History_For_This_Url_Delegate(WebSocketSession session, string e);
        public event Admin_Get_History_For_This_Url_Delegate Admin_Get_History_For_This_Url_Event;
        public void Admin_Get_History_For_This_Url_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string url = temp_Collection[1];
            string company_Name = temp_Collection[2];
            string main_String = database_Connection.get_History_For_This_Url(url, "", company_Name);
            session.SendResponse("History_Of_The_Required_Url|^|" + main_String);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Admin_Create_New_Dataset_Delegate(WebSocketSession session, string e);
        public event Admin_Create_New_Dataset_Delegate Admin_Create_New_Dataset_Event;
        public void Admin_Create_New_Dataset_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string company_Name = temp_Collection[1];
            Initialize_Tables_For_First_Time();
            session.SendResponse("Message|^|New Dataset Got Created");
            refresh_The_Admin_With_Candidates_Counts(company_Name);
            refresh_All_The_Callers_With_Data(company_Name);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        public delegate void Admin_Add_Interview_Delegate(WebSocketSession session, string e);
        public event Admin_Add_Interview_Delegate Admin_Add_Interview_Event;
        public void Admin_Add_Interview_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string content = temp_Collection[1];
            string company_Name = temp_Collection[2];
            Interview_And_Candidates_Count_Detail temp = new Interview_And_Candidates_Count_Detail(content);
            if (!database_Connection.is_This_Interview_Name_Exist(temp.interview_Name, company_Name))
            {
                database_Connection.Add_Interview(temp.interview_Name, temp.date, temp.coordinator, temp.venue, company_Name);
                refresh_The_Admin_With_Candidates_Counts(company_Name);
            }
            else
                session.SendResponse("Message|^|That interview name already exist");
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

        }

        private void Logout_All_The_Users()
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            List<string> tokens = new List<string>();
            foreach (string token in clients.Keys)
                tokens.Add(token);
            foreach (string token in tokens)
            {
                if (clients.ContainsKey(token))
                {
                    clients[token].get_Session().SendResponse("Make_Logout|^|");
                    clients[token].Logout_Time = System.DateTime.Now;
                    clients.Remove(token);
                }
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        private void Logout_Users_Who_Have_This_Interview_Name(string interview_Name, string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            List<string> tokens = new List<string>();
            foreach (string token in clients.Keys)
                tokens.Add(token);
            foreach (string token in tokens)
            {
                if (clients.ContainsKey(token))
                {
                    if (clients[token].get_Company_Name() == company_Name && clients[token].get_Interview_Name() == interview_Name && clients[token].get_Role() != Role.Admin)
                    {
                        clients[token].get_Session().SendResponse("Make_Logout|^|");
                        clients[token].Logout_Time = System.DateTime.Now;
                        clients.Remove(token);
                    }
                }
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

        }

        public delegate void Admin_Delete_Interview_Name_Delegate(WebSocketSession session, string e);
        public event Admin_Delete_Interview_Name_Delegate Admin_Delete_Interview_Name_Event;
        public void Admin_Delete_Interview_Name_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string interview_Name = temp_Collection[1];
            string company_Name = temp_Collection[2];
            if (database_Connection.is_This_Interview_Name_Exist(interview_Name, company_Name))
            {
                Logout_Users_Who_Have_This_Interview_Name(interview_Name, company_Name);
                database_Connection.Delete_Interview_Name(interview_Name, company_Name);
                refresh_The_Admin_With_Candidates_Counts(company_Name);
            }
            else
                session.SendResponse("Message|^|That interview name not exist");
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Admin_Start_Operation_On_Local_Database_File_Delegate(WebSocketSession session, string e);
        public event Admin_Start_Operation_On_Local_Database_File_Delegate Admin_Start_Operation_On_Local_Database_File_Event;
        public void Admin_Start_Operation_On_Local_Database_File_Function(WebSocketSession session, string e)
        {
            //The format is "Admin_Start_Operation_On_Local_Database_File|^|company_Name|^|file_Name"
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            System.Threading.ThreadPool.QueueUserWorkItem((state)=>
            {
                string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
                string company_Name = temp_Collection[1];
                string file_Name = temp_Collection[2];

                string folder_Path = database_Connection.get_Exact_FTP_Home_Folder_Path_For_This_Company_Name(company_Name);
                string server_Side_File_Path = folder_Path + "\\Local_Database\\" + file_Name;
                string web_Page_Base_Address = database_Connection.get_WebPage_Base_Address_For_This_Company(company_Name);
                //server_Side_File_Paths_For_Doc_To_Html_Server.Enqueue(server_Side_File_Path+"|^|"+web_Page_Base_Address);
                if(doc_To_Html_WebSocket.State == WebSocketState.Open)
                    doc_To_Html_WebSocket.Send(server_Side_File_Path + "|^|" + web_Page_Base_Address);
            });
            
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif 
        }

        public int get_No_Of_Expiry_Days_For_This_Company_Name(string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            DateTime expiry_Date = database_Connection.get_Expiry_Date_For_This_Company_Name(company_Name);
            int no_Of_Days = -1;

            if (expiry_Date.Year != 1)
                no_Of_Days = expiry_Date.Subtract(DateTime.Now).Days;
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return no_Of_Days;
        }

        public delegate void Logging_In_Delegate(WebSocketSession session, string e);
        public event Logging_In_Delegate Logging_In_Event;
        public void Logging_In_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            if (!base_Connections.Contains(session.RemoteEndPoint.Address.ToString()))
            {
                session.SendResponse("Base_Connection_Missing");
                return;
            }
            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string content = temp_Collection[1];
            string company_Name = crypting_Object.DecryptString(temp_Collection[2]);

            int no_Of_Days_For_Expire = get_No_Of_Expiry_Days_For_This_Company_Name(company_Name);

            string username = "";
            string role_String = "";
            string interview_Name = "";
            extract_User_Information(content, ref username, ref role_String, ref interview_Name);

            username = crypting_Object.DecryptString(username);

            Role role = (Role)(Role.Parse(typeof(Role), role_String));


            if (role == Role.Admin && interview_Name == "")
            {
                bool found = false;
                foreach (KeyValuePair<string, RV_Client> client in clients)
                {
                    if (client.Value.get_Username() == "Admin" && client.Value.get_Company_Name() == company_Name)
                    {
                        session.SendResponse("Message|^|Admin already Logged in");
                        session.SendResponse("Login_Accepted|^|Admin");
                        session.SendResponse("Users_Details|^|" + get_Users_Details_As_String(company_Name));
                        refresh_The_Admin_With_Candidates_Counts(company_Name);
                        refresh_The_Admin_With_Remaining_SMS_Count(company_Name);
                        found = true;
                    }
                }
                if (!found)
                {
                    string temp_Id_1 = add_Client(username, role, session, interview_Name, company_Name);
                    session.SendResponse("Login_Accepted|^|" + temp_Id_1);
                    //session.SendResponse("Login_Accepted|^|Admin");
                    session.SendResponse("Users_Details|^|" + get_Users_Details_As_String(company_Name));
                    refresh_The_Admin_With_Candidates_Counts(company_Name);
                    refresh_The_Admin_With_Remaining_SMS_Count(company_Name);
                }
                return;
            }

            if (role == Role.Coordinate)
            {
                string temp_Content = database_Connection.get_Interview_Details_For_This_Interview_Name_With_GraveAccent_As_Separator(interview_Name, company_Name);
                string[] temp_String_Collection = temp_Content.Split(new string[] { "`" }, StringSplitOptions.None);
                int position_Colon = temp_String_Collection[1].IndexOf(":");
                int position_Comma = temp_String_Collection[1].IndexOf(",");
                string coordinator = temp_String_Collection[1].Substring(position_Colon + 1, position_Comma - position_Colon - 1);
                if (coordinator != username)
                {
                    session.SendResponse("Wrong_Coordinator");
                    return;
                }
            }

            Presence_Status status = get_Presence_Status_Of_This_Account(username, role, interview_Name);

            switch (status)
            {
                case Presence_Status.Exact_Presence:
                    {
                        session.SendResponse("Message|^|" + "You already Logged in with this same role");
                        break;
                    }
                case Presence_Status.Partial_Presence:
                    {
                        session.SendResponse("Message|^|" + "You are taking more than one role");
                        string temp_Id_0 = add_Client(username, role, session, interview_Name, company_Name);
                        session.SendResponse("Login_Accepted|^|" + temp_Id_0 + "|^|" + username);

                        string sms_Format = database_Connection.get_SMS_Format_For_This_Username(username, interview_Name, company_Name);
                        session.SendResponse("Update_SMS_Format|^|" + sms_Format);
                        string subject_Format = database_Connection.get_Subject_Format_For_This_Username(username, interview_Name, company_Name);
                        session.SendResponse("Update_Subject_Format|^|" + subject_Format);
                        string mail_Format = database_Connection.get_Mail_Format_For_This_Username(username, interview_Name, company_Name);
                        session.SendResponse("Update_Mail_Format|^|" + mail_Format);
                        string interview_Details = database_Connection.get_Interview_Details_For_This_Interview_Name_With_GraveAccent_As_Separator(interview_Name, company_Name);
                        session.SendResponse("Update_Interview_Details|^|" + interview_Details);
                        break;
                    }
                case Presence_Status.Absent:
                    {
                        string temp_Id_1 = add_Client(username, role, session, interview_Name, company_Name);
                        session.SendResponse("Login_Accepted|^|" + temp_Id_1 + "|^|" + username);
                        string sms_Format_0 = database_Connection.get_SMS_Format_For_This_Username(username, interview_Name, company_Name);
                        session.SendResponse("Update_SMS_Format|^|" + sms_Format_0);
                        string subject_Format_0 = database_Connection.get_Subject_Format_For_This_Username(username, interview_Name, company_Name);
                        session.SendResponse("Update_Subject_Format|^|" + subject_Format_0);
                        string mail_Format_0 = database_Connection.get_Mail_Format_For_This_Username(username, interview_Name, company_Name);
                        session.SendResponse("Update_Mail_Format|^|" + mail_Format_0);
                        string interview_Details_0 = database_Connection.get_Interview_Details_For_This_Interview_Name_With_GraveAccent_As_Separator(interview_Name, company_Name);
                        session.SendResponse("Update_Interview_Details|^|" + interview_Details_0);
                        break;
                    }
            }


            if (!database_Connection.get_Interview_Names(company_Name).Contains(interview_Name))
            {
                session.SendResponse("Message|^| Interview name is not found in the database");
                return;
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Get_Interview_Names_Delegate(WebSocketSession session, string e);
        public event Get_Interview_Names_Delegate Get_Interview_Names_Event;
        public void Get_Interview_Names_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string company_Name = crypting_Object.DecryptString(temp_Collection[1]);
            session.SendResponse("InterView_Names|^|" + string.Join(";", database_Connection.get_Interview_Names(company_Name)));
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Request_For_Logging_Out_Delegate(WebSocketSession session, string e);
        public event Request_For_Logging_Out_Delegate Request_For_Logging_Out_Event;
        public void Request_For_Logging_Out_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_String_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string token = temp_String_Collection[1];

            if (clients.ContainsKey(token))
            {
                clients[token].Logout_Time = System.DateTime.Now;
                clients[token].get_Session().Close();
                clients.Remove(token);
            }
            else
                session.SendResponse("Message|^|You are not Logged in");
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        void clear_All_The_Connections_Related_To_This_Ip_Address(string ip_Address)
        {
            List<string> required_Keys = (from f in clients.Keys where clients[f].get_Session().RemoteEndPoint.Address.ToString() == ip_Address select f).ToList();

            foreach (var key in required_Keys)
            {
                clients[key].get_Session().Close();
                clients.Remove(key);
            }
        }

        public delegate void Feed_Delegate(WebSocketSession session, string e);
        public event Feed_Delegate Feed_Event;
        public void Feed_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            /*
            // The format is Feed|^|token|^|name|^|phoneNo|^|mailId|^|cookies|^|fileLink|^|time_String|^|url|^|company_Name;  where time_String = "yyyy-MM-dd HH:mm"
            string[] temp_String_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_String_Collection[1];
            string name = temp_String_Collection[2];
            string phoneNo = temp_String_Collection[3];
            string mailId = temp_String_Collection[4];
            string cookies = temp_String_Collection[5];
            string fileLink = temp_String_Collection[6];
            string time_String = temp_String_Collection[7];
            DateTime feeder_Date;
            DateTime.TryParse(time_String, out feeder_Date);
            string url = temp_String_Collection[8];
            string company_Name = temp_String_Collection[9];

            RV_Client client = clients[token];

            if (database_Connection.Is_This_Url_Exist_For_This_Interview(url, client.get_Interview_Name(), company_Name) == false)
            {
                string guid_String = Useful_Functions.Useful_Functions.get_Guid_String();
                string local_Saving_File_Path = "temp_Storage/" + guid_String;
                string local_Saving_WebPage_Path = "temp_Storage/" + guid_String;

                download_This_File_And_WebPage_Upload_It_Then_Delete_It(fileLink, cookies, local_Saving_File_Path, url, local_Saving_WebPage_Path, company_Name, client.get_Interview_Name(), token);
                database_Connection.Add_URL_Info(client.get_Username(), feeder_Date, url, client.get_Interview_Name(), company_Name, name, phoneNo, mailId);
                session.SendResponse("Feed_Url_Reply|^|Ok");
                refresh_The_Admin_With_Candidates_Counts(company_Name);
            }
            else
                session.SendResponse("Feed_Url_Reply|^|The person is already in database for this interview");
             */

            //Format is "Feed|^|token|^|cookies|^|time_String|^|company_Name|^|selected_Links.join(";")";
            string[] temp_String_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_String_Collection[1];
            string cookies = temp_String_Collection[2];
            string time_String = temp_String_Collection[3];
            DateTime feeder_Date;
            DateTime.TryParse(time_String, out feeder_Date);

            string company_Name =temp_String_Collection[4];
            string selected_Links = temp_String_Collection[5];
            string[] urls = selected_Links.Split(new string[]{";"},StringSplitOptions.None);
            string guid_String = Useful_Functions.Useful_Functions.get_Guid_String();
            string local_Saving_File_Path = "temp_Storage/" + guid_String;

            foreach(string url in urls)
                database_Connection.Add_URL_Info(clients[token].get_Username(),feeder_Date,url,clients[token].get_Interview_Name(),company_Name,"","","",cookies);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Filter_Accept_Delegate(WebSocketSession session, string e);
        public event Filter_Accept_Delegate Filter_Accept_Event;
        public void Filter_Accept_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            // format is Filter_Accept|^|token|^|datetime|^|url|^|company_Name;
            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string token = temp_Collection[1];
            string datetime_String = temp_Collection[2];
            DateTime datetime;
            DateTime.TryParse(datetime_String, out datetime);
            string url = temp_Collection[3];
            string company_Name = temp_Collection[4];

            RV_Client client = clients[token];

            if (database_Connection.Is_This_Url_Exist_For_This_Interview(url, client.get_Interview_Name(), company_Name))
            {
                database_Connection.Update_URL_Info_From_Filterer(url, true, datetime, client.get_Interview_Name(), company_Name);
                session.SendResponse("Filter_Url_Reply|^|Ok");
                refresh_All_The_Callers_With_Data(company_Name);
                refresh_The_Admin_With_Candidates_Counts(company_Name);
            }
            else
                session.SendResponse("Filter_Url_Reply|^|Url is not present in the database");
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Filter_Reject_Delegate(WebSocketSession session, string e);
        public event Filter_Reject_Delegate Filter_Reject_Event;
        public void Filter_Reject_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            // format is Filter_Reject|^|token|^|datetime|^|url|^|company_Name;
            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_Collection[1];

            string datetime_String = temp_Collection[2];
            DateTime datetime;
            DateTime.TryParse(datetime_String, out datetime);
            string url = temp_Collection[3];
            string company_Name = temp_Collection[4];

            RV_Client client = clients[token];

            if (database_Connection.Is_This_Url_Exist_For_This_Interview(url, client.get_Interview_Name(), company_Name))
            {
                database_Connection.Update_URL_Info_From_Filterer(url, false, datetime, client.get_Interview_Name(), company_Name);
                session.SendResponse("Filter_Url_Reply|^|Ok");
                refresh_All_The_Callers_With_Data(company_Name);
                refresh_The_Admin_With_Candidates_Counts(company_Name);
            }
            else
                session.SendResponse("Filter_Url_Reply|^|Url is not present in the database");
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        /*
         * public delegate void Url_Delegate(WebSocketSession session, string e);
        public event Url_Delegate Url_Event;
        public void Url_Function(WebSocketSession session, string e)
        {
            int first_Appearance = e.IndexOf("|^|");
            int second_Appearance = e.IndexOf("|^|", first_Appearance + 1);
            int third_Appearance = e.IndexOf("|^|", second_Appearance + 1);

            string token_String = e.Substring(first_Appearance + 1, second_Appearance - (first_Appearance + 1));
            int token;
            int.TryParse(token_String, out token);
            string result_String = e.Substring(second_Appearance + 1, third_Appearance - (second_Appearance + 1));
            int result;
            int.TryParse(result_String, out result);
            string url = e.Substring(third_Appearance + 1);

            RV_Client client = clients[token];
            switch (client.get_Role())
            {
                case Role.Call:
                    database_Connection.Update_URL_Info_From_Caller(url, result,client.get_Interview_Name());
                    break;
                default:
                    MessageBox.Show("Unexpected Role type");
                    break;
            }
        }*/

        public delegate void Get_Urls_For_Filter_Delegate(WebSocketSession session, string e);
        public event Get_Urls_For_Filter_Delegate Get_Urls_For_Filter_Event;
        public void Get_Urls_For_Filter_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string token = temp_Collection[1];
            string company_Name = temp_Collection[2];

            string username = clients[token].get_Username();

            System.Collections.ObjectModel.Collection<string> temp_Urls;
            temp_Urls = database_Connection.get_Urls_For_Filterer(username, no_Of_Urls_Count, clients[token].get_Interview_Name(), company_Name);

            string temp_String = string.Join("\n", temp_Urls);

            session.SendResponse("Urls|^|" + temp_String);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            //MessageBox.Show("Urls|^|" + temp_String);
        }

        public delegate void Get_Contents_For_Caller_Delegate(WebSocketSession session, string e);
        public event Get_Contents_For_Caller_Delegate Get_Contents_For_Caller_Event;
        public void Get_Contents_For_Caller_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string token = temp_Collection[1];

            string username = clients[token].get_Username();

            string network = temp_Collection[2];
            string no_Of_Lines_String = temp_Collection[3];
            string company_Name = temp_Collection[4];
            int no_Of_Lines;
            int.TryParse(no_Of_Lines_String, out no_Of_Lines);

            System.Collections.ObjectModel.Collection<string> temp_Lines;
            temp_Lines = database_Connection.Get_Contents_For_Caller(username, network, no_Of_Lines, clients[token].get_Interview_Name(), company_Name);

            string temp_String = "";
            if (temp_Lines.Count > 0)
            {
                temp_String = string.Join("~", temp_Lines);
            }

            session.SendResponse("Caller_Page|^|" + temp_String);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Get_Contents_For_Coordinator_Delegate(WebSocketSession session, string e);
        public event Get_Contents_For_Coordinator_Delegate Get_Contents_For_Coordinator_Event;
        public void Get_Contents_For_Coordinator_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string token = temp_Collection[1];
            string company_Name = temp_Collection[2];

            string username = clients[token].get_Username();

            System.Collections.ObjectModel.Collection<string> temp_Lines;
            temp_Lines = database_Connection.Get_Contents_For_Coordinator(username, clients[token].get_Interview_Name(), company_Name);

            string temp_String = "";
            if (temp_Lines.Count > 0)
                temp_String = string.Join("~", temp_Lines);

            session.SendResponse("Coordinator_Page|^|" + temp_String);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Get_Refresh_Delegate(WebSocketSession session, string e);
        public event Get_Refresh_Delegate Get_Refresh_Event;
        public void Get_Refresh_Function(WebSocketSession session, string e) // format is Get_Refresh|^|token
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_Collection[1];
            string company_Name = temp_Collection[2];

            refresh_The_Caller_With_Data(token, company_Name);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void SMS_Delegate(WebSocketSession session, string e);
        public event SMS_Delegate SMS_Event;
        public void SMS_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string phone_Nos = temp_Collection[1];
            string message = temp_Collection[2];
            string company_Name = temp_Collection[3];
            if (database_Connection.get_Remaining_SMS_Count(company_Name) == 0)
            {
                session.SendResponse("Message|^|Sorry the available sms count for you reached zero,further sms's wont be sent");
                return;
            }

            string username = "ma";
            string password = "an46dbk8it";
            string[] numbers = phone_Nos.Split(",".ToCharArray());

            foreach (string number in numbers)
            {
                string data = "<?xml version='1.0' encoding='utf-8'?>" +
                        "<MESSAGE>" +
                        "<USER USERNAME=" + surround_It_With_Single_Quote(username) + " PASSWORD=" + surround_It_With_Single_Quote(password) + "/>" +
                        "<SMS TEXT=" + surround_It_With_Single_Quote(message) + " MESGTYPE='' SENDER='TEST' PRIORITY='1' >" +
                        "<ADDRESS TO=" + surround_It_With_Single_Quote(number) + "/>" +
                        "</SMS>" +
                        "</MESSAGE>";
                /*string test_Data = "<?xml version='1.0' encoding='utf-8'?>"+
                               "<MESSAGE>"+
                               "<USER USERNAME='prabhakaran' PASSWORD='an46dbk8it'/>"+
                               "<SMS TEXT='Hello World' MESGTYPE='' SENDER='TEST' PRIORITY='1' >"+
                               "<ADDRESS TO='9442510190' />"+
                               "</SMS>"+
                               "</MESSAGE>";
                 */
                //MessageBox.Show(number + " " + message);
                data = System.Web.HttpUtility.UrlEncode(data);
                data = "http://sms.dialinfo.org/xmlapi.php?data=" + data;
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(data);
                System.Net.WebResponse temp_Response = null;
                try
                {
                    temp_Response = request.GetResponse();
                    temp_Response.Close();
                    database_Connection.reduce_The_SMS_Count_By_One(company_Name);
                    refresh_The_Admin_With_Remaining_SMS_Count(company_Name);
                }
                catch (System.InvalidOperationException exc)
                {
                    //MessageBox.Show(exc.Message);
                    if (temp_Response != null)
                        temp_Response.Close();
                    session.SendResponse("Message|^|The server was failed to send the SMS");
                    return;
                }
                catch (System.NotSupportedException exc)
                {
                    if (temp_Response != null)
                        temp_Response.Close();
                    //MessageBox.Show("System.NotSupportedException");
                    session.SendResponse("Message|^|The server was failed to send the SMS");
                    return;
                }
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        public delegate void Caller_Accept_Delegate(WebSocketSession session, string e);
        public event Caller_Accept_Delegate Caller_Accept_Event;
        public void Caller_Accept_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            //The format is Caller_Accept|^|token|^|datetime_String|^|url
            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_Collection[1];

            string time_String = temp_Collection[2];
            DateTime datetime;
            DateTime.TryParse(time_String, out datetime);

            string url = temp_Collection[3];
            string company_Name = temp_Collection[4];
            string caller = "";
            if (clients.ContainsKey(token))
            {
                caller = clients[token].get_Username();
            }

            database_Connection.Update_Call_Result(url, caller, true, clients[token].get_Interview_Name(), datetime, company_Name);
            refresh_All_The_Callers_With_Data(company_Name);
            refresh_The_Admin_With_Candidates_Counts(company_Name); // Because now the filtered count also got modified(means reduced by one)
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Caller_Reject_Delegate(WebSocketSession session, string e);
        public event Caller_Reject_Delegate Caller_Reject_Event;
        public void Caller_Reject_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            //The format is Caller_Accept|^|token|^|datetime_String|^|url
            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_Collection[1];

            string time_String = temp_Collection[2];
            DateTime datetime;
            DateTime.TryParse(time_String, out datetime);

            string url = temp_Collection[3];
            string company_Name = temp_Collection[4];
            string caller = "";
            if (clients.ContainsKey(token))
            {
                caller = clients[token].get_Username();
            }

            database_Connection.Update_Call_Result(url, caller, false, clients[token].get_Interview_Name(), datetime, company_Name);
            refresh_All_The_Callers_With_Data(company_Name);
            refresh_The_Admin_With_Candidates_Counts(company_Name); // Because now the filtered count also got modified(means reduced by one)
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void SMS_Format_Delegate(WebSocketSession session, string e);
        public event SMS_Format_Delegate SMS_Format_Event;
        public void SMS_Format_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_Collection[1];
            string sms_Format = temp_Collection[2];
            string company_Name = temp_Collection[3];

            string caller = "";
            if (clients.ContainsKey(token))
            {
                caller = clients[token].get_Username();
            }

            database_Connection.Update_SMS_Format_For_This_Username(caller, sms_Format, clients[token].get_Interview_Name(), company_Name);
            session.SendResponse("Update_SMS_Format|^|" + sms_Format);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Subject_Format_Delegate(WebSocketSession session, string e);
        public event Subject_Format_Delegate Subject_Format_Event;
        public void Subject_Format_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_Collection[1];
            string subject_Format = temp_Collection[2];
            string company_Name = temp_Collection[3];

            string caller = "";
            if (clients.ContainsKey(token))
            {
                caller = clients[token].get_Username();
            }

            database_Connection.Update_Subject_Format_For_This_Username(caller, subject_Format, clients[token].get_Interview_Name(), company_Name);
            session.SendResponse("Update_Subject_Format|^|" + subject_Format);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Mail_Format_Delegate(WebSocketSession session, string e);
        public event Mail_Format_Delegate Mail_Format_Event;
        public void Mail_Format_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_Collection[1];
            string mail_Format = temp_Collection[2];
            string company_Name = temp_Collection[3];

            string caller = "";
            if (clients.ContainsKey(token))
            {
                caller = clients[token].get_Username();
            }

            database_Connection.Update_Mail_Format_For_This_Username(caller, mail_Format, clients[token].get_Interview_Name(), company_Name);
            session.SendResponse("Update_Mail_Format|^|" + mail_Format);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Attended_Status_Delegate(WebSocketSession session, string e);
        public event Attended_Status_Delegate Attended_Status_Event;
        public void Attended_Status_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            //Format is (Attended_Status|^|token|^|True|^|time_String|^|given_Url)
            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_Collection[1];

            string status_String = temp_Collection[2];
            string time_String = temp_Collection[3];
            DateTime datetime;
            DateTime.TryParse(time_String, out datetime);
            string url = temp_Collection[4];
            string company_Name = temp_Collection[5];
            bool status = false;
            if (string.Compare(status_String, "True", true) == 0)
                status = true;
            database_Connection.Update_Attended_Status_For_This_Url(url, clients[token].get_Interview_Name(), status, datetime, company_Name);
            refresh_The_Admin_With_Candidates_Counts(company_Name);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Selected_Status_Delegate(WebSocketSession session, string e);
        public event Selected_Status_Delegate Selected_Status_Event;
        public void Selected_Status_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            //Format is (Selected_Status|^|token|^|True|^|time_String|^|given_Url)
            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_Collection[1];

            string status_String = temp_Collection[2];
            string time_String = temp_Collection[3];
            DateTime datetime;
            DateTime.TryParse(time_String, out datetime);
            string url = temp_Collection[4];
            string company_Name = temp_Collection[5];
            bool status = false;
            if (string.Compare(status_String, "True", true) == 0)
                status = true;
            database_Connection.Update_Selected_Status_For_This_Url(url, clients[token].get_Interview_Name(), status, datetime, company_Name);
            refresh_The_Admin_With_Candidates_Counts(company_Name);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public delegate void Joined_Status_Delegate(WebSocketSession session, string e);
        public event Joined_Status_Delegate Joined_Status_Event;
        public void Joined_Status_Function(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            //Format is (Joined_Status|^|token|^|True|^|time_String|^|given_Url)
            string[] temp_Collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);

            string token = temp_Collection[1];

            string status_String = temp_Collection[2];
            string time_String = temp_Collection[3];
            DateTime datetime;
            DateTime.TryParse(time_String, out datetime);
            string url = temp_Collection[4];
            string company_Name = temp_Collection[5];

            bool status = false;
            if (string.Compare(status_String, "True", true) == 0)
                status = true;
            database_Connection.Update_Joined_Status_For_This_Url(url, clients[token].get_Interview_Name(), status, datetime, company_Name);
            refresh_The_Admin_With_Candidates_Counts(company_Name);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public void WebSocketServer_NewMessageReceived(WebSocketSession session, string e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            //MessageBox.Show(e);
            string type = e.Substring(0, e.IndexOf("|^|"));
            if (type == "Admin_Change_Password") //format is Admin_Change_Password|^|old_Password|^|new_Password
                Admin_Change_Password_Event(session, e);
            else if (type == "Admin_Get_Users_Details")
                Admin_Get_Users_Details_Event(session, e);
            else if (type == "Admin_Add_New_Account")
                Admin_Add_New_Account_Event(session, e);
            else if (type == "Admin_Update_Account")  //Format is Admin_Update_Account|^|username|^|password
                Admin_Update_Account_Event(session, e);
            else if (type == "Admin_Delete_Account") //format is Admin_Delete_Account|^|username
                Admin_Delete_Account_Event(session, e);
            else if (type == "Admin_Get_Filtered_Candidates_Details")
                Admin_Get_Filtered_Candidates_Details_Event(session, e);
            else if (type == "Admin_Get_Qualified_Candidates_Details")
                Admin_Get_Qualified_Candidates_Details_Event(session, e);
            else if (type == "Admin_Get_Attended_Candidates_Details")
                Admin_Get_Attended_Candidates_Details_Event(session, e);
            else if (type == "Admin_Get_Selected_Candidates_Details")
                Admin_Get_Selected_Candidates_Details_Event(session, e);
            else if (type == "Admin_Get_Joined_Candidates_Details")
                Admin_Get_Joined_Candidates_Details_Event(session, e);
            else if (type == "Admin_Update_Interview")
                Admin_Update_Interview_Event(session, e);
            else if (type == "Admin_Add_This_Url_In_This_Interview")
                Admin_Add_This_Url_In_This_Interview_Event(session, e);
            else if (type == "Admin_Get_History_For_This_Url")
                Admin_Get_History_For_This_Url_Event(session, e);
            else if (type == "Admin_Create_New_Dataset")
                Admin_Create_New_Dataset_Event(session, e);
            else if (type == "Admin_Add_Interview")
                Admin_Add_Interview_Event(session, e);
            else if (type == "Admin_Delete_Interview_Name")
                Admin_Delete_Interview_Name_Event(session, e);
            else if (type == "Admin_Start_Operation_On_Local_Database_File")
                Admin_Start_Operation_On_Local_Database_File_Event(session, e);
            else if (type == "Logging_In")
                Logging_In_Event(session, e);
            else if (type == "Get_Interview_Names")
                Get_Interview_Names_Event(session, e);
            else if (type == "Request_For_Logging_Out")
                Request_For_Logging_Out_Event(session, e);
            else if (type == "Feed")
                Feed_Event(session, e);
            else if (type == "Filter_Accept")
                Filter_Accept_Event(session, e);
            else if (type == "Filter_Reject")
                Filter_Reject_Event(session, e);
            //else if (type == "Url")     // format is Url|^|token|^|1/-1/0|^|address_Url
            //    Url_Event(session, e);
            else if (type == "Get_Urls_For_Filter") //format is "Get_Urls_For_Filter|^|token"
                Get_Urls_For_Filter_Event(session, e);
            else if (type == "Get_Contents_For_Caller") //format is "Get_Contents_For_Caller|^|token|^|network|^|no_Of_Lines"
                Get_Contents_For_Caller_Event(session, e);
            else if (type == "Get_Contents_For_Coordinator") //format is "Get_Contents_For_Coordinator|^|token"
                Get_Contents_For_Coordinator_Event(session, e);
            else if (type == "Get_Refresh") //The format is Get_Refresh|^|token
                Get_Refresh_Event(session, e);
            else if (type == "SMS") //The format is SMS|^|nos|^|message
                SMS_Event(session, e);
            else if (type == "Caller_Accept") //The format is Caller_Accept|^|token|^|datetime_String|^|url
                Caller_Accept_Event(session, e);
            else if (type == "Caller_Reject") //The format is Caller_Reject|^|token|^|datetime_String|^|url
                Caller_Reject_Event(session, e);
            else if (type == "SMS_Format") //format is SMS_Format|^|token|^|format
                SMS_Format_Event(session, e);
            else if (type == "Subject_Format") //Format is Subject_Format|^|token|^|format
                Subject_Format_Event(session, e);
            else if (type == "Mail_Format") //Format is Mail_Format|^|token|^|format
                Mail_Format_Event(session, e);
            else if (type == "Attended_Status") //Format is (Attended_Status|^|token|^|True|^|given_Url)
                Attended_Status_Event(session, e);
            else if (type == "Selected_Status") //Format is (Selected_Status|^|token|^|True|^|given_Url)
                Selected_Status_Event(session, e);
            else if (type == "Joined_Status") //Format is (Joined_Status|^|token|^|True|^|given_Url)
                Joined_Status_Event(session, e);
            //else
            //    MessageBox.Show("Unknown Message: " + e);

            //session.SendResponse(e);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public void WebSocketServer_NewDataReceived(WebSocketSession session, byte[] e)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            //DEBUG_File.Write(e + System.Environment.NewLine);
            //session.SendResponse(e);
#if DEBUG
                 Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        private void refresh_All_The_Callers_With_Data(string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string main_String = "Refresh|^|";

            System.Collections.Generic.Dictionary<string, int> temp_Dict;

            foreach (RV_Client temp_Client in clients.Values)
                if (temp_Client.get_Role() == Role.Call)
                {
                    temp_Dict = database_Connection.get_Filtered_Candidates_Counts_Details_As_Network_Count_Pairs_For_This_Caller(temp_Client.get_Username(), temp_Client.get_Interview_Name(), company_Name);

                    foreach (System.Collections.Generic.KeyValuePair<string, int> temp_Pair in temp_Dict)
                        main_String += temp_Pair.Key + ":" + temp_Pair.Value + ";";

                    if (temp_Dict.Count > 0)
                        main_String = main_String.Remove(main_String.Length - 1);
                    temp_Client.get_Session().SendResponse(main_String);
                }

            refresh_The_Admin_With_Candidates_Counts(company_Name);
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        private void refresh_The_Caller_With_Data(string token, string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string main_String = "Refresh|^|";

            System.Collections.Generic.Dictionary<string, int> temp_Dict;

            if (clients.ContainsKey(token))
            {
                if (clients[token].get_Role() == Role.Call)
                {
                    temp_Dict = database_Connection.get_Filtered_Candidates_Counts_Details_As_Network_Count_Pairs_For_This_Caller(clients[token].get_Username(), clients[token].get_Interview_Name(), company_Name);

                    foreach (System.Collections.Generic.KeyValuePair<string, int> temp_Pair in temp_Dict)
                        main_String += temp_Pair.Key + ":" + temp_Pair.Value + ";";

                    if (temp_Dict.Count > 0)
                        main_String = main_String.Remove(main_String.Length - 1);
                    clients[token].get_Session().SendResponse(main_String);
                }
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        private void refresh_The_Admin_With_Remaining_SMS_Count(string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string main_String = "Remaining_SMS_Count|^|" + database_Connection.get_Remaining_SMS_Count(company_Name).ToString();

            foreach (KeyValuePair<string, RV_Client> client in clients)
            {
                if (client.Value.get_Role() == Role.Admin && client.Value.get_Company_Name() == company_Name)
                    client.Value.get_Session().SendResponse(main_String);
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return;
        }

        private void refresh_The_Admin_With_Candidates_Counts(string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string main_String = "Refresh_Counts|^|";

            System.Collections.Generic.Dictionary<string, Interview_And_Candidates_Count_Detail> temp_Dict = new Dictionary<string, Interview_And_Candidates_Count_Detail>();

            temp_Dict = database_Connection.get_Candidates_Counts_Details_As_Dict_With_InterviewName_As_Key(company_Name);

            System.Collections.ObjectModel.Collection<string> temp_Collection = new System.Collections.ObjectModel.Collection<string>();
            foreach (System.Collections.Generic.KeyValuePair<string, Interview_And_Candidates_Count_Detail> temp_Pair in temp_Dict)
                temp_Collection.Add(temp_Pair.Value.get_As_String());

            main_String += string.Join(";", temp_Collection);

            foreach (KeyValuePair<string, RV_Client> client in clients)
            {
                if (client.Value.get_Role() == Role.Admin && client.Value.get_Company_Name() == company_Name)
                    client.Value.get_Session().SendResponse(main_String);
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public string get_Users_Details_As_String(string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            System.Collections.Generic.Dictionary<string, User_Detail> temp_Dict = database_Connection.get_Users_Details(company_Name);

            List<string> temp = new List<string>();
            foreach (System.Collections.Generic.KeyValuePair<string, User_Detail> temp_Pair in temp_Dict)
            {
                temp.Add(temp_Pair.Key + ":" + temp_Pair.Value.get_As_String());
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return string.Join(";", temp);
        }

        public string get_Qualified_Candidates_Details_As_String(string interview_Name, string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            Dictionary<string, Dictionary<string, DBConnect.Candidate_Detail>> temp_Dict = database_Connection.get_Qualified_Candidates_Details(interview_Name, company_Name);
            var enumerator = temp_Dict.Keys.GetEnumerator();

            List<string> temp_1 = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, DBConnect.Candidate_Detail>> temp_Pair in temp_Dict)
            {
                List<string> temp_0 = new List<string>();
                foreach (KeyValuePair<string, DBConnect.Candidate_Detail> sub_Pair in temp_Pair.Value)
                {
                    temp_0.Add(sub_Pair.Value.get_Details_As_String_With_This_Separator("~"));
                }
                temp_1.Add(temp_Pair.Key + ":" + string.Join("`", temp_0));
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return string.Join(";", temp_1);
        }

        public string get_Attended_Candidates_Details_As_String(string interview_Name, string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            Dictionary<string, Dictionary<string, DBConnect.Candidate_Detail>> temp_Dict = database_Connection.get_Attended_Candidates_Details(interview_Name, company_Name);
            var enumerator = temp_Dict.Keys.GetEnumerator();

            List<string> temp_1 = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, DBConnect.Candidate_Detail>> temp_Pair in temp_Dict)
            {
                List<string> temp_0 = new List<string>();
                foreach (KeyValuePair<string, DBConnect.Candidate_Detail> sub_Pair in temp_Pair.Value)
                {
                    temp_0.Add(sub_Pair.Value.get_Details_As_String_With_This_Separator("~"));
                }
                temp_1.Add(temp_Pair.Key + ":" + string.Join("`", temp_0));
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return string.Join(";", temp_1);
        }

        public string get_Selected_Candidates_Details_As_String(string interview_Name, string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            Dictionary<string, Dictionary<string, DBConnect.Candidate_Detail>> temp_Dict = database_Connection.get_Selected_Candidates_Details(interview_Name, company_Name);
            var enumerator = temp_Dict.Keys.GetEnumerator();

            List<string> temp_1 = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, DBConnect.Candidate_Detail>> temp_Pair in temp_Dict)
            {
                List<string> temp_0 = new List<string>();
                foreach (KeyValuePair<string, DBConnect.Candidate_Detail> sub_Pair in temp_Pair.Value)
                {
                    temp_0.Add(sub_Pair.Value.get_Details_As_String_With_This_Separator("~"));
                }
                temp_1.Add(temp_Pair.Key + ":" + string.Join("`", temp_0));
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return string.Join(";", temp_1);
        }

        public string get_Joined_Candidates_Details_As_String(string interview_Name, string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            Dictionary<string, Dictionary<string, DBConnect.Candidate_Detail>> temp_Dict = database_Connection.get_Joined_Candidates_Details(interview_Name, company_Name);
            var enumerator = temp_Dict.Keys.GetEnumerator();

            List<string> temp_1 = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, DBConnect.Candidate_Detail>> temp_Pair in temp_Dict)
            {
                List<string> temp_0 = new List<string>();
                foreach (KeyValuePair<string, DBConnect.Candidate_Detail> sub_Pair in temp_Pair.Value)
                    temp_0.Add(sub_Pair.Value.get_Details_As_String_With_This_Separator("~"));

                temp_1.Add(temp_Pair.Key + ":" + string.Join("`", temp_0));
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return string.Join(";", temp_1);
        }

        public string get_Filtered_Candidates_Details_As_String(string interview_Name, string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
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
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return string.Join(";", temp_1);
        }

        public void extract_User_Information(string content, ref string username,ref string role, ref string interview_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            char[] delimeter = ",".ToCharArray();
            string[] datas = content.Split(delimeter);

            foreach (string temp_String in datas)
            {
                string type = temp_String.Substring(0, temp_String.IndexOf(":"));
                string value = temp_String.Substring(temp_String.IndexOf(":") + 1);

                switch (type)
                {
                    case "username":
                        username = value;
                        break;
                    case "role":
                        role = value;
                        break;
                    case "interview_Name":
                        interview_Name = value;
                        break;
                    default:
#if Console
                        Console.WriteLine("Wrong type");
#endif

                        //MessageBox.Show("Wrong type");
                        break;
                }
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public Presence_Status get_Presence_Status_Of_This_Account(string username, Role role, string interview_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            Presence_Status status = Presence_Status.Absent;

            foreach (RV_Client client in clients.Values)
            {
                if (client.get_Username() == username && client.get_Interview_Name() == interview_Name)
                {
                    status = Presence_Status.Partial_Presence;
                    if (client.get_Role() == role)
                    {
                        status = Presence_Status.Exact_Presence;
                        break;
                    }
                }
            }
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return status;
        }

        public string add_Client(string username, Role role, WebSocketSession session, string interview_Name, string company_Name)
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            string temp_Id = Useful_Functions.Useful_Functions.get_Guid_String();
            clients.Add(temp_Id, new RV_Client(username, role, session, interview_Name, company_Name));
            clients[temp_Id].Login_Time = System.DateTime.Now;
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            return temp_Id;

        }

        public void Initialize_Tables_For_First_Time()
        {
#if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            database_Connection.Create_UserData_Table();
            database_Connection.Create_Interview_Names_Table();
            //database_Connection.Add_User_Info("root", "password", "0000000000", "Management", "");
            //database_Connection.Add_User_Info("Admin", "admin", "0000000000", "Management", "");
            database_Connection.Create_Interview_Names_Table();
#if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

        }
    }
}
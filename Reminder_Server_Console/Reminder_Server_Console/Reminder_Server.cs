#undef DEBUG

using SuperWebSocket;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Mail;

using MySql_CSharp_Console;
using Common_Classes;

namespace Reminder_Server_Console
{
    class Reminder_Server : WebSocketServer
    {
        Dictionary<string, Dictionary<string, WebSocketSession>> connected_Users;   // <company_Name<username,session>>
        System.Timers.Timer timer;
        DBConnect db_Connect;
        Useful_Functions.SimpleAES crypting_Object;
        WebSocketSession resume_View_Server_Session;

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

        public Reminder_Server()
        {
            crypting_Object = new Useful_Functions.SimpleAES();
            load_Values_From_Registry();
            load_The_Websocket_Details();
            connected_Users = new Dictionary<string, Dictionary<string, WebSocketSession>>();
            db_Connect = new DBConnect(database_Server, database_Name, database_Uid, database_Password);
            crypting_Object = new Useful_Functions.SimpleAES();
            timer = new System.Timers.Timer(60000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Start();
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

        public void start()
        {
            assign_The_Function_For_Events();
            base.Start();
        }

        void load_The_Websocket_Details()
        {
            this.Setup(new RootConfig(), new ServerConfig
            {
                Port = Int32.Parse(reminder_Server_Port),
                Ip = "Any",
                MaxConnectionNumber = Int32.Parse(reminder_Server_Max_Connection),
                MaxCommandLength = Int32.Parse(reminder_Server_Max_Command_Length),
                Mode = SuperSocket.SocketBase.SocketMode.Async,
                Name = "Reminder Server"
            }, SocketServerFactory.Instance);

        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timer.Stop();
            DateTime current_Time = DateTime.Now.ToUniversalTime();

            List<Job> reminders =  db_Connect.get_All_The_Reminders();

            foreach (Job temp_Job in reminders)
            {
                // The below check for the status change and refreshes the creator's client window, if it finds one
                if (temp_Job.get_Status(current_Time) != temp_Job.get_Status(current_Time.AddMinutes(-1)))
                    if (connected_Users.ContainsKey(temp_Job.company_Name))
                        foreach (string user in temp_Job.job_Doiers_Names)
                            if (connected_Users[temp_Job.company_Name].ContainsKey(user))
                                if (connected_Users[temp_Job.company_Name][user].Status == SuperSocket.SocketBase.SessionStatus.Healthy)
                                    connected_Users[temp_Job.company_Name][user].SendResponse("Refresh");

                string content = temp_Job.subject +"|^|"+
                                 temp_Job.body +"|^|"+
                                 temp_Job.end_Time.ToString("yyyy-MM-dd hh:mm:ss")+"|^|"+
                                 temp_Job.get_Status(current_Time).ToString();

                // The below check for the time elapsed and send the warning to corresponding client windows, if they logged in
                // also sending SMSs,Mails
                int factor = (temp_Job.time_Gap_Type == Job.Time_Gap_Type.Hour ? 60 : 1);
                TimeSpan time_Gap = TimeSpan.FromMinutes(temp_Job.time_Gap_Of_Minutes_Or_Hours*factor);
                TimeSpan elapsed_From_The_Last_Reminded = current_Time - temp_Job.last_Reminded_Time;
                if (temp_Job.last_Reminded_Time == new DateTime(1, 1, 1) ||
                    (elapsed_From_The_Last_Reminded > time_Gap))
                {
                    db_Connect.update_Last_Reminded_Time_Of_This_Reminder(current_Time, temp_Job.id);
                    if (connected_Users.ContainsKey(temp_Job.company_Name))
                        foreach (string user in temp_Job.job_Doiers_Names)
                            if (connected_Users[temp_Job.company_Name].ContainsKey(user))
                                if (connected_Users[temp_Job.company_Name][user].Status == SuperSocket.SocketBase.SessionStatus.Healthy)
                                    connected_Users[temp_Job.company_Name][user].SendResponse("Show|^|" + content);

                    string mail_Id = db_Connect.get_Mail_Id_For_This_Username(temp_Job.creator_Name,temp_Job.company_Name);
                    string password = db_Connect.get_Encrypted_Mail_Password_For_This_Username(temp_Job.creator_Name,temp_Job.company_Name);;
                    password = crypting_Object.DecryptString(password);
                    foreach (string job_Doier_Mail_Id in temp_Job.job_Doiers_Mail_Ids)
                        send_Mail(mail_Id, job_Doier_Mail_Id, temp_Job.subject, temp_Job.body, password);

                    if (db_Connect.get_Remaining_SMS_Count(temp_Job.company_Name) == 0)
                    {
                        if (connected_Users.ContainsKey(temp_Job.company_Name))
                            if (connected_Users[temp_Job.company_Name].ContainsKey(temp_Job.creator_Name))
                            {
                                connected_Users[temp_Job.company_Name][temp_Job.creator_Name].SendResponse("Message|^|Sorry , the available SMS count in your account reached zero");
                                return;
                            }
                    }
                    else
                    {
                        if ((current_Time.Add(temp_Job.creators_Time_Zone.BaseUtcOffset).Hour > 6) &&
                            (current_Time.Add(temp_Job.creators_Time_Zone.BaseUtcOffset).Hour < 20))
                        {
                            send_SMS(temp_Job.job_Doiers_Mobile_Nos, temp_Job.company_Name);
                        }
                    }
                    
                }
            }
            timer.Start();
        }

        #region Mail Sending
        static void send_Mail(string from_Mail_Address,string to_Mail_Address,string subject,string body,string password)
        {
            MailAddress from_EMail_Address = new MailAddress(from_Mail_Address);
            MailAddress to_EMail_Address = new MailAddress(to_Mail_Address);
            MailMessage mail_Message = new MailMessage(from_EMail_Address, to_EMail_Address);
            mail_Message.Sender = from_EMail_Address;
            mail_Message.Subject = subject;
            mail_Message.SubjectEncoding = System.Text.Encoding.UTF8;
            mail_Message.Body = body;
            mail_Message.BodyEncoding = System.Text.Encoding.UTF8;

            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.SendCompleted += new SendCompletedEventHandler(client_SendCompleted);

            //System.Net.ServicePointManager.ServerCertificateValidationCallback = (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) => { return true; };
            System.Net.NetworkCredential temp_Credential = new System.Net.NetworkCredential(from_Mail_Address, password);
            client.UseDefaultCredentials = false;
            client.Credentials = temp_Credential;
            client.EnableSsl = true;

            /*
             * Port 25 is the normal unencrypted pop port; not available on gmail.
             * The other two ports have encryption; 587 uses TLS, 465 uses SSL.
             * To use 587 you should set SmtpClient.EnableSsl = true.
             * 465 wont work with SmtpClient, it will work with the deprecated class SmtpMail instead.
             */

            client.Port = 587;
            client.Timeout = 10000;
            string status = "";
            try
            {
                client.SendAsync(mail_Message, status);
            }
            catch (SmtpException excp)
            {
                Console.WriteLine(excp.Message);
            }
        }

        static void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //Console.WriteLine("Mail Asynchronously Sent");
            //temp_Event.Set();
        }

        /*
         * private static void SendEmailMessageGmail(System.Net.Mail.MailMessage message)
        {
            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential("prabhakaran.solutions@gmail.com", "password_567");
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.UseDefaultCredentials = false;
            smtp.EnableSsl = true;
            smtp.Credentials = cred;
            smtp.Port = 587;
            smtp.Send(message);
        }*/
        #endregion

        #region SMS sending
        void send_SMS(List<string> mobile_Nos,string company_Name)
        {
            //Send SMS and update SMS count for this company
        }
        #endregion

        void assign_The_Function_For_Events()
        {
            this.NewSessionConnected += new SessionEventHandler<WebSocketSession>(this.WebSocketServer_NewSessionConnected);
            this.NewDataReceived += new SessionEventHandler<WebSocketSession, byte[]>(this.WebSocketServer_NewDataReceived);
            this.NewMessageReceived += new SessionEventHandler<WebSocketSession, string>(this.WebSocketServer_NewMessageReceived);
            this.SessionClosed += new SessionEventHandler<WebSocketSession, SuperSocket.SocketBase.CloseReason>(this.WebSocketServer_SessionClosed);
        }

        public void WebSocketServer_NewMessageReceived(WebSocketSession session, string e)
        {
#if My_DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            //Console.WriteLine(e);
            string[] collection = e.Split(new string[] { "|^|" }, StringSplitOptions.None);
            string type = collection[0];
            if (type == "Get_Token")
            {
                string username = collection[1];
                string company_Name = collection[2];
                if (!connected_Users.ContainsKey(company_Name))
                {
                    Dictionary<string,WebSocketSession> temp_Dict = new Dictionary<string,WebSocketSession>();
                    connected_Users.Add(company_Name, temp_Dict);
                }
                if (!connected_Users[company_Name].ContainsKey(username))
                    connected_Users[company_Name].Add(username, session);
                if (resume_View_Server_Session != null)
                    if(resume_View_Server_Session.Status == SessionStatus.Healthy)
                        resume_View_Server_Session.SendResponse("New_Base_Connection|^|" + session.RemoteEndPoint.Address.ToString());
            }
            else if (type == "Resume_View_Server")
            {
                resume_View_Server_Session = session;
            }

            //else
            //    MessageBox.Show("Unknown Message: " + e);

            //session.SendResponse(e);
#if My_DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public void WebSocketServer_NewDataReceived(WebSocketSession session, byte[] e)
        {
#if My_DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            //DEBUG_File.Write(e + System.Environment.NewLine);
            //session.SendResponse(e);
#if My_DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public void WebSocketServer_NewSessionConnected(WebSocketSession session)
        {
#if My_DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            //DEBUG_File.Write("connected : " + session.Host + System.Environment.NewLine);
            //MessageBox.Show("connected : " + session.Host + System.Environment.NewLine);
            //session.SendResponse("Message|^|Welcome client");
#if My_DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }

        public void WebSocketServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason close_Reason)
        {
#if My_DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
            string ip_Address = session.RemoteEndPoint.Address.ToString();
            //MessageBox.Show("Host "+ host + " requested a disconnect");
            System.Collections.ObjectModel.Collection<string> keys = new System.Collections.ObjectModel.Collection<string>();

            List<string> company_Names = connected_Users.Keys.ToList();
            bool found = false;
            string required_Company_Name = "";
            string required_User_Name = "";
            foreach (string company_Name in connected_Users.Keys)
                foreach (string user_Name in connected_Users[company_Name].Keys)
                    if (connected_Users[company_Name][user_Name].RemoteEndPoint.Address.ToString() == session.RemoteEndPoint.Address.ToString())
                    {
                        found = true;
                        required_Company_Name = company_Name;
                        required_User_Name = user_Name;
                        break;
                    }

            if (found)
            {
                connected_Users[required_Company_Name][required_User_Name].Close();
                connected_Users[required_Company_Name].Remove(required_User_Name);
            }
            if (resume_View_Server_Session != null)
                if(resume_View_Server_Session.Status == SessionStatus.Healthy)
                    resume_View_Server_Session.SendResponse("Base_Connection_Disconnected|^|"+ip_Address);
#if My_DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif
        }
    }
}

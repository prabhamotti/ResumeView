using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql_CSharp_Console;

namespace resumeView_Precursor
{
    class Program
    {
        static string database_Server = "127.0.0.1";
        static string database_Name = "resumeview_DB";
        static string database_Uid = "imikoRV";
        static string database_Password = "alpha123";
        static string ftp_Server = "127.0.0.1";
        static string ftp_Username = "root";
        static string ftp_Password = "password";
        static string doc_To_Html_Server_Address = "127.0.0.1";
        static string doc_To_Html_Server_Port = "2012";
        static string doc_To_Html_Server_Max_Connection = "10";
        static string doc_To_Html_Server_Max_Command_Length = "10000";
        static string resume_View_Server_Address = "127.0.0.1";
        static string resume_View_Server_Port = "2013";
        static string resume_View_Server_Max_Connection = "1000";
        static string resume_View_Server_Max_Command_Length = "100000";
        static string reminder_Server_Address = "127.0.0.1";
        static string reminder_Server_Port = "2014";
        static string reminder_Server_Max_Connection = "1000";
        static string reminder_Server_Max_Command_Length = "10000";
        static Useful_Functions.SimpleAES crypting_Object = new Useful_Functions.SimpleAES();
        public static bool Is_MySQL_Service_Exist()
        {
            foreach (System.ServiceProcess.ServiceController temp_Service in System.ServiceProcess.ServiceController.GetServices())
                if (temp_Service.ServiceName == "MySQL")
                    return true;

            return false;
        }

        public static bool is_MySQL_Running()
        {
            System.ServiceProcess.ServiceController mysql_Service = new System.ServiceProcess.ServiceController("MySQL");
            if (mysql_Service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                return true;
            return false;
        }

        public static void Start_MySQL_Service()
        {
            System.ServiceProcess.ServiceController mysql_Service = new System.ServiceProcess.ServiceController("MySQL");
            if (mysql_Service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                mysql_Service.Start();
        }

        private static void load_Settings_To_Registry()
        {
            Microsoft.Win32.RegistryKey temp = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey temp_0 = temp.OpenSubKey("SOFTWARE", true);
            if (!temp_0.GetSubKeyNames().Contains("Resume View"))
                temp_0.CreateSubKey("Resume View");

            Microsoft.Win32.RegistryKey resume_View_Key = temp_0.OpenSubKey("Resume View", true);

            string encrypted_String = "";
            encrypted_String = crypting_Object.EncryptToString(database_Server);
            resume_View_Key.SetValue("database_Server", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(database_Name);
            resume_View_Key.SetValue("database_Name", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(database_Uid);
            resume_View_Key.SetValue("database_Uid", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(database_Password);
            resume_View_Key.SetValue("database_Password", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(ftp_Server);
            resume_View_Key.SetValue("ftp_Server", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(ftp_Username);
            resume_View_Key.SetValue("ftp_Username", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(ftp_Password);
            resume_View_Key.SetValue("ftp_Password", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(doc_To_Html_Server_Address);

            resume_View_Key.SetValue("doc_To_Html_Server_Address", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(doc_To_Html_Server_Port);
            resume_View_Key.SetValue("doc_To_Html_Server_Port", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(doc_To_Html_Server_Max_Connection);
            resume_View_Key.SetValue("doc_To_Html_Server_Max_Connection", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(doc_To_Html_Server_Max_Command_Length);
            resume_View_Key.SetValue("doc_To_Html_Server_Max_Command_Length", encrypted_String);

            encrypted_String = crypting_Object.EncryptToString(resume_View_Server_Address);
            resume_View_Key.SetValue("resume_View_Server_Address", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(resume_View_Server_Port);
            resume_View_Key.SetValue("resume_View_Server_Port", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(resume_View_Server_Max_Connection);
            resume_View_Key.SetValue("resume_View_Server_Max_Connection", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(resume_View_Server_Max_Command_Length);
            resume_View_Key.SetValue("resume_View_Server_Max_Command_Length", encrypted_String);


            encrypted_String = crypting_Object.EncryptToString(reminder_Server_Address);
            resume_View_Key.SetValue("reminder_Server_Address", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(reminder_Server_Port);
            resume_View_Key.SetValue("reminder_Server_Port", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(reminder_Server_Max_Connection);
            resume_View_Key.SetValue("reminder_Server_Max_Connection", encrypted_String);
            encrypted_String = crypting_Object.EncryptToString(reminder_Server_Max_Command_Length);
            resume_View_Key.SetValue("reminder_Server_Max_Command_Length", encrypted_String);
        }

        static void Main(string[] args)
        {
            /*if (Is_MySQL_Service_Exist())
            {
                Console.WriteLine("Mysql Service has been found");
                if (!is_MySQL_Running())
                {
                    Console.WriteLine("Mysql Service is not running, attempting to start it");
                    while (!is_MySQL_Running())
                        Start_MySQL_Service();
                }
                if (is_MySQL_Running())
                    Console.WriteLine("Mysql Service is running");
            }
            */

            Console.WriteLine("Registering the values of Resume View");
            load_Settings_To_Registry();
            Console.WriteLine("Succesfully registered the values");
            Console.WriteLine("Making tables in database");
            DBConnect database_Connection = new DBConnect(database_Server, database_Name, database_Uid, database_Password);

            if (!database_Connection.Check_For_Table("interview_Names"))
                database_Connection.Create_Interview_Names_Table();

            if (!database_Connection.Check_For_Table("main_Data"))
                database_Connection.create_Main_Data_Table();

            if (!database_Connection.Check_For_Table("userinfo"))
                database_Connection.Create_UserData_Table();

            if (!database_Connection.Check_For_Table("company_Info"))
                database_Connection.create_Company_Info_Table();

            if (!database_Connection.Check_For_Table("html_Structure"))
                database_Connection.create_Html_Structure_Table();

            if (!database_Connection.Check_For_Table("reminders"))
                database_Connection.create_Reminder_Table();

            if (!database_Connection.Check_For_Table("formats"))
                database_Connection.create_Formats_Table();

            if(!database_Connection.Check_User_Account("Admin",crypting_Object.EncryptToString("admin"),"imiko"))
                database_Connection.Add_User_Info("Admin", crypting_Object.EncryptToString("admin"),  "9442510190", "Management", "imiko", "http://127.0.0.1/MyWeb/Photos/Admin_imiko.jpg","");


            Console.WriteLine("Tables got created Successfully");
            string a = Console.ReadLine();
        }
    }
}

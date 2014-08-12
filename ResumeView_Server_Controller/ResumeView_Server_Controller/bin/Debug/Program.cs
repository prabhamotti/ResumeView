using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace resumeView_Precursor
{
    class Program
    {
        static public Useful_Functions.SimpleAES crypting_Object;

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

        static string company_Name = "h";

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

            encrypted_String = crypting_Object.EncryptToString(company_Name);
            resume_View_Key.SetValue("company_Name", encrypted_String);
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
            crypting_Object = new Useful_Functions.SimpleAES();
            load_Settings_To_Registry();
            Console.WriteLine("Succesfully registered the values");
            string a = Console.ReadLine();
        }
    }
}

// main_Data (name,phoneNo,mailId,url,interview_Name,feeder,feed_Date,filterer,filter_Result,filter_Date,caller,call_Result,call_Date,coordinator,attended_Status,attended_Status_Date,selected_Status,selected_Status_Date,joined_Status,joined_Status_Date)
// This will give the size of the database SELECT table_schema,sum( data_length + index_length ) / 1024 /1024 "Data Base Size in MB",sum( data_free )/ 1024 / 1024 "Free Space in MB" FROM information_schema.TABLES GROUP BY table_schema ; 

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Common_Classes;
using Useful_Functions;

namespace MySql_CSharp_Console
{
    class DBConnect
    {
        private string database_Server;
        private string database_Name;
        private string database_Uid;
        private string database_Password;

        string connection_String;

        public DBConnect(string given_Database_Server,string given_Database_Name,string given_Database_Uid,string given_Database_Password)
        {
            database_Server = given_Database_Server;
            database_Name = given_Database_Name;
            database_Uid = given_Database_Uid;
            database_Password = given_Database_Password;

            connection_String = "SERVER=" + database_Server + ";" + "DATABASE=" + database_Name + ";" +
                                        "UID=" + database_Uid + ";" + "PASSWORD=" + database_Password + ";";
        }

        private bool Open_This_Mysql_Connection(ref MySqlConnection connection)
        {
            //Below code was taken from http://stackoverflow.com/questions/280413/c-sharp-how-do-you-find-the-caller-function
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
            //System.Diagnostics.StackFrame sf = st.GetFrame(0);
            //string msg = sf.GetMethod().DeclaringType.FullName + "." +
            //sf.GetMethod().Name;
            //MessageBox.Show("openconnection: " + msg);
            try
            {
                connection.Open();
                return true;
            }

            catch (MySqlException exception)
            {
                #if WindowsApp
                MessageBox.Show("Cannot connect to the Mysql server");
                MessageBox.Show(exception.Message);
                #elif Console
                Console.WriteLine("Cannot connect to the Mysql server");
                Console.WriteLine(exception.Message);
                #elif WindowsService
                #endif

                switch (exception.Number)
                {
                    case 0:
                        #if WindowsApp
                        MessageBox.Show("Cannot connect to the server.Contact Administrator");
                        #elif Console
                        Console.WriteLine("Cannot connect to the server.Contact Administrator");
                        #elif WindowsService
                        #endif
                        break;
                    case 1045:
                        #if WindowsApp
                        MessageBox.Show("Invalid Username/Password");
                        #elif Console
                        Console.WriteLine("Invalid Username/Password");
                        #elif WindowsService
                        #endif
                        break;
                }
                return false;
            }
        }

        private bool Close_This_Mysql_Connection(ref MySqlConnection connection)
        {
            //Below code was taken from http://stackoverflow.com/questions/280413/c-sharp-how-do-you-find-the-caller-function
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
            //System.Diagnostics.StackFrame sf = st.GetFrame(0);
            //string msg = sf.GetMethod().DeclaringType.FullName + "." +
            //sf.GetMethod().Name;
            //MessageBox.Show("close connection: " + msg);
            try
            {
                connection.Close();
                return true;
            }

            catch (MySqlException exception)
            {
                //MessageBox.Show(exception.Message);
                return false;
            }
        }

        public bool check_For_Mysql_Connection()
        {
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                Close_This_Mysql_Connection(ref connection);
                return true;
            }
            return false;
        }
        
        /*public bool Open_Connection()
        {
            //Below code was taken from http://stackoverflow.com/questions/280413/c-sharp-how-do-you-find-the-caller-function
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
            //System.Diagnostics.StackFrame sf = st.GetFrame(0);
            //string msg = sf.GetMethod().DeclaringType.FullName + "." +
            //sf.GetMethod().Name;
            //Console.WriteLine("openconnection: " + msg);
            try
            {
                connection.Open();
                return true;
            }

            catch (MySqlException exception)
            {
                Console.WriteLine("Cannot connect to the Mysql server");
                switch (exception.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to the server.Contact Administrator");
                        break;
                    case 1045:
                        Console.WriteLine("Invalid Username/Password");
                        break;
                }
                return false;
            }
        }

        private bool Open_Network_Table_Connection()
        {
            //Below code was taken from http://stackoverflow.com/questions/280413/c-sharp-how-do-you-find-the-caller-function
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
            //System.Diagnostics.StackFrame sf = st.GetFrame(0);
            //string msg = sf.GetMethod().DeclaringType.FullName + "." +
            //sf.GetMethod().Name;
            //Console.WriteLine("open network connection: "+msg);

            try
            {
                network_Table_Connection.Open();
                return true;
            }

            catch (MySqlException exception)
            {
                Console.WriteLine("Cannot connect to the Mysql server");
                switch (exception.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to the server.Contact Administrator");
                        break;
                    case 1045:
                        Console.WriteLine("Invalid Username/Password");
                        break;
                }
                return false;
            }
        }

        public bool Close_Connection()
        {
            //Below code was taken from http://stackoverflow.com/questions/280413/c-sharp-how-do-you-find-the-caller-function
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
            //System.Diagnostics.StackFrame sf = st.GetFrame(0);
            //string msg = sf.GetMethod().DeclaringType.FullName + "." +
            //sf.GetMethod().Name;
            //Console.WriteLine("close connection: " + msg);
            try
            {
                connection.Close();
                return true;
            }

            catch (MySqlException exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }
        }

        private bool Close_Network_Table_Connection()
        {
            //Below code was taken from http://stackoverflow.com/questions/280413/c-sharp-how-do-you-find-the-caller-function
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
            //System.Diagnostics.StackFrame sf = st.GetFrame(0);
            //string msg = sf.GetMethod().DeclaringType.FullName + "." +
            //sf.GetMethod().Name;
            //Console.WriteLine("close network connection: "+msg);
            try
            {
                network_Table_Connection.Close();
                return true;
            }

            catch (MySqlException exception)
            {
                Console.WriteLine(exception.Message);
                return false;
            }
        }*/

        //public void Insert()
        //{
        //    string query = "INSERT INTO tableInfo (name,age) VALUES('John Smith','33');";

        //    if (Open_This_Mysql_Connection(ref connection) == 1)
        //    {
        //        MySqlCommand cmd = new MySqlCommand(query, connection);
        //        cmd.ExecuteNonQuery();

        //        Close_This_Mysql_Connection(ref connection);
        //    }
        //}

        //public void Update()
        //{
        //    string query = "UPDATE tableInfo SET name ='Joe',age='22' WHERE name='John Smith'";

        //    if (Open_This_Mysql_Connection(ref connection) == 1)
        //    {
        //        MySqlCommand cmd = new MySqlCommand(query, connection);
        //        cmd.ExecuteNonQuery();
        //        Close_This_Mysql_Connection(ref connection);
        //    }
        //}

        //public void Delete()
        //{
        //    string query = "Delete FROM tableInfo WHERE name='John Smith'";

        //    if (Open_This_Mysql_Connection(ref connection) == 1)
        //    {
        //        MySqlCommand cmd = new MySqlCommand(query, connection);
        //        cmd.ExecuteNonQuery();
        //        Close_This_Mysql_Connection(ref connection);
        //    }
        //}

        //public List<string>[] select()
        //{
        //    string query = "SELECT * FROM tableInfo";
        //    List<string>[] list = new List<string>[3];
        //    list[0] = new List<string>();
        //    list[1] = new List<string>();
        //    list[2] = new List<string>();

        //    if (Open_This_Mysql_Connection(ref connection) == 1)
        //    {
        //        MySqlCommand cmd = new MySqlCommand(query, connection);
        //        MySqlDataReader data_Reader = cmd.ExecuteReader();

        //        while (data_Reader.Read())
        //        {
        //            list[0].Add(Convert.ToString(data_Reader["id"]));
        //            list[1].Add(Convert.ToString(data_Reader["name"]));
        //            list[2].Add(Convert.ToString(data_Reader["age"]));
        //        }

        //        data_Reader.Close();
        //        Close_This_Mysql_Connection(ref connection);

        //        return list;
        //    }

        //    else
        //    {
        //        return list;
        //    }
        //}

        //public int Count()
        //{
        //    string query = "SELECT COUNT(*) FROM tableInfo";

        //    int count = -1;

        //    if (Open_This_Mysql_Connection(ref connection) == 1)
        //    {
        //        MySqlCommand cmd = new MySqlCommand(query,connection);
        //        count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));

        //        Close_This_Mysql_Connection(ref connection);
        //    }

        //    return count;
        //}

        public string get_Database_Details_As_String()
        {
            string query  = "SELECT table_schema,sum( data_length + index_length ) / 1024 /1024 'Data Base Size in MB',sum( data_free )/ 1024 / 1024 'Free Space in MB' FROM information_schema.TABLES WHERE table_schema="+Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(database_Name) ;

            string result_String = "";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["Data Base Size in MB"] != DBNull.Value)
                        result_String += reader["Data Base Size in MB"].ToString();
                    result_String += ",";
                    if (reader["Free Space in MB"] != DBNull.Value)
                        result_String += reader["Free Space in MB"].ToString();
                }

                Close_This_Mysql_Connection(ref connection);
            }

            return result_String;
        }

        public List<string> get_Company_Names()
        {
            string query = "SELECT company_Name FROM company_Info";
            List<string> result = new List<string>();
            MySqlConnection connection = new MySqlConnection(connection_String);
            if(Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    if (reader["company_Name"] != DBNull.Value)
                        result.Add((string)(reader["company_Name"]));

                Close_This_Mysql_Connection(ref connection);
            }

            return result;
        }

        public bool is_This_Company_Name_Exist(string company_Name)
        {
            string query = "SELECT COUNT(*) FROM company_Info WHERE company_Name='"+company_Name+"'";

            bool result=false;

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                int count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));
                result =(count ==0?false:true);

                Close_This_Mysql_Connection(ref connection);
            }

            return result;
        }

        public bool Check_For_Table(string table_Name)
        {
            string query = "SHOW TABLES";

            bool result = false;
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["Tables_in_" + database_Name] != DBNull.Value )
                    {
                        string temp_Table_Name = (string)(reader["Tables_in_" + database_Name]);
                        if(string.Compare(temp_Table_Name,table_Name,true) == 0)
                        {
                            result = true;
                            break;
                        }
                    }
                }
                Close_This_Mysql_Connection(ref connection);
            }

            return result;
        }

        public void create_Company_Info_Table()
        {
            string query = "CREATE TABLE company_Info (company_Name VARCHAR(50),webPage_Base_Address VARCHAR(50),ftp_Home_Folder_Path VARCHAR(100),expiry_Date DATETIME,message VARCHAR(100),message_Image_Path VARCHAR(200),sms_Count MEDIUMINT)";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void add_Company_Info(string company_Name,string webPage_Base_Address,
            DateTime expiry_Date,string message,string message_Image_Path,double sms_Count,string ftp_Home_Folder_Path)
        {
            string query = "INSERT INTO company_Info (company_Name,webPage_Base_Address,"+
                            "expiry_Date,message,message_Image_Path,sms_Count,"+
                            "ftp_Home_Folder_Path)"+
                            "VALUES("+
                                "'"+company_Name +"'"+","+
                                "'" + webPage_Base_Address + "'" + "," +
                                "'" + expiry_Date.ToString("yyyy-MM-dd hh:mm:ss") + "'" + "," +
                                "'" + message + "'" + "," +
                                "'" + message_Image_Path + "'" + "," +
                                sms_Count.ToString() + "," +
                                "'" + ftp_Home_Folder_Path + "'" +
                          ")";

            MySqlConnection connection = new MySqlConnection(connection_String);

            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void update_Company_Info(string company_Name, string webPage_Base_Address,
            DateTime expiry_Date, string message, string message_Image_Path, double sms_Count, string ftp_Home_Folder_Path)
        {
            string query = "UPDATE company_Info SET expiry_Date='" + expiry_Date.ToString("yyyy-MM-dd hh:mm:ss") + "'" + "," +
                                "message='" + message + "',message_Image_Path='" + message_Image_Path + "',sms_Count=" + sms_Count.ToString() + " WHERE company_Name='"+company_Name+"'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void create_Html_Structure_Table()
        {
            string query = "CREATE TABLE html_Structure (website_Name VARCHAR(20),file_Link_Element_Class_Or_Id_Name VARCHAR(200))";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void delete_Company_Info_Table()
        {
            string query = "DROP TABLE company_Info";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }
        
        public void Create_UserData_Table()
        {
            string query = "CREATE TABLE userinfo (name VARCHAR(30),encrypted_General_Password VARCHAR(50),phone_No VARCHAR(30),type VARCHAR(20),column_Name VARCHAR(50),company_Name VARCHAR(50),image_Path VARCHAR(200),mail_Id VARCHAR(50),encrypted_Mail_Password VARCHAR(50))";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Delete_UserInfo_Table()
        {
            string query = "DROP TABLE userinfo";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void reduce_The_SMS_Count_By_One(string company_Name)
        {
            double sms_Count = get_Remaining_SMS_Count(company_Name)-1;

            string query = "UPDATE company_Info SET sms_Count="+sms_Count.ToString() + " WHERE company_Name='"+company_Name+"'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public List<Job> get_All_The_Reminders()
        {
            List<Job> jobs = new List<Job>();

            string query = "SELECT * FROM reminders";
            MySqlConnection connection = new MySqlConnection(connection_String);

            string image_Path = "";
            string company_Name = "";
            List<string> job_Doiers_Names = new List<string>();
            List<string> job_Doiers_Mail_Ids = new List<string>();
            List<string> job_Doiers_Mobile_Nos = new List<string>();
            string creator_Name = "";
            string subject = "";
            string body = "";
            string notes = "";
            int time_Gap_Of_Minutes_Or_Hours = -1;
            Job.Time_Gap_Type time_Gap_Type = Job.Time_Gap_Type.Hour;
            List<Job.Reminder_Type> reminder_Types = new List<Job.Reminder_Type>();
            DateTime create_Time = new DateTime(1, 1, 1);
            DateTime start_Time = new DateTime(1, 1, 1);
            DateTime end_Time = new DateTime(1, 1, 1);
            string id = "";
            DateTime last_Reminded_Time = new DateTime(1, 1, 1);
            TimeZoneInfo creators_Time_Zone = TimeZoneInfo.Local;
            if(Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["image_Path"] != DBNull.Value) 
                        image_Path=(string)(reader["image_Path"]);
                    if (reader["company_Name"] != DBNull.Value) 
                        company_Name =(string)(reader["company_Name"]);
                    if (reader["job_Doiers_Names"] != DBNull.Value) 
                        job_Doiers_Names=((string)(reader["job_Doiers_Names"])).Split(new string[]{","},StringSplitOptions.None).ToList();
                    if (reader["job_Doiers_Mail_Ids"] != DBNull.Value)
                        job_Doiers_Mail_Ids = ((string)(reader["job_Doiers_Mail_Ids"])).Split(new string[] { "," }, StringSplitOptions.None).ToList();
                    if (reader["job_Doiers_Mobile_Nos"] != DBNull.Value)
                        job_Doiers_Mobile_Nos = ((string)(reader["job_Doiers_Mobile_Nos"])).Split(new string[] { "," }, StringSplitOptions.None).ToList();
                    if (reader["creator_Name"] != DBNull.Value) 
                        creator_Name=(string)(reader["creator_Name"]);
                    if (reader["subject"] != DBNull.Value) 
                        subject=(string)(reader["subject"]);
                    if (reader["body"] != DBNull.Value) 
                        body=(string)(reader["body"]);
                    if (reader["notes"] != DBNull.Value) 
                        notes=(string)(reader["notes"]);
                    if (reader["time_Gap_Of_Minutes_Or_Hours"] != DBNull.Value) 
                        time_Gap_Of_Minutes_Or_Hours=(Int32)(reader["time_Gap_Of_Minutes_Or_Hours"]);
                    if (reader["time_Gap_Type"] != DBNull.Value) 
                        time_Gap_Type = (Job.Time_Gap_Type)Enum.Parse(typeof(Job.Time_Gap_Type),(string)(reader["time_Gap_Type"]));
                    if (reader["reminder_Types"] != DBNull.Value) 
                    {
                         string[] string_Collection = ((string)(reader["reminder_Types"])).Split(new string[]{","},StringSplitOptions.None);
                         foreach(string p in string_Collection)
                         {
                             Job.Reminder_Type temp_Reminder_Type = (Job.Reminder_Type)(Enum.Parse(typeof(Job.Reminder_Type),p));
                             reminder_Types.Add(temp_Reminder_Type);
                         }
                    }
                    if (reader["create_Time"] != DBNull.Value)
                        create_Time = (DateTime)(reader["create_Time"]);
                    if (reader["start_Time"] != DBNull.Value)
                        start_Time = (DateTime)(reader["start_Time"]);
                    if (reader["end_Time"] != DBNull.Value)
                        end_Time = (DateTime)(reader["end_Time"]);
                    if (reader["id"] != DBNull.Value) 
                        id=(string)(reader["id"]);
                    if (reader["last_Reminded_Time"] != DBNull.Value)
                        last_Reminded_Time = (DateTime)(reader["last_Reminded_Time"]);
                    if (reader["creators_Time_Zone"] != DBNull.Value)
                    {
                        string temp = (string)(reader["creators_Time_Zone"]);
                        creators_Time_Zone = (from f in TimeZoneInfo.GetSystemTimeZones() where f.StandardName==temp select f).First();
                    }

                    jobs.Add(new Job(
                        image_Path,
                        company_Name,
                        job_Doiers_Names,
                        job_Doiers_Mail_Ids,
                        job_Doiers_Mobile_Nos,
                        creator_Name,
                        subject,
                        body,
                        notes,
                        time_Gap_Of_Minutes_Or_Hours,
                        time_Gap_Type,
                        reminder_Types,
                        create_Time,
                        start_Time,
                        end_Time,
                        id,
                        last_Reminded_Time,
                        creators_Time_Zone
                        ));
                }

                Close_This_Mysql_Connection(ref connection);
            }

            return jobs;
        }

        public double get_Remaining_SMS_Count(string company_Name)
        {
            string query = "SELECT sms_Count FROM company_Info WHERE company_Name='" + company_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            int sms_Count = 0;
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["sms_Count"] != DBNull.Value)
                        sms_Count = Convert.ToInt32(reader["sms_Count"]);
                }

                Close_This_Mysql_Connection(ref connection);
            }

            return sms_Count;
        }

        public bool Is_This_User_Name_Exist(string username,string company_Name)
        {
            string query = "SELECT COUNT(*) FROM userinfo WHERE name=" + "\"" + username + "\" AND company_Name='"+company_Name+"'";

            int count = -1;

            MySqlConnection connection = new MySqlConnection(connection_String);
            if(Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));

                Close_This_Mysql_Connection(ref connection);
            }

            if (count == 0)
                return false;
            else
                return true;
        }

        public void Delete_Formats_For_This_User(string username,string company_Name)
        {
            string query = "DELETE FROM formats WHERE name='"+username+"' AND company_Name='"+company_Name+"'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Add_User_Info(string username, string encrypted_General_Password,string phone_No,string type,string company_Name,string image_Path,string mail_Id)
        {
            string column_Name = Useful_Functions.Useful_Functions.get_Guid_String();
            string query = "INSERT INTO userinfo (name,encrypted_General_Password,phone_No,type,column_Name,company_Name,image_Path,mail_Id) VALUES("+ Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(username) + "," + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(encrypted_General_Password) + "," + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(phone_No)+","+Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(type)+","+ Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(column_Name) +","+"'"+company_Name+"','"+ image_Path +"','"+mail_Id+"')";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Delete_User_Info(string username,string company_Name)
        {
            Delete_Formats_For_This_User(username,company_Name);
            string query = "DELETE FROM userinfo WHERE name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(username)+"AND company_Name='"+company_Name+"'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Update_User_Info(string username,string password,string phone_No,string mail_Id,string type,string image_Path,string company_Name)
        {
            string content_String = "encrypted_General_Password='" + password + "'," +
                                    "phone_No='" + phone_No + "'," +
                                    "mail_Id='" + mail_Id + "'," +
                                    "type='" + type + "'," +
                                    "image_Path='" + image_Path + "'";
            string query = "UPDATE userinfo SET "+ content_String +" WHERE name=" + "\"" + username + "\" AND company_Name='"+company_Name+"'"+" AND image_Path='"+image_Path+"'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void save_This_Reminder_In_Database(Job given_Job)
        {
            string query = "INSERT reminders (image_Path,company_Name,job_Doiers_Names,job_Doiers_Mail_Ids,job_Doiers_Mobile_Nos,creator_Name,subject,body,notes,time_Gap_Of_Minutes_Or_Hours,time_Gap_Type,reminder_Types,create_Time,start_Time,end_Time,id,last_Reminded_Time,creators_Time_Zone) VALUES (" +
                                "'" + given_Job.image_Path + "'" + "," +
                                "'" + given_Job.company_Name + "'" + "," +
                                "'" + string.Join(",",given_Job.job_Doiers_Names) + "'" + "," +
                                "'" + string.Join(",", given_Job.job_Doiers_Mail_Ids) + "'" + "," +
                                "'" + string.Join(",", given_Job.job_Doiers_Mobile_Nos) + "'" + "," +
                                "'" + given_Job.creator_Name + "'" + "," +
                                "'" + given_Job.subject + "'" + "," +
                                "'" + given_Job.body + "'" + "," +
                                "'" + given_Job.notes + "'" + "," +
                                given_Job.time_Gap_Of_Minutes_Or_Hours.ToString() + "," +
                                "'" + given_Job.time_Gap_Type.ToString() + "'" + "," +
                                "'" + string.Join(",", from n in given_Job.reminder_Types select n.ToString()) + "'" + "," +
                                "'" + given_Job.create_Time.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                "'" + given_Job.start_Time.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                "'" + given_Job.end_Time.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                "'" + given_Job.id + "'" + "," +
                                "'" + given_Job.last_Reminded_Time.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                "'" + given_Job.creators_Time_Zone.StandardName + "'" +
                                  ")";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query,connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void update_Settings_For_This_User(string username,string company_Name,
            string mail_Id,string encrypted_Mail_Password,string encrypted_General_Password)
        {
            string query = "UPDATE userinfo SET mail_Id='"+mail_Id+
                "', encrypted_Mail_Password='"+encrypted_Mail_Password +"'"+
                ",encrypted_General_Password='"+encrypted_General_Password+"'"+
                " WHERE name='"+username+"' AND company_Name='"+company_Name+"'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public bool Check_User_Account(string username,string encrypted_General_Password,string company_Name)
        {
            string query = "SELECT * FROM userinfo WHERE name="+ Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(username)+" AND company_Name='"+company_Name+"'";

            bool result = false;

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //Console.WriteLine((reader["Tables_in_" + database] + ""));
                    string temp_Username = "";
                    string temp_Encrypted_General_Password = "";
                    if(reader["name"] != DBNull.Value)
                        temp_Username = (string)(reader["name"]);
                    if(reader["encrypted_General_Password"] != DBNull.Value)
                        temp_Encrypted_General_Password = (string)(reader["encrypted_General_Password"]);
                    if (temp_Username == username && temp_Encrypted_General_Password == encrypted_General_Password)
                    {
                        result = true;
                        break;
                    }
                }
                Close_This_Mysql_Connection(ref connection);
            }

            return result;
        }

        public Company_Info get_Company_Info_For_This_Company_Name(string company_Name)
        {
            string query = "SELECT * FROM company_Info WHERE company_Name ='"+company_Name+"'";

            string webPage_Base_Address ="";
            string ftp_Home_Folder_Path ="";
            DateTime expiry_Date = DateTime.Now;
            string message ="";
            string message_Image_Path ="";
            int sms_Count = 0;
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection)) 
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["webPage_Base_Address"] != DBNull.Value)
                        webPage_Base_Address = (string)(reader["webPage_Base_Address"]);
                    if (reader["ftp_Home_Folder_Path"] != DBNull.Value)
                        ftp_Home_Folder_Path = (string)(reader["ftp_Home_Folder_Path"]);
                    if (reader["expiry_Date"] != DBNull.Value)
                        expiry_Date = (DateTime)(reader["expiry_Date"]);
                    if (reader["message"] != DBNull.Value)
                        message = (string)(reader["message"]);
                    if(reader["message_Image_Path"] != DBNull.Value)
                        message_Image_Path = (string)(reader["message_Image_Path"]);
                    if(reader["sms_Count"]!=DBNull.Value)
                        sms_Count = (Int32)(reader["sms_Count"]);
                }
                Close_This_Mysql_Connection(ref connection);
            }

            if (webPage_Base_Address == "")
                return null;
            else
                return new Company_Info(company_Name, webPage_Base_Address, ftp_Home_Folder_Path,
                    expiry_Date, message, message_Image_Path, sms_Count);
        }

        public DateTime get_Expiry_Date_For_This_Company_Name(string company_Name)
        {
            string query = "SELECT expiry_Date FROM company_Info WHERE company_Name='"+company_Name+"'";
            DateTime temp = new DateTime();
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    if (reader["expiry_Date"] != DBNull.Value)
                        temp = (DateTime)(reader["expiry_Date"]);
                Close_This_Mysql_Connection(ref connection);
            }

            return temp;
        }

        public string get_Message_For_This_Company_Name(string company_Name)
        {
            string query = "SELECT message FROM company_Info WHERE company_Name='" + company_Name + "'";
            string temp = "";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    if (reader["message"] != DBNull.Value)
                        temp = (string)(reader["message"]);
                Close_This_Mysql_Connection(ref connection);
            }

            return temp;
        }

        public void Update_SMS_Format_For_This_Username(string username,string sms_Format,string interview_Name,string company_Name)
        {
            string query = "UPDATE formats SET sms_Format='"+sms_Format+"' WHERE name='"+username+"' AND company_Name='"+company_Name+"' AND interview_Name='"+interview_Name+"'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection)) 
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }
         
        public void Update_Subject_Format_For_This_Username(string username,string subject_Format,string interview_Name,string company_Name)
        {
            string query = "UPDATE formats SET subject_Format='" + subject_Format + "' WHERE name='" + username + "' AND company_Name='" + company_Name + "' AND interview_Name='" + interview_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
      }

        public void Update_Mail_Format_For_This_Username(string username, string mail_Format,string interview_Name,string company_Name)
        {
            string query = "UPDATE formats SET mail_Format='" + mail_Format + "' WHERE name='" + username + "' AND company_Name='" + company_Name + "' AND interview_Name='" + interview_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Update_Attended_Status_For_This_Url(string url, string interview_Name, bool status,DateTime datetime,string company_Name)
        {
            string query = "UPDATE main_Data SET attended_Status="+status+ ",attended_Status_Date="+ Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(datetime.ToString("yyyy-MM-dd HH:mm:ss")) +" WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name)+" AND company_Name='"+company_Name+"'" ;

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Update_Selected_Status_For_This_Url(string url, string interview_Name, bool status,DateTime datetime,string company_Name)
        {
            string query = "UPDATE main_Data SET selected_Status=" + status + ",selected_Status_Date=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(datetime.ToString("yyyy-MM-dd HH:mm:ss")) + " WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Update_Joined_Status_For_This_Url(string url, string interview_Name, bool status,DateTime datetime,string company_Name)
        {
            string query = "UPDATE main_Data SET joined_Status=" + status + ",joined_Status_Date=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(datetime.ToString("yyyy-MM-dd HH:mm:ss")) + " WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public string get_SMS_Format_For_This_Username(string username,string interview_Name,string company_Name)
        {
            string column_Name = get_ColumnName_Of_This_User(username,company_Name) + "_SMS_Format";
            string query = "SELECT "+ column_Name +" FROM interview_Names WHERE interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='"+company_Name+"'";
            string return_SMS_Format = "";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader[column_Name] == DBNull.Value)
                        return_SMS_Format = "";
                    else
                        return_SMS_Format = (string)(reader[column_Name]);
                }
                reader.Close();
                Close_This_Mysql_Connection(ref connection);
            }
            return return_SMS_Format;
        }

        public string get_Subject_Format_For_This_Username(string username,string interview_Name,string company_Name)
        {
            string column_Name = get_ColumnName_Of_This_User(username,company_Name) + "_Subject_Format";
            string query = "SELECT " + column_Name + " FROM interview_Names WHERE interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
            string return_Subject_Format = "";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader[column_Name] == DBNull.Value)
                        return_Subject_Format = "";
                    else
                        return_Subject_Format = (string)(reader[column_Name]);
                }
                reader.Close();
                Close_This_Mysql_Connection(ref connection);
            }
            return return_Subject_Format;
        }

        public List<Job> get_All_Reminders_Created_By_This_Person(string username,string company_Name)
        {
            List<Job> temp_List = new List<Job>();

            string query = "SELECT * FROM reminders";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string image_Path = "";
                    List<string> job_Doiers_Names = new List<string>();
                    List<string> job_Doiers_Mail_Ids = new List<string>();
                    List<string> job_Doiers_Mobile_Nos = new List<string>();
                    string creator_Name = "";
                    string subject = "";
                    string body = "";
                    string notes = "";
                    int time_Gap_Of_Minutes_Or_Hours = -1;
                    Job.Time_Gap_Type time_Gap_Type = Job.Time_Gap_Type.Hour;
                    List<Job.Reminder_Type> reminder_Types = new List<Job.Reminder_Type>();
                    DateTime create_Time = DateTime.Now;
                    DateTime start_Time = DateTime.Now;
                    DateTime end_Time = DateTime.Now;
                    string id = "";
                    DateTime last_Reminded_Time = DateTime.Now;
                    TimeZoneInfo creators_Time_Zone = TimeZoneInfo.Local;
                    if (reader["image_Path"] != DBNull.Value)
                        image_Path = (string)(reader["image_Path"]);
                    if (reader["job_Doiers_Names"] != DBNull.Value)
                        job_Doiers_Names = ((string)(reader["job_Doiers_Names"])).Split(new string[] { "," }, StringSplitOptions.None).ToList();
                    if (reader["job_Doiers_Mail_Ids"] != DBNull.Value)
                        job_Doiers_Mail_Ids = ((string)(reader["job_Doiers_Mail_Ids"])).Split(new string[] { "," }, StringSplitOptions.None).ToList();
                    if (reader["job_Doiers_Mobile_Nos"] != DBNull.Value)
                        job_Doiers_Mobile_Nos = ((string)(reader["job_Doiers_Mobile_Nos"])).Split(new string[] { "," }, StringSplitOptions.None).ToList();
                    if (reader["creator_Name"] != DBNull.Value)
                        creator_Name = (string)(reader["creator_Name"]);
                    if (reader["subject"] != DBNull.Value)
                        subject = (string)(reader["subject"]);
                    if (reader["body"] != DBNull.Value)
                        body = (string)(reader["body"]);
                    if (reader["notes"] != DBNull.Value)
                        notes = (string)(reader["notes"]);
                    if (reader["time_Gap_Of_Minutes_Or_Hours"] != DBNull.Value)
                        time_Gap_Of_Minutes_Or_Hours = (Int32)(reader["time_Gap_Of_Minutes_Or_Hours"]);
                    if (reader["time_Gap_Type"] != DBNull.Value)
                        time_Gap_Type =(Job.Time_Gap_Type)Enum.Parse(typeof(Job.Time_Gap_Type), (reader["time_Gap_Type"]).ToString());
                    if (reader["reminder_Types"] != DBNull.Value)
                        reminder_Types =  (from f in reader["reminder_Types"].ToString().Split(new string[]{","},StringSplitOptions.None) select (Job.Reminder_Type)Enum.Parse(typeof(Job.Reminder_Type),f)).ToList() ;
                    if (reader["create_Time"] != DBNull.Value)
                        create_Time = (DateTime)(reader["create_Time"]);
                    if (reader["start_Time"] != DBNull.Value)
                        start_Time = (DateTime)(reader["start_Time"]);
                    if (reader["end_Time"] != DBNull.Value)
                        end_Time = (DateTime)(reader["end_Time"]);
                    if (reader["id"] != DBNull.Value)
                        id = (string)(reader["id"]);
                    if (reader["last_Reminded_Time"] != DBNull.Value)
                        last_Reminded_Time = (DateTime)(reader["last_Reminded_Time"]);
                    if (reader["creators_Time_Zone"] != DBNull.Value)
                    {
                        string temp = (string)(reader["creators_Time_Zone"]);
                        creators_Time_Zone = (from f in TimeZoneInfo.GetSystemTimeZones() where f.StandardName == temp select f).First();
                    }

                    temp_List.Add(new Job(image_Path, company_Name, job_Doiers_Names,
                        job_Doiers_Mail_Ids,job_Doiers_Mobile_Nos,
                        creator_Name,
                        subject, body, notes, time_Gap_Of_Minutes_Or_Hours,
                        time_Gap_Type, reminder_Types, create_Time, start_Time, end_Time,id,
                        last_Reminded_Time,creators_Time_Zone));
                }
                Close_This_Mysql_Connection(ref connection);
            }

            return temp_List;
        }

        public string get_Mail_Format_For_This_Username(string username,string interview_Name,string company_Name)
        {
            string column_Name = get_ColumnName_Of_This_User(username,company_Name) + "_Mail_Format";
            string query = "SELECT " + column_Name + " FROM interview_Names WHERE interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
            string return_Mail_Format = "";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader[column_Name] == DBNull.Value)
                        return_Mail_Format = "";
                    else
                        return_Mail_Format = (string)(reader[column_Name]);
                }
                reader.Close();
                Close_This_Mysql_Connection(ref connection);
            }
            return return_Mail_Format;
        }

        public string get_Interview_Details_For_This_Interview_Name_With_GraveAccent_As_Separator(string interview_Name,string company_Name) // Format is interview_Name`contact_Details`date`time`venue
        {
            string query = "SELECT * FROM interview_Names WHERE interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            string date_String = "";
            string coordinator="";
            string venue = "";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if(Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query,connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    if(reader["date"] != DBNull.Value)
                    {
                        DateTime temp = (DateTime)(reader["date"]);
                        date_String = temp.ToShortDateString();
                    }

                    if(reader["coordinator"] != DBNull.Value)
                        coordinator = (string)(reader["coordinator"]);

                    if(reader["venue"] != DBNull.Value)
                        venue = (string)(reader["venue"]);
                }

                Close_This_Mysql_Connection(ref connection);
            }

            string contact_Details = "";
            if (Is_This_User_Name_Exist(coordinator,company_Name))
            {
                User_Detail temp = get_UserDetails_For_This_Username(coordinator,company_Name);
                contact_Details = "Contact Person:" + temp.username + ",Contact No:" + temp.phone_No;
            }

            return interview_Name + "`" +contact_Details +"`" +date_String +"`"+venue;
        }
        
        User_Detail get_UserDetails_For_This_Username(string username,string company_Name)
        {
            string query = "SELECT * FROM userinfo WHERE name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(username) + " AND company_Name='" + company_Name + "'";

            string encrypted_General_Password = "";
            string phone_No = "";
            string type = "";
            string column_Name = "";
            string image_Path = "";
            string mail_Id = "";
            string encrypted_Mail_Password = "";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if(Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query,connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["encrypted_General_Password"] != DBNull.Value)
                        encrypted_General_Password = (string)(reader["encrypted_General_Password"]);
                    
                    if (reader["phone_No"] != DBNull.Value)
                        phone_No = (string)(reader["phone_No"]);

                    if (reader["type"] != DBNull.Value)
                        type = (string)(reader["type"]);

                    if (reader["column_Name"] != DBNull.Value)
                        column_Name = (string)(reader["column_Name"]);

                    if (reader["image_Path"] != DBNull.Value)
                        image_Path = (string)(reader["image_Path"]);

                    if (reader["mail_Id"] != DBNull.Value)
                        mail_Id = (string)(reader["mail_Id"]);

                    if (reader["encrypted_Mail_Password"] != DBNull.Value)
                        encrypted_Mail_Password = (string)(reader["encrypted_Mail_Password"]);
                }
                Close_This_Mysql_Connection(ref connection);
            }

            return new User_Detail(username,encrypted_General_Password, phone_No, type, column_Name, image_Path,mail_Id,encrypted_Mail_Password);
        }

        public User_Detail get_User_Details_For_This_User(string given_User_Name,string company_Name)
        {
            string query = "SELECT * FROM userinfo WHERE company_Name='" + company_Name + "' AND Name='"+given_User_Name+"'";

            string username = "";
            string encrypted_General_Password = "";
            string phone_No = "";
            string column_Name = "";
            string type = "";
            string image_Path = "";
            string mail_Id = "";
            string encrypted_Mail_Password = "";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["name"] != DBNull.Value)
                        username = (string)(reader["name"]);

                    if (reader["encrypted_General_Password"] != DBNull.Value)
                        encrypted_General_Password = (string)(reader["encrypted_General_Password"]);

                    if (reader["phone_No"] != DBNull.Value)
                        phone_No = (string)(reader["phone_No"]);

                    if (reader["column_Name"] != DBNull.Value)
                        column_Name = (string)(reader["column_Name"]);

                    if (reader["type"] != DBNull.Value)
                        type = (string)(reader["type"]);

                    if (reader["image_Path"] != DBNull.Value)
                        image_Path = (string)(reader["image_Path"]);

                    if (reader["mail_Id"] != DBNull.Value)
                        mail_Id = (string)(reader["mail_Id"]);

                    if (reader["encrypted_Mail_Password"] != DBNull.Value)
                        encrypted_Mail_Password = (string)(reader["encrypted_Mail_Password"]);
                }
                Close_This_Mysql_Connection(ref connection);
            }
            return new User_Detail(username, encrypted_General_Password, phone_No, type, column_Name, image_Path,mail_Id,encrypted_Mail_Password);
        }

        public System.Collections.Generic.Dictionary<string, User_Detail> get_Users_Details(string company_Name)
        {
            System.Collections.Generic.Dictionary<string, User_Detail> return_Dict = new System.Collections.Generic.Dictionary<string,User_Detail>();

            string query = "SELECT * FROM userinfo WHERE company_Name='"+company_Name+"'" ;

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string username = "";
                    if(reader["name"] != DBNull.Value)
                        username =(string)(reader["name"]);

                    string encrypted_General_Password = "";
                    if (reader["encrypted_General_Password"] != DBNull.Value)
                        encrypted_General_Password = (string)(reader["encrypted_General_Password"]);

                    string phone_No = "";
                    if (reader["phone_No"] != DBNull.Value)
                        phone_No = (string)(reader["phone_No"]);

                    string column_Name = "";
                    if(reader["column_Name"] != DBNull.Value)
                        column_Name = (string)(reader["column_Name"]);

                    string type = "";
                    if (reader["type"] != DBNull.Value)
                        type = (string)(reader["type"]);

                    string image_Path = "";
                    if (reader["image_Path"] != DBNull.Value)
                        image_Path = (string)(reader["image_Path"]);

                    string mail_Id = "";
                    if (reader["mail_Id"] != DBNull.Value)
                        mail_Id = (string)(reader["mail_Id"]);

                    string encrypted_Mail_Password = "";
                    if (reader["encrypted_Mail_Password"] != DBNull.Value)
                        encrypted_Mail_Password = (string)(reader["encrypted_Mail_Password"]);

                    return_Dict.Add(username,new User_Detail(username,encrypted_General_Password,phone_No,type,column_Name,image_Path,mail_Id,encrypted_Mail_Password));
                }
                Close_This_Mysql_Connection(ref connection);
            }

            return return_Dict;
        }

        public string get_ColumnName_Of_This_User(string username,string company_Name)
        {
            string query = "SELECT column_Name FROM userinfo WHERE name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(username) + " AND company_Name='" + company_Name + "'";

            string column_Name = "";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["column_Name"] != DBNull.Value)
                        column_Name = (string)(reader["column_Name"]);
                }
                Close_This_Mysql_Connection(ref connection);
            }

            return column_Name;
        }

        public string get_Id_Of_This_Interview_Name(string interview_Name,string company_Name)
        {
            string query = "SELECT id FROM interview_Names WHERE interview_Name =" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            string id = "";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query,connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    if(reader["id"] != DBNull.Value)
                        id = (string)(reader["id"]);
                
                Close_This_Mysql_Connection(ref connection);
            }
            return id;
        }

        public void delete_This_Reminder(string id)
        {
            string query = "DELETE FROM reminders WHERE id='"+id+"'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void update_Last_Reminded_Time_Of_This_Reminder(DateTime last_Reminded_Time,string id)
        {
            string query = "UPDATE reminders SET last_Reminded_Time='"+ last_Reminded_Time.ToString("yyyy-MM-dd hh:mm:ss") +"' WHERE id='" + id + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void create_Reminder_Table()
        {
            if (!Check_For_Table("reminders"))
            {
                string query = "CREATE TABLE reminders (image_Path VARCHAR(200),company_Name VARCHAR(50),"+
                            "job_Doiers_Names VARCHAR(300),job_Doiers_Mail_Ids VARCHAR(300),job_Doiers_Mobile_Nos VARCHAR(300),"+
                            "creator_Name VARCHAR(20),subject VARCHAR(80)," +
                            "body VARCHAR(300),notes VARCHAR(300),time_Gap_Of_Minutes_Or_Hours INTEGER,"+
                            "time_Gap_Type VARCHAR(7),reminder_Types VARCHAR(20),create_Time DATETIME,"+
                            "start_Time DATETIME,end_Time DATETIME,id VARCHAR(50),last_Reminded_Time DATETIME,"+
                            "creators_Time_Zone VARCHAR(50))";

                MySqlConnection connection = new MySqlConnection(connection_String);
                if (Open_This_Mysql_Connection(ref connection))
                {
                    MySqlCommand cmd = new MySqlCommand(query,connection);
                    cmd.ExecuteNonQuery();
                    Close_This_Mysql_Connection(ref connection);
                }
            }
        }

        public void create_Formats_Table()
        {
            if (!Check_For_Table("formats"))
            {
                string query = "CREATE TABLE formats (name VARCHAR(50),company_Name VARCHAR(50),"+
                                "sms_Format VARCHAR(1000),subject_Format VARCHAR(1000)," +
                            "mail_Format VARCHAR(1000),interview_Name VARCHAR(50))";

                MySqlConnection connection = new MySqlConnection(connection_String);
                if (Open_This_Mysql_Connection(ref connection))
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    Close_This_Mysql_Connection(ref connection);
                }
            }

        }

        public void create_Main_Data_Table()
        {
            if (!Check_For_Table("main_Data"))
            {
                string query = "CREATE TABLE main_Data (name VARCHAR(50),phoneNo VARCHAR(50),mailId VARCHAR(100),url VARCHAR(300),interview_Name VARCHAR(50),feeder VARCHAR(50),feed_Date DATETIME,filterer VARCHAR(50),filter_Result BOOLEAN,filter_Date DATETIME,caller VARCHAR(50),call_Result BOOLEAN,call_Date DATETIME,coordinator VARCHAR(50),attended_Status BOOLEAN,attended_Status_Date DATETIME,selected_Status BOOLEAN,selected_Status_Date DATETIME,joined_Status BOOLEAN,joined_Status_Date DATETIME,company_Name VARCHAR(50),server_Side_Resume_File_Name VARCHAR(100),cookies VARCHAR(500))";

                MySqlConnection connection = new MySqlConnection(connection_String);
                if(Open_This_Mysql_Connection(ref connection))
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();

                    Close_This_Mysql_Connection(ref connection);
                }
            }
        }

        public void delete_Main_Data_Table()
        {
            if (Check_For_Table("main_Data"))
            {
                string query = "DROP TABLE main_Data";

                MySqlConnection connection = new MySqlConnection(connection_String);
                if (Open_This_Mysql_Connection(ref connection))
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();

                    Close_This_Mysql_Connection(ref connection);
                }
            }
        }

        public void Create_Interview_Names_Table()
        {
            if (!Check_For_Table("interview_Names"))
            {
                string query_0 = "CREATE TABLE interview_Names (interview_Name VARCHAR(50),id VARCHAR(50),date DATE,VENUE VARCHAR(500),coordinator varchar(50),payment INT,settled BOOLEAN,company_Name VARCHAR(50))";

                MySqlConnection connection = new MySqlConnection(connection_String);
                if (Open_This_Mysql_Connection(ref connection) == true)
                {
                    MySqlCommand cmd = new MySqlCommand(query_0, connection);
                    cmd.ExecuteNonQuery();
                    Close_This_Mysql_Connection(ref connection);
                }
            }

            /*if (get_Users_Details().Count == 0)
            {
                return;
            }
            List<string> temp_SMS_Column_Names = new List<string>();
            List<string> temp_Subject_Column_Names = new List<string>();
            List<string> temp_Mail_Column_Names = new List<string>();
            foreach (KeyValuePair<string,User_Detail> temp in get_Users_Details())      //username as key
            {
                string column_Name = temp.Value.column_Name;
                temp_SMS_Column_Names.Add(column_Name+"_SMS_Format VARCHAR(50)");
                temp_Subject_Column_Names.Add(column_Name + "_Subject_Format VARCHAR(50)");
                temp_Mail_Column_Names.Add(column_Name + "_Mail_Format VARCHAR(50)");
            }
            //string query = "CREATE TABLE interview_Names (interview_Name VARCHAR(150),table_Name VARCHAR(150)"+","+string.Join(",",temp_SMS_Column_Names)+","+ string.Join(",",temp_Subject_Column_Names) + ","+ string.Join(",",temp_Mail_Column_Names) + ","+ string.Join(",",temp_FeederCount_Column_Names) + ")";
            temp_SMS_Column_Names = temp_SMS_Column_Names.Select(x => true ?"ADD COLUMN " + x : "").ToList();
            temp_Subject_Column_Names = temp_Subject_Column_Names.Select(x => true ? "ADD COLUMN " + x : "").ToList();
            temp_Mail_Column_Names = temp_Mail_Column_Names.Select(x => true ? "ADD COLUMN " + x : "").ToList();

            string main_String = string.Join(",", temp_SMS_Column_Names) + "," + string.Join(",", temp_Subject_Column_Names) + "," + string.Join(",", temp_Mail_Column_Names);

            string query = "ALTER TABLE interview_Names " + main_String;

            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
             */
        }

        public bool is_This_Interview_Name_Exist(string given_Name,string company_Name)
        {
            string query = "SELECT COUNT(*) FROM interview_Names WHERE interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(given_Name) + " AND company_Name='" + company_Name + "'";

            int count = -1;

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));

                Close_This_Mysql_Connection(ref connection);
            }

            if (count == 0)
                return false;
            else
                return true;
        }

        public void Add_Interview(string given_Interview_Name, DateTime date, string coordinator,string venue,string company_Name)
        {
            string id = Useful_Functions.Useful_Functions.get_Guid_String();
            string date_String = date.ToString("yyyy-MM-dd");
            string query = "INSERT INTO interview_Names (interview_Name,id,date,coordinator,venue,company_Name) VALUES("+ Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(given_Interview_Name) +","+ Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(id) +","+Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(date_String)+","+Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(coordinator)+ ","+ Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(venue)+",'"+company_Name+"')";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public string get_Mail_Id_For_This_Username(string username,string company_Name)
        {
            string query = "SELECT mail_Id FROM userinfo WHERE name='"+username+"' AND company_Name='"+company_Name+"'";
            string mail_Id = "";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                
                while(reader.Read())
                    mail_Id = (string)(reader["mail_Id"]);
                Close_This_Mysql_Connection(ref connection);
            }
            return mail_Id;
        }

        public string get_Phone_No_For_This_Username(string username, string company_Name)
        {
            string query = "SELECT phone_No FROM userinfo WHERE name='" + username + "' AND company_Name='" + company_Name + "'";
            string phone_No = "";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    phone_No = (string)(reader["phone_No"]);
                Close_This_Mysql_Connection(ref connection);
            }
            return phone_No;
        }

        public string get_Encrypted_Mail_Password_For_This_Username(string username, string company_Name)
        {
            string query = "SELECT encrypted_Mail_Password FROM userinfo WHERE name='" + username + "' AND company_Name='" + company_Name + "'";
            string encrypted_Mail_Password = "";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                    encrypted_Mail_Password = (string)(reader["encrypted_Mail_Password"]);
                Close_This_Mysql_Connection(ref connection);
            }
            return encrypted_Mail_Password;
        }

        public void Update_Interview_Details(string given_Interview_Name, DateTime given_Date, string given_Coordinator,string given_Venue, decimal given_Payment, bool given_Settled,string company_Name)
        {
            string date_String = given_Date.ToString("yyyy-MM-dd");
            string query = "UPDATE interview_Names SET date=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(date_String) +
                        ",coordinator=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(given_Coordinator) +
                        ",venue=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(given_Venue) +
                        ",payment=" + given_Payment + 
                        ",settled="+(given_Settled?1:0).ToString()+
                        " WHERE interview_Name =" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(given_Interview_Name) + " AND company_Name='" + company_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Delete_Interview_Name(string given_Interview_Name,string company_Name)
        {
            string query = "DELETE FROM interview_Names WHERE interview_Name =" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(given_Interview_Name) + " AND company_Name='" + company_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }

            query = "DELETE FROM main_Data WHERE interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(given_Interview_Name) + " AND company_Name='" + company_Name + "'";

            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public System.Collections.ObjectModel.Collection<string> get_Interview_Names(string company_Name)
        {
            System.Collections.ObjectModel.Collection<string> temp = new System.Collections.ObjectModel.Collection<string>();
            string query = "SELECT interview_Name FROM interview_Names WHERE company_Name='"+company_Name +"'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                    if (reader["interview_Name"] != DBNull.Value)
                        temp.Add((string)(reader["interview_Name"]));

                Close_This_Mysql_Connection(ref connection);
            }

            return temp;
        }

        public string get_Exact_FTP_Home_Folder_Path_For_This_Company_Name(string company_Name)
        {
            string query = "SELECT  ftp_Home_Folder_Path FROM company_Info WHERE company_Name='"+company_Name+"'";
            string ftp_Home_Folder_Path = "";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    if (reader["ftp_Home_Folder_Path"] != DBNull.Value)
                        ftp_Home_Folder_Path = (string)(reader["ftp_Home_Folder_Path"]);

                Close_This_Mysql_Connection(ref connection);
            }

            return ftp_Home_Folder_Path;
        }

        public bool Is_This_Url_Exist_For_This_Interview(string url,string interview_Name,string company_Name)
        {
            string query = "SELECT COUNT(*) FROM main_Data WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            int count = -1;
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));

                Close_This_Mysql_Connection(ref connection);
            }

            if (count == 0)
                return false;
            else
                return true;
        }

        public void update_File_Location(string url,string file_Http_Location,string interview_Name,string company_Name)
        {
            string query = "UPDATE main_Data SET file_Location='"+file_Http_Location+"' WHERE url='"+url+"' AND interview_Name='"+interview_Name+"' AND company_Name='"+company_Name+"'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public string get_Coordinator_For_This_Interview_Name(string interview_Name,string company_Name)
        {
            string query = "SELECT coordinator FROM interview_Names WHERE interview_Name='"+interview_Name+"' AND company_Name='"+company_Name+"'";
            string coordinator = "";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                    if(reader["coordinator"] != DBNull.Value)
                        coordinator = (string)(reader["coordinator"]);

                Close_This_Mysql_Connection(ref connection);
            }
            return coordinator;
        }

        public void supply_This_Urls_Details_To_This_New_Interview(string url,string old_Interview_Name,string new_Interview_Name,string company_Name,DateTime feed_Date)
        {
            if (Is_This_Url_Exist_For_This_Interview(url, old_Interview_Name, company_Name))
            {
                string coordinator = get_Coordinator_For_This_Interview_Name(new_Interview_Name,company_Name);
                string query = @"INSERT INTO main_Data (name,phoneNo,mailId,url,interview_Name,feeder,feed_Date,coordinator,company_Name,cookies)
                                 SELECT name,phoneNo,mailId,url,'"+new_Interview_Name+"','Admin','"+feed_Date.ToString("yyyy-MM-dd HH:mm:ss")+"','"+coordinator+"',company_Name,cookies FROM main_Data WHERE url='"+ url+"' AND interview_Name='"+old_Interview_Name+"' AND company_Name='"+company_Name+"'";
                MySqlConnection connection = new MySqlConnection(connection_String);
                if (Open_This_Mysql_Connection(ref connection))
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    Close_This_Mysql_Connection(ref connection);
                }
            }
        }
        
        public void Add_URL_Info(string feeder, DateTime feed_Date,string url, string interview_Name,string company_Name,string name, string phoneNo, string mailId,string cookies)
        {
            if(Is_This_Url_Exist_For_This_Interview(url,interview_Name,company_Name))
                return;

            string coordinator="";
            Dictionary<string, Interview_And_Candidates_Count_Detail> temp_Dict = get_Candidates_Counts_Details_As_Dict_With_InterviewName_As_Key(company_Name);
            if (temp_Dict.ContainsKey(interview_Name))
                coordinator = temp_Dict[interview_Name].coordinator;
            string query = "INSERT INTO main_Data (feeder,feed_Date,url,interview_Name,coordinator,company_Name,name,phoneNo,mailId,cookies) VALUES("+ Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(feeder) +","+ Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(feed_Date.ToString("yyyy-MM-dd")) +","+Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url)+","+Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name)+","+ Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(coordinator) +",'"+company_Name+"','"+name+"','"+phoneNo+"','"+mailId+"','"+ cookies +"')";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if(Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query,connection);
                cmd.ExecuteNonQuery();

                // main_Data (url,interview_Name,feeder,feed_Date,filterer,filterer_Result,caller,caller_Result,coordinator,selected_Status,joined_Status)
                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Update_URL_Info_From_Filterer(string url, bool filter_Result,DateTime datetime,string interview_Name,string company_Name)
        {
            if (filter_Result)
            {
                string query = "UPDATE main_Data SET filter_Result=" + filter_Result +",filter_Date=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(datetime.ToString("yyyy-MM-dd HH:mm:ss")) + " WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND interview_Name='"+interview_Name+"'"+"AND company_Name='"+company_Name+"'";

                MySqlConnection connection = new MySqlConnection(connection_String);
                if (Open_This_Mysql_Connection(ref connection))
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();

                    Close_This_Mysql_Connection(ref connection);
                }
            }
            else
            {
                string query = "UPDATE main_Data SET filter_Result=" + filter_Result + ",filter_Date=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(datetime.ToString("yyyy-MM-dd HH:mm:ss")) + " WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND interview_Name='" + interview_Name + "'" + "AND company_Name='" + company_Name + "'";

                MySqlConnection connection = new MySqlConnection(connection_String);
                if (Open_This_Mysql_Connection(ref connection))
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();

                    Close_This_Mysql_Connection(ref connection);
                }
            }
        }

        public void Update_URL_Info_From_Caller(string url, bool caller_Result, string interview_Name,string company_Name)
        {
            string query = "UPDATE main_Data SET call_Result=" + caller_Result + " WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void set_This_Filterer_For_This_url(string filterer,string url,string interview_Name,string company_Name)
        {
            string query = "UPDATE main_Data SET filterer=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(filterer) + " WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public System.Collections.ObjectModel.Collection<string> get_Urls_For_Filterer(string filterer, int no_Of_Urls_Count, string interview_Name,string company_Name)
        {
            System.Collections.ObjectModel.Collection<string> urls = new System.Collections.ObjectModel.Collection<string>();

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                //string query = "SELECT url FROM main_Data WHERE filterer=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(filterer) + " AND filter_Result IS NULL" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "' AND file_Location IS NOT NULL AND url IS NOT NULL";
                string query = "SELECT url FROM main_Data WHERE filterer=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(filterer) + " AND filter_Result IS NULL" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read() && urls.Count < no_Of_Urls_Count)
                    urls.Add((string)(reader["url"]));

                if (!reader.IsClosed)
                    reader.Close();

                if (urls.Count < no_Of_Urls_Count)
                {
                    //string query_2 = "SELECT url FROM main_Data WHERE filterer IS NULL AND filter_Result IS NULL" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "' AND file_Location IS NOT NULL AND url IS NOT NULL";
                    string query_2 = "SELECT url FROM main_Data WHERE filterer IS NULL AND filter_Result IS NULL" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

                    cmd = new MySqlCommand(query_2, connection);

                    reader = cmd.ExecuteReader();

                    while (reader.Read() && urls.Count < no_Of_Urls_Count)
                    {
                        urls.Add((string)(reader["url"]));
                    }
                }
                if (!reader.IsClosed)
                {
                    reader.Close();
                }

                foreach (string url_Temp in urls)
                {
                    string query_3 = "UPDATE main_Data SET filterer=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(filterer) + " WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url_Temp) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                    cmd = new MySqlCommand(query_3, connection);
                    cmd.ExecuteNonQuery();
                }

                Close_This_Mysql_Connection(ref connection);
            }

            return urls;
        }

        public string get_History_For_This_Url(string url,string interview_Name,string company_Name)
        {
            string query = "SELECT * FROM main_Data WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND company_Name='" + company_Name + "'";

            string main_String = "";
            MySqlConnection temp_Connection = new MySqlConnection(connection_String);
            temp_Connection.Open();
            {
                MySqlCommand cmd = new MySqlCommand(query, temp_Connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                List<string> interview_Names = new List<string>();
                List<string> call_Results = new List<string>();
                List<string> attended_Statuses = new List<string>();
                List<string> selected_Statuses = new List<string>();
                List<string> joined_Statuses = new List<string>();
                while (reader.Read())
                {
                    // main_Data (name,phoneNo,mailId,url,interview_Name,feeder,feed_Date,filterer,filter_Result,caller,call_Result,coordinator,attended_Status,selected_Status,joined_Status)
                    if (reader["interview_Name"] != DBNull.Value)
                        interview_Names.Add((string)(reader["interview_Name"]));
                    else
                        interview_Names.Add("null");

                    if (reader["call_Result"] != DBNull.Value)
                    {
                        bool temp = (bool)(reader["call_Result"]);
                        call_Results.Add(temp.ToString());
                    }
                    else
                        call_Results.Add("null");

                    if (reader["attended_Status"] != DBNull.Value)
                    {
                        //Console.WriteLine("! " + reader["attended_Status"].ToString());
                        bool temp = (bool)(reader["attended_Status"]);
                        attended_Statuses.Add(temp.ToString());
                    }
                    else
                    {
                        //Console.WriteLine("|^| "+reader["attended_Status"].ToString());
                        attended_Statuses.Add("null");
                    }

                    if (reader["selected_Status"] != DBNull.Value)
                    {
                        //Console.WriteLine(reader["selected_Status"].ToString());
                        bool temp = (bool)(reader["selected_Status"]);
                        selected_Statuses.Add(temp.ToString());
                    }
                    else
                        selected_Statuses.Add("null");

                    if (reader["joined_Status"] != DBNull.Value)
                    {
                        bool temp = (bool)(reader["joined_Status"]);
                        joined_Statuses.Add(temp.ToString());
                    }
                    else
                        joined_Statuses.Add("null");
                }

                
                for (int i = 0; i < interview_Names.Count; ++i)
                {
                    if (interview_Names[i] == interview_Name)
                        continue;

                    main_String += interview_Names[i] + "\n";
                    string[] temp = get_Interview_Details_For_This_Interview_Name_With_GraveAccent_As_Separator(interview_Name,company_Name).Split("`".ToCharArray()); //interview_Name`contact_Details`date`time`venue
                    main_String += temp[1] + "\n";
                    string temp_Result = "";
                    if (string.Compare(joined_Statuses[i],"true",true) == 0)
                        temp_Result = "Joined";
                    else if (string.Compare(joined_Statuses[i], "false", true) == 0)
                        temp_Result = "Selected, But Didnt Join";
                    else if (string.Compare(joined_Statuses[i], "null", true) == 0)
                    {
                        if (string.Compare(selected_Statuses[i], "false", true) == 0)
                            temp_Result = "Attended the Interview and got Rejected";
                        else if (string.Compare(selected_Statuses[i], "null", true) == 0)
                        {
                            if (string.Compare(attended_Statuses[i], "false", true) == 0)
                                temp_Result = "Qualified by the caller. But Didn't attend the Interview";
                            else if (string.Compare(attended_Statuses[i], "null", true) == 0)
                            {
                                if (string.Compare(call_Results[i], "false", true) == 0)
                                    temp_Result = "Rejected by the caller";
                                else if (string.Compare(call_Results[i], "null", true) == 0)
                                    temp_Result = "Caller has to call him";
                            }
                        }
                    }

                    main_String += temp_Result + "\n";
                    main_String += "==x====x====x====x====x====x====x====x====x==" + "\n";
                }
                temp_Connection.Close();
            }
            return main_String;
        }

        public System.Collections.ObjectModel.Collection<string> Get_Contents_For_Coordinator(string coordinator,string interview_Name,string company_Name)
        {
            System.Collections.ObjectModel.Collection<string> temp = new System.Collections.ObjectModel.Collection<string>();

            string query = "SELECT * FROM main_Data WHERE joined_Status IS NULL AND coordinator=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(coordinator) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            query = "SELECT * FROM (" + query + ") AS alias_0 WHERE ((selected_Status IS NULL) OR (selected_Status=1))";
            query = "SELECT * FROM (" + query + ") AS alias_1 WHERE ((attended_Status IS NULL) OR (attended_Status=1))";
            query = "SELECT * FROM (" + query + ") AS alias_2 WHERE call_Result=1";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string name = "";
                    string phoneNo = "";
                    string mailId = "";
                    string url = "";

                    string attended_Status = "";
                    string selected_Status = "";
                    string joined_Status = "";

                    if (reader["name"] != DBNull.Value)
                        name = (string)(reader["name"]);
                    if (reader["phoneNo"] != DBNull.Value)
                        phoneNo = (string)(reader["phoneNo"]);
                    if (reader["mailId"] != DBNull.Value)
                       mailId = (string)(reader["mailId"]);
                    if (reader["url"] != DBNull.Value)
                        url = (string)(reader["url"]);

                    if (reader["attended_Status"] == DBNull.Value)
                        attended_Status = "null";
                    else
                        attended_Status = reader["attended_Status"].ToString();

                    if (reader["selected_Status"] == DBNull.Value)
                        selected_Status = "null";
                    else
                        selected_Status = reader["selected_Status"].ToString();

                    if (reader["joined_Status"] == DBNull.Value)
                        joined_Status = "null";
                    else
                       joined_Status = reader["joined_Status"].ToString();

                    temp.Add(name + "|^|" + phoneNo+"|^|" + mailId +"|^|" +attended_Status + "|^|" + selected_Status +"|^|" +joined_Status+"|^|"+ url);
                }
                Close_This_Mysql_Connection(ref connection);
            }
            return temp;
        }

        public Dictionary<string, List<Tuple<string, string>>> get_File_Link_Element_Class_Or_Id_Names()
        {
            Dictionary<string, List<Tuple<string, string>>> temp = new Dictionary<string,List<Tuple<string,string>>>();

            string query = "SELECT * FROM html_Structure";
            MySqlConnection connection = new MySqlConnection(connection_String);

            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string website_Name = "";
                    if (reader["website_Name"] != DBNull.Value)
                        website_Name = (string)(reader["website_Name"]);
                    List<Tuple<string, string>> temp_List = new List<Tuple<string, string>>();
                    if (reader["file_Link_Element_Class_Or_Id_Name"] != DBNull.Value)
                    {
                        string temp_String = (string)(reader["file_Link_Element_Class_Or_Id_Name"]);
                        string[] temp_Collection = temp_String.Split(new string[] { ";" }, StringSplitOptions.None);
                        foreach (string single_Element in temp_Collection)
                        {
                            string type = single_Element.Substring(0,single_Element.IndexOf("="));
                            string content = single_Element.Substring(single_Element.IndexOf("=")+1);
                            temp_List.Add(new Tuple<string, string>(type, content));
                        }
                    }
                    temp.Add(website_Name, temp_List);
                }
                Close_This_Mysql_Connection(ref connection);
            }
            return temp;
        }

        public System.Collections.ObjectModel.Collection<string> Get_Contents_For_Caller(string caller, string given_Network, int no_Of_Lines, string interview_Name,string company_Name)
        {
            System.Collections.ObjectModel.Collection<string> lines = new System.Collections.ObjectModel.Collection<string>();
            System.Collections.ObjectModel.Collection<string> urls = new System.Collections.ObjectModel.Collection<string>();

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                System.Collections.ObjectModel.Collection<string> temp_Names = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_PhoneNos = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_MailIds = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_Urls = new System.Collections.ObjectModel.Collection<string>();

                string query = "SELECT * FROM main_Data WHERE caller=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(caller) + " AND call_Result IS NULL AND filter_Result=1" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                //TABLE tempdata (feeder VARCHAR(30),url VARCHAR(200),filterer VARCHAR(30),filter_Result TINYINT(1),name VARCHAR(30),phoneNo VARCHAR(30),mailId VARCHAR(30), caller VARCHAR(30),call_Result TINYINT(1))";

                while (reader.Read())
                {
                    //Console.WriteLine("Entering caller");
                    if (reader["name"] == DBNull.Value)
                        temp_Names.Add("");
                    else
                        temp_Names.Add((string)(reader["name"]));
                    if (reader["phoneNo"] == DBNull.Value)
                        temp_PhoneNos.Add("");
                    else
                        temp_PhoneNos.Add((string)(reader["phoneNo"]));
                    if (reader["mailId"] == DBNull.Value)
                        temp_MailIds.Add("");
                    else
                        temp_MailIds.Add((string)(reader["mailId"]));
                    if (reader["url"] == DBNull.Value)
                        temp_Urls.Add("");
                    else
                        temp_Urls.Add((string)(reader["url"]));
                }

                if (!reader.IsClosed)
                {
                    // we have to close the reader here, because get_Network_Details uses one reader
                    // only one reader can exist at any given time in the given database connection
                    reader.Close();
                }

                for (int i = 0; i < temp_PhoneNos.Count; ++i)
                {
                    string network_Detail = get_Network_Details(temp_PhoneNos[i]);
                    string separator = ";";

                    string[] temps = network_Detail.Split(separator.ToCharArray());

                    string network = temps[1].Substring(temps[1].IndexOf(":") + 1);

                    if ((network == given_Network) && (lines.Count < no_Of_Lines))
                    {
                        urls.Add(temp_Urls[i]);
                        string history = get_History_For_This_Url(temp_Urls[i],interview_Name,company_Name);
                        lines.Add(temp_Names[i] + "|^|" + temp_PhoneNos[i] + "|^|" + temp_MailIds[i] + "|^|" + network_Detail + "|^|" + history +"|^|" + temp_Urls[i]);
                    }
                }

                temp_Names.Clear();
                temp_PhoneNos.Clear();
                temp_MailIds.Clear();
                temp_Urls.Clear();

                if (!reader.IsClosed)
                    reader.Close();

                if (lines.Count < no_Of_Lines)
                {
                    string query_2 = "SELECT * FROM main_Data WHERE caller IS NULL AND call_Result IS NULL AND filter_Result=1" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                    cmd = new MySqlCommand(query_2, connection);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        //Console.WriteLine("Entering");
                        if (reader["name"] == DBNull.Value)
                            temp_Names.Add("");
                        else
                            temp_Names.Add((string)(reader["name"]));

                        if (reader["phoneNo"] == DBNull.Value)
                            temp_PhoneNos.Add("");
                        else
                            temp_PhoneNos.Add((string)(reader["phoneNo"]));

                        if (reader["mailId"] == DBNull.Value)
                            temp_MailIds.Add("");
                        else
                            temp_MailIds.Add((string)(reader["mailId"]));

                        if (reader["url"] == DBNull.Value)
                            temp_Urls.Add("");
                        else
                            temp_Urls.Add((string)(reader["url"]));
                    }

                    if (!reader.IsClosed)
                    {
                        // we have to close the reader here, because get_Network_Details uses one reader
                        // only one reader can exist at any given time in the given database connection
                        reader.Close();
                    }

                    for (int i = 0; i < temp_PhoneNos.Count; ++i)
                    {
                        string network_Detail = get_Network_Details(temp_PhoneNos[i]);
                        string separator = ";";

                        string[] temps = network_Detail.Split(separator.ToCharArray());

                        string network = temps[1].Substring(temps[1].IndexOf(":") + 1);
                        if ((network == given_Network) && (lines.Count < no_Of_Lines))
                        {
                            urls.Add(temp_Urls[i]);
                            string history = get_History_For_This_Url(temp_Urls[i], interview_Name,company_Name);
                            lines.Add(temp_Names[i] + "|^|" + temp_PhoneNos[i] + "|^|" + temp_MailIds[i] + "|^|" + network_Detail + "|^|" + history +"|^|"+temp_Urls[i]);
                        }
                    }
                }

                foreach (string url_Temp in urls)
                {
                    string query_3 = "UPDATE main_Data SET caller=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(caller) + " WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url_Temp) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                    cmd = new MySqlCommand(query_3, connection);
                    cmd.ExecuteNonQuery();
                }
                //Console.WriteLine("Server=>" +"no_Of_Lines:" + no_Of_Lines +",lines_Count:"+lines.Count);
                Close_This_Mysql_Connection(ref connection);
            }
            return lines;
        }

        public void Update_Filter_Result(string url, string filterer, bool filter_Result,string interview_Name,string company_Name)
        {
            string query = "UPDATE main_Data SET filterer=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(filterer) + ",filter_Result=" + (filter_Result ? 1 : 0).ToString() + " WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Update_Call_Result(string url, string caller, bool caller_Result, string interview_Name,DateTime datetime,string company_Name)
        {
            string query = "UPDATE main_Data SET caller=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(caller) + ",call_Result=" + (caller_Result ? 1 : 0).ToString() + ",call_Date=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(datetime.ToString("yyyy-MM-dd HH:mm:ss")) + " WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Delete_Entry_By_Referring_This_Url(string url, string interview_Name, string company_Name)
        {
            string query = "DELETE FROM main_Data WHERE url=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(url) + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                Close_This_Mysql_Connection(ref connection);
            }
        }

        public void Delete_Interview_Names_Table()
        {
            string query = "DROP TABLE interview_Names";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Close_This_Mysql_Connection(ref connection);
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
            public string get_Details_As_String_With_This_Separator(string separator)
            {
                return name + separator + phone_No + separator + mail_Id;
            }
            //private Candidate_Detail() { }
        }

        public Dictionary<string, Dictionary<string, Candidate_Detail>> get_Qualified_Candidates_Details(string interview_Name,string company_Name)
        {
            Dictionary<string, Dictionary<string, Candidate_Detail>> main = new System.Collections.Generic.Dictionary<string, Dictionary<string, Candidate_Detail>>();  // <network, <number,candidate_Details> >

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                System.Collections.ObjectModel.Collection<string> temp_Names = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_PhoneNos = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_MailIds = new System.Collections.ObjectModel.Collection<string>();

                string query = "SELECT * FROM main_Data WHERE call_Result=1 AND attended_Status is NULL" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                //TABLE tempdata (feeder VARCHAR(30),url VARCHAR(200),filterer VARCHAR(30),filter_Result TINYINT(1),name VARCHAR(30),phoneNo VARCHAR(30),mailId VARCHAR(30), caller VARCHAR(30),call_Result TINYINT(1))";

                while (reader.Read())
                {
                    //Console.WriteLine("Entering caller");
                    if (reader["name"] == DBNull.Value)
                        temp_Names.Add("");
                    else
                        temp_Names.Add((string)(reader["name"]));
                    if (reader["phoneNo"] == DBNull.Value)
                        temp_PhoneNos.Add("");
                    else
                        temp_PhoneNos.Add((string)(reader["phoneNo"]));
                    if (reader["mailId"] == DBNull.Value)
                        temp_MailIds.Add("");
                    else
                        temp_MailIds.Add((string)(reader["mailId"]));
                }

                if (!reader.IsClosed)
                {
                    // we have to close the reader here, because get_Network_Details uses one reader
                    // only one reader can exist at any given time in the given database connection
                    reader.Close();
                }

                for (int i = 0; i < temp_PhoneNos.Count; ++i)
                {
                    string network_Detail = get_Network_Details(temp_PhoneNos[i]);
                    string separator = ";";

                    string[] temps = network_Detail.Split(separator.ToCharArray());

                    string network = temps[1].Substring(temps[1].IndexOf(":") + 1);
                    Candidate_Detail temp_Candidate = new Candidate_Detail(temp_Names[i], temp_PhoneNos[i], temp_MailIds[i]);
                    if (main.ContainsKey(network))
                    {
                        if (!main[network].ContainsKey(temp_PhoneNos[i]))
                            main[network].Add(temp_PhoneNos[i], temp_Candidate);
                    }
                    else
                    {
                        System.Collections.Generic.Dictionary<string, Candidate_Detail> temp_Sub_Dict = new Dictionary<string, Candidate_Detail>();
                        temp_Sub_Dict.Add(temp_PhoneNos[i], temp_Candidate);
                        main.Add(network, temp_Sub_Dict);
                    }
                }

                if (!reader.IsClosed)
                    reader.Close();

                Close_This_Mysql_Connection(ref connection);
            }

            return main;
        }

        public Dictionary<string, Dictionary<string, Candidate_Detail>> get_Attended_Candidates_Details(string interview_Name,string company_Name)
        {
            Dictionary<string, Dictionary<string, Candidate_Detail>> main = new System.Collections.Generic.Dictionary<string, Dictionary<string, Candidate_Detail>>();  // <network, <number,candidate_Details> >

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                System.Collections.ObjectModel.Collection<string> temp_Names = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_PhoneNos = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_MailIds = new System.Collections.ObjectModel.Collection<string>();

                string query = "SELECT * FROM main_Data WHERE attended_Status=1 AND selected_Status is null" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                //TABLE tempdata (feeder VARCHAR(30),url VARCHAR(200),filterer VARCHAR(30),filter_Result TINYINT(1),name VARCHAR(30),phoneNo VARCHAR(30),mailId VARCHAR(30), caller VARCHAR(30),call_Result TINYINT(1))";

                while (reader.Read())
                {
                    //Console.WriteLine("Entering caller");
                    if (reader["name"] == DBNull.Value)
                        temp_Names.Add("");
                    else
                        temp_Names.Add((string)(reader["name"]));
                    if (reader["phoneNo"] == DBNull.Value)
                        temp_PhoneNos.Add("");
                    else
                        temp_PhoneNos.Add((string)(reader["phoneNo"]));
                    if (reader["mailId"] == DBNull.Value)
                        temp_MailIds.Add("");
                    else
                        temp_MailIds.Add((string)(reader["mailId"]));
                }

                if (!reader.IsClosed)
                {
                    // we have to close the reader here, because get_Network_Details uses one reader
                    // only one reader can exist at any given time in the given database connection
                    reader.Close();
                }

                for (int i = 0; i < temp_PhoneNos.Count; ++i)
                {
                    string network_Detail = get_Network_Details(temp_PhoneNos[i]);
                    string separator = ";";

                    string[] temps = network_Detail.Split(separator.ToCharArray());

                    string network = temps[1].Substring(temps[1].IndexOf(":") + 1);
                    Candidate_Detail temp_Candidate = new Candidate_Detail(temp_Names[i], temp_PhoneNos[i], temp_MailIds[i]);
                    if (main.ContainsKey(network))
                    {
                        if (!main[network].ContainsKey(temp_PhoneNos[i]))
                            main[network].Add(temp_PhoneNos[i], temp_Candidate);
                    }
                    else
                    {
                        System.Collections.Generic.Dictionary<string, Candidate_Detail> temp_Sub_Dict = new Dictionary<string, Candidate_Detail>();
                        temp_Sub_Dict.Add(temp_PhoneNos[i], temp_Candidate);
                        main.Add(network, temp_Sub_Dict);
                    }
                }

                if (!reader.IsClosed)
                    reader.Close();

                Close_This_Mysql_Connection(ref connection);
            }

            return main;
        }

        public Dictionary<string, Dictionary<string, Candidate_Detail>> get_Selected_Candidates_Details(string interview_Name,string company_Name)
        {
            Dictionary<string, Dictionary<string, Candidate_Detail>> main = new System.Collections.Generic.Dictionary<string, Dictionary<string, Candidate_Detail>>();  // <network, <number,candidate_Details> >

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                System.Collections.ObjectModel.Collection<string> temp_Names = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_PhoneNos = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_MailIds = new System.Collections.ObjectModel.Collection<string>();

                string query = "SELECT * FROM main_Data WHERE selected_Status=1 AND joined_Status is null" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                //TABLE tempdata (feeder VARCHAR(30),url VARCHAR(200),filterer VARCHAR(30),filter_Result TINYINT(1),name VARCHAR(30),phoneNo VARCHAR(30),mailId VARCHAR(30), caller VARCHAR(30),call_Result TINYINT(1))";

                while (reader.Read())
                {
                    //Console.WriteLine("Entering caller");
                    if (reader["name"] == DBNull.Value)
                        temp_Names.Add("");
                    else
                        temp_Names.Add((string)(reader["name"]));
                    if (reader["phoneNo"] == DBNull.Value)
                        temp_PhoneNos.Add("");
                    else
                        temp_PhoneNos.Add((string)(reader["phoneNo"]));
                    if (reader["mailId"] == DBNull.Value)
                        temp_MailIds.Add("");
                    else
                        temp_MailIds.Add((string)(reader["mailId"]));
                }

                if (!reader.IsClosed)
                {
                    // we have to close the reader here, because get_Network_Details uses one reader
                    // only one reader can exist at any given time in the given database connection
                    reader.Close();
                }

                for (int i = 0; i < temp_PhoneNos.Count; ++i)
                {
                    string network_Detail = get_Network_Details(temp_PhoneNos[i]);
                    string separator = ";";

                    string[] temps = network_Detail.Split(separator.ToCharArray());

                    string network = temps[1].Substring(temps[1].IndexOf(":") + 1);
                    Candidate_Detail temp_Candidate = new Candidate_Detail(temp_Names[i], temp_PhoneNos[i], temp_MailIds[i]);
                    if (main.ContainsKey(network))
                    {
                        if (!main[network].ContainsKey(temp_PhoneNos[i]))
                            main[network].Add(temp_PhoneNos[i], temp_Candidate);
                    }
                    else
                    {
                        System.Collections.Generic.Dictionary<string, Candidate_Detail> temp_Sub_Dict = new Dictionary<string, Candidate_Detail>();
                        temp_Sub_Dict.Add(temp_PhoneNos[i], temp_Candidate);
                        main.Add(network, temp_Sub_Dict);
                    }
                }

                if (!reader.IsClosed)
                    reader.Close();

                Close_This_Mysql_Connection(ref connection);
            }

            return main;
        }

        public Dictionary<string, Dictionary<string, Candidate_Detail>> get_Joined_Candidates_Details(string interview_Name,string company_Name)
        {
            Dictionary<string, Dictionary<string, Candidate_Detail>> main = new System.Collections.Generic.Dictionary<string, Dictionary<string, Candidate_Detail>>();  // <network, <number,candidate_Details> >

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                System.Collections.ObjectModel.Collection<string> temp_Names = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_PhoneNos = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_MailIds = new System.Collections.ObjectModel.Collection<string>();

                string query = "SELECT * FROM main_Data WHERE joined_Status=1" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                //TABLE tempdata (feeder VARCHAR(30),url VARCHAR(200),filterer VARCHAR(30),filter_Result TINYINT(1),name VARCHAR(30),phoneNo VARCHAR(30),mailId VARCHAR(30), caller VARCHAR(30),call_Result TINYINT(1))";

                while (reader.Read())
                {
                    //Console.WriteLine("Entering caller");
                    if (reader["name"] == DBNull.Value)
                        temp_Names.Add("");
                    else
                        temp_Names.Add((string)(reader["name"]));
                    if (reader["phoneNo"] == DBNull.Value)
                        temp_PhoneNos.Add("");
                    else
                        temp_PhoneNos.Add((string)(reader["phoneNo"]));
                    if (reader["mailId"] == DBNull.Value)
                        temp_MailIds.Add("");
                    else
                        temp_MailIds.Add((string)(reader["mailId"]));
                }

                if (!reader.IsClosed)
                {
                    // we have to close the reader here, because get_Network_Details uses one reader
                    // only one reader can exist at any given time in the given database connection
                    reader.Close();
                }

                for (int i = 0; i < temp_PhoneNos.Count; ++i)
                {
                    string network_Detail = get_Network_Details(temp_PhoneNos[i]);
                    string separator = ";";

                    string[] temps = network_Detail.Split(separator.ToCharArray());

                    string network = temps[1].Substring(temps[1].IndexOf(":") + 1);
                    Candidate_Detail temp_Candidate = new Candidate_Detail(temp_Names[i], temp_PhoneNos[i], temp_MailIds[i]);
                    if (main.ContainsKey(network))
                    {
                        if (!main[network].ContainsKey(temp_PhoneNos[i]))
                            main[network].Add(temp_PhoneNos[i], temp_Candidate);
                    }
                    else
                    {
                        System.Collections.Generic.Dictionary<string, Candidate_Detail> temp_Sub_Dict = new Dictionary<string, Candidate_Detail>();
                        temp_Sub_Dict.Add(temp_PhoneNos[i], temp_Candidate);
                        main.Add(network, temp_Sub_Dict);
                    }
                }

                if (!reader.IsClosed)
                    reader.Close();

                Close_This_Mysql_Connection(ref connection);
            }

            return main;
        }

        public Dictionary<string, Dictionary<string, Candidate_Detail>> get_Filtered_Candidates_Details(string interview_Name,string company_Name)       //<network,<phone_No,Candidate_Detail> >
        {
            Dictionary<string, Dictionary<string, Candidate_Detail>> main = new System.Collections.Generic.Dictionary<string, Dictionary<string, Candidate_Detail>>();  // <network, <number,candidate_Details> >

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                System.Collections.ObjectModel.Collection<string> temp_Names = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_PhoneNos = new System.Collections.ObjectModel.Collection<string>();
                System.Collections.ObjectModel.Collection<string> temp_MailIds = new System.Collections.ObjectModel.Collection<string>();

                string query = "SELECT * FROM main_Data WHERE caller IS NULL AND filter_Result=1" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                //TABLE tempdata (feeder VARCHAR(30),url VARCHAR(200),filterer VARCHAR(30),filter_Result TINYINT(1),name VARCHAR(30),phoneNo VARCHAR(30),mailId VARCHAR(30), caller VARCHAR(30),call_Result TINYINT(1))";

                while (reader.Read())
                {
                    //Console.WriteLine("Entering caller");
                    if (reader["name"] == DBNull.Value)
                        temp_Names.Add("");
                    else
                        temp_Names.Add((string)(reader["name"]));
                    if (reader["phoneNo"] == DBNull.Value)
                        temp_PhoneNos.Add("");
                    else
                        temp_PhoneNos.Add((string)(reader["phoneNo"]));
                    if (reader["mailId"] == DBNull.Value)
                        temp_MailIds.Add("");
                    else
                        temp_MailIds.Add((string)(reader["mailId"]));
                }

                if (!reader.IsClosed)
                {
                    // we have to close the reader here, because get_Network_Details uses one reader
                    // only one reader can exist at any given time in the given database connection
                    reader.Close();
                }

                for (int i = 0; i < temp_PhoneNos.Count; ++i)
                {
                    string network_Detail = get_Network_Details(temp_PhoneNos[i]);
                    string separator = ";";

                    string[] temps = network_Detail.Split(separator.ToCharArray());

                    string network = temps[1].Substring(temps[1].IndexOf(":") + 1);
                    Candidate_Detail temp_Candidate = new Candidate_Detail(temp_Names[i], temp_PhoneNos[i], temp_MailIds[i]);
                    if (main.ContainsKey(network))
                    {
                        if (!main[network].ContainsKey(temp_PhoneNos[i]))
                            main[network].Add(temp_PhoneNos[i], temp_Candidate);
                    }
                    else
                    {
                        System.Collections.Generic.Dictionary<string, Candidate_Detail> temp_Sub_Dict = new Dictionary<string, Candidate_Detail>();
                        temp_Sub_Dict.Add(temp_PhoneNos[i], temp_Candidate);
                        main.Add(network, temp_Sub_Dict);
                    }
                }

                if (!reader.IsClosed)
                    reader.Close();

                Close_This_Mysql_Connection(ref connection);
            }

            return main;
        }

        public System.Collections.Generic.Dictionary<string,Interview_And_Candidates_Count_Detail> get_Candidates_Counts_Details_As_Dict_With_InterviewName_As_Key(string company_Name)
        {
            System.Collections.Generic.Dictionary<string, Interview_And_Candidates_Count_Detail> temp = new Dictionary<string, Interview_And_Candidates_Count_Detail>();

            string query = "";
            MySqlCommand cmd = null;
            MySqlDataReader reader = null;
            foreach (string interview_Name in get_Interview_Names(company_Name))
            {
                int feed_Count = 0;

                MySqlConnection connection = new MySqlConnection(connection_String);
                foreach (User_Detail temp_User_Detail in get_Users_Details(company_Name).Values)
                {
                    query = "SELECT COUNT(*) FROM main_Data WHERE interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                    
                    if (Open_This_Mysql_Connection(ref connection))
                    {
                        cmd = new MySqlCommand(query, connection);
                        feed_Count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));

                        Close_This_Mysql_Connection(ref connection);
                    }
                }

                query = "SELECT COUNT(*) FROM main_Data WHERE filter_Result=1 AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                
                int filtered_Count = 0;

                
                if (Open_This_Mysql_Connection(ref connection))
                {
                    cmd = new MySqlCommand(query, connection);
                    filtered_Count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));

                    Close_This_Mysql_Connection(ref connection);
                }

                query = "SELECT COUNT(*) FROM main_Data WHERE filter_Result=0 AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

                int feeded_But_Not_Filtered_Count = 0;
                if (Open_This_Mysql_Connection(ref connection))
                {
                    cmd = new MySqlCommand(query, connection);
                    feeded_But_Not_Filtered_Count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));

                    Close_This_Mysql_Connection(ref connection);
                }

                query = "SELECT COUNT(*) FROM main_Data WHERE call_Result=1 AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                int qualified_Count = 0;
                if (Open_This_Mysql_Connection(ref connection))
                {
                    cmd = new MySqlCommand(query, connection);
                    qualified_Count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));

                    Close_This_Mysql_Connection(ref connection);
                }

                query = "SELECT COUNT(*) FROM main_Data WHERE call_Result=0 AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                int filtered_But_Not_Qualified_Count = 0;
                if (Open_This_Mysql_Connection(ref connection))
                {
                    cmd = new MySqlCommand(query, connection);
                    filtered_But_Not_Qualified_Count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));

                    Close_This_Mysql_Connection(ref connection);
                }

                query = "SELECT * FROM interview_Names WHERE interview_Name =" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                DateTime temp_Date = new DateTime(1,1,1) ;
                string coordinator = "";
                string venue = "";
                decimal payment = 0;
                bool settled = false;
                if(Open_This_Mysql_Connection(ref connection))
                {
                    cmd = new MySqlCommand(query,connection);
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader["date"] != DBNull.Value)
                            temp_Date = (DateTime)(reader["date"]);
                        if (reader["coordinator"] != DBNull.Value)
                            coordinator = (string)(reader["coordinator"]);
                        if (reader["venue"] != DBNull.Value)
                            venue = (string)(reader["venue"]);
                        if (reader["payment"] != DBNull.Value)
                            payment = Convert.ToDecimal(reader["payment"]);
                        if (reader["settled"] != DBNull.Value)
                            settled = Convert.ToBoolean(reader["settled"]);
                    }

                    //if(temp_String != "")
                    //    DateTime.TryParseExact(temp_String,"yyyy-MM-dd HH:mm",new System.Globalization.CultureInfo("en-US"),System.Globalization.DateTimeStyles.None,out temp_Date);

                    Close_This_Mysql_Connection(ref connection);
                }

                if (!reader.IsClosed)
                    reader.Close();

                int attended_Count = 0;
                query = "SELECT COUNT(*) FROM main_Data WHERE attended_Status=1 AND interview_Name =" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                if(Open_This_Mysql_Connection(ref connection))
                {
                    cmd = new MySqlCommand(query,connection);
                    attended_Count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));
                    Close_This_Mysql_Connection(ref connection);
                }

                int qualified_But_Not_Attended_Count = 0;
                query = "SELECT COUNT(*) FROM main_Data WHERE attended_Status=0 AND interview_Name =" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                if (Open_This_Mysql_Connection(ref connection))
                {
                    cmd = new MySqlCommand(query, connection);
                    qualified_But_Not_Attended_Count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));
                    Close_This_Mysql_Connection(ref connection);
                }

                int selected_Count = 0;
                query = "SELECT COUNT(*) FROM main_Data WHERE selected_Status=1 AND interview_Name =" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                if(Open_This_Mysql_Connection(ref connection))
                {
                    cmd = new MySqlCommand(query,connection);
                    selected_Count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));
                    Close_This_Mysql_Connection(ref connection);
                }

                int attended_But_Not_Selected_Count = 0;
                query = "SELECT COUNT(*) FROM main_Data WHERE selected_Status=0 AND interview_Name =" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                if (Open_This_Mysql_Connection(ref connection))
                {
                    cmd = new MySqlCommand(query, connection);
                    attended_But_Not_Selected_Count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));
                    Close_This_Mysql_Connection(ref connection);
                }

                int joined_Count = 0;
                query = "SELECT COUNT(*) FROM main_Data WHERE joined_Status=1 AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                if(Open_This_Mysql_Connection(ref connection))
                {
                    cmd = new MySqlCommand(query,connection);
                    joined_Count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));
                    Close_This_Mysql_Connection(ref connection);
                }

                int selected_But_Not_Joined_Count = 0;
                query = "SELECT COUNT(*) FROM main_Data WHERE joined_Status=0 AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";
                if (Open_This_Mysql_Connection(ref connection))
                {
                    cmd = new MySqlCommand(query, connection);
                    selected_But_Not_Joined_Count = int.Parse(Convert.ToString(cmd.ExecuteScalar()));
                    Close_This_Mysql_Connection(ref connection);
                }

                temp.Add(interview_Name, new Interview_And_Candidates_Count_Detail(interview_Name,feed_Count ,filtered_Count,feeded_But_Not_Filtered_Count, qualified_Count,filtered_But_Not_Qualified_Count,temp_Date,coordinator,venue, attended_Count,qualified_But_Not_Attended_Count,selected_Count,attended_But_Not_Selected_Count,joined_Count,selected_But_Not_Joined_Count,payment,settled));
            }

            return temp;
        }

        public System.Collections.Generic.Dictionary<string, int> get_Filtered_Candidates_Counts_Details_As_Network_Count_Pairs_For_This_Caller(string caller, string interview_Name,string company_Name)
        {
            //string query;
            //if(caller == "Admin")
            //    query = "SELECT phoneNo FROM tempdata WHERE filter_Result=1 And (caller IS NULL)";
            //else

            string query = "SELECT phoneNo FROM main_Data WHERE filter_Result=1 And (caller IS NULL OR caller=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(caller) + " AND call_Result IS NULL) " + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            System.Collections.ObjectModel.Collection<string> temp_Phone_Nos = new System.Collections.ObjectModel.Collection<string>();

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["phoneNo"] == DBNull.Value)
                        temp_Phone_Nos.Add("");
                    else
                        temp_Phone_Nos.Add((string)(reader["phoneNo"]));
                }
                Close_This_Mysql_Connection(ref connection);
            }

            System.Collections.Generic.Dictionary<string, int> counts = new Dictionary<string, int>(); //Dictionary<network_Short_Form,count>
            foreach (string temp_Phone_No in temp_Phone_Nos)
            {
                string network_Short_Form = get_Network_For_This_No(temp_Phone_No);
                if (counts.ContainsKey(network_Short_Form))
                    counts[network_Short_Form] += 1;
                else
                    counts.Add(network_Short_Form, 1);
            }

            return counts;
        }

        public System.Collections.Generic.Dictionary<string, int> get_Qualified_Candidates_Counts_Details(string interview_Name,string company_Name)
        {
            string query = "SELECT phoneNo FROM main_Data WHERE call_Result=1 AND attended_Status IS NULL" + " AND interview_Name=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(interview_Name) + " AND company_Name='" + company_Name + "'";

            System.Collections.ObjectModel.Collection<string> temp_Phone_Nos = new System.Collections.ObjectModel.Collection<string>();

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection) == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["phoneNo"] == DBNull.Value)
                        temp_Phone_Nos.Add("");
                    else
                        temp_Phone_Nos.Add((string)(reader["phoneNo"]));
                }
                Close_This_Mysql_Connection(ref connection);
            }

            System.Collections.Generic.Dictionary<string, int> counts = new Dictionary<string, int>(); //Dictionary<network_Short_Form,count>
            foreach (string temp in temp_Phone_Nos)
            {
                string temp_Phone_No = temp;
                if (temp_Phone_No.Contains(","))
                    temp_Phone_No = temp_Phone_No.Substring(0, temp_Phone_No.IndexOf(","));
                string network_Short_Form = get_Network_For_This_No(temp_Phone_No);
                if (counts.ContainsKey(network_Short_Form))
                    counts[network_Short_Form] += 1;
                else
                    counts.Add(network_Short_Form, 1);
            }

            return counts;
        }

        public string get_WebPage_Base_Address_For_This_Company(string company_Name)
        {
            string webPage_Base_Address = "";
            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                string query = "SELECT webPage_Base_Address FROM company_Info WHERE company_Name ='"+company_Name+"'";

                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["webPage_Base_Address"] != DBNull.Value)
                        webPage_Base_Address = (string)(reader["webPage_Base_Address"]);
                }
                Close_This_Mysql_Connection(ref connection);
            }
            return webPage_Base_Address;
        }
//        public static string add_Double_Quotes_To_Comma_Separated_String(string given_String)
//        {
//            string divider = ",";
//            string[] temps = given_String.Split(divider.ToCharArray());
            
//            for(int i=0;i<temps.Length;++i)
//            {
//                temps[i] = Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(temps[i]);
//            }

//            return String.Join(",", temps);
//        }

        public string get_Network_Details(string given_No)
        {
            if (given_No.Length < 10)
                return "no:" + given_No + ";" + "network:" + "" + ";" + "telecom_Circle:" + "";
            string key_No = given_No.Substring(given_No.Length - 10);
            string query = "";

            key_No = key_No.Substring(0, 4);
            query = "SELECT * FROM telephonenos WHERE number='" + key_No + "'";
            string network = "";
            string telecom_Circle = "";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                string network_Shorform = "";
                string place_Shortform = "";
                while (reader.Read())
                {
                    network_Shorform = (string)(reader["network"]);
                    place_Shortform = (string)(reader["place"]);
                }

                if (!reader.IsClosed)
                    reader.Close();

                query = "SELECT network FROM networks WHERE shortname='" + network_Shorform + "'";
                cmd = new MySqlCommand(query, connection);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    network = (string)(reader["network"]);
                }

                if (!reader.IsClosed)
                    reader.Close();

                query = "SELECT telecom_Circle FROM telecom_Circles WHERE shortname='" + place_Shortform + "'";

                cmd = new MySqlCommand(query, connection);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    telecom_Circle = (string)(reader["telecom_Circle"]);
                }

                if (!reader.IsClosed)
                    reader.Close();

                Close_This_Mysql_Connection(ref connection);
            }
            
            return "no:" + given_No + ";" + "network:" + network + ";" + "telecom_Circle:" + telecom_Circle;
        }

        public string get_Network_For_This_No(string given_No)
        {
            if (given_No.Length < 10)
                return "";
            string key_No = given_No.Substring(given_No.Length - 10);
            string query = "";

            key_No = key_No.Substring(0, 4);
            query = "SELECT * FROM telephonenos WHERE number='" + key_No + "'";

            string network = "";

            MySqlConnection connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                string network_Shortform = "";
                while (reader.Read())
                {
                    if (reader["network"] == DBNull.Value)
                        network_Shortform = "";
                    else
                        network_Shortform = (string)(reader["network"]);
                }

                if (!reader.IsClosed)
                    reader.Close();

                query = "SELECT network FROM networks WHERE shortname='" + network_Shortform + "'";
                cmd = new MySqlCommand(query, connection);
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    network = (string)(reader["network"]);
                }

                if (!reader.IsClosed)
                    reader.Close();

                Close_This_Mysql_Connection(ref connection);
            }

            return network;
        }

//      /*  static void Main(string[] args)
//        {
//            try
//            {
//                DBConnect temp_Connection = new DBConnect();
        
             
//               if (temp_Connection.Check_For_Table("userinfo") == 0)
//                {
//                    temp_Connection.Create_UserData_Table(); 
//                }
//                if (temp_Connection.Is_This_User_Name_Exist("user") == 0)
//                {
//                    temp_Connection.Add_User_Info("user", "password");
//                }
//                if (temp_Connection.Is_This_User_Name_Exist("root") == 1)
//                {
//                    temp_Connection.Delete_User_Info("root");
//                }
//                if (temp_Connection.Is_This_User_Name_Exist("user") == 1)
//                {
//                    temp_Connection.Update_User_Info("user", "password");
//                }
//                if (temp_Connection.Check_For_Table("userinfo") == 1)
//                {
//                    temp_Connection.Delete_UserData_Table();
//                }
//                if (temp_Connection.Check_For_Table("userinfo") == 1)
//                {
//                    if (temp_Connection.Check_User_Account("user", "password") == 1)
//                    {
//                        Console.WriteLine("User Exists");
//                    }
//                }
              
           
//               if (temp_Connection.Check_For_Table("tempdata") == 0)
//                {
//                    temp_Connection.Create_TempData_Table();
//                }

//                if (temp_Connection.Is_This_Url_Exist("url") == 0)
//                {
//                    temp_Connection.Add_URL_Info_From_Feeder("feeder","url","filterer");
//                }
//                if (temp_Connection.Is_This_Url_Exist("url") == 1)
//                {
//                    temp_Connection.Update_Filter_Result("url","filter", 1);
//                }

//                if (temp_Connection.Is_This_Url_Exist("url") == 1)
//                {
//                    temp_Connection.Update_Call_Result("url", "filter", -1);
//                }

//                if (temp_Connection.Is_This_Url_Exist("url") == 1)
//                {
//                    temp_Connection.Delete_Entry("url");
//                }

//                if (temp_Connection.Check_For_Table("tempdata") == 1)
//                {
//                    temp_Connection.Delete_TempData_Table();
//                }
//            }
//            catch (MySqlException exception)
//            {
//                Console.WriteLine(exception.Message);
//                string a =Console.ReadLine();
//            }
//            string b = Console.ReadLine();
//        }*/
    }
}

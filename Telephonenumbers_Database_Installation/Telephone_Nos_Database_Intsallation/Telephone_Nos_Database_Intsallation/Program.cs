using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Telephone_Nos_Database_Intsallation
{
    class Program
    {
        public static string Surround_It_With_Double_Quote(string given_String)
        {
            return ("\"" + given_String + "\"");
        }

        public static string add_Double_Quotes_To_Comma_Separated_String(string given_String)
        {
            string divider = ",";
            string[] temps = given_String.Split(divider.ToCharArray());

            for (int i = 0; i < temps.Length; ++i)
            {
                temps[i] = Surround_It_With_Double_Quote(temps[i]);
            }

            return String.Join(",", temps);
        }

        public static void Create_Cell_Number_Database()
        {
            MySqlConnection connection;

            string server ="";// = "localhost";
            string database = "";// = "connectcsharptomysql";
            string uid = "";// = "root";//"username";
            string password = "";//= "saradhabai"; //"password";

            System.IO.StreamReader temp_Reader = new System.IO.StreamReader("settings.txt");

            while (!temp_Reader.EndOfStream)
            {
                string content = temp_Reader.ReadLine();

                string type = content.Substring(0, content.IndexOf(":"));
                string data = content.Substring(content.IndexOf(":")+1);

                switch(type)
                {
                    case "server":
                        server = data;
                        break;
                    case "database":
                        database = data;
                        break;
                    case "uid":
                        uid = data;
                        break;
                    case "password":
                        password = data;
                        break;
                    default:
                        Console.WriteLine("Unexpected switch case,exiting");
                        break;
                }
            }

            Console.WriteLine("Table telephonenos is going to be created");
            string connection_String = "SERVER=" + server + ";" + "DATABASE=" + database + ";" +
                                        "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            connection = new MySqlConnection(connection_String);
            try
            {
                connection.Open();

                string query = "";
                MySqlCommand cmd;
                System.IO.StreamReader reader;

                query = "CREATE TABLE telephonenos (number VARCHAR(30) UNIQUE,network VARCHAR(50),place VARCHAR(6))";
                cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                reader = new System.IO.StreamReader("telephonenos.txt");
                while (!reader.EndOfStream)
                {
                    string content = reader.ReadLine();

                    query = "INSERT INTO telephonenos (number,network,place) VALUES(" + add_Double_Quotes_To_Comma_Separated_String(content) + ")";

                    cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Data loaded in telephonenos");
                reader.Close();

                query = "CREATE TABLE networks (shortname VARCHAR(3) UNIQUE,network VARCHAR(20),corporation VARCHAR(50))";
                cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                reader = new System.IO.StreamReader("networks.txt");
                while (!reader.EndOfStream)
                {
                    string content = reader.ReadLine();
                    //Console.WriteLine(content);
                    query = "INSERT INTO networks (shortname,network,corporation) VALUES(" + add_Double_Quotes_To_Comma_Separated_String(content) + ")";
                    cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Data loaded in networks");

                query = "CREATE TABLE telecom_Circles (shortname VARCHAR(2) UNIQUE,telecom_Circle VARCHAR(140))";
                cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();

                reader = new System.IO.StreamReader("telecom_Circles.txt");
                while (!reader.EndOfStream)
                {
                    string content = reader.ReadLine();
                    string shortname = content.Substring(0, content.IndexOf(","));
                    string telecom_Circle = content.Substring(content.IndexOf(",") + 1);
                    query = "INSERT INTO telecom_Circles (shortname,telecom_Circle) VALUES(" + Surround_It_With_Double_Quote(shortname) + "," + Surround_It_With_Double_Quote(telecom_Circle) + ")";
                    //Console.WriteLine(query);
                    cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Data loaded in telecom_Circles");
                connection.Close();
            }

            catch (MySqlException exception)
            {
                Console.WriteLine("Cannot connect to the Mysql server:" + exception.Message);
                switch (exception.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to the server.Contact Administrator");
                        break;
                    case 1045:
                        Console.WriteLine("Invalid Username/Password");
                        break;
                }
            }
        }

        public static bool Is_MySQL_Service_Exist()
        {
            foreach (System.ServiceProcess.ServiceController temp_Service in System.ServiceProcess.ServiceController.GetServices())
                if (temp_Service.ServiceName == "MySQL")
                    return true;

            return false;
        }
        public static void Start_MySQL_Service()
        {
            System.ServiceProcess.ServiceController mysql_Service = new System.ServiceProcess.ServiceController("MySQL");
            if (mysql_Service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                mysql_Service.Start();
        }
        public static bool is_MySQL_Running()
        {
            System.ServiceProcess.ServiceController mysql_Service = new System.ServiceProcess.ServiceController("MySQL");
            if (mysql_Service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                return true;
            return false;
        }
        static void Main(string[] args)
        {
            /*if (Is_MySQL_Service_Exist())
            {
                while (!is_MySQL_Running())
                {
                    Start_MySQL_Service();
                }
            }*/
            Create_Cell_Number_Database();
            string stopper = Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

using MySql_CSharp_Console;

using Common_Classes;

namespace ResumeView_Server_Controller
{
    public partial class Form_ResumeView_Server_Controller : Form
    {
        DBConnect db_Connect;
        string image_Path;

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


        Useful_Functions.SimpleAES crypting_Object;

        AutoCompleteStringCollection company_Names;

        public Form_ResumeView_Server_Controller()
        {
            InitializeComponent();
            image_Path = "";

            crypting_Object = new Useful_Functions.SimpleAES();
            initialize_FTP_Settings();
            db_Connect = new DBConnect(database_Server, database_Name, database_Uid, database_Password);

            company_Names = new AutoCompleteStringCollection();
            textBox_Company_Name.AutoCompleteCustomSource = company_Names;
            load_The_Company_Names();
        }

        void load_The_Company_Names()
        {
            company_Names.Clear();
            List<string> company_Names_List = db_Connect.get_Company_Names();
            company_Names.AddRange(company_Names_List.ToArray());
        }

        private string create_This_Folder_Through_FTP_And_Return_WebPage_Base_Address(string folder_Name)
        {
            string uri = String.Format("ftp://{0}/MyWeb/{1}", ftp_Server, folder_Name);

            WebRequest request = WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(ftp_Username, ftp_Password);
            try
            {
                FtpWebResponse response = (FtpWebResponse)(request.GetResponse());
            }
            catch (WebException excep)
            {
                string status_Desc = excep.Message;
                if (!System.Text.RegularExpressions.Regex.IsMatch(status_Desc, "^2[0-9]{2}"))
                {
                    MessageBox.Show("Server is not allowing to create the folder, Aborting");
                    return "";
                }
            }
            
            return uri.Substring(uri.IndexOf("://")+"://".Length);
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            if (textBox_Company_Name.Text == "")
            {
                MessageBox.Show("Company Name can't be empty");
                return;
            }

            string web_Page_Base_Address = create_This_Folder_Through_FTP_And_Return_WebPage_Base_Address(textBox_Company_Name.Text);
            if (web_Page_Base_Address == "")
            {
                MessageBox.Show("Ftp server is not allowing to create a folder,Aborting");
                return;
            }

            if (create_This_Folder_Through_FTP_And_Return_WebPage_Base_Address(textBox_Company_Name.Text + "/LocalDatabase") != "")
            {
                create_This_Folder_Through_FTP_And_Return_WebPage_Base_Address(textBox_Company_Name.Text + "/LocalDatabase/DOC");
                create_This_Folder_Through_FTP_And_Return_WebPage_Base_Address(textBox_Company_Name.Text + "/LocalDatabase/PDF");
                create_This_Folder_Through_FTP_And_Return_WebPage_Base_Address(textBox_Company_Name.Text + "/LocalDatabase/HTML");
            }
            create_This_Folder_Through_FTP_And_Return_WebPage_Base_Address(textBox_Company_Name.Text + "/Photos");

            string ftp_Home_Folder_Path = "C:\\\\Inetpub\\\\wwwroot\\\\MyWeb\\\\"+textBox_Company_Name.Text;

            string file_Name = "message_"+Useful_Functions.Useful_Functions.get_Guid_String();
            string file_Type ="";
            if(image_Path != "")
                file_Type = image_Path.Substring(image_Path.LastIndexOf("."));
            file_Name += file_Type;
            string image_Web_Path = "";
            if(image_Path != "")
                image_Web_Path = upload_The_File_Return_Web_Address(file_Name, image_Path);

            db_Connect.add_Company_Info(textBox_Company_Name.Text,
                web_Page_Base_Address+"/",
                dateTimePicker_Expiry_Date.Value,
                textBox_Message.Text,
                image_Web_Path,
                Convert.ToInt32(numericUpDown_SMS_Count.Value),
                ftp_Home_Folder_Path
                );

            db_Connect.Add_User_Info("Admin",
                                     crypting_Object.EncryptToString("admin"),
                                    "0000000000",
                                    "Management",
                                    textBox_Company_Name.Text,
                                    image_Path,
                                    "mail Id");

            // This will create client side precursor file
            string temp_Content = System.IO.File.ReadAllText("Program.cs");
            temp_Content = temp_Content.Replace("\"imiko\"", "\"" + textBox_Company_Name.Text + "\"");
            System.IO.File.WriteAllText("Program.cs", temp_Content);

            load_The_Company_Names();
            clear_Fields(true);

            MessageBox.Show("Company Got Added Successfully");
        }

        string upload_The_File_Return_Web_Address(string server_Side_File_Name, string local_File_Path) // Returns the web address of the image
        {
            string uri = "ftp://" +ftp_Server+"/MyWeb/"+textBox_Company_Name.Text +"/Photos/"+server_Side_File_Name;
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
            return uri.Replace("ftp://","http://");
        }

        private void initialize_FTP_Settings()
        {
#if LOG
            log_Queue.Enqueue("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
#endif

            Microsoft.Win32.RegistryKey temp = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey temp_0 = temp.OpenSubKey("SOFTWARE", true);
            if (!temp_0.GetSubKeyNames().Contains("Resume View"))
                return;

            Microsoft.Win32.RegistryKey resume_View_Key = temp_0.OpenSubKey("Resume View", true);

            ftp_Server = (string)(resume_View_Key.GetValue("ftp_Server"));
            ftp_Server = crypting_Object.DecryptString(ftp_Server);
            ftp_Username = (string)(resume_View_Key.GetValue("ftp_Username"));
            ftp_Username = crypting_Object.DecryptString(ftp_Username);
            ftp_Password = (string)(resume_View_Key.GetValue("ftp_Password"));
            ftp_Password = crypting_Object.DecryptString(ftp_Password);
        }

        private void pictureBox_Message_Image_Click(object sender, EventArgs e)
        {
            OpenFileDialog file_Dialog = new OpenFileDialog();
            //file_Dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            file_Dialog.Filter = "Image Files(*.Bmp;*.Emf;*.Exif;*.Gif;*.Guid;*.Icon;*.Jpeg;*.Jpg;*.MemoryBmp;*.Png;*.Tiff;*.Wmf)|*.BMP;*.EMF;*.EXIF;*.GIF;*.GUID;*.ICON;*.JPEG;*.JPG;*.MEMORYBMP;*.PNG;*.TIFF;*.WMF";
            file_Dialog.ShowDialog();
            if (file_Dialog.FileName == "")
                return;
            image_Path = file_Dialog.FileName;
            pictureBox_Message_Image.Image = System.Drawing.Image.FromFile(image_Path);
        }

        private void clear_Fields(bool clear_Name_Too)
        {
            if(clear_Name_Too == true)
                textBox_Company_Name.Text = "";
            dateTimePicker_Expiry_Date.Value = DateTime.Now;
            numericUpDown_SMS_Count.Value = 0;
            textBox_Message.Text = "";
            pictureBox_Message_Image.Image = null;
            image_Path = "";
        }

        private void textBox_Company_Name_TextChanged(object sender, EventArgs e)
        {
            if (textBox_Company_Name.Text.Contains(","))
            {
                MessageBox.Show("Comma is not allowed");
                textBox_Company_Name.Text = textBox_Company_Name.Text.Replace(",", "");
            }

            if (textBox_Company_Name.Text.Contains(":"))
            {
                MessageBox.Show("Colon is not allowed");
                textBox_Company_Name.Text = textBox_Company_Name.Text.Replace(":", "");
            }

            if (textBox_Company_Name.Text.Contains(" "))
            {
                MessageBox.Show("Space is not allowed");
                textBox_Company_Name.Text = textBox_Company_Name.Text.Replace(" ", "");
            }

            button_Add.Enabled = !(company_Names.Contains(textBox_Company_Name.Text));
            button_Update.Enabled = (company_Names.Contains(textBox_Company_Name.Text));

            if (textBox_Company_Name.Text == "")
            {
                button_Add.Enabled = false;
                button_Update.Enabled = false;
            }

            if (!(company_Names.Contains(textBox_Company_Name.Text)))
            {
                clear_Fields(false);
            }
            else
            {
                Company_Info temp_Company_Info = db_Connect.get_Company_Info_For_This_Company_Name(textBox_Company_Name.Text);
                load_This_Company_Info(temp_Company_Info);
            }
        }

        void load_This_Company_Info(Company_Info given_Company_Info)
        {
            textBox_Company_Name.Text = given_Company_Info.company_Name;
            dateTimePicker_Expiry_Date.Value = given_Company_Info.expiry_Date;
            numericUpDown_SMS_Count.Value = given_Company_Info.sms_Count;
            textBox_Message.Text = given_Company_Info.message;
            if (given_Company_Info.message_Image_Path != "")
                pictureBox_Message_Image.Load(given_Company_Info.message_Image_Path);
            else
                pictureBox_Message_Image.Image = null;
            image_Path = given_Company_Info.message_Image_Path;
        }

        private void button_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_Update_Click(object sender, EventArgs e)
        {
            string ftp_Home_Folder_Path = db_Connect.get_WebPage_Base_Address_For_This_Company(textBox_Company_Name.Text);
            
            string file_Name = "message_" + Useful_Functions.Useful_Functions.get_Guid_String();
            string file_Type = "";
            if (image_Path != "")
                file_Type = image_Path.Substring(image_Path.LastIndexOf("."));
            file_Name += file_Type;
            string image_Web_Path = image_Path;
            Company_Info temp_Company_Info = db_Connect.get_Company_Info_For_This_Company_Name(textBox_Company_Name.Text);
            if ((image_Path != "") && (temp_Company_Info.message_Image_Path != image_Path))
                image_Web_Path = upload_The_File_Return_Web_Address(file_Name, image_Path);

            string web_Page_Base_Address = db_Connect.get_WebPage_Base_Address_For_This_Company(textBox_Company_Name.Text);
            db_Connect.update_Company_Info(textBox_Company_Name.Text,
                web_Page_Base_Address + "/",
                dateTimePicker_Expiry_Date.Value,
                textBox_Message.Text,
                image_Web_Path,
                Convert.ToInt32(numericUpDown_SMS_Count.Value),
                ftp_Home_Folder_Path
                );

            clear_Fields(true);
            MessageBox.Show("Company Got Updated Successfully");
        }
    }
}

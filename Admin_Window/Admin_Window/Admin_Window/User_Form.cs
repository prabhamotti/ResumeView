using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MySql_CSharp_Console;
using Common_Classes;

namespace Admin_Window
{
    public partial class User_Form : Form
    {
        public User_Detail user_Detail;
        Useful_Functions.SimpleAES crypting_Object;
        public User_Form(string given_Action,User_Detail given_User_Detail)
        {
            InitializeComponent();
            crypting_Object = new Useful_Functions.SimpleAES();
            button_Add_Or_Update_User_Form.Text = given_Action;
            if (given_Action == "Update")
                textBox_Name_User_Form.ReadOnly = true;
            user_Detail = given_User_Detail;

            if (user_Detail != null)
            {
                textBox_Name_User_Form.Text = user_Detail.username;
                textBox_Password_User_Form.Text = crypting_Object.DecryptString(user_Detail.encrypted_General_Password);
                textBox_Email_Id_User_Form.Text = user_Detail.mail_Id;

                textBox_Phone_No_User_Form.Text = user_Detail.phone_No;
                comboBox_Type_User_Form.Text = user_Detail.type;
                pictureBox_User_Form.Load(user_Detail.image_Path);
                textBox_Browse_User_Form.Text = user_Detail.image_Path;
            }
        }

        private void button_Browse_User_Form_Click(object sender, EventArgs e)
        {
            OpenFileDialog file_Dialog = new OpenFileDialog();
            //file_Dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            file_Dialog.Filter = "Image Files(*.Bmp;*.Emf;*.Exif;*.Gif;*.Guid;*.Icon;*.Jpeg;*.Jpg;*.MemoryBmp;*.Png;*.Tiff;*.Wmf)|*.BMP;*.EMF;*.EXIF;*.GIF;*.GUID;*.ICON;*.JPEG;*.JPG;*.MEMORYBMP;*.PNG;*.TIFF;*.WMF";
            file_Dialog.ShowDialog();
            string file_Path = file_Dialog.FileName;
            if (file_Path == "")
                return;
            pictureBox_User_Form.Image = System.Drawing.Image.FromFile(file_Path);
            textBox_Browse_User_Form.Text = file_Path;
        }

        private void button_Cancel_User_Form_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        public bool check_Form(ref string message)
        {
            if(textBox_Name_User_Form.Text == "")
            {
                message = "Name Field Can't be Empty";
                return false;
            }
            if(textBox_Password_User_Form.Text == "")
            {
                message = "Password Field Can't be Empty";
                return false;
            }
            if(textBox_Phone_No_User_Form.Text == "")
            {
                message = "Phone No Field Can't be Empty";
                return false;
            }
            if(comboBox_Type_User_Form.Text == "")
            {
                message = "Type Field Can't be Unselected";
                return false;
            }
            if(textBox_Email_Id_User_Form.Text == "")
            {
                message = "Mail Id Field Can't be Empty";
                return false;
            }
            if(textBox_Browse_User_Form.Text == "")
            {
                message = "Photo can't be empty";
                return false;
            }
            return true;
        }

    }
}

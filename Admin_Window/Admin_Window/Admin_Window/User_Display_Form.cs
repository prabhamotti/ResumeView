using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Common_Classes;

namespace Admin_Window
{
    public partial class User_Display_Form : Form
    {
        Useful_Functions.SimpleAES crypting_Object;
        public User_Display_Form()
        {
            InitializeComponent();
            crypting_Object = new Useful_Functions.SimpleAES();
        }

        public void update(User_Detail given_User_Detail)
        {
            this.Text = given_User_Detail.username;
            label_Name_User_Display_Form.Text = given_User_Detail.username;
            label_Password_User_Display_Form.Text = crypting_Object.DecryptString(given_User_Detail.encrypted_General_Password);
            label_Phone_No_User_Display_Form.Text = given_User_Detail.phone_No;
            label_Email_Id_User_Display_Form.Text = given_User_Detail.mail_Id;
            label_Type_User_Display_Form.Text = given_User_Detail.type;
            if (given_User_Detail.image_Path != "")
                pictureBox_User_Form.Load(given_User_Detail.image_Path);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MySql_CSharp_Console;

namespace Reminder
{
    public partial class Reminder_Creator_Form : Form
    {
        public string image_Path;
        ToolTip temp_Tool_Tip;
        DBConnect db_Connect;

        public Reminder_Creator_Form(string action,string database_Server,string database_Name,string database_Uid,string database_Password)
        {
            InitializeComponent();

            db_Connect = new DBConnect(database_Server,database_Name,database_Uid,database_Password);
            button_Add_Or_Update_Reminder_Creator_Form.Text = action;

            temp_Tool_Tip = new ToolTip();

            textBox_Subject_Reminder_Creator_Form.MouseHover += new EventHandler(textBox_Subject_Reminder_Creator_Form_MouseHover);
            textBox_Subject_Reminder_Creator_Form.MouseLeave += new EventHandler(textBox_Subject_Reminder_Creator_Form_MouseLeave);

            textBox_Body_Reminder_Creator_Form.MouseHover += new EventHandler(textBox_Body_Reminder_Creator_Form_MouseHover);
            textBox_Body_Reminder_Creator_Form.MouseLeave += new EventHandler(textBox_Body_Reminder_Creator_Form_MouseLeave);

            textBox_Notes_Reminder_Creator_Form.MouseHover += new EventHandler(textBox_Notes_Reminder_Creator_Form_MouseHover);
            textBox_Notes_Reminder_Creator_Form.MouseLeave += new EventHandler(textBox_Notes_Reminder_Creator_Form_MouseLeave);

            image_Path = Form_Reminder.application_Dir+"\\I_Forgot.jpg";
            pictureBox_Picture_Reminder_Creator_Form.Image = Form_Reminder.i_Forgot_Jpg;
        }



        void textBox_Subject_Reminder_Creator_Form_MouseLeave(object sender, EventArgs e)
        {
            temp_Tool_Tip.Hide(this);
        }

        void textBox_Subject_Reminder_Creator_Form_MouseHover(object sender, EventArgs e)
        {
            temp_Tool_Tip.Show("Subject", this, this.PointToClient(new Point(Control.MousePosition.X + 10, Control.MousePosition.Y + 30)));
        }

        void textBox_Body_Reminder_Creator_Form_MouseLeave(object sender, EventArgs e)
        {
            temp_Tool_Tip.Hide(this);
        }

        void textBox_Body_Reminder_Creator_Form_MouseHover(object sender, EventArgs e)
        {
            temp_Tool_Tip.Show("Body", this, this.PointToClient(new Point(Control.MousePosition.X + 10, Control.MousePosition.Y + 30)));
        }

        void textBox_Notes_Reminder_Creator_Form_MouseLeave(object sender, EventArgs e)
        {
            temp_Tool_Tip.Hide(this);
        }

        void textBox_Notes_Reminder_Creator_Form_MouseHover(object sender, EventArgs e)
        {
            temp_Tool_Tip.Show("Notes", this, this.PointToClient(new Point(Control.MousePosition.X + 10, Control.MousePosition.Y + 30)));
        }

        private void pictureBox_Picture_Click(object sender, EventArgs e)
        {
            OpenFileDialog file_Dialog = new OpenFileDialog();
            //file_Dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            file_Dialog.Filter = "Image Files(*.Bmp;*.Emf;*.Exif;*.Gif;*.Guid;*.Icon;*.Jpeg;*.Jpg;*.MemoryBmp;*.Png;*.Tiff;*.Wmf)|*.BMP;*.EMF;*.EXIF;*.GIF;*.GUID;*.ICON;*.JPEG;*.JPG;*.MEMORYBMP;*.PNG;*.TIFF;*.WMF";
            file_Dialog.ShowDialog();
            image_Path = file_Dialog.FileName;
            if (image_Path == "")
                return;
            pictureBox_Picture_Reminder_Creator_Form.Image = System.Drawing.Image.FromFile(image_Path);
            //textBox_Browse_User_Form.Text = file_Path;
        }

        private void checkBox_Notes_Remainder_Creator_Form_CheckedChanged(object sender, EventArgs e)
        {
            textBox_Notes_Reminder_Creator_Form.Enabled = checkBox_Notes_Reminder_Creator_Form.Checked;
        }

        public bool check_Form(ref string message)
        {
            if (textBox_Subject_Reminder_Creator_Form.Text == "")
            {
                message = "Subject can't be empty";
                return false;
            }

            if (textBox_Body_Reminder_Creator_Form.Text == "")
            {
                message = "Body can't be empty";
                return false;
            }

            if (dateTimePicker_DeadLine_Reminder_Creator_Form.Value == dateTimePicker_Start_Reminder_Creator_Form.Value)
            {
                message = "It seems you forgot to fix the deadline";
                return false;
            }

            if ((radioButton_Hour_Reminder_Creator_Form.Checked == true) && (numericUpDown_Reminder_Creator_Form.Value <1))
            {
                message = "You forgot to mention the hour for reminding";
                return false;
            }

            if ((radioButton_Minute_Reminder_Creator_Form.Checked == true) && (numericUpDown_Reminder_Creator_Form.Value < 1))
            {
                message = "It is not allowed to enter a value below 15 to mention the minutes for reminding";
                return false;
            }

            return true;
        }

        private void button_Cancel_Reminder_Creator_Form_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void checkBox_Start_Reminder_Creator_Form_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker_Start_Reminder_Creator_Form.Enabled = checkBox_Start_Reminder_Creator_Form.Checked;
        }
    }
}
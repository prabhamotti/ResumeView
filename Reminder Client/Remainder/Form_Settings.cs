using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reminder
{
    public partial class Form_Settings : Form
    {
        public Form_Settings(string mail_Account_Type,string mail_Id,string mail_Password,string general_Password)
        {
            InitializeComponent();
            comboBox_Mail_Account.SelectedText = "Gmail";
            textBox_Mail_Id.Text = mail_Id;
            textBox_Mail_Password.Text = mail_Password;
            textBox_General_Password.Text = general_Password;
        }
    }
}

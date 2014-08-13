using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Admin_Window
{
    public partial class Record : Form
    {
        public string main_Uri;
        public Record()
        {
            InitializeComponent();
            main_Uri = "";
        }

        private void button_Go_Record_Click(object sender, EventArgs e)
        {
            webBrowser_Record.Url = new Uri(textBox_Uri_Record.Text);
        }

    }
}

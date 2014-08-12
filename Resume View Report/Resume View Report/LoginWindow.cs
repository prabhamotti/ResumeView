using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Resume_View_Report
{
    public partial class LoginWindow : Form
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void button_Cancel_Login_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.Dispose();
        }
    }
}

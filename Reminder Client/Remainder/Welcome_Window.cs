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
    public partial class Welcome_Window : Form
    {
        public Welcome_Window(string image_Path,string message)
        {
            InitializeComponent();
            this.pictureBox_Message_Image.Load(image_Path);
        }
    }
}

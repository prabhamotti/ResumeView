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
    public partial class form_Notification : Form
    {
        System.Timers.Timer timer;
        public form_Notification(string subject,string body,DateTime deadline)
        {
            InitializeComponent();
            textBox_Subject.Text = subject;
            textBox_Body.Text = body;
            label_DeadLine.Text = deadline.ToString("MM/dd/yyyy hh:mm:ss");
            this.button_Dismiss.Click += new EventHandler((sender,e) => { this.Close();});

            timer = new System.Timers.Timer();
            timer.Interval = 10000;

            timer.Elapsed += new System.Timers.ElapsedEventHandler((sender, evt) =>
            {
                if (InvokeRequired)
                    this.Invoke(new Action<Size,Point>((given_Size,given_Location) =>
                    {
                        int new_Width = given_Size.Width ;
                        int new_Height = given_Size.Height ;
                        if (given_Size.Width+20 <= 700)
                            new_Width = given_Size.Width+20;
                        if (given_Size.Height+20 <= 400)
                            new_Height = given_Size.Height+20;
                        this.Size = new Size(new_Width, new_Height);

                        int new_X = given_Location.X;
                        int new_Y = given_Location.Y;
                        if(given_Location.X-20 >= 5)
                            new_X = given_Location.X-20;
                        if(given_Location.Y-20 >= 5)
                            new_Y = given_Location.Y-20;
                        this.Location =new Point(new_X,new_Y);
                    }),this.Size,this.Location);
                else
                    this.Size = new Size(this.Size.Width + 10, this.Size.Height + 10);
            });
            timer.Start();
        }
    }
}

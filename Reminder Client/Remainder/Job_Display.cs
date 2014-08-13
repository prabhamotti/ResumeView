using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Common_Classes;

namespace Reminder
{
    public partial class Job_Display_Form : Form
    {
        public Job_Display_Form()
        {
            InitializeComponent();
        }

        public void update(Job given_Job)
        {
            pictureBox_Picture_Job_Display_Form.Load(given_Job.image_Path);
            string temp = given_Job.time_Gap_Of_Minutes_Or_Hours.ToString() +
                (given_Job.time_Gap_Type == Job.Time_Gap_Type.Hour ? "Hr" : "Min");
            label_Reminding_Job_Display_Form.Text = temp;
            dateTimePicker_DeadLine_Job_Display_Form.Value = given_Job.end_Time;
            dateTimePicker_Start_Job_Display_Form.Value = given_Job.start_Time;
            textBox_Subject_Job_Display_Form.Text = given_Job.subject;
            textBox_Body_Job_Display_Form.Text = given_Job.body;
            textBox_Notes_Job_Display_Form.Text = given_Job.notes;

            for (int i = 0; i < given_Job.job_Doiers_Names.Count; ++i)
            {
                ListViewItem temp_ListViewItem = new ListViewItem(given_Job.job_Doiers_Names[i]);
                temp_ListViewItem.SubItems.Add(given_Job.job_Doiers_Mobile_Nos[i]);
                temp_ListViewItem.SubItems.Add(given_Job.job_Doiers_Mail_Ids[i]);
                listView_Receivers_Job_Display_Form.Items.Add(temp_ListViewItem);
            }
                
            if (given_Job.reminder_Types.Contains(Job.Reminder_Type.Computer))
                pictureBox_Computer.Visible = true;
            if (given_Job.reminder_Types.Contains(Job.Reminder_Type.Mail))
                pictureBox_Mail.Visible = true;
            if (given_Job.reminder_Types.Contains(Job.Reminder_Type.SMS))
                pictureBox_SMS.Visible = true;
        }
    }
}

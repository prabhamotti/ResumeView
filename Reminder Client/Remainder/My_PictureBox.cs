using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Common_Classes;

namespace Reminder
{
    class My_PictureBox: System.Windows.Forms.PictureBox
    {
        public Job job{get;private set;}
        System.Drawing.Image image;
        public int width = 100;//483;
        public int height = 100;//300;
        int border_In_Horizontal = 5;//30;
        int border_In_Vertical = 5;//30;
        System.Drawing.Color string_Color;
        Image drawn_Image;
        
        //float internal_Image_Height_Ratio= 0.6f;
        
        public My_PictureBox(Job given_Job):base()
        {
            job = given_Job;
            string temp_Image_Path = given_Job.image_Path.Replace("ftp","http");
            image = Useful_Functions.Useful_Functions.get_Image_From_This_Http_Address(temp_Image_Path);

            ColorConverter con = new ColorConverter();
            string_Color = (Color)con.ConvertFromString("#91b9ff");

            this.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Width = width;
            this.Height = height;
            this.MouseHover += new EventHandler(My_PictureBox_MouseHover);

            initialize_Drawn_Image();

        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs pe)
        {
            var graphics = pe.Graphics;
            graphics.Clear(this.BackColor);

            Image temp_Image = null;
            Job.Status status = job.get_Status(DateTime.Now);
            switch (status)
            {
                case Job.Status.Beyond_The_Limit:
                    temp_Image = Form_Reminder.white_Border;
                    break;
                case Job.Status.End:
                    temp_Image = Form_Reminder.red_Border;
                    break;
                case Job.Status.Middle:
                    temp_Image = Form_Reminder.pink_Border;
                    break;
                case Job.Status.Start:
                    temp_Image = Form_Reminder.green_Border;
                    break;
            }
            graphics.DrawImage(temp_Image, 0, 0, this.Width, this.Height);
            //graphics.DrawImage(image, 2*border_In_Horizontal, 2*border_In_Vertical, this.Width-(4*border_In_Horizontal), (Convert.ToInt32(this.Height * 0.6))-(2*border_In_Vertical));
            graphics.DrawImage(image, 2 * border_In_Horizontal, 2 * border_In_Vertical, this.Width - (4 * border_In_Horizontal), (Convert.ToInt32(this.Height * 0.6)) - (2 * border_In_Vertical));

            System.Drawing.SolidBrush solid_Brush = new SolidBrush(string_Color);
            System.Drawing.Font temp_Font = new System.Drawing.Font("Verdana", 8, FontStyle.Bold);
            System.Drawing.SizeF temp_Size = graphics.MeasureString(job.subject, temp_Font);
            System.Drawing.StringFormat temp_Format = new System.Drawing.StringFormat();
            temp_Format.FormatFlags = StringFormatFlags.LineLimit;
            temp_Format.Alignment = System.Drawing.StringAlignment.Center;
            System.Drawing.RectangleF temp_Rectangle = new System.Drawing.RectangleF(border_In_Horizontal, (float)(this.Height * 0.6) + border_In_Vertical, (float)this.Width - (2 * border_In_Horizontal), (float)(this.Height * 0.4) - (2 * border_In_Vertical));

            graphics.DrawString(job.subject, temp_Font, solid_Brush, temp_Rectangle, temp_Format);
        }

        void initialize_Drawn_Image()
        {

        }

        void My_PictureBox_MouseHover(object sender, EventArgs e)
        {
            Job_Display_Form temp = new Job_Display_Form();
            temp.Show();
        }
    }
}

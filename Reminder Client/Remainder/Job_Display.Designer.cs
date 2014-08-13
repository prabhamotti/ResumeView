namespace Reminder
{
    partial class Job_Display_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Job_Display_Form));
            this.pictureBox_Computer = new System.Windows.Forms.PictureBox();
            this.pictureBox_Mail = new System.Windows.Forms.PictureBox();
            this.pictureBox_SMS = new System.Windows.Forms.PictureBox();
            this.textBox_Notes_Job_Display_Form = new System.Windows.Forms.TextBox();
            this.dateTimePicker_DeadLine_Job_Display_Form = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_Start_Job_Display_Form = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Body_Job_Display_Form = new System.Windows.Forms.TextBox();
            this.textBox_Subject_Job_Display_Form = new System.Windows.Forms.TextBox();
            this.pictureBox_Picture_Job_Display_Form = new System.Windows.Forms.PictureBox();
            this.label_Reminding_Job_Display_Form = new System.Windows.Forms.Label();
            this.listView_Receivers_Job_Display_Form = new System.Windows.Forms.ListView();
            this.CH_Names = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CH_Phone_Nos = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CH_Mail_Ids = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Computer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Mail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SMS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Picture_Job_Display_Form)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_Computer
            // 
            this.pictureBox_Computer.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_Computer.Image")));
            this.pictureBox_Computer.Location = new System.Drawing.Point(172, 243);
            this.pictureBox_Computer.Name = "pictureBox_Computer";
            this.pictureBox_Computer.Size = new System.Drawing.Size(31, 34);
            this.pictureBox_Computer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_Computer.TabIndex = 51;
            this.pictureBox_Computer.TabStop = false;
            // 
            // pictureBox_Mail
            // 
            this.pictureBox_Mail.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_Mail.Image")));
            this.pictureBox_Mail.Location = new System.Drawing.Point(106, 242);
            this.pictureBox_Mail.Name = "pictureBox_Mail";
            this.pictureBox_Mail.Size = new System.Drawing.Size(31, 34);
            this.pictureBox_Mail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_Mail.TabIndex = 49;
            this.pictureBox_Mail.TabStop = false;
            // 
            // pictureBox_SMS
            // 
            this.pictureBox_SMS.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_SMS.Image")));
            this.pictureBox_SMS.Location = new System.Drawing.Point(39, 241);
            this.pictureBox_SMS.Name = "pictureBox_SMS";
            this.pictureBox_SMS.Size = new System.Drawing.Size(31, 34);
            this.pictureBox_SMS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_SMS.TabIndex = 47;
            this.pictureBox_SMS.TabStop = false;
            // 
            // textBox_Notes_Job_Display_Form
            // 
            this.textBox_Notes_Job_Display_Form.Enabled = false;
            this.textBox_Notes_Job_Display_Form.Location = new System.Drawing.Point(229, 215);
            this.textBox_Notes_Job_Display_Form.Multiline = true;
            this.textBox_Notes_Job_Display_Form.Name = "textBox_Notes_Job_Display_Form";
            this.textBox_Notes_Job_Display_Form.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Notes_Job_Display_Form.Size = new System.Drawing.Size(252, 70);
            this.textBox_Notes_Job_Display_Form.TabIndex = 44;
            // 
            // dateTimePicker_DeadLine_Job_Display_Form
            // 
            this.dateTimePicker_DeadLine_Job_Display_Form.CustomFormat = "dd-MMM-yyyy  hh:mm:ss";
            this.dateTimePicker_DeadLine_Job_Display_Form.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_DeadLine_Job_Display_Form.Location = new System.Drawing.Point(76, 190);
            this.dateTimePicker_DeadLine_Job_Display_Form.Name = "dateTimePicker_DeadLine_Job_Display_Form";
            this.dateTimePicker_DeadLine_Job_Display_Form.Size = new System.Drawing.Size(141, 20);
            this.dateTimePicker_DeadLine_Job_Display_Form.TabIndex = 41;
            // 
            // dateTimePicker_Start_Job_Display_Form
            // 
            this.dateTimePicker_Start_Job_Display_Form.CustomFormat = "dd-MMM-yyyy  hh:mm:ss";
            this.dateTimePicker_Start_Job_Display_Form.Enabled = false;
            this.dateTimePicker_Start_Job_Display_Form.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker_Start_Job_Display_Form.Location = new System.Drawing.Point(76, 216);
            this.dateTimePicker_Start_Job_Display_Form.Name = "dateTimePicker_Start_Job_Display_Form";
            this.dateTimePicker_Start_Job_Display_Form.Size = new System.Drawing.Size(140, 20);
            this.dateTimePicker_Start_Job_Display_Form.TabIndex = 40;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(21, 196);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 39;
            this.label5.Text = "Dead Line";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(34, 220);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 38;
            this.label4.Text = "Start";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(18, 172);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Remind For Every";
            // 
            // textBox_Body_Job_Display_Form
            // 
            this.textBox_Body_Job_Display_Form.Location = new System.Drawing.Point(229, 55);
            this.textBox_Body_Job_Display_Form.Multiline = true;
            this.textBox_Body_Job_Display_Form.Name = "textBox_Body_Job_Display_Form";
            this.textBox_Body_Job_Display_Form.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_Body_Job_Display_Form.Size = new System.Drawing.Size(252, 85);
            this.textBox_Body_Job_Display_Form.TabIndex = 33;
            // 
            // textBox_Subject_Job_Display_Form
            // 
            this.textBox_Subject_Job_Display_Form.Location = new System.Drawing.Point(229, 12);
            this.textBox_Subject_Job_Display_Form.Multiline = true;
            this.textBox_Subject_Job_Display_Form.Name = "textBox_Subject_Job_Display_Form";
            this.textBox_Subject_Job_Display_Form.Size = new System.Drawing.Size(252, 37);
            this.textBox_Subject_Job_Display_Form.TabIndex = 32;
            // 
            // pictureBox_Picture_Job_Display_Form
            // 
            this.pictureBox_Picture_Job_Display_Form.Location = new System.Drawing.Point(15, 9);
            this.pictureBox_Picture_Job_Display_Form.Name = "pictureBox_Picture_Job_Display_Form";
            this.pictureBox_Picture_Job_Display_Form.Size = new System.Drawing.Size(200, 158);
            this.pictureBox_Picture_Job_Display_Form.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_Picture_Job_Display_Form.TabIndex = 31;
            this.pictureBox_Picture_Job_Display_Form.TabStop = false;
            // 
            // label_Reminding_Job_Display_Form
            // 
            this.label_Reminding_Job_Display_Form.AutoSize = true;
            this.label_Reminding_Job_Display_Form.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_Reminding_Job_Display_Form.Location = new System.Drawing.Point(115, 172);
            this.label_Reminding_Job_Display_Form.Name = "label_Reminding_Job_Display_Form";
            this.label_Reminding_Job_Display_Form.Size = new System.Drawing.Size(35, 13);
            this.label_Reminding_Job_Display_Form.TabIndex = 52;
            this.label_Reminding_Job_Display_Form.Text = "xx xxx";
            // 
            // listView_Receivers_Job_Display_Form
            // 
            this.listView_Receivers_Job_Display_Form.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CH_Names,
            this.CH_Phone_Nos,
            this.CH_Mail_Ids});
            this.listView_Receivers_Job_Display_Form.Location = new System.Drawing.Point(229, 146);
            this.listView_Receivers_Job_Display_Form.Name = "listView_Receivers_Job_Display_Form";
            this.listView_Receivers_Job_Display_Form.Size = new System.Drawing.Size(252, 61);
            this.listView_Receivers_Job_Display_Form.TabIndex = 53;
            this.listView_Receivers_Job_Display_Form.UseCompatibleStateImageBehavior = false;
            // 
            // CH_Names
            // 
            this.CH_Names.Text = "Names";
            // 
            // CH_Phone_Nos
            // 
            this.CH_Phone_Nos.Text = "Phone Nos";
            // 
            // CH_Mail_Ids
            // 
            this.CH_Mail_Ids.Text = "Mail Ids";
            // 
            // Job_Display_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(494, 297);
            this.Controls.Add(this.listView_Receivers_Job_Display_Form);
            this.Controls.Add(this.label_Reminding_Job_Display_Form);
            this.Controls.Add(this.pictureBox_Computer);
            this.Controls.Add(this.pictureBox_Mail);
            this.Controls.Add(this.pictureBox_SMS);
            this.Controls.Add(this.textBox_Notes_Job_Display_Form);
            this.Controls.Add(this.dateTimePicker_DeadLine_Job_Display_Form);
            this.Controls.Add(this.dateTimePicker_Start_Job_Display_Form);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_Body_Job_Display_Form);
            this.Controls.Add(this.textBox_Subject_Job_Display_Form);
            this.Controls.Add(this.pictureBox_Picture_Job_Display_Form);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Job_Display_Form";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Job_Display";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Computer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Mail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_SMS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Picture_Job_Display_Form)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_Computer;
        private System.Windows.Forms.PictureBox pictureBox_Mail;
        private System.Windows.Forms.PictureBox pictureBox_SMS;
        internal System.Windows.Forms.TextBox textBox_Notes_Job_Display_Form;
        internal System.Windows.Forms.DateTimePicker dateTimePicker_DeadLine_Job_Display_Form;
        internal System.Windows.Forms.DateTimePicker dateTimePicker_Start_Job_Display_Form;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.TextBox textBox_Body_Job_Display_Form;
        internal System.Windows.Forms.TextBox textBox_Subject_Job_Display_Form;
        private System.Windows.Forms.PictureBox pictureBox_Picture_Job_Display_Form;
        private System.Windows.Forms.Label label_Reminding_Job_Display_Form;
        private System.Windows.Forms.ListView listView_Receivers_Job_Display_Form;
        private System.Windows.Forms.ColumnHeader CH_Names;
        private System.Windows.Forms.ColumnHeader CH_Phone_Nos;
        private System.Windows.Forms.ColumnHeader CH_Mail_Ids;
    }
}
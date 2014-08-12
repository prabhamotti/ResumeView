namespace ResumeView_Server_Controller
{
    partial class Form_ResumeView_Server_Controller
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Company_Name = new System.Windows.Forms.TextBox();
            this.button_Add = new System.Windows.Forms.Button();
            this.button_Cancel = new System.Windows.Forms.Button();
            this.dateTimePicker_Expiry_Date = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_Message = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button_Update = new System.Windows.Forms.Button();
            this.pictureBox_Message_Image = new System.Windows.Forms.PictureBox();
            this.numericUpDown_SMS_Count = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Message_Image)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SMS_Count)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Company Name:";
            // 
            // textBox_Company_Name
            // 
            this.textBox_Company_Name.AutoCompleteCustomSource.AddRange(new string[] {
            "iiii"});
            this.textBox_Company_Name.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.textBox_Company_Name.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.textBox_Company_Name.Location = new System.Drawing.Point(112, 17);
            this.textBox_Company_Name.Name = "textBox_Company_Name";
            this.textBox_Company_Name.Size = new System.Drawing.Size(203, 20);
            this.textBox_Company_Name.TabIndex = 2;
            this.textBox_Company_Name.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_Company_Name.TextChanged += new System.EventHandler(this.textBox_Company_Name_TextChanged);
            // 
            // button_Add
            // 
            this.button_Add.Enabled = false;
            this.button_Add.Location = new System.Drawing.Point(23, 285);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(75, 23);
            this.button_Add.TabIndex = 3;
            this.button_Add.Text = "Add";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // button_Cancel
            // 
            this.button_Cancel.Location = new System.Drawing.Point(240, 285);
            this.button_Cancel.Name = "button_Cancel";
            this.button_Cancel.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel.TabIndex = 4;
            this.button_Cancel.Text = "Cancel";
            this.button_Cancel.UseVisualStyleBackColor = true;
            this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
            // 
            // dateTimePicker_Expiry_Date
            // 
            this.dateTimePicker_Expiry_Date.Location = new System.Drawing.Point(112, 43);
            this.dateTimePicker_Expiry_Date.Name = "dateTimePicker_Expiry_Date";
            this.dateTimePicker_Expiry_Date.Size = new System.Drawing.Size(203, 20);
            this.dateTimePicker_Expiry_Date.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Expiry Date:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Message:";
            // 
            // textBox_Message
            // 
            this.textBox_Message.Location = new System.Drawing.Point(112, 109);
            this.textBox_Message.Multiline = true;
            this.textBox_Message.Name = "textBox_Message";
            this.textBox_Message.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Message.Size = new System.Drawing.Size(203, 63);
            this.textBox_Message.TabIndex = 8;
            this.textBox_Message.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 193);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Message Image:";
            // 
            // button_Update
            // 
            this.button_Update.Enabled = false;
            this.button_Update.Location = new System.Drawing.Point(130, 285);
            this.button_Update.Name = "button_Update";
            this.button_Update.Size = new System.Drawing.Size(75, 23);
            this.button_Update.TabIndex = 10;
            this.button_Update.Text = "Update";
            this.button_Update.UseVisualStyleBackColor = true;
            this.button_Update.Click += new System.EventHandler(this.button_Update_Click);
            // 
            // pictureBox_Message_Image
            // 
            this.pictureBox_Message_Image.Location = new System.Drawing.Point(112, 189);
            this.pictureBox_Message_Image.Name = "pictureBox_Message_Image";
            this.pictureBox_Message_Image.Size = new System.Drawing.Size(203, 80);
            this.pictureBox_Message_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_Message_Image.TabIndex = 11;
            this.pictureBox_Message_Image.TabStop = false;
            this.pictureBox_Message_Image.Click += new System.EventHandler(this.pictureBox_Message_Image_Click);
            // 
            // numericUpDown_SMS_Count
            // 
            this.numericUpDown_SMS_Count.Location = new System.Drawing.Point(112, 76);
            this.numericUpDown_SMS_Count.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericUpDown_SMS_Count.Name = "numericUpDown_SMS_Count";
            this.numericUpDown_SMS_Count.Size = new System.Drawing.Size(203, 20);
            this.numericUpDown_SMS_Count.TabIndex = 12;
            this.numericUpDown_SMS_Count.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_SMS_Count.ThousandsSeparator = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "SMS Count:";
            // 
            // Form_ResumeView_Server_Controller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 317);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numericUpDown_SMS_Count);
            this.Controls.Add(this.pictureBox_Message_Image);
            this.Controls.Add(this.button_Update);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_Message);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateTimePicker_Expiry_Date);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_Add);
            this.Controls.Add(this.textBox_Company_Name);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "Form_ResumeView_Server_Controller";
            this.Text = "Resume View Server Controller";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Message_Image)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_SMS_Count)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Company_Name;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.Button button_Cancel;
        private System.Windows.Forms.DateTimePicker dateTimePicker_Expiry_Date;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_Message;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_Update;
        private System.Windows.Forms.PictureBox pictureBox_Message_Image;
        private System.Windows.Forms.NumericUpDown numericUpDown_SMS_Count;
        private System.Windows.Forms.Label label5;
    }
}


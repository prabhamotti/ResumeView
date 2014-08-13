namespace Reminder
{
    partial class Form_Settings
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
            this.comboBox_Mail_Account = new System.Windows.Forms.ComboBox();
            this.textBox_Mail_Id = new System.Windows.Forms.TextBox();
            this.textBox_Mail_Password = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_General_Password = new System.Windows.Forms.TextBox();
            this.button_Ok_Settings_Form = new System.Windows.Forms.Button();
            this.button_Cancel_Settings_Form = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox_Mail_Account
            // 
            this.comboBox_Mail_Account.FormattingEnabled = true;
            this.comboBox_Mail_Account.Items.AddRange(new object[] {
            "Gmail"});
            this.comboBox_Mail_Account.Location = new System.Drawing.Point(97, 19);
            this.comboBox_Mail_Account.Name = "comboBox_Mail_Account";
            this.comboBox_Mail_Account.Size = new System.Drawing.Size(135, 21);
            this.comboBox_Mail_Account.TabIndex = 0;
            // 
            // textBox_Mail_Id
            // 
            this.textBox_Mail_Id.Location = new System.Drawing.Point(97, 51);
            this.textBox_Mail_Id.Name = "textBox_Mail_Id";
            this.textBox_Mail_Id.Size = new System.Drawing.Size(135, 20);
            this.textBox_Mail_Id.TabIndex = 1;
            // 
            // textBox_Mail_Password
            // 
            this.textBox_Mail_Password.Location = new System.Drawing.Point(97, 83);
            this.textBox_Mail_Password.Name = "textBox_Mail_Password";
            this.textBox_Mail_Password.Size = new System.Drawing.Size(135, 20);
            this.textBox_Mail_Password.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Mail Account";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Mail Id";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Mail Password";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBox_Mail_Account);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_Mail_Id);
            this.groupBox1.Controls.Add(this.textBox_Mail_Password);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(15, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(238, 124);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mail Settings";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBox_General_Password);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(15, 130);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(238, 73);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General Settings";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "General Password";
            // 
            // textBox_General_Password
            // 
            this.textBox_General_Password.Location = new System.Drawing.Point(97, 24);
            this.textBox_General_Password.Name = "textBox_General_Password";
            this.textBox_General_Password.Size = new System.Drawing.Size(135, 20);
            this.textBox_General_Password.TabIndex = 2;
            // 
            // button_Ok_Settings_Form
            // 
            this.button_Ok_Settings_Form.Location = new System.Drawing.Point(15, 209);
            this.button_Ok_Settings_Form.Name = "button_Ok_Settings_Form";
            this.button_Ok_Settings_Form.Size = new System.Drawing.Size(114, 34);
            this.button_Ok_Settings_Form.TabIndex = 8;
            this.button_Ok_Settings_Form.Text = "Ok";
            this.button_Ok_Settings_Form.UseVisualStyleBackColor = true;
            // 
            // button_Cancel_Settings_Form
            // 
            this.button_Cancel_Settings_Form.Location = new System.Drawing.Point(130, 209);
            this.button_Cancel_Settings_Form.Name = "button_Cancel_Settings_Form";
            this.button_Cancel_Settings_Form.Size = new System.Drawing.Size(123, 34);
            this.button_Cancel_Settings_Form.TabIndex = 9;
            this.button_Cancel_Settings_Form.Text = "Cancel";
            this.button_Cancel_Settings_Form.UseVisualStyleBackColor = true;
            // 
            // Form_Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 255);
            this.Controls.Add(this.button_Cancel_Settings_Form);
            this.Controls.Add(this.button_Ok_Settings_Form);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form_Settings";
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.Button button_Ok_Settings_Form;
        public System.Windows.Forms.Button button_Cancel_Settings_Form;
        public System.Windows.Forms.TextBox textBox_Mail_Id;
        public System.Windows.Forms.TextBox textBox_Mail_Password;
        public System.Windows.Forms.TextBox textBox_General_Password;
        public System.Windows.Forms.ComboBox comboBox_Mail_Account;
    }
}
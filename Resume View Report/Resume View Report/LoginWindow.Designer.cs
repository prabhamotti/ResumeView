namespace Resume_View_Report
{
    partial class LoginWindow
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
            this.textBox_Username_Login = new System.Windows.Forms.TextBox();
            this.textBox_Password_Login = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Ok_Login = new System.Windows.Forms.Button();
            this.button_Cancel_Login = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username";
            // 
            // textBox_Username_Login
            // 
            this.textBox_Username_Login.Location = new System.Drawing.Point(148, 26);
            this.textBox_Username_Login.Name = "textBox_Username_Login";
            this.textBox_Username_Login.Size = new System.Drawing.Size(156, 20);
            this.textBox_Username_Login.TabIndex = 1;
            // 
            // textBox_Password_Login
            // 
            this.textBox_Password_Login.Location = new System.Drawing.Point(148, 59);
            this.textBox_Password_Login.Name = "textBox_Password_Login";
            this.textBox_Password_Login.PasswordChar = '*';
            this.textBox_Password_Login.Size = new System.Drawing.Size(156, 20);
            this.textBox_Password_Login.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            // 
            // button_Ok_Login
            // 
            this.button_Ok_Login.Location = new System.Drawing.Point(78, 95);
            this.button_Ok_Login.Name = "button_Ok_Login";
            this.button_Ok_Login.Size = new System.Drawing.Size(75, 23);
            this.button_Ok_Login.TabIndex = 4;
            this.button_Ok_Login.Text = "OK";
            this.button_Ok_Login.UseVisualStyleBackColor = true;
            // 
            // button_Cancel_Login
            // 
            this.button_Cancel_Login.Location = new System.Drawing.Point(188, 95);
            this.button_Cancel_Login.Name = "button_Cancel_Login";
            this.button_Cancel_Login.Size = new System.Drawing.Size(75, 23);
            this.button_Cancel_Login.TabIndex = 5;
            this.button_Cancel_Login.Text = "Cancel";
            this.button_Cancel_Login.UseVisualStyleBackColor = true;
            this.button_Cancel_Login.Click += new System.EventHandler(this.button_Cancel_Login_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_Username_Login);
            this.groupBox1.Controls.Add(this.button_Cancel_Login);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button_Ok_Login);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_Password_Login);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(354, 143);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            // 
            // LoginWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 143);
            this.Controls.Add(this.groupBox1);
            this.Name = "LoginWindow";
            this.Text = "Login";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Cancel_Login;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button button_Ok_Login;
        public System.Windows.Forms.TextBox textBox_Username_Login;
        public System.Windows.Forms.TextBox textBox_Password_Login;
    }
}
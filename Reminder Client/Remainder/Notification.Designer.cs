namespace Reminder
{
    partial class form_Notification
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_Notification));
            this.textBox_Subject = new System.Windows.Forms.TextBox();
            this.textBox_Body = new System.Windows.Forms.TextBox();
            this.label_DeadLine = new System.Windows.Forms.Label();
            this.button_Dismiss = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_Subject
            // 
            this.textBox_Subject.Location = new System.Drawing.Point(12, 54);
            this.textBox_Subject.Multiline = true;
            this.textBox_Subject.Name = "textBox_Subject";
            this.textBox_Subject.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Subject.Size = new System.Drawing.Size(171, 37);
            this.textBox_Subject.TabIndex = 0;
            // 
            // textBox_Body
            // 
            this.textBox_Body.Location = new System.Drawing.Point(12, 98);
            this.textBox_Body.Multiline = true;
            this.textBox_Body.Name = "textBox_Body";
            this.textBox_Body.ReadOnly = true;
            this.textBox_Body.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_Body.Size = new System.Drawing.Size(171, 135);
            this.textBox_Body.TabIndex = 1;
            // 
            // label_DeadLine
            // 
            this.label_DeadLine.AutoSize = true;
            this.label_DeadLine.ForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.label_DeadLine.Location = new System.Drawing.Point(45, 236);
            this.label_DeadLine.Name = "label_DeadLine";
            this.label_DeadLine.Size = new System.Drawing.Size(114, 13);
            this.label_DeadLine.TabIndex = 3;
            this.label_DeadLine.Text = "MM/dd/yyyy hh:mm:ss";
            // 
            // button_Dismiss
            // 
            this.button_Dismiss.Location = new System.Drawing.Point(11, 24);
            this.button_Dismiss.Name = "button_Dismiss";
            this.button_Dismiss.Size = new System.Drawing.Size(171, 23);
            this.button_Dismiss.TabIndex = 4;
            this.button_Dismiss.Text = "Dismiss";
            this.button_Dismiss.UseVisualStyleBackColor = true;
            // 
            // form_Notification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(195, 266);
            this.Controls.Add(this.button_Dismiss);
            this.Controls.Add(this.label_DeadLine);
            this.Controls.Add(this.textBox_Body);
            this.Controls.Add(this.textBox_Subject);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "form_Notification";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Subject;
        private System.Windows.Forms.TextBox textBox_Body;
        private System.Windows.Forms.Label label_DeadLine;
        public System.Windows.Forms.Button button_Dismiss;
    }
}
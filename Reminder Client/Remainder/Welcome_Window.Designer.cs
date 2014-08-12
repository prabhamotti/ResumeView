namespace Reminder
{
    partial class Welcome_Window
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
            this.pictureBox_Message_Image = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Message_Image)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_Message_Image
            // 
            this.pictureBox_Message_Image.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox_Message_Image.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_Message_Image.Name = "pictureBox_Message_Image";
            this.pictureBox_Message_Image.Size = new System.Drawing.Size(398, 116);
            this.pictureBox_Message_Image.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_Message_Image.TabIndex = 0;
            this.pictureBox_Message_Image.TabStop = false;
            // 
            // Welcome_Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 116);
            this.Controls.Add(this.pictureBox_Message_Image);
            this.Name = "Welcome_Window";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Message_Image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_Message_Image;
    }
}
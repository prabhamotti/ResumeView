namespace Admin_Window
{
    partial class Statistics
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
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_Save_Statistics = new System.Windows.Forms.Button();
            this.button_Generate = new System.Windows.Forms.Button();
            this.dateTimePicker_To_Y_Statistics = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_From_Y_Statistics = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_To_X_Statistics = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_From_X_Statistics = new System.Windows.Forms.DateTimePicker();
            this.checkedListBox_Y_Statistics = new System.Windows.Forms.CheckedListBox();
            this.chart_Statistics = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_X_Statistics = new System.Windows.Forms.ComboBox();
            this.button_Queue_Up_Statistics = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox_Candidate_Name_Data_Statistics = new System.Windows.Forms.TextBox();
            this.button_Clear_Data_Statistics = new System.Windows.Forms.Button();
            this.label_Pattern = new System.Windows.Forms.Label();
            this.comboBox_Type_Statistics = new System.Windows.Forms.ComboBox();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox_Duration_Data_Statistics = new System.Windows.Forms.GroupBox();
            this.dateTimePicker_To_Data_Statistics = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_From_Data_Statistics = new System.Windows.Forms.DateTimePicker();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.checkedListBox_Values_Statistics = new System.Windows.Forms.CheckedListBox();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Statistics)).BeginInit();
            this.groupBox_Duration_Data_Statistics.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Silver;
            this.groupBox2.Controls.Add(this.button_Save_Statistics);
            this.groupBox2.Controls.Add(this.button_Generate);
            this.groupBox2.Controls.Add(this.dateTimePicker_To_Y_Statistics);
            this.groupBox2.Controls.Add(this.dateTimePicker_From_Y_Statistics);
            this.groupBox2.Controls.Add(this.dateTimePicker_To_X_Statistics);
            this.groupBox2.Controls.Add(this.dateTimePicker_From_X_Statistics);
            this.groupBox2.Controls.Add(this.checkedListBox_Y_Statistics);
            this.groupBox2.Controls.Add(this.chart_Statistics);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.comboBox_X_Statistics);
            this.groupBox2.Controls.Add(this.button_Queue_Up_Statistics);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.textBox_Candidate_Name_Data_Statistics);
            this.groupBox2.Controls.Add(this.button_Clear_Data_Statistics);
            this.groupBox2.Controls.Add(this.label_Pattern);
            this.groupBox2.Controls.Add(this.comboBox_Type_Statistics);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.groupBox_Duration_Data_Statistics);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.checkedListBox_Values_Statistics);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(711, 616);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // button_Save_Statistics
            // 
            this.button_Save_Statistics.BackColor = System.Drawing.Color.LavenderBlush;
            this.button_Save_Statistics.Enabled = false;
            this.button_Save_Statistics.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Save_Statistics.Location = new System.Drawing.Point(515, 159);
            this.button_Save_Statistics.Name = "button_Save_Statistics";
            this.button_Save_Statistics.Size = new System.Drawing.Size(67, 39);
            this.button_Save_Statistics.TabIndex = 40;
            this.button_Save_Statistics.Text = "Save";
            this.button_Save_Statistics.UseVisualStyleBackColor = false;
            this.button_Save_Statistics.Click += new System.EventHandler(this.button_Save_Statistics_Click);
            // 
            // button_Generate
            // 
            this.button_Generate.BackColor = System.Drawing.Color.LavenderBlush;
            this.button_Generate.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Generate.Location = new System.Drawing.Point(588, 159);
            this.button_Generate.Name = "button_Generate";
            this.button_Generate.Size = new System.Drawing.Size(75, 39);
            this.button_Generate.TabIndex = 39;
            this.button_Generate.Text = "Generate";
            this.button_Generate.UseVisualStyleBackColor = false;
            this.button_Generate.Click += new System.EventHandler(this.button_Generate_Click);
            // 
            // dateTimePicker_To_Y_Statistics
            // 
            this.dateTimePicker_To_Y_Statistics.Enabled = false;
            this.dateTimePicker_To_Y_Statistics.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker_To_Y_Statistics.Location = new System.Drawing.Point(669, 115);
            this.dateTimePicker_To_Y_Statistics.Name = "dateTimePicker_To_Y_Statistics";
            this.dateTimePicker_To_Y_Statistics.Size = new System.Drawing.Size(84, 20);
            this.dateTimePicker_To_Y_Statistics.TabIndex = 38;
            this.dateTimePicker_To_Y_Statistics.Visible = false;
            // 
            // dateTimePicker_From_Y_Statistics
            // 
            this.dateTimePicker_From_Y_Statistics.Enabled = false;
            this.dateTimePicker_From_Y_Statistics.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker_From_Y_Statistics.Location = new System.Drawing.Point(669, 87);
            this.dateTimePicker_From_Y_Statistics.Name = "dateTimePicker_From_Y_Statistics";
            this.dateTimePicker_From_Y_Statistics.Size = new System.Drawing.Size(84, 20);
            this.dateTimePicker_From_Y_Statistics.TabIndex = 37;
            this.dateTimePicker_From_Y_Statistics.Visible = false;
            this.dateTimePicker_From_Y_Statistics.ValueChanged += new System.EventHandler(this.dateTimePicker_From_Y_Statistics_ValueChanged);
            // 
            // dateTimePicker_To_X_Statistics
            // 
            this.dateTimePicker_To_X_Statistics.Enabled = false;
            this.dateTimePicker_To_X_Statistics.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker_To_X_Statistics.Location = new System.Drawing.Point(669, 43);
            this.dateTimePicker_To_X_Statistics.Name = "dateTimePicker_To_X_Statistics";
            this.dateTimePicker_To_X_Statistics.Size = new System.Drawing.Size(84, 20);
            this.dateTimePicker_To_X_Statistics.TabIndex = 36;
            this.dateTimePicker_To_X_Statistics.Visible = false;
            // 
            // dateTimePicker_From_X_Statistics
            // 
            this.dateTimePicker_From_X_Statistics.Enabled = false;
            this.dateTimePicker_From_X_Statistics.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker_From_X_Statistics.Location = new System.Drawing.Point(669, 15);
            this.dateTimePicker_From_X_Statistics.Name = "dateTimePicker_From_X_Statistics";
            this.dateTimePicker_From_X_Statistics.Size = new System.Drawing.Size(84, 20);
            this.dateTimePicker_From_X_Statistics.TabIndex = 35;
            this.dateTimePicker_From_X_Statistics.Visible = false;
            this.dateTimePicker_From_X_Statistics.ValueChanged += new System.EventHandler(this.dateTimePicker_From_X_Statistics_ValueChanged);
            // 
            // checkedListBox_Y_Statistics
            // 
            this.checkedListBox_Y_Statistics.BackColor = System.Drawing.Color.White;
            this.checkedListBox_Y_Statistics.Enabled = false;
            this.checkedListBox_Y_Statistics.FormattingEnabled = true;
            this.checkedListBox_Y_Statistics.Location = new System.Drawing.Point(515, 74);
            this.checkedListBox_Y_Statistics.Name = "checkedListBox_Y_Statistics";
            this.checkedListBox_Y_Statistics.Size = new System.Drawing.Size(148, 79);
            this.checkedListBox_Y_Statistics.TabIndex = 34;
            this.checkedListBox_Y_Statistics.SelectedIndexChanged += new System.EventHandler(this.checkedListBox_Y_Statistics_SelectedIndexChanged);
            // 
            // chart_Statistics
            // 
            this.chart_Statistics.Dock = System.Windows.Forms.DockStyle.Bottom;
            legend1.Name = "Legend1";
            this.chart_Statistics.Legends.Add(legend1);
            this.chart_Statistics.Location = new System.Drawing.Point(3, 202);
            this.chart_Statistics.Name = "chart_Statistics";
            this.chart_Statistics.Size = new System.Drawing.Size(705, 411);
            this.chart_Statistics.TabIndex = 4;
            this.chart_Statistics.Text = "chart";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(512, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 31;
            this.label1.Text = "X Axis:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(512, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Y Axis:";
            // 
            // comboBox_X_Statistics
            // 
            this.comboBox_X_Statistics.BackColor = System.Drawing.Color.White;
            this.comboBox_X_Statistics.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_X_Statistics.Enabled = false;
            this.comboBox_X_Statistics.FormattingEnabled = true;
            this.comboBox_X_Statistics.Location = new System.Drawing.Point(515, 28);
            this.comboBox_X_Statistics.Name = "comboBox_X_Statistics";
            this.comboBox_X_Statistics.Size = new System.Drawing.Size(148, 21);
            this.comboBox_X_Statistics.TabIndex = 33;
            this.comboBox_X_Statistics.SelectedIndexChanged += new System.EventHandler(this.comboBox_X_Statistics_SelectedIndexChanged);
            // 
            // button_Queue_Up_Statistics
            // 
            this.button_Queue_Up_Statistics.BackColor = System.Drawing.Color.LavenderBlush;
            this.button_Queue_Up_Statistics.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Queue_Up_Statistics.Location = new System.Drawing.Point(441, 36);
            this.button_Queue_Up_Statistics.Name = "button_Queue_Up_Statistics";
            this.button_Queue_Up_Statistics.Size = new System.Drawing.Size(65, 23);
            this.button_Queue_Up_Statistics.TabIndex = 28;
            this.button_Queue_Up_Statistics.Text = "Queue Up";
            this.button_Queue_Up_Statistics.UseVisualStyleBackColor = false;
            this.button_Queue_Up_Statistics.Click += new System.EventHandler(this.button_Queue_Up_Statistics_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 65);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(89, 13);
            this.label13.TabIndex = 27;
            this.label13.Text = "Candidate Name:";
            // 
            // textBox_Candidate_Name_Data_Statistics
            // 
            this.textBox_Candidate_Name_Data_Statistics.AcceptsReturn = true;
            this.textBox_Candidate_Name_Data_Statistics.BackColor = System.Drawing.Color.White;
            this.textBox_Candidate_Name_Data_Statistics.Enabled = false;
            this.textBox_Candidate_Name_Data_Statistics.Location = new System.Drawing.Point(17, 81);
            this.textBox_Candidate_Name_Data_Statistics.Name = "textBox_Candidate_Name_Data_Statistics";
            this.textBox_Candidate_Name_Data_Statistics.Size = new System.Drawing.Size(159, 20);
            this.textBox_Candidate_Name_Data_Statistics.TabIndex = 26;
            // 
            // button_Clear_Data_Statistics
            // 
            this.button_Clear_Data_Statistics.BackColor = System.Drawing.Color.LavenderBlush;
            this.button_Clear_Data_Statistics.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Clear_Data_Statistics.Location = new System.Drawing.Point(441, 74);
            this.button_Clear_Data_Statistics.Name = "button_Clear_Data_Statistics";
            this.button_Clear_Data_Statistics.Size = new System.Drawing.Size(65, 23);
            this.button_Clear_Data_Statistics.TabIndex = 25;
            this.button_Clear_Data_Statistics.Text = "Clear";
            this.button_Clear_Data_Statistics.UseVisualStyleBackColor = false;
            this.button_Clear_Data_Statistics.Click += new System.EventHandler(this.button_Clear_Data_Statistics_Click);
            // 
            // label_Pattern
            // 
            this.label_Pattern.AutoSize = true;
            this.label_Pattern.BackColor = System.Drawing.Color.DarkGray;
            this.label_Pattern.Location = new System.Drawing.Point(32, 112);
            this.label_Pattern.MaximumSize = new System.Drawing.Size(600, 0);
            this.label_Pattern.Name = "label_Pattern";
            this.label_Pattern.Size = new System.Drawing.Size(41, 13);
            this.label_Pattern.TabIndex = 24;
            this.label_Pattern.Text = "Pattern";
            // 
            // comboBox_Type_Statistics
            // 
            this.comboBox_Type_Statistics.BackColor = System.Drawing.Color.White;
            this.comboBox_Type_Statistics.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Type_Statistics.FormattingEnabled = true;
            this.comboBox_Type_Statistics.Items.AddRange(new object[] {
            "Interview Name",
            "User Name",
            "User Role",
            "Filtered Status",
            "Qualified Status",
            "Attended Status",
            "Selected Status",
            "Joined Status",
            "Date",
            "Candidate Name Starts With",
            "Candidate Name Has"});
            this.comboBox_Type_Statistics.Location = new System.Drawing.Point(18, 38);
            this.comboBox_Type_Statistics.Name = "comboBox_Type_Statistics";
            this.comboBox_Type_Statistics.Size = new System.Drawing.Size(173, 21);
            this.comboBox_Type_Statistics.TabIndex = 23;
            this.comboBox_Type_Statistics.SelectedIndexChanged += new System.EventHandler(this.comboBox_Type_Statistics_SelectedIndexChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(321, 19);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(47, 13);
            this.label15.TabIndex = 22;
            this.label15.Text = "Duration";
            // 
            // groupBox_Duration_Data_Statistics
            // 
            this.groupBox_Duration_Data_Statistics.Controls.Add(this.dateTimePicker_To_Data_Statistics);
            this.groupBox_Duration_Data_Statistics.Controls.Add(this.dateTimePicker_From_Data_Statistics);
            this.groupBox_Duration_Data_Statistics.Location = new System.Drawing.Point(324, 22);
            this.groupBox_Duration_Data_Statistics.Name = "groupBox_Duration_Data_Statistics";
            this.groupBox_Duration_Data_Statistics.Size = new System.Drawing.Size(111, 80);
            this.groupBox_Duration_Data_Statistics.TabIndex = 21;
            this.groupBox_Duration_Data_Statistics.TabStop = false;
            // 
            // dateTimePicker_To_Data_Statistics
            // 
            this.dateTimePicker_To_Data_Statistics.Enabled = false;
            this.dateTimePicker_To_Data_Statistics.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker_To_Data_Statistics.Location = new System.Drawing.Point(6, 43);
            this.dateTimePicker_To_Data_Statistics.Name = "dateTimePicker_To_Data_Statistics";
            this.dateTimePicker_To_Data_Statistics.Size = new System.Drawing.Size(84, 20);
            this.dateTimePicker_To_Data_Statistics.TabIndex = 3;
            // 
            // dateTimePicker_From_Data_Statistics
            // 
            this.dateTimePicker_From_Data_Statistics.CalendarMonthBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(226)))), ((int)(((byte)(175)))));
            this.dateTimePicker_From_Data_Statistics.CalendarTitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(226)))), ((int)(((byte)(175)))));
            this.dateTimePicker_From_Data_Statistics.Enabled = false;
            this.dateTimePicker_From_Data_Statistics.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker_From_Data_Statistics.Location = new System.Drawing.Point(6, 17);
            this.dateTimePicker_From_Data_Statistics.Name = "dateTimePicker_From_Data_Statistics";
            this.dateTimePicker_From_Data_Statistics.Size = new System.Drawing.Size(84, 20);
            this.dateTimePicker_From_Data_Statistics.TabIndex = 2;
            this.dateTimePicker_From_Data_Statistics.ValueChanged += new System.EventHandler(this.dateTimePicker_From_Data_Statistics_ValueChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(194, 19);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(39, 13);
            this.label12.TabIndex = 20;
            this.label12.Text = "Values";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 19);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Select";
            // 
            // checkedListBox_Values_Statistics
            // 
            this.checkedListBox_Values_Statistics.BackColor = System.Drawing.Color.White;
            this.checkedListBox_Values_Statistics.FormattingEnabled = true;
            this.checkedListBox_Values_Statistics.Location = new System.Drawing.Point(197, 38);
            this.checkedListBox_Values_Statistics.Name = "checkedListBox_Values_Statistics";
            this.checkedListBox_Values_Statistics.Size = new System.Drawing.Size(117, 64);
            this.checkedListBox_Values_Statistics.TabIndex = 18;
            // 
            // Statistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 616);
            this.Controls.Add(this.groupBox2);
            this.Name = "Statistics";
            this.Text = "Statistics";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Statistics)).EndInit();
            this.groupBox_Duration_Data_Statistics.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Statistics;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_X_Statistics;
        private System.Windows.Forms.Button button_Queue_Up_Statistics;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox_Candidate_Name_Data_Statistics;
        private System.Windows.Forms.Button button_Clear_Data_Statistics;
        private System.Windows.Forms.Label label_Pattern;
        private System.Windows.Forms.ComboBox comboBox_Type_Statistics;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.GroupBox groupBox_Duration_Data_Statistics;
        private System.Windows.Forms.DateTimePicker dateTimePicker_To_Data_Statistics;
        private System.Windows.Forms.DateTimePicker dateTimePicker_From_Data_Statistics;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckedListBox checkedListBox_Values_Statistics;
        private System.Windows.Forms.CheckedListBox checkedListBox_Y_Statistics;
        private System.Windows.Forms.DateTimePicker dateTimePicker_To_Y_Statistics;
        private System.Windows.Forms.DateTimePicker dateTimePicker_From_Y_Statistics;
        private System.Windows.Forms.DateTimePicker dateTimePicker_To_X_Statistics;
        private System.Windows.Forms.DateTimePicker dateTimePicker_From_X_Statistics;
        private System.Windows.Forms.Button button_Generate;
        private System.Windows.Forms.Button button_Save_Statistics;


    }
}
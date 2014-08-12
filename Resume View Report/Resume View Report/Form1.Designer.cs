namespace Resume_View_Report
{
    partial class Form1
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_Excel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button_Generate_Report = new System.Windows.Forms.Button();
            this.dateTimePicker_To_Report = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_From_Report = new System.Windows.Forms.DateTimePicker();
            this.comboBox_Type_Report = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tp_User_Based_View = new System.Windows.Forms.TabPage();
            this.chart_User_Based_View = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tp_Interview_Based_View = new System.Windows.Forms.TabPage();
            this.chart_Interview_Based_View = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tp_Comparison_View = new System.Windows.Forms.TabPage();
            this.chart_Comparison_View = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tp_User_Based_View.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_User_Based_View)).BeginInit();
            this.tp_Interview_Based_View.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Interview_Based_View)).BeginInit();
            this.tp_Comparison_View.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart_Comparison_View)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Silver;
            this.groupBox1.Controls.Add(this.button_Excel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button_Generate_Report);
            this.groupBox1.Controls.Add(this.dateTimePicker_To_Report);
            this.groupBox1.Controls.Add(this.dateTimePicker_From_Report);
            this.groupBox1.Controls.Add(this.comboBox_Type_Report);
            this.groupBox1.Enabled = false;
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(668, 82);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // button_Excel
            // 
            this.button_Excel.Location = new System.Drawing.Point(583, 18);
            this.button_Excel.Name = "button_Excel";
            this.button_Excel.Size = new System.Drawing.Size(75, 45);
            this.button_Excel.TabIndex = 7;
            this.button_Excel.Text = "Generate Excel";
            this.button_Excel.UseVisualStyleBackColor = true;
            this.button_Excel.Click += new System.EventHandler(this.button_Excel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(350, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "To";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(208, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "From";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Type";
            // 
            // button_Generate_Report
            // 
            this.button_Generate_Report.Location = new System.Drawing.Point(503, 18);
            this.button_Generate_Report.Name = "button_Generate_Report";
            this.button_Generate_Report.Size = new System.Drawing.Size(75, 45);
            this.button_Generate_Report.TabIndex = 3;
            this.button_Generate_Report.Text = "Generate Chart";
            this.button_Generate_Report.UseVisualStyleBackColor = true;
            this.button_Generate_Report.Click += new System.EventHandler(this.button_Generate_Report_Click);
            // 
            // dateTimePicker_To_Report
            // 
            this.dateTimePicker_To_Report.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker_To_Report.Location = new System.Drawing.Point(382, 30);
            this.dateTimePicker_To_Report.Name = "dateTimePicker_To_Report";
            this.dateTimePicker_To_Report.Size = new System.Drawing.Size(96, 20);
            this.dateTimePicker_To_Report.TabIndex = 2;
            // 
            // dateTimePicker_From_Report
            // 
            this.dateTimePicker_From_Report.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePicker_From_Report.Location = new System.Drawing.Point(244, 30);
            this.dateTimePicker_From_Report.Name = "dateTimePicker_From_Report";
            this.dateTimePicker_From_Report.Size = new System.Drawing.Size(96, 20);
            this.dateTimePicker_From_Report.TabIndex = 1;
            // 
            // comboBox_Type_Report
            // 
            this.comboBox_Type_Report.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Type_Report.FormattingEnabled = true;
            this.comboBox_Type_Report.Items.AddRange(new object[] {
            "Daily",
            "Weekly",
            "Monthly"});
            this.comboBox_Type_Report.Location = new System.Drawing.Point(61, 29);
            this.comboBox_Type_Report.Name = "comboBox_Type_Report";
            this.comboBox_Type_Report.Size = new System.Drawing.Size(121, 21);
            this.comboBox_Type_Report.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tp_User_Based_View);
            this.tabControl1.Controls.Add(this.tp_Interview_Based_View);
            this.tabControl1.Controls.Add(this.tp_Comparison_View);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.Enabled = false;
            this.tabControl1.Location = new System.Drawing.Point(0, 94);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(692, 572);
            this.tabControl1.TabIndex = 1;
            // 
            // tp_User_Based_View
            // 
            this.tp_User_Based_View.AutoScroll = true;
            this.tp_User_Based_View.Controls.Add(this.chart_User_Based_View);
            this.tp_User_Based_View.Location = new System.Drawing.Point(4, 22);
            this.tp_User_Based_View.Name = "tp_User_Based_View";
            this.tp_User_Based_View.Padding = new System.Windows.Forms.Padding(3);
            this.tp_User_Based_View.Size = new System.Drawing.Size(684, 546);
            this.tp_User_Based_View.TabIndex = 0;
            this.tp_User_Based_View.Text = "User Based View";
            this.tp_User_Based_View.UseVisualStyleBackColor = true;
            // 
            // chart_User_Based_View
            // 
            chartArea1.Name = "ChartArea1";
            this.chart_User_Based_View.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart_User_Based_View.Legends.Add(legend1);
            this.chart_User_Based_View.Location = new System.Drawing.Point(91, 59);
            this.chart_User_Based_View.Name = "chart_User_Based_View";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart_User_Based_View.Series.Add(series1);
            this.chart_User_Based_View.Size = new System.Drawing.Size(243, 118);
            this.chart_User_Based_View.TabIndex = 0;
            this.chart_User_Based_View.Text = "chart1";
            this.chart_User_Based_View.Visible = false;
            // 
            // tp_Interview_Based_View
            // 
            this.tp_Interview_Based_View.Controls.Add(this.chart_Interview_Based_View);
            this.tp_Interview_Based_View.Location = new System.Drawing.Point(4, 22);
            this.tp_Interview_Based_View.Name = "tp_Interview_Based_View";
            this.tp_Interview_Based_View.Padding = new System.Windows.Forms.Padding(3);
            this.tp_Interview_Based_View.Size = new System.Drawing.Size(684, 546);
            this.tp_Interview_Based_View.TabIndex = 1;
            this.tp_Interview_Based_View.Text = "Interview Based View";
            this.tp_Interview_Based_View.UseVisualStyleBackColor = true;
            // 
            // chart_Interview_Based_View
            // 
            chartArea2.Name = "ChartArea1";
            this.chart_Interview_Based_View.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart_Interview_Based_View.Legends.Add(legend2);
            this.chart_Interview_Based_View.Location = new System.Drawing.Point(194, 214);
            this.chart_Interview_Based_View.Name = "chart_Interview_Based_View";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart_Interview_Based_View.Series.Add(series2);
            this.chart_Interview_Based_View.Size = new System.Drawing.Size(243, 118);
            this.chart_Interview_Based_View.TabIndex = 1;
            this.chart_Interview_Based_View.Text = "chart1";
            this.chart_Interview_Based_View.Visible = false;
            // 
            // tp_Comparison_View
            // 
            this.tp_Comparison_View.Controls.Add(this.chart_Comparison_View);
            this.tp_Comparison_View.Location = new System.Drawing.Point(4, 22);
            this.tp_Comparison_View.Name = "tp_Comparison_View";
            this.tp_Comparison_View.Size = new System.Drawing.Size(684, 546);
            this.tp_Comparison_View.TabIndex = 2;
            this.tp_Comparison_View.Text = "Comparison View";
            this.tp_Comparison_View.UseVisualStyleBackColor = true;
            // 
            // chart_Comparison_View
            // 
            chartArea3.Name = "ChartArea1";
            this.chart_Comparison_View.ChartAreas.Add(chartArea3);
            this.chart_Comparison_View.Dock = System.Windows.Forms.DockStyle.Fill;
            legend3.Name = "Legend1";
            this.chart_Comparison_View.Legends.Add(legend3);
            this.chart_Comparison_View.Location = new System.Drawing.Point(0, 0);
            this.chart_Comparison_View.Name = "chart_Comparison_View";
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend1";
            series3.Name = "Series1";
            this.chart_Comparison_View.Series.Add(series3);
            this.chart_Comparison_View.Size = new System.Drawing.Size(684, 546);
            this.chart_Comparison_View.TabIndex = 2;
            this.chart_Comparison_View.Text = "chart1";
            this.chart_Comparison_View.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 666);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Report";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mouse_Got_Clicked_In_The_Form);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tp_User_Based_View.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_User_Based_View)).EndInit();
            this.tp_Interview_Based_View.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_Interview_Based_View)).EndInit();
            this.tp_Comparison_View.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart_Comparison_View)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_Generate_Report;
        private System.Windows.Forms.DateTimePicker dateTimePicker_To_Report;
        private System.Windows.Forms.DateTimePicker dateTimePicker_From_Report;
        private System.Windows.Forms.ComboBox comboBox_Type_Report;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tp_User_Based_View;
        private System.Windows.Forms.TabPage tp_Interview_Based_View;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_User_Based_View;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Interview_Based_View;
        private System.Windows.Forms.TabPage tp_Comparison_View;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart_Comparison_View;
        private System.Windows.Forms.Button button_Excel;
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Excel = Microsoft.Office.Interop.Excel;

using MySql.Data.MySqlClient;

namespace Resume_View_Report
{
    public partial class Form1 : Form
    {
        private string server;
        private string database;
        private string uid;
        private string password;
        string connection_String;

        string company_Name;

        public Form1()
        {
            InitializeComponent();
            load_Mysql_Settings();
            load_General_Settings();
        }

        private bool Open_This_Mysql_Connection(ref MySqlConnection connection)
        {
            //Below code was taken from http://stackoverflow.com/questions/280413/c-sharp-how-do-you-find-the-caller-function
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
            //System.Diagnostics.StackFrame sf = st.GetFrame(0);
            //string msg = sf.GetMethod().DeclaringType.FullName + "." +
            //sf.GetMethod().Name;
            //MessageBox.Show("openconnection: " + msg);
            try
            {
                connection.Open();
                return true;
            }

            catch (MySqlException exception)
            {
                MessageBox.Show("Cannot connect to the Mysql server");
                MessageBox.Show(exception.Message);
                switch (exception.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to the server.Contact Administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid Username/Password");
                        break;
                }
                return false;
            }
        }

        private bool Close_This_Mysql_Connection(ref MySqlConnection connection)
        {
            //Below code was taken from http://stackoverflow.com/questions/280413/c-sharp-how-do-you-find-the-caller-function
            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(1);
            //System.Diagnostics.StackFrame sf = st.GetFrame(0);
            //string msg = sf.GetMethod().DeclaringType.FullName + "." +
            //sf.GetMethod().Name;
            //MessageBox.Show("close connection: " + msg);
            try
            {
                connection.Close();
                return true;
            }

            catch (MySqlException exception)
            {
                MessageBox.Show(exception.Message);
                return false;
            }
        }

        private void load_Mysql_Settings()
        {
            Microsoft.Win32.RegistryKey temp = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey temp_0 = temp.OpenSubKey("SOFTWARE", true);
            if (!temp_0.GetSubKeyNames().Contains("Resume View"))
            {
                Console.WriteLine("There is no reg entry for the key value \"Resume View\"");
                return;
            }

            Microsoft.Win32.RegistryKey resume_View_Key = temp_0.OpenSubKey("Resume View", true);

            Useful_Functions.SimpleAES temp_Crypting_Object = new Useful_Functions.SimpleAES();
            server = (string)(resume_View_Key.GetValue("database_Server"));
            server = temp_Crypting_Object.DecryptString(server);
            database = (string)(resume_View_Key.GetValue("database_Name"));
            database = temp_Crypting_Object.DecryptString(database);
            uid = (string)(resume_View_Key.GetValue("database_Uid"));
            uid = temp_Crypting_Object.DecryptString(uid);
            password = (string)(resume_View_Key.GetValue("database_Password"));
            password = temp_Crypting_Object.DecryptString(password);

            connection_String = "SERVER=" + server + ";" + "DATABASE=" + database + ";" +
                                        "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            //connection = new MySqlConnection(connection_String);
        }

        void load_General_Settings()
        {
            Microsoft.Win32.RegistryKey temp = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
            Microsoft.Win32.RegistryKey temp_0 = temp.OpenSubKey("SOFTWARE", true);
            if (!temp_0.GetSubKeyNames().Contains("Resume View"))
            {
                Console.WriteLine("There is no reg entry for the key value \"Resume View\"");
                return;
            }

            Microsoft.Win32.RegistryKey resume_View_Key = temp_0.OpenSubKey("Resume View", true);

            Useful_Functions.SimpleAES temp_Crypting_Object = new Useful_Functions.SimpleAES();
            company_Name = (string)(resume_View_Key.GetValue("company_Name"));
            company_Name = temp_Crypting_Object.DecryptString(company_Name);
        }

        private void get_WeekRanges_Start_End_Dates_Which_Has_This_Dates(DateTime start_Date,DateTime end_Date,ref DateTime week_Ranges_Start_Date,ref DateTime week_Ranges_End_Date)
        {
            week_Ranges_Start_Date = start_Date;
            while (week_Ranges_Start_Date.DayOfWeek.ToString() != "Sunday")
                week_Ranges_Start_Date = week_Ranges_Start_Date.AddDays(-1);

            week_Ranges_End_Date = end_Date;
            while (week_Ranges_End_Date.DayOfWeek.ToString() != "Saturday")
                week_Ranges_End_Date = week_Ranges_End_Date.AddDays(1);
        }

        private void get_MonthRanges_Start_End_Dates_Which_Has_This_Dates(DateTime start_Date,DateTime end_Date,ref DateTime month_Ranges_Start_Date,ref DateTime month_Ranges_End_Date)
        {
            month_Ranges_Start_Date = new DateTime(start_Date.Year,start_Date.Month,start_Date.Day);

            month_Ranges_End_Date = new DateTime(end_Date.Year,end_Date.Month,DateTime.DaysInMonth(end_Date.Year,end_Date.Month));
        }

        private void button_Generate_Report_Click(object sender, EventArgs e)
        {
            if (comboBox_Type_Report.SelectedItem == null)
            {
                MessageBox.Show("Type is not selected");
                return;
            }

            if (dateTimePicker_To_Report.Value.Subtract(dateTimePicker_From_Report.Value).Days < 0)
            {
                MessageBox.Show("\"To Date\" Can't be lesser than the \"From Date\"");
                return;
            }

            List<string> user_Names= new List<string>();
            List<string> interview_Names = new List<string>();

            MySqlConnection mySql_Connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref mySql_Connection))
            {
                string query = "SELECT DISTINCT name FROM userinfo";
                MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    if (reader["name"] != DBNull.Value)
                        user_Names.Add(reader["name"].ToString());
                if (!reader.IsClosed)
                    reader.Close();

                query = "SELECT DISTINCT interview_Name FROM main_Data WHERE company_Name='"+company_Name+"' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd")+"'";
                cmd = new MySqlCommand(query, mySql_Connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                    if (reader["interview_Name"] != DBNull.Value)
                        interview_Names.Add(reader["interview_Name"].ToString());
                if (!reader.IsClosed)
                    reader.Close();
                Close_This_Mysql_Connection(ref mySql_Connection);
            }

            DateTime range_Start_Date = dateTimePicker_From_Report.Value;
            DateTime range_End_Date = dateTimePicker_To_Report.Value;
            if (comboBox_Type_Report.SelectedItem.ToString() == "Weekly")
                get_WeekRanges_Start_End_Dates_Which_Has_This_Dates(dateTimePicker_From_Report.Value, dateTimePicker_To_Report.Value, ref range_Start_Date, ref range_End_Date);
            else if(comboBox_Type_Report.SelectedItem.ToString() == "Monthly")
                get_MonthRanges_Start_End_Dates_Which_Has_This_Dates(dateTimePicker_From_Report.Value, dateTimePicker_To_Report.Value, ref range_Start_Date, ref range_End_Date);

            // Chart_User_Based_View
            chart_User_Based_View.Legends.Clear();
            chart_User_Based_View.ChartAreas.Clear();
            chart_User_Based_View.Series.Clear();
            chart_User_Based_View.Titles.Clear();

            chart_User_Based_View.Width = (interview_Names.Count + 1) * 250;
            chart_User_Based_View.Height = user_Names.Count * 250;
            int size = 60;
            for (int i = 0; i < user_Names.Count; ++i)
            {
                //int j = 0;
                for (int j = 0; j <= interview_Names.Count; ++j)
                {
                    string key_String = "";
                    if (j == 0)
                        key_String = user_Names[i];
                    else
                        key_String = user_Names[i]+"_"+interview_Names[j-1];
                    chart_User_Based_View.ChartAreas.Add(key_String);
                    chart_User_Based_View.ChartAreas[key_String].Position = new System.Windows.Forms.DataVisualization.Charting.ElementPosition((100 / (interview_Names.Count + 1)) * j, (100 / user_Names.Count) * i, 100 / (interview_Names.Count + 1), 100 / user_Names.Count);
                    chart_User_Based_View.ChartAreas[key_String].InnerPlotPosition = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(20, 10, size, size);   //(0,(100-size)/2,size, size);
                    chart_User_Based_View.ChartAreas[key_String].BorderColor = Color.Black;
                    chart_User_Based_View.ChartAreas[key_String].BorderWidth = 1;
                    chart_User_Based_View.ChartAreas[key_String].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                    if (j == 0)
                    {
                        chart_User_Based_View.ChartAreas[key_String].Area3DStyle.Enable3D = true;
                        chart_User_Based_View.ChartAreas[key_String].Area3DStyle.PointDepth = 40;
                    }

                    System.Windows.Forms.DataVisualization.Charting.Title title = new System.Windows.Forms.DataVisualization.Charting.Title(key_String);
                    if (j == 0)
                        title.Text = user_Names[i];
                    else
                        title.Text = interview_Names[j-1]+"("+user_Names[i]+")";
                    title.Name = key_String;
                    title.DockedToChartArea = key_String;
                    title.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
                    float x_Title = (100 / (interview_Names.Count + 1));// + ((100/(interview_Names.Count+1))*(20/(float)100));
                    float y_Title = (100 / user_Names.Count);
                    title.Position = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(x_Title * j, y_Title * i, x_Title, y_Title * (10 / (float)100));//(0, temp_2, size, (100 - size) / 2);
                    title.IsDockedInsideChartArea = false;
                    title.Font = new Font(title.Font, FontStyle.Underline);
                    chart_User_Based_View.Titles.Add(title);

                    chart_User_Based_View.Legends.Add(key_String);
                    chart_User_Based_View.Legends[key_String].DockedToChartArea = key_String;
                    chart_User_Based_View.Legends[key_String].IsDockedInsideChartArea = false;
                    chart_User_Based_View.Legends[key_String].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
                    float x_Legend = (100 / (interview_Names.Count + 1));
                    float y_Legend = (100 / user_Names.Count);
                    float temp_y = (y_Legend * i) + (y_Legend * ((size + 10) / (float)100));
                    float temp_x = (x_Legend * (20 / (float)100));// (x_Legend * j) + (x_Legend * (20 / (float)100));
                    chart_User_Based_View.Legends[key_String].Position = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(temp_x, temp_y, 20, (100 - size - 10) / 2);
                    //chart_User_Based_View.Legends[key_String].Position = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(temp_x, temp_y, 20, 30);
                    //chart_User_Based_View.Legends[user_Names[i]].bre

                    System.Windows.Forms.DataVisualization.Charting.LegendCellColumn symbol_Column = new System.Windows.Forms.DataVisualization.Charting.LegendCellColumn();
                    symbol_Column.HeaderText = "Symbol";
                    //symbol_Column.HeaderFont = new Font("Arial",8,FontStyle.Regular);
                    symbol_Column.HeaderBackColor = Color.FromArgb(255, 252, 243, 244);
                    symbol_Column.ColumnType = System.Windows.Forms.DataVisualization.Charting.LegendCellColumnType.SeriesSymbol;
                    chart_User_Based_View.Legends[key_String].CellColumns.Add(symbol_Column);

                    System.Windows.Forms.DataVisualization.Charting.LegendCellColumn type_Column = new System.Windows.Forms.DataVisualization.Charting.LegendCellColumn();
                    type_Column.HeaderText = "Type";
                    //type_Column.HeaderFont = new Font("Arial", 8, FontStyle.Regular);
                    type_Column.HeaderBackColor = Color.FromArgb(255, 252, 243, 244);
                    type_Column.Text = "#AXISLABEL";
                    type_Column.Alignment = ContentAlignment.MiddleCenter;
                    type_Column.ColumnType = System.Windows.Forms.DataVisualization.Charting.LegendCellColumnType.Text;
                    chart_User_Based_View.Legends[key_String].CellColumns.Add(type_Column);

                    System.Windows.Forms.DataVisualization.Charting.LegendCellColumn count_Column = new System.Windows.Forms.DataVisualization.Charting.LegendCellColumn();
                    count_Column.HeaderText = "Count";
                    //count_Column.HeaderFont = new Font("Arial", 8, FontStyle.Regular);
                    count_Column.HeaderBackColor = Color.FromArgb(255, 252, 243, 244);
                    count_Column.Text = "#VALY";
                    count_Column.Alignment = ContentAlignment.MiddleCenter;
                    count_Column.ColumnType = System.Windows.Forms.DataVisualization.Charting.LegendCellColumnType.Text;
                    chart_User_Based_View.Legends[key_String].CellColumns.Add(count_Column);

                    chart_User_Based_View.Series.Add(key_String);
                    int total_Feed_Count = 0;
                    int total_Filter_Count = 0;
                    int total_Call_Count = 0;
                    if (j == 0)
                    {
                        if (Open_This_Mysql_Connection(ref mySql_Connection))
                        {
                            string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='"+company_Name+"' AND feeder='" + user_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "'";
                            MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                            total_Feed_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND filterer='" + user_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "'";
                            cmd = new MySqlCommand(query, mySql_Connection);
                            total_Filter_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND caller='" + user_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "'";
                            cmd = new MySqlCommand(query, mySql_Connection);
                            total_Call_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            Close_This_Mysql_Connection(ref mySql_Connection);
                        }
                    }
                    else
                    {
                        if (Open_This_Mysql_Connection(ref mySql_Connection))
                        {
                            string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND feeder='" + user_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "' AND interview_Name='" + interview_Names[j - 1] + "'";
                            MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                            total_Feed_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND filterer='" + user_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "' AND interview_Name='" + interview_Names[j - 1] + "'";
                            cmd = new MySqlCommand(query, mySql_Connection);
                            total_Filter_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND caller='" + user_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "' AND interview_Name='" + interview_Names[j - 1] + "'";
                            cmd = new MySqlCommand(query, mySql_Connection);
                            total_Call_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            Close_This_Mysql_Connection(ref mySql_Connection);
                        }
                    }

                    chart_User_Based_View.Series[key_String].Points.DataBindXY(new List<string> { "Feed", "Filter", "Call" }, new List<int> { total_Feed_Count, total_Filter_Count, total_Call_Count });
                    //chart_User_Based_View.Series[key_String].Points.DataBindXY(new List<string> { "Feed", "Filter", "Call" }, new List<int> { 5, 1, 2 });
                    chart_User_Based_View.Series[key_String].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
                    chart_User_Based_View.Series[key_String].ChartArea = key_String;
                    chart_User_Based_View.Series[key_String].IsValueShownAsLabel = true;
                    chart_User_Based_View.Series[key_String].LabelForeColor = Color.FromArgb(225,255,240,220);
                    chart_User_Based_View.Series[key_String].Legend = key_String;
                    chart_User_Based_View.Series[key_String].IsVisibleInLegend = true;
                    chart_User_Based_View.Series[key_String]["PieDrawingStyle"] = "SoftEdge";
                    chart_User_Based_View.Series[key_String].Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
                }
            }

            chart_User_Based_View.Visible = true;
            chart_User_Based_View.Location = new Point(0, 0);

            //Chart_Interview_Based_View

            List<string> roll_Names = new List<string> { "Feed","Filter","Call"};

            chart_Interview_Based_View.Legends.Clear();
            chart_Interview_Based_View.ChartAreas.Clear();
            chart_Interview_Based_View.Series.Clear();
            chart_Interview_Based_View.Titles.Clear();

            chart_Interview_Based_View.Width = (roll_Names.Count + 1) * 250;
            chart_Interview_Based_View.Height = interview_Names.Count * 250;
            //int size = 60;
            for (int i = 0; i < interview_Names.Count; ++i)
            {
                //int j = 0;
                for (int j = 0; j <= roll_Names.Count; ++j)
                {
                    string key_String = "";
                    if (j == 0)
                        key_String = interview_Names[i];
                    else
                        key_String = interview_Names[i] + "_" + roll_Names[j - 1];
                    chart_Interview_Based_View.ChartAreas.Add(key_String);
                    chart_Interview_Based_View.ChartAreas[key_String].Position = new System.Windows.Forms.DataVisualization.Charting.ElementPosition((100 / (roll_Names.Count + 1)) * j, (100 / interview_Names.Count) * i, 100 / (roll_Names.Count + 1), 100 / interview_Names.Count);
                    chart_Interview_Based_View.ChartAreas[key_String].InnerPlotPosition = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(20, 10, size, size);   //(0,(100-size)/2,size, size);
                    chart_Interview_Based_View.ChartAreas[key_String].BorderColor = Color.Black;
                    chart_Interview_Based_View.ChartAreas[key_String].BorderWidth = 1;
                    chart_Interview_Based_View.ChartAreas[key_String].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                    if (j == 0)
                    {
                        chart_Interview_Based_View.ChartAreas[key_String].Area3DStyle.Enable3D = true;
                        chart_Interview_Based_View.ChartAreas[key_String].Area3DStyle.PointDepth = 40;
                    }

                    System.Windows.Forms.DataVisualization.Charting.Title title = new System.Windows.Forms.DataVisualization.Charting.Title(key_String);
                    if (j == 0)
                        title.Text = interview_Names[i];
                    else
                        title.Text = roll_Names[j - 1] + "(" + interview_Names[i] + ")";
                    title.Name = key_String;
                    title.DockedToChartArea = key_String;
                    title.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
                    float x_Title = (100 / (roll_Names.Count + 1));// + ((100/(roll_Names.Count+1))*(20/(float)100));
                    float y_Title = (100 / interview_Names.Count);
                    title.Position = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(x_Title * j, y_Title * i, x_Title, y_Title * (10 / (float)100));//(0, temp_2, size, (100 - size) / 2);
                    title.IsDockedInsideChartArea = false;
                    title.Font = new Font(title.Font, FontStyle.Underline);
                    chart_Interview_Based_View.Titles.Add(title);

                    chart_Interview_Based_View.Legends.Add(key_String);
                    chart_Interview_Based_View.Legends[key_String].DockedToChartArea = key_String;
                    chart_Interview_Based_View.Legends[key_String].IsDockedInsideChartArea = false;
                    chart_Interview_Based_View.Legends[key_String].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
                    float x_Legend = (100 / (roll_Names.Count + 1));
                    float y_Legend = (100 / interview_Names.Count);
                    float temp_y = (y_Legend * i) + (y_Legend * ((size + 10) / (float)100));
                    float temp_x = x_Legend * j + (x_Legend * (10 / (float)100));
                    chart_Interview_Based_View.Legends[key_String].Position = new System.Windows.Forms.DataVisualization.Charting.ElementPosition(temp_x, temp_y, 20, 20 );
                    //chart_Interview_Based_View.Legends[interview_Names[i]].bre

                    System.Windows.Forms.DataVisualization.Charting.LegendCellColumn symbol_Column = new System.Windows.Forms.DataVisualization.Charting.LegendCellColumn();
                    symbol_Column.HeaderText = "Symbol";
                    //symbol_Column.HeaderFont = new Font("Arial",8,FontStyle.Regular);
                    symbol_Column.HeaderBackColor = Color.FromArgb(255, 252, 243, 244);
                    symbol_Column.ColumnType = System.Windows.Forms.DataVisualization.Charting.LegendCellColumnType.SeriesSymbol;
                    chart_Interview_Based_View.Legends[key_String].CellColumns.Add(symbol_Column);

                    System.Windows.Forms.DataVisualization.Charting.LegendCellColumn type_Column = new System.Windows.Forms.DataVisualization.Charting.LegendCellColumn();
                    type_Column.HeaderText = "Type";
                    //type_Column.HeaderFont = new Font("Arial", 8, FontStyle.Regular);
                    type_Column.HeaderBackColor = Color.FromArgb(255, 252, 243, 244);
                    type_Column.Text = "#AXISLABEL";
                    type_Column.Alignment = ContentAlignment.MiddleCenter;
                    type_Column.ColumnType = System.Windows.Forms.DataVisualization.Charting.LegendCellColumnType.Text;
                    chart_Interview_Based_View.Legends[key_String].CellColumns.Add(type_Column);

                    System.Windows.Forms.DataVisualization.Charting.LegendCellColumn count_Column = new System.Windows.Forms.DataVisualization.Charting.LegendCellColumn();
                    count_Column.HeaderText = "Count";
                    //count_Column.HeaderFont = new Font("Arial", 8, FontStyle.Regular);
                    count_Column.HeaderBackColor = Color.FromArgb(255, 252, 243, 244);
                    count_Column.Text = "#VALY";
                    count_Column.Alignment = ContentAlignment.MiddleCenter;
                    count_Column.ColumnType = System.Windows.Forms.DataVisualization.Charting.LegendCellColumnType.Text;
                    chart_Interview_Based_View.Legends[key_String].CellColumns.Add(count_Column);

                    chart_Interview_Based_View.Series.Add(key_String);
                    int total_Feed_Count = 0;
                    int total_Filter_Count = 0;
                    int total_Call_Count = 0;
                    List<int> user_Feed_Count = new List<int>();
                    List<int> user_Filter_Count = new List<int>();
                    List<int> user_Call_Count = new List<int>();
                    if (j == 0)
                    {
                        if (Open_This_Mysql_Connection(ref mySql_Connection))
                        {
                            string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND feeder IS NOT NULL AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "'";
                            MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                            total_Feed_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND filterer IS NOT NULL AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "'";
                            cmd = new MySqlCommand(query, mySql_Connection);
                            total_Filter_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND caller IS NOT NULL AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "'";
                            cmd = new MySqlCommand(query, mySql_Connection);
                            total_Call_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            Close_This_Mysql_Connection(ref mySql_Connection);
                        }
                    }
                    else
                    {
                        if (Open_This_Mysql_Connection(ref mySql_Connection))
                        {
                            for (int k = 0; k < user_Names.Count; ++k)
                            {
                                if (roll_Names[j-1] == "Feed")
                                {
                                    string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND feeder='" + user_Names[k] + "' AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "'";
                                    MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                    user_Feed_Count.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                                }

                                if (roll_Names[j-1] == "Filter")
                                {
                                    string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND filterer='" + user_Names[k] + "' AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "'";
                                    MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                    user_Filter_Count.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                                }

                                if (roll_Names[j-1] == "Call")
                                {
                                    string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND caller='" + user_Names[i] + "' AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + range_Start_Date.ToString("yyyy-MM-dd") + "' AND '" + range_End_Date.ToString("yyyy-MM-dd") + "'";
                                    MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                    user_Call_Count.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                                }
                            }
                            
                            Close_This_Mysql_Connection(ref mySql_Connection);
                        }
                    }
                    if (j == 0)
                        chart_Interview_Based_View.Series[key_String].Points.DataBindXY(new List<string> { "Feed", "Filter", "Call" }, new List<int> { total_Feed_Count, total_Filter_Count, total_Call_Count });
                    else
                    {
                        if(roll_Names[j-1] == "Feed")
                            chart_Interview_Based_View.Series[key_String].Points.DataBindXY(user_Names, user_Feed_Count);
                        else if(roll_Names[j-1] == "Filter")
                            chart_Interview_Based_View.Series[key_String].Points.DataBindXY(user_Names, user_Filter_Count);
                        else if(roll_Names[j-1] == "Call")
                            chart_Interview_Based_View.Series[key_String].Points.DataBindXY(user_Names, user_Call_Count);
                    }
                        
                    //chart_Interview_Based_View.Series[key_String].Points.DataBindXY(new List<string> { "Feed", "Filter", "Call" }, new List<int> { 5, 1, 2 });
                    chart_Interview_Based_View.Series[key_String].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
                    chart_Interview_Based_View.Series[key_String].ChartArea = key_String;
                    chart_Interview_Based_View.Series[key_String].IsValueShownAsLabel = true;
                    chart_Interview_Based_View.Series[key_String].Legend = key_String;
                    chart_Interview_Based_View.Series[key_String].IsVisibleInLegend = true;
                    chart_Interview_Based_View.Series[key_String]["PieDrawingStyle"] = "SoftEdge";
                    chart_Interview_Based_View.Series[key_String].Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;

                }
            }

            chart_Interview_Based_View.Visible = true;
            chart_Interview_Based_View.Location = new Point(0, 0);

            //chart_Comparison_View
            chart_Comparison_View.Legends.Clear();
            chart_Comparison_View.ChartAreas.Clear();
            chart_Comparison_View.Series.Clear();
            chart_Comparison_View.Titles.Clear();

            chart_Comparison_View.ChartAreas.Add("main");
            chart_Comparison_View.ChartAreas["main"].Area3DStyle.Enable3D = true;
            chart_Comparison_View.ChartAreas["main"].Area3DStyle.Inclination = 70;
            //chart_Comparison_View.ChartAreas["main"].Area3DStyle.Rotation = 40;
            chart_Comparison_View.ChartAreas["main"].Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
            chart_Comparison_View.ChartAreas["main"].Area3DStyle.Perspective = 1;
            chart_Comparison_View.ChartAreas["main"].Area3DStyle.IsClustered = true;

            chart_Comparison_View.Legends.Add("legend");
            chart_Comparison_View.Legends["legend"].DockedToChartArea = "main";
            chart_Comparison_View.Legends["legend"].IsDockedInsideChartArea = false;

            //temp_Series.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDot;

            List<string> x_Values = new List<string>();
            
            if (comboBox_Type_Report.SelectedItem.ToString() == "Daily")
            {
                for (DateTime temp_Datetime = dateTimePicker_From_Report.Value;dateTimePicker_To_Report.Value.Date.Subtract(temp_Datetime.Date).Days >= 0 ; temp_Datetime = temp_Datetime.AddDays(1))
                    x_Values.Add(temp_Datetime.ToShortDateString());

                foreach (string user_Name in user_Names)
                {
                    Color temp_Color = Color.FromArgb(0, 0, 0, 0);
                    for (int i = 0; i < roll_Names.Count; ++i)
                    {
                        string roll_Handler = "";
                        string date_Type = "";
                        if (roll_Names[i] == "Feed")
                        {
                            roll_Handler = "feeder";
                            date_Type = "feed_Date";
                        }
                        if (roll_Names[i] == "Filter")
                        {
                            roll_Handler = "filterer";
                            date_Type = "filter_Date";
                        }
                        if (roll_Names[i] == "Call")
                        {
                            roll_Handler = "caller";
                            date_Type = "call_Date";
                        }

                        List<int> y_Values = new List<int>();
                        if (Open_This_Mysql_Connection(ref mySql_Connection))
                        {
                            for (DateTime temp_Datetime = dateTimePicker_From_Report.Value; dateTimePicker_To_Report.Value.Date.Subtract(temp_Datetime.Date).Days >= 0; temp_Datetime = temp_Datetime.AddDays(1))
                            {
                                string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND " + roll_Handler + "='" + user_Name + "' AND DATE_FORMAT(" + date_Type + ",'%Y-%c-%d')='" + temp_Datetime.ToString("yyyy-M-dd") + "'";
                                MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                y_Values.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                            }
                            Close_This_Mysql_Connection(ref mySql_Connection);
                        }

                        string key_String = user_Name + "_" + roll_Names[i];
                        chart_Comparison_View.Series.Add(key_String);
                        
                        if (roll_Names[i] == "Feed")
                        {
                            chart_Comparison_View.Series[key_String].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                        }
                        if (roll_Names[i] == "Filter")
                        {
                            chart_Comparison_View.Series[key_String].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                        }
                        if (roll_Names[i] == "Call")
                            chart_Comparison_View.Series[key_String].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;

                        chart_Comparison_View.Series[key_String].Points.DataBindXY(x_Values, y_Values);

                        foreach (System.Windows.Forms.DataVisualization.Charting.DataPoint single_Point in chart_Comparison_View.Series[key_String].Points)
                        {
                            if (single_Point.YValues[0] == 0)
                            {
                                single_Point.IsValueShownAsLabel = false;
                                single_Point.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                            }
                            else
                            {
                                single_Point.IsValueShownAsLabel = true;
                                single_Point.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                            }
                        }

                        //chart_Comparison_View.ApplyPaletteColors();
                        //chart_Comparison_View.Series[key_String].Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
                        chart_Comparison_View.Series[key_String].ChartArea = "main";
                        chart_Comparison_View.Series[key_String].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                        chart_Comparison_View.Series[key_String]["ShowMarkerLines"] = "True";
                        chart_Comparison_View.Series[key_String]["PixelPointDepth"] = "2";
                        chart_Comparison_View.Series[key_String]["PixelPointGapDepth"] = "15";
                        chart_Comparison_View.Series[key_String].Legend = "legend";
                        chart_Comparison_View.Series[key_String].LegendText = user_Name +"("+roll_Names[i]+")";
                        
                        //MessageBox.Show(chart_Comparison_View.Series[key_String].Color.A.ToString()+"_"+chart_Comparison_View.Series[key_String].Color.R.ToString()+"_"+ chart_Comparison_View.Series[key_String].Color.B.ToString());
                        //if (temp_Color.A == 0 && temp_Color.R == 0 && temp_Color.G == 0 && temp_Color.B == 0)
                        //    temp_Color = chart_Comparison_View.Series[key_String].Color;
                        //else
                        //    chart_Comparison_View.Series[key_String].Color = temp_Color;
                    }
                }
                
            }
                //chart_Comparison_View.Legends[0].rows
            else if (comboBox_Type_Report.SelectedItem.ToString() == "Weekly")
            {
                List<DateTime> temp_From = get_This_Dates_Month_As_Weeks(dateTimePicker_From_Report.Value);
                int a_From = get_The_Week_No_Of_This_Date(dateTimePicker_From_Report.Value);
                List<DateTime> temp_To = get_This_Dates_Month_As_Weeks(dateTimePicker_To_Report.Value);
                int a_To = get_The_Week_No_Of_This_Date(dateTimePicker_To_Report.Value);

                List<string> week_Names = new List<string>();
                List<DateTime> week_Start_Dates = new List<DateTime>();

                for (int i = a_From - 1; i < temp_From.Count; ++i)
                {
                    if (dateTimePicker_To_Report.Value.Subtract(temp_From[i]).Days > 0)
                    {
                        week_Start_Dates.Add(temp_From[i]);
                        week_Names.Add((i + 1).ToString() + " Week " + temp_From[i].ToString("yyyy-MMM"));
                    }
                }

                for (int i = 0; i < a_To ; ++i)
                {
                    if (!week_Start_Dates.Contains(temp_To[i]))
                    {
                        bool smaller = false;

                        foreach (DateTime date in week_Start_Dates)
                            if (temp_To[i].Subtract(date).Days < 0)
                            {
                                smaller = true;
                                break;
                            }
                        if (!smaller)
                        {
                            if (temp_To[i].Subtract(dateTimePicker_To_Report.Value).Days < 7)
                            {
                                week_Start_Dates.Add(temp_To[i]);
                                week_Names.Add((i + 1).ToString() + " Week " + temp_To[i].ToString("yyyy-MMM"));
                            }
                        }
                    }
                }

                foreach (string user_Name in user_Names)
                {
                    Color temp_Color = Color.FromArgb(0, 0, 0, 0);
                    for (int i = 0; i < roll_Names.Count; ++i)
                    {
                        string roll_Handler = "";
                        string date_Type = "";
                        if (roll_Names[i] == "Feed")
                        {
                            roll_Handler = "feeder";
                            date_Type = "feed_Date";
                        }
                        if (roll_Names[i] == "Filter")
                        {
                            roll_Handler = "filterer";
                            date_Type = "filter_Date";
                        }
                        if (roll_Names[i] == "Call")
                        {
                            roll_Handler = "caller";
                            date_Type = "call_Date";
                        }

                        List<int> y_Values = new List<int>();
                        if (Open_This_Mysql_Connection(ref mySql_Connection))
                        {
                            for (int j=0;j<week_Start_Dates.Count;++j)
                            {
                                string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND " + roll_Handler + "='" + user_Name + "' AND DATE(" + date_Type + ") >= '" + week_Start_Dates[j].ToString("yyyy-M-dd") + "' AND DATE(" + date_Type + ") <= '" + week_Start_Dates[j].AddDays(6).ToString("yyyy-M-dd") + "'";
                                MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                y_Values.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                            }
                            Close_This_Mysql_Connection(ref mySql_Connection);
                        }

                        string key_String = user_Name + "_" + roll_Names[i];
                        chart_Comparison_View.Series.Add(key_String);

                        if (roll_Names[i] == "Feed")
                        {
                            chart_Comparison_View.Series[key_String].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                        }
                        if (roll_Names[i] == "Filter")
                        {
                            chart_Comparison_View.Series[key_String].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                        }
                        if (roll_Names[i] == "Call")
                            chart_Comparison_View.Series[key_String].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;

                        chart_Comparison_View.Series[key_String].Points.DataBindXY(week_Names, y_Values);

                        foreach (System.Windows.Forms.DataVisualization.Charting.DataPoint single_Point in chart_Comparison_View.Series[key_String].Points)
                        {
                            if (single_Point.YValues[0] == 0)
                            {
                                single_Point.IsValueShownAsLabel = false;
                                single_Point.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                            }
                            else
                            {
                                single_Point.IsValueShownAsLabel = true;
                                single_Point.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                            }
                        }

                        //chart_Comparison_View.ApplyPaletteColors();
                        //chart_Comparison_View.Series[key_String].Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
                        chart_Comparison_View.Series[key_String].ChartArea = "main";
                        chart_Comparison_View.Series[key_String].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                        chart_Comparison_View.Series[key_String]["ShowMarkerLines"] = "True";
                        chart_Comparison_View.Series[key_String]["PixelPointDepth"] = "2";
                        chart_Comparison_View.Series[key_String]["PixelPointGapDepth"] = "15";
                        chart_Comparison_View.Series[key_String].Legend = "legend";
                        chart_Comparison_View.Series[key_String].LegendText = user_Name + "(" + roll_Names[i] + ")";

                        //MessageBox.Show(chart_Comparison_View.Series[key_String].Color.A.ToString()+"_"+chart_Comparison_View.Series[key_String].Color.R.ToString()+"_"+ chart_Comparison_View.Series[key_String].Color.B.ToString());
                        //if (temp_Color.A == 0 && temp_Color.R == 0 && temp_Color.G == 0 && temp_Color.B == 0)
                        //    temp_Color = chart_Comparison_View.Series[key_String].Color;
                        //else
                        //    chart_Comparison_View.Series[key_String].Color = temp_Color;
                    }
                }
            }
            else if (comboBox_Type_Report.SelectedItem.ToString() == "Monthly")
            {
                List<string> x_Titles = new List<string>();
                DateTime temp_DateTime = new DateTime(dateTimePicker_From_Report.Value.Date.Year,dateTimePicker_From_Report.Value.Date.Month,1);
                for (; temp_DateTime.Year <= dateTimePicker_To_Report.Value.Date.Year && temp_DateTime.Month <= dateTimePicker_To_Report.Value.Date.Month; temp_DateTime = temp_DateTime.AddMonths(1))
                    x_Titles.Add(temp_DateTime.ToString("yyyy-MMM"));

                    foreach (string user_Name in user_Names)
                    {
                        Color temp_Color = Color.FromArgb(0, 0, 0, 0);
                        for (int i = 0; i < roll_Names.Count; ++i)
                        {
                            string roll_Handler = "";
                            string date_Type = "";
                            if (roll_Names[i] == "Feed")
                            {
                                roll_Handler = "feeder";
                                date_Type = "feed_Date";
                            }
                            if (roll_Names[i] == "Filter")
                            {
                                roll_Handler = "filterer";
                                date_Type = "filter_Date";
                            }
                            if (roll_Names[i] == "Call")
                            {
                                roll_Handler = "caller";
                                date_Type = "call_Date";
                            }

                            List<int> y_Values = new List<int>();
                            if (Open_This_Mysql_Connection(ref mySql_Connection))
                            {
                                DateTime temp_DateTime_1 = new DateTime(dateTimePicker_From_Report.Value.Date.Year, dateTimePicker_From_Report.Value.Date.Month, 1);
                                for (; temp_DateTime_1.Year <= dateTimePicker_To_Report.Value.Date.Year && temp_DateTime_1.Month <= dateTimePicker_To_Report.Value.Date.Month; temp_DateTime_1 = temp_DateTime_1.AddMonths(1))
                                {
                                    string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND " + roll_Handler + "='" + user_Name + "' AND DATE_FORMAT(" + date_Type + ",'%Y') = '" + temp_DateTime_1.ToString("yyyy") + "' AND DATE_FORMAT(" + date_Type + ",'%c') = '" + temp_DateTime_1.Month.ToString() + "'";
                                    MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                    y_Values.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                                }
                                Close_This_Mysql_Connection(ref mySql_Connection);
                            }

                            string key_String = user_Name + "_" + roll_Names[i];
                            chart_Comparison_View.Series.Add(key_String);

                            if (roll_Names[i] == "Feed")
                            {
                                chart_Comparison_View.Series[key_String].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                            }
                            if (roll_Names[i] == "Filter")
                            {
                                chart_Comparison_View.Series[key_String].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
                            }
                            if (roll_Names[i] == "Call")
                                chart_Comparison_View.Series[key_String].BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;

                            chart_Comparison_View.Series[key_String].Points.DataBindXY(x_Titles, y_Values);

                            foreach (System.Windows.Forms.DataVisualization.Charting.DataPoint single_Point in chart_Comparison_View.Series[key_String].Points)
                            {
                                if (single_Point.YValues[0] == 0)
                                {
                                    single_Point.IsValueShownAsLabel = false;
                                    single_Point.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
                                }
                                else
                                {
                                    single_Point.IsValueShownAsLabel = true;
                                    single_Point.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                                }
                            }

                            //chart_Comparison_View.ApplyPaletteColors();
                            //chart_Comparison_View.Series[key_String].Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Excel;
                            chart_Comparison_View.Series[key_String].ChartArea = "main";
                            chart_Comparison_View.Series[key_String].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                            chart_Comparison_View.Series[key_String]["ShowMarkerLines"] = "True";
                            chart_Comparison_View.Series[key_String]["PixelPointDepth"] = "2";
                            chart_Comparison_View.Series[key_String]["PixelPointGapDepth"] = "15";
                            chart_Comparison_View.Series[key_String].Legend = "legend";
                            chart_Comparison_View.Series[key_String].LegendText = user_Name + "(" + roll_Names[i] + ")";

                            //MessageBox.Show(chart_Comparison_View.Series[key_String].Color.A.ToString()+"_"+chart_Comparison_View.Series[key_String].Color.R.ToString()+"_"+ chart_Comparison_View.Series[key_String].Color.B.ToString());
                            //if (temp_Color.A == 0 && temp_Color.R == 0 && temp_Color.G == 0 && temp_Color.B == 0)
                            //    temp_Color = chart_Comparison_View.Series[key_String].Color;
                            //else
                            //    chart_Comparison_View.Series[key_String].Color = temp_Color;
                        }
                    }
            }
            chart_Comparison_View.Visible = true;
        }

        private int get_The_Week_No_Of_This_Date(DateTime given_DateTime)
        {
            List<DateTime> temp_Collection = get_This_Dates_Month_As_Weeks(given_DateTime);

            int week_No = -1;
            int gap = 1000;
            for(int i=0;i<temp_Collection.Count;++i)
            {
                if (given_DateTime.Date.Subtract(temp_Collection[i].Date).Days < 7 &&
                    given_DateTime.Date.Subtract(temp_Collection[i].Date).Days >= 0)
                    if (gap > given_DateTime.Date.Subtract(temp_Collection[i].Date).Days)
                    {
                        gap = given_DateTime.Date.Subtract(temp_Collection[i].Date).Days;
                        week_No = i + 1;
                    }
            }

            return week_No;
        }

        private List<DateTime> get_This_Dates_Month_As_Weeks(DateTime given_DateTime)
        {
            List<DateTime> temp_Collection = new List<DateTime>();
            DateTime temp_DateTime = new DateTime(given_DateTime.Year, given_DateTime.Month, 1);

            for (; temp_DateTime.Month == given_DateTime.Month; temp_DateTime = temp_DateTime.AddDays(1))
            {
                if (temp_DateTime.Day == 1)
                    temp_Collection.Add(temp_DateTime);
                else if(temp_DateTime.DayOfWeek.ToString() == "Sunday")
                    temp_Collection.Add(temp_DateTime);
            }

            return temp_Collection;
        }

        private void button_Excel_Click(object sender, EventArgs e)
        {
            if (comboBox_Type_Report.SelectedItem == null)
            {
                MessageBox.Show("Type is not selected");
                return;
            }

            if (dateTimePicker_To_Report.Value.Subtract(dateTimePicker_From_Report.Value).Days < 0)
            {
                MessageBox.Show("\"To Date\" Can't be lesser than the \"From Date\"");
                return;
            }

            Excel.Application excel_App = new Excel.Application();

            Object missing_Value = System.Reflection.Missing.Value;
            Excel.Workbook temp_Workbook = excel_App.Workbooks.Add(missing_Value);

            List<string> user_Names = new List<string>();
            List<string> interview_Names = new List<string>();

            MySqlConnection mySql_Connection = new MySqlConnection(connection_String);
            if (Open_This_Mysql_Connection(ref mySql_Connection))
            {
                string query = "SELECT DISTINCT name FROM userinfo";
                MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                    if (reader["name"] != DBNull.Value)
                        user_Names.Add(reader["name"].ToString());
                if (!reader.IsClosed)
                    reader.Close();

                query = "SELECT DISTINCT interview_Name FROM main_Data WHERE company_Name='" + company_Name + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "'";
                cmd = new MySqlCommand(query, mySql_Connection);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                    if (reader["interview_Name"] != DBNull.Value)
                        interview_Names.Add(reader["interview_Name"].ToString());
                if (!reader.IsClosed)
                    reader.Close();
                Close_This_Mysql_Connection(ref mySql_Connection);
            }

            // Chart_User_Based_View

            {
                Excel.Worksheet user_Based_Worksheet = temp_Workbook.Worksheets.get_Item(1);
                user_Based_Worksheet.Name = "User Based View";

                user_Based_Worksheet.Cells[1, 1] = "Users";
                user_Based_Worksheet.Cells[1, 2] = "Roll";
                user_Based_Worksheet.Cells[1, 3] = "Total";

                Excel.Range title_Range_User_Based = user_Based_Worksheet.get_Range(user_Based_Worksheet.Cells[1, 1] as Object, user_Based_Worksheet.Cells[1, 3] as object);
                title_Range_User_Based.Interior.Color = Color.AliceBlue;
                title_Range_User_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                title_Range_User_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                title_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
                title_Range_User_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                title_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;

                for (int i = 0; i < user_Names.Count; ++i)
                {
                    Excel.Range temp_Range_User_Based = user_Based_Worksheet.get_Range(user_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2, 1] as Excel.Range, user_Based_Worksheet.Cells[1 + ((i + 1) * 3), 1] as Excel.Range);
                    temp_Range_User_Based.Merge();
                    temp_Range_User_Based.set_Value(missing_Value, user_Names[i] as Object);
                    temp_Range_User_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    temp_Range_User_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                    temp_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    temp_Range_User_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                    if (i != 0)
                        temp_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    temp_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    temp_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;

                    Random random = new Random();
                    int r = random.Next(200, 255);
                    int g = random.Next(200, 255);
                    int b = random.Next(220, 255);
                    temp_Range_User_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                    Excel.Range feed_Range_User_Based = user_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 0, 2] as Excel.Range;
                    feed_Range_User_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    feed_Range_User_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    feed_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    feed_Range_User_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                    if (i != 0)
                        feed_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    feed_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    feed_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    feed_Range_User_Based.set_Value(missing_Value, "Feed" as Object);
                    feed_Range_User_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                    Excel.Range filter_Range_User_Based = user_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 1, 2] as Excel.Range;
                    filter_Range_User_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    filter_Range_User_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    filter_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    filter_Range_User_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                    filter_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    filter_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    filter_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    filter_Range_User_Based.set_Value(missing_Value, "Filter" as Object);
                    filter_Range_User_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                    Excel.Range call_Range_User_Based = user_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 2, 2] as Excel.Range;
                    call_Range_User_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    call_Range_User_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                    call_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    call_Range_User_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                    call_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    call_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    call_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    call_Range_User_Based.set_Value(missing_Value, "Call" as Object);
                    call_Range_User_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                    //int j = 0;
                    for (int j = 0; j <= interview_Names.Count; ++j)
                    {
                        //string key_String = "";
                        //if (j == 0)
                        //    key_String = user_Names[i];
                        //else
                        //    key_String = user_Names[i] + "_" + interview_Names[j - 1];

                        if (j > 0)
                        {
                            Excel.Range interview_Title_Range_User_Based = user_Based_Worksheet.Cells[1, j + 3] as Excel.Range;
                            interview_Title_Range_User_Based.Interior.Color = Color.AliceBlue;
                            interview_Title_Range_User_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                            interview_Title_Range_User_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                            interview_Title_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
                            interview_Title_Range_User_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                            interview_Title_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                            interview_Title_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                            interview_Title_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                            interview_Title_Range_User_Based.set_Value(missing_Value, interview_Names[j - 1]);
                        }

                        int total_Feed_Count = 0;
                        int total_Filter_Count = 0;
                        int total_Call_Count = 0;
                        if (j == 0)
                        {
                            if (Open_This_Mysql_Connection(ref mySql_Connection))
                            {
                                string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND feeder='" + user_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "'";
                                MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                total_Feed_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                                query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND filterer='" + user_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "'";
                                cmd = new MySqlCommand(query, mySql_Connection);
                                total_Filter_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                                query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND caller='" + user_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "'";
                                cmd = new MySqlCommand(query, mySql_Connection);
                                total_Call_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                                Close_This_Mysql_Connection(ref mySql_Connection);
                            }
                        }
                        else
                        {
                            if (Open_This_Mysql_Connection(ref mySql_Connection))
                            {
                                string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND feeder='" + user_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "' AND interview_Name='" + interview_Names[j - 1] + "'";
                                MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                total_Feed_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                                query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND filterer='" + user_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "' AND interview_Name='" + interview_Names[j - 1] + "'";
                                cmd = new MySqlCommand(query, mySql_Connection);
                                total_Filter_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                                query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND caller='" + user_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "' AND interview_Name='" + interview_Names[j - 1] + "'";
                                cmd = new MySqlCommand(query, mySql_Connection);
                                total_Call_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                                Close_This_Mysql_Connection(ref mySql_Connection);
                            }
                        }

                        Excel.Range feed_Total_Range_User_Based = user_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 0, j + 3] as Excel.Range;
                        feed_Total_Range_User_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        feed_Total_Range_User_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        feed_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                        feed_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                        if (i != 0)
                            feed_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        feed_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                        feed_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                        feed_Total_Range_User_Based.set_Value(missing_Value, total_Feed_Count as Object);
                        feed_Total_Range_User_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                        Excel.Range filter_Total_Range_User_Based = user_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 1, j + 3] as Excel.Range;
                        filter_Total_Range_User_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        filter_Total_Range_User_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        filter_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                        filter_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                        filter_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        filter_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                        filter_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                        filter_Total_Range_User_Based.set_Value(missing_Value, total_Filter_Count as Object);
                        filter_Total_Range_User_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                        Excel.Range call_Total_Range_User_Based = user_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 2, j + 3] as Excel.Range;
                        call_Total_Range_User_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        call_Total_Range_User_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        call_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                        call_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                        call_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        call_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                        call_Total_Range_User_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                        call_Total_Range_User_Based.set_Value(missing_Value, total_Call_Count as Object);
                        call_Total_Range_User_Based.Interior.Color = Color.FromArgb(255, r, g, b);
                    }
                }
            }
            
            //Chart_Interview_Based_View
            List<string> roll_Names = new List<string> { "Feed", "Filter", "Call" };
            Excel.Worksheet interview_Based_Worksheet = temp_Workbook.Worksheets.get_Item(2);
            interview_Based_Worksheet.Name = "Interview Based View";

            interview_Based_Worksheet.Cells[1, 1] = "Interviews";
            interview_Based_Worksheet.Cells[1, 2] = "Roll";
            interview_Based_Worksheet.Cells[1, 3] = "Total";

            Excel.Range title_Range_Interview_Based = interview_Based_Worksheet.get_Range(interview_Based_Worksheet.Cells[1, 1] as Object, interview_Based_Worksheet.Cells[1, 3] as object);
            title_Range_Interview_Based.Interior.Color = Color.AliceBlue;
            title_Range_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            title_Range_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            title_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
            title_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
            title_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;

            for (int z = 0; z < user_Names.Count; ++z)
            {
                Excel.Range interview_Title_Range_Interview_Based = interview_Based_Worksheet.Cells[1, z + 4] as Excel.Range;
                interview_Title_Range_Interview_Based.Interior.Color = Color.AliceBlue;
                interview_Title_Range_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                interview_Title_Range_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                interview_Title_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
                interview_Title_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                interview_Title_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                interview_Title_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                interview_Title_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                interview_Title_Range_Interview_Based.set_Value(missing_Value, user_Names[z]);
            }

            for (int i = 0; i < interview_Names.Count; ++i)
            {
                Excel.Range temp_Range_Interview_Based = interview_Based_Worksheet.get_Range(interview_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2, 1] as Excel.Range, interview_Based_Worksheet.Cells[1 + ((i + 1) * 3), 1] as Excel.Range);
                temp_Range_Interview_Based.Merge();
                temp_Range_Interview_Based.set_Value(missing_Value, interview_Names[i] as Object);
                temp_Range_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                temp_Range_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                temp_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                temp_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                if (i != 0)
                    temp_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                temp_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                temp_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;

                Random random = new Random();
                int r = random.Next(200, 255);
                int g = random.Next(200, 255);
                int b = random.Next(220, 255);
                temp_Range_Interview_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                Excel.Range feed_Range_Interview_Based = interview_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 0, 2] as Excel.Range;
                feed_Range_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                feed_Range_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                feed_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                feed_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                if (i != 0)
                    feed_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                feed_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                feed_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                feed_Range_Interview_Based.set_Value(missing_Value, "Feed" as Object);
                feed_Range_Interview_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                Excel.Range filter_Range_Interview_Based = interview_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 1, 2] as Excel.Range;
                filter_Range_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                filter_Range_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                filter_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                filter_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                filter_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                filter_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                filter_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                filter_Range_Interview_Based.set_Value(missing_Value, "Filter" as Object);
                filter_Range_Interview_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                Excel.Range call_Range_Interview_Based = interview_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 2, 2] as Excel.Range;
                call_Range_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                call_Range_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                call_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                call_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                call_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                call_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                call_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                call_Range_Interview_Based.set_Value(missing_Value, "Call" as Object);
                call_Range_Interview_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                for (int j = 0; j <= roll_Names.Count; ++j)
                {
                    int total_Feed_Count = 0;
                    int total_Filter_Count = 0;
                    int total_Call_Count = 0;
                    List<int> user_Feed_Count = new List<int>();
                    List<int> user_Filter_Count = new List<int>();
                    List<int> user_Call_Count = new List<int>();

                    if (j == 0)
                    {
                        if (Open_This_Mysql_Connection(ref mySql_Connection))
                        {
                            string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND feeder IS NOT NULL AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "'";
                            MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                            total_Feed_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND filterer IS NOT NULL AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "'";
                            cmd = new MySqlCommand(query, mySql_Connection);
                            total_Filter_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND caller IS NOT NULL AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "'";
                            cmd = new MySqlCommand(query, mySql_Connection);
                            total_Call_Count = Convert.ToInt32(cmd.ExecuteScalar().ToString());

                            Close_This_Mysql_Connection(ref mySql_Connection);
                        }


                    }
                    else
                    {
                        if (Open_This_Mysql_Connection(ref mySql_Connection))
                        {
                            for (int k = 0; k < user_Names.Count; ++k)
                            {
                                if (roll_Names[j - 1] == "Feed")
                                {
                                    string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND feeder='" + user_Names[k] + "' AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "'";
                                    MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                    user_Feed_Count.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                                }

                                if (roll_Names[j - 1] == "Filter")
                                {
                                    string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND filterer='" + user_Names[k] + "' AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "'";
                                    MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                    user_Filter_Count.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                                }

                                if (roll_Names[j - 1] == "Call")
                                {
                                    string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND caller='" + user_Names[k] + "' AND interview_Name='" + interview_Names[i] + "' AND feed_Date BETWEEN '" + dateTimePicker_From_Report.Value.ToString("yyyy-MM-dd") + "' AND '" + dateTimePicker_To_Report.Value.ToString("yyyy-MM-dd") + "'";
                                    MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                    user_Call_Count.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                                }
                            }
                            Close_This_Mysql_Connection(ref mySql_Connection);
                        }
                    }

                    if (j == 0)
                    {
                        Excel.Range feed_Total_Range_Interview_Based = interview_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 0, j + 3] as Excel.Range;
                        feed_Total_Range_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        feed_Total_Range_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        feed_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                        feed_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                        if (i != 0)
                            feed_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        feed_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                        feed_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                        feed_Total_Range_Interview_Based.set_Value(missing_Value, total_Feed_Count as Object);
                        feed_Total_Range_Interview_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                        Excel.Range filter_Total_Range_Interview_Based = interview_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 1, j + 3] as Excel.Range;
                        filter_Total_Range_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        filter_Total_Range_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        filter_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                        filter_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                        filter_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        filter_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                        filter_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                        filter_Total_Range_Interview_Based.set_Value(missing_Value, total_Filter_Count as Object);
                        filter_Total_Range_Interview_Based.Interior.Color = Color.FromArgb(255, r, g, b);

                        Excel.Range call_Total_Range_Interview_Based = interview_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 2, j + 3] as Excel.Range;
                        call_Total_Range_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        call_Total_Range_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                        call_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                        call_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                        call_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        call_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                        call_Total_Range_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                        call_Total_Range_Interview_Based.set_Value(missing_Value, total_Call_Count as Object);
                        call_Total_Range_Interview_Based.Interior.Color = Color.FromArgb(255, r, g, b);
                    }

                    else
                    {
                        if (roll_Names[j - 1] == "Feed")
                        {
                            for(int z=0;z<user_Names.Count;++z)
                            {
                                Excel.Range feed_Range_Of_Particularuser_Interview_Based = interview_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 0, z + 4] as Excel.Range;
                                feed_Range_Of_Particularuser_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                feed_Range_Of_Particularuser_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                feed_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                                feed_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                                if (i != 0)
                                    feed_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                                feed_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                                feed_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                                feed_Range_Of_Particularuser_Interview_Based.set_Value(missing_Value, user_Feed_Count[z]);
                                feed_Range_Of_Particularuser_Interview_Based.Interior.Color = Color.FromArgb(255, r, g, b);
                            }
                       }

                       else if (roll_Names[j - 1] == "Filter")
                       {
                           for(int z=0;z<user_Names.Count;++z)
                           {
                                Excel.Range filter_Range_Of_Particularuser_Interview_Based = interview_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 1, z + 4] as Excel.Range;
                                filter_Range_Of_Particularuser_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                filter_Range_Of_Particularuser_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                filter_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                                filter_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                                filter_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                                filter_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                                filter_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                                filter_Range_Of_Particularuser_Interview_Based.set_Value(missing_Value, user_Filter_Count[z]);
                                filter_Range_Of_Particularuser_Interview_Based.Interior.Color = Color.FromArgb(255, r, g, b);
                           }
                       }

                       else if (roll_Names[j - 1] == "Call")
                       {
                           for(int z=0;z<user_Names.Count;++z)
                           {
                                Excel.Range call_Range_Of_Particularuser_Interview_Based = interview_Based_Worksheet.Cells[1 + ((i + 1) * 3) - 2 + 2, z + 4] as Excel.Range;
                                call_Range_Of_Particularuser_Interview_Based.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                                call_Range_Of_Particularuser_Interview_Based.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                                call_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                                call_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                                call_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                                call_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                                call_Range_Of_Particularuser_Interview_Based.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                                call_Range_Of_Particularuser_Interview_Based.set_Value(missing_Value, user_Call_Count[z]);
                                call_Range_Of_Particularuser_Interview_Based.Interior.Color = Color.FromArgb(255, r, g, b);
                           }
                       }

                   }

               }

           }

           //comparison_Worksheet
           List<string> x_Values = new List<string>();
           Excel.Worksheet comparison_Worksheet = temp_Workbook.Worksheets.get_Item(3);
           comparison_Worksheet.Name = "Comparison View" + "(" + comboBox_Type_Report.SelectedItem.ToString() + ")";

           Excel.Range title_Range_Comparison_0 = comparison_Worksheet.Cells[1, 1];
           title_Range_Comparison_0.Interior.Color = Color.AliceBlue;
           title_Range_Comparison_0.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
           title_Range_Comparison_0.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
           title_Range_Comparison_0.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
           title_Range_Comparison_0.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
           title_Range_Comparison_0.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
           title_Range_Comparison_0.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
           title_Range_Comparison_0.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
           title_Range_Comparison_0.ShrinkToFit = true;
           title_Range_Comparison_0.Value = "Users";

           Excel.Range title_Range_Comparison_1 = comparison_Worksheet.Cells[1, 2];
           title_Range_Comparison_1.Interior.Color = Color.AliceBlue;
           title_Range_Comparison_1.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
           title_Range_Comparison_1.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
           title_Range_Comparison_1.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
           title_Range_Comparison_1.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
           title_Range_Comparison_1.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
           title_Range_Comparison_1.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
           title_Range_Comparison_1.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
           title_Range_Comparison_1.ShrinkToFit = true;
           title_Range_Comparison_1.Value = "Roll";
           
           Dictionary<string, Dictionary<string, List<int>>> main_Dict = new Dictionary<string, Dictionary<string, List<int>>>();

           if (comboBox_Type_Report.SelectedItem.ToString() == "Daily")
           {
               int s = 0;
               for (DateTime temp_Datetime = dateTimePicker_From_Report.Value; dateTimePicker_To_Report.Value.Date.Subtract(temp_Datetime.Date).Days >= 0; temp_Datetime = temp_Datetime.AddDays(1))
               {
                   x_Values.Add(temp_Datetime.ToShortDateString());
                   Excel.Range title_Range_Comparison = comparison_Worksheet.Cells[1, s + 3];
                   title_Range_Comparison.Interior.Color = Color.AliceBlue;
                   title_Range_Comparison.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                   title_Range_Comparison.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.WrapText = true;
                   title_Range_Comparison.Value = temp_Datetime.ToShortDateString();
                   ++s;
               }

                for(int j=0;j<user_Names.Count;++j)
                {
                    Color temp_Color = Color.FromArgb(0, 0, 0, 0);
                    Dictionary<string, List<int>> temp_Dict = new Dictionary<string, List<int>>();
                    for (int i = 0; i < roll_Names.Count; ++i)
                    {
                        string roll_Handler = "";
                        string date_Type = "";
                        if (roll_Names[i] == "Feed")
                        {
                            roll_Handler = "feeder";
                            date_Type = "feed_Date";
                        }
                        if (roll_Names[i] == "Filter")
                        {
                            roll_Handler = "filterer";
                            date_Type = "filter_Date";
                        }
                        if (roll_Names[i] == "Call")
                        {
                            roll_Handler = "caller";
                            date_Type = "call_Date";
                        }

                        List<int> y_Values = new List<int>();
                        if (Open_This_Mysql_Connection(ref mySql_Connection))
                        {
                            for (DateTime temp_Datetime = dateTimePicker_From_Report.Value; dateTimePicker_To_Report.Value.Date.Subtract(temp_Datetime.Date).Days >= 0; temp_Datetime = temp_Datetime.AddDays(1))
                            {
                                string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND " + roll_Handler + "='" + user_Names[j] + "' AND DATE_FORMAT(" + date_Type + ",'%Y-%c-%d')='" + temp_Datetime.ToString("yyyy-M-dd") + "'";
                                MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                                y_Values.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                            }
                            Close_This_Mysql_Connection(ref mySql_Connection);
                        }
                        temp_Dict.Add(roll_Names[i], y_Values);
                    }
                    main_Dict.Add(user_Names[j], temp_Dict);
                }
            }
           else if (comboBox_Type_Report.SelectedItem.ToString() == "Weekly")
           {
               List<DateTime> temp_From = get_This_Dates_Month_As_Weeks(dateTimePicker_From_Report.Value);
               int a_From = get_The_Week_No_Of_This_Date(dateTimePicker_From_Report.Value);
               List<DateTime> temp_To = get_This_Dates_Month_As_Weeks(dateTimePicker_To_Report.Value);
               int a_To = get_The_Week_No_Of_This_Date(dateTimePicker_To_Report.Value);

               List<string> week_Names = new List<string>();
               List<DateTime> week_Start_Dates = new List<DateTime>();

               for (int i = a_From - 1; i < temp_From.Count; ++i)
               {
                   if (dateTimePicker_To_Report.Value.Subtract(temp_From[i]).Days > 0)
                   {
                       week_Start_Dates.Add(temp_From[i]);
                       week_Names.Add((i + 1).ToString() + " Week " + temp_From[i].ToString("yyyy-MMM"));
                   }
               }

               for (int i = 0; i < a_To; ++i)
               {
                   if (!week_Start_Dates.Contains(temp_To[i]))
                   {
                       bool smaller = false;

                       foreach (DateTime date in week_Start_Dates)
                           if (temp_To[i].Subtract(date).Days < 0)
                           {
                               smaller = true;
                               break;
                           }
                       if (!smaller)
                       {
                           if (temp_To[i].Subtract(dateTimePicker_To_Report.Value).Days < 7)
                           {
                               week_Start_Dates.Add(temp_To[i]);
                               week_Names.Add((i + 1).ToString() + " Week " + temp_To[i].ToString("yyyy-MMM"));
                           }
                       }
                   }
               }

               for (int s = 0; s < week_Names.Count; ++s)
               {
                   Excel.Range title_Range_Comparison = comparison_Worksheet.Cells[1, s + 3];
                   title_Range_Comparison.Interior.Color = Color.AliceBlue;
                   title_Range_Comparison.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                   title_Range_Comparison.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.WrapText = true;
                   title_Range_Comparison.Value = week_Names[s];
               }

               for(int j=0;j<user_Names.Count;++j)
               {
                   Color temp_Color = Color.FromArgb(0, 0, 0, 0);
                   Dictionary<string, List<int>> temp_Dict = new Dictionary<string, List<int>>();
                   for (int i = 0; i < roll_Names.Count; ++i)
                   {
                       string roll_Handler = "";
                       string date_Type = "";
                       if (roll_Names[i] == "Feed")
                       {
                           roll_Handler = "feeder";
                           date_Type = "feed_Date";
                       }
                       if (roll_Names[i] == "Filter")
                       {
                           roll_Handler = "filterer";
                           date_Type = "filter_Date";
                       }
                       if (roll_Names[i] == "Call")
                       {
                           roll_Handler = "caller";
                           date_Type = "call_Date";
                       }

                       List<int> y_Values = new List<int>();
                       if (Open_This_Mysql_Connection(ref mySql_Connection))
                       {
                           for (int j1 = 0; j1 < week_Start_Dates.Count; ++j1)
                           {
                               string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND " + roll_Handler + "='" + user_Names[j] + "' AND DATE(" + date_Type + ") >= '" + week_Start_Dates[j1].ToString("yyyy-M-dd") + "' AND DATE(" + date_Type + ") <= '" + week_Start_Dates[j1].AddDays(6).ToString("yyyy-M-dd") + "'";
                               MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                               y_Values.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                           }
                           Close_This_Mysql_Connection(ref mySql_Connection);
                       }
                       temp_Dict.Add(roll_Names[i], y_Values);
                   }
                   main_Dict.Add(user_Names[j], temp_Dict);
               }
           }

           else if (comboBox_Type_Report.SelectedItem.ToString() == "Monthly")
           {
               List<string> x_Titles = new List<string>();
               DateTime temp_DateTime = new DateTime(dateTimePicker_From_Report.Value.Date.Year, dateTimePicker_From_Report.Value.Date.Month, 1);
               int s = 0;
               for (; temp_DateTime.Year <= dateTimePicker_To_Report.Value.Date.Year && temp_DateTime.Month <= dateTimePicker_To_Report.Value.Date.Month; temp_DateTime = temp_DateTime.AddMonths(1))
               {
                   x_Titles.Add(temp_DateTime.ToString("yyyy-MMM"));
                   Excel.Range title_Range_Comparison = comparison_Worksheet.Cells[1, s + 3];
                   title_Range_Comparison.Interior.Color = Color.AliceBlue;
                   title_Range_Comparison.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                   title_Range_Comparison.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                   title_Range_Comparison.WrapText = true;
                   title_Range_Comparison.Value = temp_DateTime.ToString("yyyy-MMM");
                   ++s;
               }

               for(int j=0;j<user_Names.Count;++j)
               {
                   Color temp_Color = Color.FromArgb(0, 0, 0, 0);
                   Dictionary<string, List<int>> temp_Dict = new Dictionary<string, List<int>>();
                   for (int i = 0; i < roll_Names.Count; ++i)
                   {
                       string roll_Handler = "";
                       string date_Type = "";
                       if (roll_Names[i] == "Feed")
                       {
                           roll_Handler = "feeder";
                           date_Type = "feed_Date";
                       }
                       if (roll_Names[i] == "Filter")
                       {
                           roll_Handler = "filterer";
                           date_Type = "filter_Date";
                       }
                       if (roll_Names[i] == "Call")
                       {
                           roll_Handler = "caller";
                           date_Type = "call_Date";
                       }

                       List<int> y_Values = new List<int>();
                       if (Open_This_Mysql_Connection(ref mySql_Connection))
                       {
                           DateTime temp_DateTime_1 = new DateTime(dateTimePicker_From_Report.Value.Date.Year, dateTimePicker_From_Report.Value.Date.Month, 1);
                           for (; temp_DateTime_1.Year <= dateTimePicker_To_Report.Value.Date.Year && temp_DateTime_1.Month <= dateTimePicker_To_Report.Value.Date.Month; temp_DateTime_1 = temp_DateTime_1.AddMonths(1))
                           {
                               string query = "SELECT COUNT(*) FROM main_Data WHERE company_Name='" + company_Name + "' AND " + roll_Handler + "='" + user_Names[j] + "' AND DATE_FORMAT(" + date_Type + ",'%Y') = '" + temp_DateTime_1.ToString("yyyy") + "' AND DATE_FORMAT(" + date_Type + ",'%c') = '" + temp_DateTime_1.Month.ToString() + "'";
                               MySqlCommand cmd = new MySqlCommand(query, mySql_Connection);
                               y_Values.Add(Convert.ToInt32(cmd.ExecuteScalar().ToString()));
                           }
                           Close_This_Mysql_Connection(ref mySql_Connection);
                       }
                       temp_Dict.Add(roll_Names[i], y_Values);
                   }
                   main_Dict.Add(user_Names[j], temp_Dict);
               }
           }

           int d = 0;
           foreach (KeyValuePair<string, Dictionary<string, List<int>>> temp_Pair in main_Dict)
           {
               Excel.Range temp_Range_Comparison = comparison_Worksheet.get_Range(comparison_Worksheet.Cells[1 + ((d + 1) * 3) - 2, 1] as Excel.Range, comparison_Worksheet.Cells[1 + ((d + 1) * 3), 1] as Excel.Range);
               temp_Range_Comparison.Merge();
               temp_Range_Comparison.set_Value(missing_Value, temp_Pair.Key as Object);
               temp_Range_Comparison.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
               temp_Range_Comparison.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

               temp_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
               temp_Range_Comparison.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
               if (d != 0)
                   temp_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
               temp_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
               temp_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;

               Random random = new Random();
               int r = random.Next(200, 255);
               int g = random.Next(200, 255);
               int b = random.Next(220, 255);
               temp_Range_Comparison.Interior.Color = Color.FromArgb(255, r, g, b);

               Excel.Range feed_Range_Comparison = comparison_Worksheet.Cells[1 + ((d + 1) * 3) - 2 + 0, 2] as Excel.Range;
               feed_Range_Comparison.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
               feed_Range_Comparison.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
               feed_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
               feed_Range_Comparison.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
               if (d != 0)
                   feed_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
               feed_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
               feed_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
               feed_Range_Comparison.set_Value(missing_Value, "Feed" as Object);
               feed_Range_Comparison.Interior.Color = Color.FromArgb(255, r, g, b);

               for (int k = 0; k < temp_Pair.Value["Feed"].Count; ++k)
               {
                   Excel.Range temp_Feed_Range_Comparison = comparison_Worksheet.Cells[1 + ((d + 1) * 3) - 2 + 0, k + 3] as Excel.Range;
                   temp_Feed_Range_Comparison.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                   temp_Feed_Range_Comparison.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                   temp_Feed_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Feed_Range_Comparison.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                   if (d != 0)
                       temp_Feed_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Feed_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Feed_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Feed_Range_Comparison.set_Value(missing_Value, temp_Pair.Value["Feed"][k]);
                   temp_Feed_Range_Comparison.Interior.Color = Color.FromArgb(255, r, g, b);
               }

               Excel.Range filter_Range_Comparison = comparison_Worksheet.Cells[1 + ((d + 1) * 3) - 2 + 1, 2] as Excel.Range;
               filter_Range_Comparison.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
               filter_Range_Comparison.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
               filter_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
               filter_Range_Comparison.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
               filter_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
               filter_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
               filter_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
               filter_Range_Comparison.set_Value(missing_Value, "Filter" as Object);
               filter_Range_Comparison.Interior.Color = Color.FromArgb(255, r, g, b);

               for (int k = 0; k < temp_Pair.Value["Filter"].Count; ++k)
               {
                   Excel.Range temp_Filter_Range_Comparison = comparison_Worksheet.Cells[1 + ((d + 1) * 3) - 2 + 1, k + 3] as Excel.Range;
                   temp_Filter_Range_Comparison.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                   temp_Filter_Range_Comparison.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                   temp_Filter_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Filter_Range_Comparison.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Filter_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Filter_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Filter_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Filter_Range_Comparison.set_Value(missing_Value, temp_Pair.Value["Filter"][k]);
                   temp_Filter_Range_Comparison.Interior.Color = Color.FromArgb(255, r, g, b);
               }

               Excel.Range call_Range_Comparison = comparison_Worksheet.Cells[1 + ((d + 1) * 3) - 2 + 2, 2] as Excel.Range;
               call_Range_Comparison.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
               call_Range_Comparison.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
               call_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
               call_Range_Comparison.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
               call_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
               call_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
               call_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
               call_Range_Comparison.set_Value(missing_Value, "Call" as Object);
               call_Range_Comparison.Interior.Color = Color.FromArgb(255, r, g, b);

               for (int k = 0; k < temp_Pair.Value["Call"].Count; ++k)
               {
                   Excel.Range temp_Call_Range_Comparison = comparison_Worksheet.Cells[1 + ((d + 1) * 3) - 2 + 2, k + 3] as Excel.Range;
                   temp_Call_Range_Comparison.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                   temp_Call_Range_Comparison.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                   temp_Call_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Call_Range_Comparison.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Call_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Call_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Call_Range_Comparison.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                   temp_Call_Range_Comparison.set_Value(missing_Value, temp_Pair.Value["Call"][k]);
                   temp_Call_Range_Comparison.Interior.Color = Color.FromArgb(255, r, g, b);
               }

               ++d;
           }
           excel_App.Visible = true;
        }

        void mouse_Got_Clicked_In_The_Form(object sender,System.Windows.Forms.MouseEventArgs e)
        {
            if (!groupBox1.Enabled)
            {
                LoginWindow login_Window = new LoginWindow();
                login_Window.button_Ok_Login.Click += new EventHandler(login_Ok_Button_Got_Clicked);
                login_Window.StartPosition = FormStartPosition.CenterScreen;
                login_Window.Show();
            }
        }

        void login_Ok_Button_Got_Clicked(object sender,EventArgs e)
        {
            Button ok_Button = sender as Button;
            LoginWindow login_Window = ok_Button.Parent.Parent as LoginWindow;
            string username = login_Window.textBox_Username_Login.Text;
            string password = login_Window.textBox_Password_Login.Text;

            string type = "Management";

            string query = "SELECT type FROM userinfo WHERE name='"+username+"' AND password='"+password+"' AND company_Name='"+company_Name+"'";
            MySqlConnection connection = new MySqlConnection(connection_String);
            string type_From_Database = "";
            if (Open_This_Mysql_Connection(ref connection))
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if(reader["type"] != DBNull.Value)
                        type_From_Database = (string)(reader["type"]);
                }
                Close_This_Mysql_Connection(ref connection);
            }

            if (type_From_Database == type)
            {
                login_Window.Hide();
                login_Window.Dispose();

                this.groupBox1.Enabled = true;
                this.tabControl1.Enabled = true;
                MessageBox.Show("Login Accepted");
                return;
            }
            else if(type_From_Database != "")
            {
                MessageBox.Show("Your privileges are not sufficient to access this");
            }
            else if (type_From_Database == "")
            {
                int count = 0;
                string username_Query = "SELECT COUNT(*) FROM userinfo WHERE name='"+username+"' AND company_Name='"+company_Name+"'";
                MySqlConnection small_Connection = new MySqlConnection(connection_String);
                if (Open_This_Mysql_Connection(ref small_Connection))
                {
                    MySqlCommand cmd = new MySqlCommand(username_Query, small_Connection);
                    count = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                    Close_This_Mysql_Connection(ref small_Connection);
                }

                if (count != 0)
                    MessageBox.Show("Password is wrong");
                else
                    MessageBox.Show("Username is wrong");
            }

            login_Window.textBox_Username_Login.Text = "";
            login_Window.textBox_Password_Login.Text = "";
            login_Window.Hide();
            login_Window.Dispose();
        }
    }
}

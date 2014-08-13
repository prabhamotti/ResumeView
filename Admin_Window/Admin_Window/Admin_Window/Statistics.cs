using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MySql.Data.MySqlClient;

namespace Admin_Window
{
    public partial class Statistics : Form
    {
        string data_Query_String;
        string pattern_String;
        int alias_Count;


        MySqlConnection mysql_Connection;
        System.Collections.Generic.Dictionary<string,string> combobox_Statistics_Values_Dict; //shown_Value,database_Value
        public Statistics(string given_Connection_String)
        {

            InitializeComponent();

            alias_Count = 0;
            data_Query_String = "";
            combobox_Statistics_Values_Dict = new Dictionary<string, string>();

            mysql_Connection = new MySqlConnection(given_Connection_String);
            string query = "SHOW COLUMNS IN main_Data";
            mysql_Connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, mysql_Connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            comboBox_X_Statistics.Items.Clear();
            checkedListBox_Y_Statistics.Items.Clear();
            while (reader.Read())
            {
                if(reader["Field"] != DBNull.Value)
                {
                    string value = (string)(reader["Field"]);
                    if (value == "call_Result")
                        combobox_Statistics_Values_Dict.Add("Qualified Status", value);
                    else
                    {
                        string replaced_Value = Char.ToUpper(value[0]) + value.Substring(1);
                        replaced_Value = replaced_Value.Replace("_", " ");
                        combobox_Statistics_Values_Dict.Add(replaced_Value, value);
                    }
                }
            }

            //combobox_Statistics_Values_Dict.Add("Daily", "Daily");
            //combobox_Statistics_Values_Dict.Add("Weekly", "Weekly");
            //combobox_Statistics_Values_Dict.Add("Monthly", "Monthly");

            comboBox_X_Statistics.Items.AddRange(combobox_Statistics_Values_Dict.Keys.ToArray());
            checkedListBox_Y_Statistics.Items.AddRange(combobox_Statistics_Values_Dict.Keys.Where(x => x != "Daily" && x != "Weekly" && x != "Monthly").Select(x => x).ToArray());


            mysql_Connection.Close();
        }

        private void comboBox_X_Statistics_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            dateTimePicker_From_X_Statistics.Enabled = false;
            dateTimePicker_To_X_Statistics.Enabled = false;
            dateTimePicker_From_Y_Statistics.Enabled = false;
            dateTimePicker_To_Y_Statistics.Enabled = false;

            if (comboBox_X_Statistics.SelectedItem.ToString() == "Daily"
                || comboBox_X_Statistics.SelectedItem.ToString() == "Weekly"
                || comboBox_X_Statistics.SelectedItem.ToString() == "Monthly")
            {
                dateTimePicker_From_X_Statistics.Enabled = true;
                checkedListBox_Y_Statistics.Items.Clear();

                string[] temp_Array = {"feeder","feed_Date","filterer","filter_Result","filter_Date",
                                       "caller","call_Result","call_Date","attended_Status",
                                       "attended_Status_Date","selected_Status",
                                       "selected_Status_Date","joined_Status","joined_Status_Date"};
                checkedListBox_Y_Statistics.Items.AddRange(temp_Array);
            }
            else
            {
                checkedListBox_Y_Statistics.Items.Clear();
                checkedListBox_Y_Statistics.Items.AddRange(combobox_Statistics_Values_Dict.Keys.Where(x => x != "Daily" && x != "Weekly" && x != "Monthly").Select(x => x).ToArray());
            }
        }

        private void button_Queue_Up_Statistics_Click(object sender, EventArgs e)
        {
            alias_Count += 1;

            if (data_Query_String == "")
                data_Query_String = "SELECT * FROM main_Data AS alias_" + alias_Count.ToString();
            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Interview Name")
            {
                List<string> interview_Names = new List<string>();
                for (int i = 0; i < checkedListBox_Values_Statistics.CheckedItems.Count; ++i)
                    interview_Names.Add(checkedListBox_Values_Statistics.CheckedItems[i].ToString());
                if (interview_Names.Contains("All"))
                {
                    interview_Names.Clear();
                    for (int i = 0; i < checkedListBox_Values_Statistics.Items.Count; ++i)
                        if (checkedListBox_Values_Statistics.Items[i].ToString() != "All")
                            interview_Names.Add(checkedListBox_Values_Statistics.Items[i].ToString());
                }
                if (interview_Names.Count > 0)
                {
                    interview_Names = interview_Names.Select(x => true ? "\"" + x + "\"" : x).ToList();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE interview_Name IN (" + string.Join(",", interview_Names) + ")";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "interview Names => (" + string.Join(",", interview_Names) + ")";
                }
            }

            if (comboBox_Type_Statistics.SelectedItem.ToString() == "User Name")
            {
                List<string> user_Names = new List<string>();
                for (int i = 0; i < checkedListBox_Values_Statistics.CheckedItems.Count; ++i)
                    user_Names.Add(checkedListBox_Values_Statistics.CheckedItems[i].ToString());
                if (user_Names.Contains("All"))
                {
                    user_Names.Clear();
                    for (int i = 0; i < checkedListBox_Values_Statistics.Items.Count; ++i)
                        if (checkedListBox_Values_Statistics.Items[i].ToString() != "All")
                            user_Names.Add(checkedListBox_Values_Statistics.Items[i].ToString());
                }
                if (user_Names.Count > 0)
                {
                    user_Names = user_Names.Select(x => true ? "\"" + x + "\"" : x).ToList();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE " +
                                        "feeder IN (" + string.Join(",", user_Names) + ") OR" +
                                        " filterer IN (" + string.Join(",", user_Names) + ") OR" +
                                        " caller IN (" + string.Join(",", user_Names) + ") OR" +
                                        " coordinator IN (" + string.Join(",", user_Names) + ")";


                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }
                    pattern_String += "User Names => (" + string.Join(",", user_Names) + ")";
                }
            }

            if (comboBox_Type_Statistics.SelectedItem.ToString() == "User Role")
            {
                List<string> user_Roles = new List<string>();
                for (int i = 0; i < checkedListBox_Values_Statistics.CheckedItems.Count; ++i)
                    user_Roles.Add(checkedListBox_Values_Statistics.CheckedItems[i].ToString());
                if (user_Roles.Contains("All"))
                {
                    user_Roles.Clear();
                    for (int i = 0; i < checkedListBox_Values_Statistics.Items.Count; ++i)
                        if (checkedListBox_Values_Statistics.Items[i].ToString() != "All")
                        {
                            string temp = checkedListBox_Values_Statistics.Items[i].ToString();
                            temp = char.ToLower(temp[0]) + temp.Substring(1);
                            user_Roles.Add((temp));
                        }
                }
                if (user_Roles.Count > 0)
                {
                    List<string> temp_0 = new List<string>();
                    foreach (string role in user_Roles)
                        temp_0.Add(role + " IS NOT NULL");
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE " +
                        string.Join(" OR ", temp_0);

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }
                    pattern_String += "User Roles => (" + string.Join(",", user_Roles) + ")";
                }
            }

            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Filtered Status")
            {
                List<string> user_Roles = new List<string>();
                for (int i = 0; i < checkedListBox_Values_Statistics.CheckedItems.Count; ++i)
                    user_Roles.Add(checkedListBox_Values_Statistics.CheckedItems[i].ToString());

                if (user_Roles.Contains("All") || (user_Roles.Contains("Filtered") && user_Roles.Contains("Blocked")))
                {
                    user_Roles.Clear();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE filter_Result IS NOT NULL";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Filtered Status => All ";
                }
                else if (user_Roles.Contains("Filtered"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE filter_Result=true";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Filtered Status => true ";
                }
                else if (user_Roles.Contains("Blocked"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE filter_Result=false";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Filtered Status => false ";
                }
            }

            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Qualified Status")
            {
                List<string> user_Roles = new List<string>();
                for (int i = 0; i < checkedListBox_Values_Statistics.CheckedItems.Count; ++i)
                    user_Roles.Add(checkedListBox_Values_Statistics.CheckedItems[i].ToString());

                if (user_Roles.Contains("All") || (user_Roles.Contains("Qualified") && user_Roles.Contains("Rejected")))
                {
                    user_Roles.Clear();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE call_Result IS NOT NULL";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Qualified Status => All ";
                }
                else if (user_Roles.Contains("Qualified"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE call_Result=true";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Qualified Status => true ";
                }
                else if (user_Roles.Contains("Rejected"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE call_Result=false";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Qualified Status => false ";
                }
            }

            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Attended Status")
            {
                List<string> attended_Statuses = new List<string>();
                for (int i = 0; i < checkedListBox_Values_Statistics.CheckedItems.Count; ++i)
                    attended_Statuses.Add(checkedListBox_Values_Statistics.CheckedItems[i].ToString());

                if (attended_Statuses.Contains("All") || (attended_Statuses.Contains("Attended") && attended_Statuses.Contains("Not Attended")))
                {
                    attended_Statuses.Clear();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE attended_Status IS NOT NULL";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Attended Status => All ";
                }
                else if (attended_Statuses.Contains("Attended"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE attended_Status=true";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Attended Status => true ";
                }
                else if (attended_Statuses.Contains("Not Attended"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE attended_Status=false";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Attended Status => false ";
                }
            }

            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Selected Status")
            {
                List<string> selected_Statuses = new List<string>();
                for (int i = 0; i < checkedListBox_Values_Statistics.CheckedItems.Count; ++i)
                    selected_Statuses.Add(checkedListBox_Values_Statistics.CheckedItems[i].ToString());

                if (selected_Statuses.Contains("All") || (selected_Statuses.Contains("Selected") && selected_Statuses.Contains("Not Selected")))
                {
                    selected_Statuses.Clear();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE selected_Status IS NOT NULL";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Selected Status => All ";
                }
                else if (selected_Statuses.Contains("Selected"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE selected_Status=true";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Selected Status => true ";
                }
                else if (selected_Statuses.Contains("Not Selected"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE selected_Status=false";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Selected Status => false ";
                }
            }
            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Joined Status")
            {
                List<string> joined_Statuses = new List<string>();
                for (int i = 0; i < checkedListBox_Values_Statistics.CheckedItems.Count; ++i)
                    joined_Statuses.Add(checkedListBox_Values_Statistics.CheckedItems[i].ToString());

                if (joined_Statuses.Contains("All") || (joined_Statuses.Contains("Joined") && joined_Statuses.Contains("Not Joined")))
                {
                    joined_Statuses.Clear();
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE joined_Status IS NOT NULL";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Joined Status => All ";
                }
                else if (joined_Statuses.Contains("Joined"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE joined_Status=true";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }

                    pattern_String += "Joined Status => true ";
                }
                else if (joined_Statuses.Contains("Not Joined"))
                {
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE joined_Status=false";

                    if (pattern_String != "")
                    {
                        pattern_String = pattern_String.Substring(0, pattern_String.LastIndexOf(";") + 1);
                        if (pattern_String != "")
                            pattern_String += ";";
                    }
                    pattern_String += "Joined Status => false ";
                }
            }
            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Date")
            {
                alias_Count += 1;
                if (data_Query_String != "")
                {
                    string from_Date_String = dateTimePicker_From_Data_Statistics.Value.ToString("yyyy-MM-dd");
                    from_Date_String = "\"" + from_Date_String + "\"";
                    string to_Date_String = dateTimePicker_To_Data_Statistics.Value.ToString("yyyy-MM-dd");
                    to_Date_String = "\"" + to_Date_String + "\"";
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE feeder_Date BETWEEN " + from_Date_String + " AND " + to_Date_String;
                    if (pattern_String != "")
                        pattern_String += ";";
                    pattern_String += "Duration => " + from_Date_String + " -> " + to_Date_String;

                    label_Pattern.Text = pattern_String;
                }
            }
            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Candidate Name Starts With" ||
                comboBox_Type_Statistics.SelectedItem.ToString() == "Candidate Name Has")
            {
                alias_Count += 1;
                if (comboBox_Type_Statistics.SelectedItem.ToString() == "Candidate Name Starts With")
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE  name LIKE " + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(textBox_Candidate_Name_Data_Statistics.Text + "%");
                else if (comboBox_Type_Statistics.SelectedItem.ToString() == "Candidate Name Has")
                    data_Query_String = "SELECT * FROM (" + data_Query_String + ") as alias_" + alias_Count.ToString() + " WHERE  name LIKE " + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote("%" + textBox_Candidate_Name_Data_Statistics.Text + "%");
                if (pattern_String != "")
                    pattern_String += ";";
                if (comboBox_Type_Statistics.SelectedItem.ToString() == "Candidate Name Starts With")
                    pattern_String += "Candidate Name Starts With => " + textBox_Candidate_Name_Data_Statistics.Text;
                else if (comboBox_Type_Statistics.SelectedItem.ToString() == "Candidate Name Has")
                    pattern_String += "Candidate Name Has => " + textBox_Candidate_Name_Data_Statistics.Text;
                label_Pattern.Text = pattern_String;
            }
            while (pattern_String.IndexOf(";;") != -1)
                pattern_String = pattern_String.Replace(";;", ";");

            label_Pattern.Text = pattern_String;
            comboBox_Type_Statistics_SelectedIndexChanged(null, EventArgs.Empty);
            comboBox_X_Statistics.Enabled = true;
            checkedListBox_Y_Statistics.Enabled = true;
        }

        private void button_Clear_Data_Statistics_Click(object sender, EventArgs e)
        {
            data_Query_String = "";
            pattern_String = "";
            label_Pattern.Text = pattern_String;
            checkedListBox_Values_Statistics.Items.Clear();
            textBox_Candidate_Name_Data_Statistics.Text = "";
            textBox_Candidate_Name_Data_Statistics.Enabled = false;
            dateTimePicker_From_Data_Statistics.Enabled = false;
            dateTimePicker_To_Data_Statistics.Enabled = false;
            comboBox_X_Statistics.Enabled = false;
            checkedListBox_Y_Statistics.ClearSelected();
            checkedListBox_Y_Statistics.Items.Clear();
            checkedListBox_Y_Statistics.Enabled = false;
            button_Save_Statistics.Enabled = false;

            chart_Statistics.Series.Clear();
            chart_Statistics.ChartAreas.Clear();
        }

        private void comboBox_Type_Statistics_SelectedIndexChanged(object sender, EventArgs e)
        {
            dateTimePicker_From_Data_Statistics.Enabled = false;
            dateTimePicker_To_Data_Statistics.Enabled = false;
            textBox_Candidate_Name_Data_Statistics.Enabled = false;
            textBox_Candidate_Name_Data_Statistics.Text = "";
            pattern_String += ";";

            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Interview Name")
            {
                checkedListBox_Values_Statistics.Items.Clear();
                checkedListBox_Values_Statistics.Items.Add("All");
                foreach (string key in form_Control_Panel.Interviews_And_Candidates_Count_Details.Keys)
                    checkedListBox_Values_Statistics.Items.Add(key);
            }

            if (comboBox_Type_Statistics.SelectedItem.ToString() == "User Name")
            {
                checkedListBox_Values_Statistics.Items.Clear();
                checkedListBox_Values_Statistics.Items.Add("All");
                foreach (string key in form_Control_Panel.users_Details.Keys)
                    if (key != "Admin")
                        checkedListBox_Values_Statistics.Items.Add(key);
            }

            if (comboBox_Type_Statistics.SelectedItem.ToString() == "User Role")
            {
                checkedListBox_Values_Statistics.Items.Clear();
                checkedListBox_Values_Statistics.Items.Add("All");
                checkedListBox_Values_Statistics.Items.Add("Feeder");
                checkedListBox_Values_Statistics.Items.Add("Filterer");
                checkedListBox_Values_Statistics.Items.Add("Caller");
                checkedListBox_Values_Statistics.Items.Add("Coordinator");
            }
            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Filtered Status")
            {
                checkedListBox_Values_Statistics.Items.Clear();
                checkedListBox_Values_Statistics.Items.Add("All");
                checkedListBox_Values_Statistics.Items.Add("Filtered");
                checkedListBox_Values_Statistics.Items.Add("Blocked");
            }
            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Qualified Status")
            {
                checkedListBox_Values_Statistics.Items.Clear();
                checkedListBox_Values_Statistics.Items.Add("All");
                checkedListBox_Values_Statistics.Items.Add("Qualified");
                checkedListBox_Values_Statistics.Items.Add("Rejected");
            }
            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Attended Status")
            {
                checkedListBox_Values_Statistics.Items.Clear();
                checkedListBox_Values_Statistics.Items.Add("All");
                checkedListBox_Values_Statistics.Items.Add("Attended");
                checkedListBox_Values_Statistics.Items.Add("Not Attended");
            }
            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Selected Status")
            {
                checkedListBox_Values_Statistics.Items.Clear();
                checkedListBox_Values_Statistics.Items.Add("All");
                checkedListBox_Values_Statistics.Items.Add("Selected");
                checkedListBox_Values_Statistics.Items.Add("Not Selected");
            }
            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Joined Status")
            {
                checkedListBox_Values_Statistics.Items.Clear();
                checkedListBox_Values_Statistics.Items.Add("All");
                checkedListBox_Values_Statistics.Items.Add("Joined");
                checkedListBox_Values_Statistics.Items.Add("Not Joined");
            }

            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Date")
            {
                checkedListBox_Values_Statistics.Items.Clear();
                dateTimePicker_From_Data_Statistics.Enabled = true;
            }

            if (comboBox_Type_Statistics.SelectedItem.ToString() == "Candidate Name Starts With" ||
                comboBox_Type_Statistics.SelectedItem.ToString() == "Candidate Name Has")
            {
                checkedListBox_Values_Statistics.Items.Clear();
                textBox_Candidate_Name_Data_Statistics.Enabled = true;
            }
        }

        private void dateTimePicker_From_Data_Statistics_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker_To_Data_Statistics.Enabled = true;
            dateTimePicker_To_Data_Statistics.MinDate = dateTimePicker_From_Data_Statistics.Value;
            dateTimePicker_To_Data_Statistics.Value = dateTimePicker_From_Data_Statistics.Value;
        }

        private void button_Generate_Click(object sender, EventArgs e)
        {
            if (comboBox_X_Statistics.SelectedItem == null)
            {
                MessageBox.Show("Selection wasn't made for X Axis Type");
                return;
            }
            if (checkedListBox_Y_Statistics.CheckedItems.Count == 0)
            {
                MessageBox.Show("No Item has been selected for the Y Axis types");
                return;
            }
            if (chart_Statistics.ChartAreas.Count == 0)
            {
                System.Windows.Forms.DataVisualization.Charting.ChartArea temp_ChartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
                temp_ChartArea.AlignmentOrientation = System.Windows.Forms.DataVisualization.Charting.AreaAlignmentOrientations.All;
                temp_ChartArea.AlignmentStyle = System.Windows.Forms.DataVisualization.Charting.AreaAlignmentStyles.All;
                chart_Statistics.ChartAreas.Add(temp_ChartArea);
            }

            chart_Statistics.Series.Clear();

            string type_X_Axis = combobox_Statistics_Values_Dict[comboBox_X_Statistics.SelectedItem.ToString()];
            string query = "";
            MySqlCommand cmd;
            MySqlDataReader reader;
            List<string> temp_X_List = new List<string>();
            if ((type_X_Axis == "Daily")
                ||(type_X_Axis == "Weekly")
                ||(type_X_Axis == "Monthly")
                )
            {
                DateTime start_Date = dateTimePicker_From_X_Statistics.Value;
                DateTime end_Date = dateTimePicker_To_X_Statistics.Value;
                if(type_X_Axis == "Daily")
                {
                    for(;end_Date >= start_Date ;start_Date = start_Date.AddDays(1))
                    temp_X_List.Add(start_Date.ToString("yyyy-MM-dd"));
                }
                else if(type_X_Axis == "Weekly")
                {
                    for(;end_Date >= start_Date ;start_Date = start_Date.AddDays(7))
                    {
                        int week_No = Useful_Functions.Useful_Functions.GetWeekInMonth(start_Date);
                        if (temp_X_List.Count > 0)
                            if(temp_X_List[temp_X_List.Count-1].Contains("Week 5") && week_No ==2 )
                                temp_X_List.Add(start_Date.ToString("yyyy-MM-") + "Week 1");
                        temp_X_List.Add(start_Date.ToString("yyyy-MM-")+"Week "+week_No);
                    }
                }
                else if(type_X_Axis == "Monthly")
                {
                    for(;end_Date >= start_Date ;start_Date = start_Date.AddMonths(1))
                        temp_X_List.Add(start_Date.ToString("yyyy-MMM"));
                }
            }
            else if (
                        (type_X_Axis == "name")
                        || (type_X_Axis == "phoneNo")
                        || (type_X_Axis == "mailId")
                        || (type_X_Axis == "url")
                        )
            {
                query = "SELECT DISTINCT " + type_X_Axis + " FROM (" + data_Query_String + ") AS alias_" + alias_Count;
                mysql_Connection.Open();
                cmd = new MySqlCommand(query, mysql_Connection);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                    if (reader[type_X_Axis] != DBNull.Value)
                        temp_X_List.Add((string)(reader[type_X_Axis]));
                    else if (!temp_X_List.Contains("NULL"))
                        temp_X_List.Add("NULL");
                mysql_Connection.Close();
                if (!reader.IsClosed)
                    reader.Close();
            }
            else if (
                        (type_X_Axis == "interview_Name")
                        || (type_X_Axis == "feeder")
                        || (type_X_Axis == "filterer")
                        || (type_X_Axis == "caller")
                        || (type_X_Axis == "coordinator")
                        )
            {
                query = "SELECT DISTINCT " + type_X_Axis + " FROM (" + data_Query_String + ") AS alias_" + alias_Count;
                mysql_Connection.Open();
                cmd = new MySqlCommand(query, mysql_Connection);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                    if (reader[type_X_Axis] != DBNull.Value)
                        temp_X_List.Add((string)(reader[type_X_Axis]));
                    else if (!temp_X_List.Contains("NULL"))
                        temp_X_List.Add("NULL");
                mysql_Connection.Close();
                if (!reader.IsClosed)
                    reader.Close();
            }
            else if (
                        (type_X_Axis == "feed_Date")
                        || (type_X_Axis == "filter_Date")
                        || (type_X_Axis == "call_Date")
                        || (type_X_Axis == "attended_Status_Date")
                        || (type_X_Axis == "selected_Status_Date")
                        || (type_X_Axis == "joined_Status_Date")
                        )
            {
                query = "SELECT DISTINCT DATE_FORMAT(" + type_X_Axis + ", '%Y-%c-%e') AS 'date' FROM(" + data_Query_String + ") AS alias_" + alias_Count ;

                //query = "SELECT DATE_FORMAT("+type_X_Axis +", '%e/%c/%Y') as 'date', COUNT(*) FROM("+ data_Query_String +") AS alias_"+ alias_Count+" GROUP BY 'date'";
                mysql_Connection.Open();
                cmd = new MySqlCommand(query, mysql_Connection);
                reader = cmd.ExecuteReader();
                
                while (reader.Read())
                    if (reader["date"] != DBNull.Value)
                    {
                        string temp_Date_String = (string)(reader["date"]);
                        DateTime temp_Datetime;
                        DateTime.TryParseExact(temp_Date_String, "yyyy-M-d", new System.Globalization.CultureInfo("en-US"), System.Globalization.DateTimeStyles.None, out temp_Datetime);
                        temp_X_List.Add(temp_Datetime.ToString("yyyy-MM-dd"));
                    }
                    else if (!temp_X_List.Contains("NULL"))
                        temp_X_List.Add("NULL");
                mysql_Connection.Close();
                if (!reader.IsClosed)
                    reader.Close();
            }
            else if ((type_X_Axis == "filter_Result")
                        || (type_X_Axis == "call_Result")
                        || (type_X_Axis == "attended_Status")
                        || (type_X_Axis == "selected_Status")
                        || (type_X_Axis == "joined_Status")
                        )
            {
                temp_X_List.AddRange(new string[]{"True","False","NULL"});
            }

            //List<List<int>> main_List = new List<List<int>>();

            Dictionary<string, Dictionary<string, int>> list_Of_Dict = new Dictionary<string, Dictionary<string, int>>();

            foreach (int selected_Index in checkedListBox_Y_Statistics.CheckedIndices)
            {
                string selected_String = checkedListBox_Y_Statistics.Items[selected_Index].ToString();
                string database_String_X = combobox_Statistics_Values_Dict[comboBox_X_Statistics.SelectedItem.ToString()];
                string database_String_Y = combobox_Statistics_Values_Dict[selected_String] ;
                List<int> temp_Y = new List<int>();

                /*
                if((database_String_Y == "Daily")
                    || (database_String_Y == "Weekly")
                    || (database_String_Y == "Monthly")
                    )
                {
                    //query = "SELECT COUNT(*) FROM("+data_Query_String +") as alias_" + alias_Count + 
                }
                 */
                if (
                    (database_String_Y == "name")
                    || (database_String_Y == "phoneNo")
                    || (database_String_Y == "mailId")
                    || (database_String_Y == "url")

                    || (database_String_Y == "interview_Name")
                    || (database_String_Y == "feeder")
                    || (database_String_Y == "filterer")
                    || (database_String_Y == "caller")
                    || (database_String_Y == "coordinator")

                    ||(database_String_Y == "feed_Date")
                    || (database_String_Y == "filter_Date")
                    || (database_String_Y == "call_Date")
                    || (database_String_Y == "attended_Status_Date")
                    || (database_String_Y == "selected_Status_Date")
                    || (database_String_Y == "joined_Status_Date")

                    ||(database_String_Y == "filter_Result")
                    || (database_String_Y == "call_Result")
                    || (database_String_Y == "attended_Status")
                    || (database_String_Y == "selected_Status")
                    || (database_String_Y == "joined_Status")
                    )
                {

                    List<string> temp = new List<string>();
                    if (
                        (database_String_Y.Contains("Result") || database_String_Y.Contains("Status"))
                        && (!database_String_Y.Contains("Date"))
                        )
                    {
                        temp.AddRange(new string[] { "true", "false", "null" });
                    }
                    else
                    {
                        if (database_String_Y.Contains("Date"))
                            query = "SELECT DISTINCT DATE_FORMAT(" + database_String_Y + ",'%Y-%c-%e') AS 'date' FROM (" + data_Query_String + ") AS alias_" + alias_Count;
                        else
                            query = "SELECT DISTINCT " + database_String_Y + " FROM(" + data_Query_String + ") AS alias_" + alias_Count;

                        mysql_Connection.Open();
                        cmd = new MySqlCommand(query, mysql_Connection);
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            if (database_String_Y.Contains("Date"))
                            {
                                if (reader["date"] != DBNull.Value)
                                    temp.Add((string)(reader["date"]));
                                else
                                    temp.Add("null");
                            }
                            else
                            {
                                if (reader[database_String_Y] != DBNull.Value)
                                    temp.Add((string)(reader[database_String_Y]));
                                else
                                    temp.Add("null");
                            }
                        }

                        if (!reader.IsClosed)
                            reader.Close();
                        mysql_Connection.Close();
                    }

                    string group_by_String ="";
                    if(database_String_X.Contains("Date"))
                        group_by_String = "GROUP BY DATE_FORMAT(" + database_String_X+",'%Y-%c-%e')";
                    else
                        group_by_String = "GROUP BY " + database_String_X;
                    foreach (string temp_String_0 in temp)
                    {
                        if(
                            (database_String_Y.Contains("Result") || database_String_Y.Contains("Status"))
                        && (!database_String_Y.Contains("Date"))
                            )
                        {
                            if (temp_String_0 == "null")
                                query = "SELECT " + database_String_X + ",COUNT(*) FROM (" + data_Query_String + ") AS alias_" + alias_Count + " WHERE " + database_String_Y + " IS NULL " + group_by_String;
                            else
                                query = "SELECT " + database_String_X + ",COUNT(" + database_String_Y + ") FROM (" + data_Query_String + ") AS alias_" + alias_Count + " WHERE " + database_String_Y + "=" + temp_String_0 + " " + group_by_String;
                        }
                        else if (database_String_Y.Contains("Date"))
                        {
                            if (temp_String_0 == "null")
                                query = "SELECT " + database_String_X + ",COUNT(*) FROM (" + data_Query_String + ") AS alias_" + alias_Count + " WHERE " + database_String_Y + " IS NULL " + group_by_String;
                            else
                                query = "SELECT " + database_String_X + ",COUNT(" + database_String_Y + ") FROM (" + data_Query_String + ") AS alias_" + alias_Count + " WHERE DATE_FORMAT(" + database_String_Y + ",'%Y-%c-%e')=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(temp_String_0) + " " + group_by_String;
                        }
                        else
                        {
                            if (temp_String_0 == "null")
                                query = "SELECT " + database_String_X + ",COUNT(*) FROM (" + data_Query_String + ") AS alias_" + alias_Count + " WHERE " + database_String_Y + " IS NULL " + group_by_String;
                            else
                                query = "SELECT " + database_String_X + ",COUNT(" + database_String_Y + ") FROM (" + data_Query_String + ") AS alias_" + alias_Count + " WHERE " + database_String_Y + "=" + Useful_Functions.Useful_Functions.Surround_It_With_Double_Quote(temp_String_0) + " " + group_by_String;
                        }
                        
                        Dictionary<string, int> temp_Dict = new Dictionary<string, int>();
                        temp_X_List.ForEach(x => temp_Dict.Add(x, 0));

                        mysql_Connection.Open();
                        cmd = new MySqlCommand(query, mysql_Connection);
                        reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string key = "";
                            int value = 0;
                            if (
                            (database_String_X.Contains("Result") || database_String_X.Contains("Status"))
                        && (!database_String_X.Contains("Date"))
                            )
                            {
                                if (reader[database_String_X] != DBNull.Value)
                                {
                                    bool temp_Status = reader[database_String_X].ToString()=="0"?false:true;
                                    key = temp_Status.ToString();
                                }
                            }
                            else if (database_String_X.Contains("Date"))
                            {
                                if (reader[database_String_X] != DBNull.Value)
                                {
                                    DateTime temp_Datetime = (DateTime)(reader[database_String_X]);
                                    key = temp_Datetime.ToString("yyyy-MM-dd");
                                }
                            }
                            else
                            {
                                if (reader[database_String_X] != DBNull.Value)
                                    key = (string)(reader[database_String_X]);
                            }
                            
                            if (temp_String_0 == "null")
                            {
                                if (reader["COUNT(*)"] != DBNull.Value)
                                    value = Convert.ToInt32(reader["COUNT(*)"]);
                            }
                            else
                            {
                                if (reader["COUNT(" + database_String_Y + ")"] != DBNull.Value)
                                    value = Convert.ToInt32(reader["COUNT(" + database_String_Y + ")"]);
                            }
                            
                            if (temp_Dict.ContainsKey(key))
                                temp_Dict[key] = value;
                        }
                        if (!reader.IsClosed)
                            reader.Close();
                        mysql_Connection.Close();
                        string final_String = temp_String_0 + " " + selected_String;
                        if (final_String == "true Filter Result") final_String = "Filtered";
                        if (final_String == "false Filter Result") final_String = "Not Filtered";
                        if (final_String == "null Filter Result") final_String = "null Filtered Result";
                        if (final_String == "true Qualified Status") final_String = "Qualified";
                        if (final_String == "false Qualified Status") final_String = "Not Qualified";
                        if (final_String == "null Qualified Status") final_String = "null Qualified Status";
                        if (final_String == "true Attended Status") final_String = "Attended";
                        if (final_String == "false Attended Status") final_String = "Didn't Attend";
                        if (final_String == "null Attended Status") final_String = "null Attended Status";
                        if (final_String == "true Selected Status") final_String = "Selected";
                        if (final_String == "false Selected Status") final_String = "Not Selected";
                        if (final_String == "null Selected Status") final_String = "null Selected Status";
                        if (final_String == "true Joined Status") final_String = "Joined";
                        if (final_String == "false Joined Status") final_String = "Didn't Join";
                        if (final_String == "null Joined Status") final_String = "null Joined Status";
                            
                        list_Of_Dict.Add(final_String, temp_Dict);
                    }

                }
                chart_Statistics.Series.Clear();

                foreach (KeyValuePair<string,Dictionary<string,int>> temp_Pair in list_Of_Dict)
                {
                    
                    System.Windows.Forms.DataVisualization.Charting.Series series = new System.Windows.Forms.DataVisualization.Charting.Series(temp_Pair.Key);
                    series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
                    series.IsValueShownAsLabel = true;
                    List<int> temp_List = new List<int>();
                    for (int i = 0; i < temp_X_List.Count; ++i)
                        temp_List.Add(temp_Pair.Value[temp_X_List[i]]);
                    series.Points.DataBindXY(temp_X_List, temp_List);
                    chart_Statistics.Series.Add(series);
                }
            }
            
            chart_Statistics.ChartAreas[0].Area3DStyle.Enable3D = true;
            chart_Statistics.ChartAreas[0].AxisX.LabelStyle.Angle = 45;
            chart_Statistics.ChartAreas[0].AxisX.Title = comboBox_X_Statistics.SelectedItem.ToString();
            Font temp_Font = new Font(chart_Statistics.ChartAreas[0].AxisX.TitleFont,FontStyle.Bold) ;
            chart_Statistics.ChartAreas[0].AxisX.TitleFont = temp_Font;
            button_Save_Statistics.Enabled = true;
        
            //chart_Statistics.ChartAreas[0].AxisX.TitleForeColor = Color.Silver;
            //chart_Statistics.ChartAreas[0].AxisX.LabelStyle.ForeColor = System.Drawing.Color.
        }

        private void checkedListBox_Y_Statistics_SelectedIndexChanged(object sender, EventArgs e)
        {
            for(int i=0;i<checkedListBox_Y_Statistics.SelectedItems.Count;++i)
                if ((checkedListBox_Y_Statistics.SelectedItems[i].ToString() == "Daily")
                    || (checkedListBox_Y_Statistics.SelectedItems[i].ToString() == "Weekly")
                    || (checkedListBox_Y_Statistics.SelectedItems[i].ToString() == "Monthly")
                    )
                {
                    dateTimePicker_From_Y_Statistics.Enabled = true;
                    //dateTimePicker_To_Y_Statistics.Enabled = true;
                }
        }

        private void dateTimePicker_From_X_Statistics_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker_To_X_Statistics.Enabled = true;
            dateTimePicker_To_X_Statistics.MinDate = dateTimePicker_From_X_Statistics.Value;
            dateTimePicker_To_X_Statistics.Value = dateTimePicker_From_X_Statistics.Value;
        }

        private void dateTimePicker_From_Y_Statistics_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker_To_Y_Statistics.Enabled = true;
            dateTimePicker_To_Y_Statistics.MinDate = dateTimePicker_From_Y_Statistics.Value;
            dateTimePicker_To_Y_Statistics.Value = dateTimePicker_From_Y_Statistics.Value;
        }

        private void button_Save_Statistics_Click(object sender, EventArgs e)
        {
            SaveFileDialog temp = new SaveFileDialog();
            temp.Filter = "GIF Format(*.gif)|*.gif|JPEG Format(*.jpeg)|*.jpeg|PNG Format(*.png)|*.png";
            temp.ShowDialog();
            temp.AddExtension = true;
            
            if (temp.FileName != "")
            {
                System.Drawing.Imaging.ImageFormat temp_Format = System.Drawing.Imaging.ImageFormat.Jpeg;
                if (temp.FilterIndex == 0)
                    temp_Format = System.Drawing.Imaging.ImageFormat.Gif;
                else if (temp.FilterIndex == 1)
                    temp_Format = System.Drawing.Imaging.ImageFormat.Jpeg;
                else if (temp.FilterIndex == 2)
                    temp_Format = System.Drawing.Imaging.ImageFormat.Png;

                chart_Statistics.SaveImage(temp.FileName,temp_Format);
                MessageBox.Show("Image has been saved");
            }

        }
    }
}

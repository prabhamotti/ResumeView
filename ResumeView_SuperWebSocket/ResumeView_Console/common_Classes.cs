using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common_Classes
{
    public class User_Detail
    {
        public string username { private set; get; }
        public string encrypted_General_Password { private set; get; }
        public string phone_No { private set; get; }
        public string type { private set; get; }
        public string column_Name { private set; get; }
        public string image_Path { private set; get; }
        public string mail_Id { private set; get; }
        public string encrypted_Mail_Password { private set; get; }
        //public System.Drawing.Image image;
        public User_Detail(string given_Username, string given_Encrypted_General_Password, string given_Phone_No, string given_Type, string given_Column_Name, string given_Image_Path, string given_Mail_Id, string given_Encrypted_Mail_Password)
        {
            username = given_Username;
            encrypted_General_Password = given_Encrypted_General_Password;
            phone_No = given_Phone_No;
            type = given_Type;
            column_Name = given_Column_Name;
            image_Path = given_Image_Path;
            mail_Id = given_Mail_Id;
            encrypted_Mail_Password = given_Encrypted_Mail_Password;
        }

        public User_Detail(string content_String)
        {
            string[] temp = content_String.Split("\u00A9".ToCharArray());
            username = temp[0];
            encrypted_General_Password = temp[1];
            phone_No = temp[2];
            type = temp[3];
            column_Name = temp[4];
            image_Path = temp[5];
            mail_Id = temp[6];
            encrypted_Mail_Password = temp[7];
        }

        public string get_As_String()
        {
            return username + "\u00A9" + encrypted_General_Password + "\u00A9" + phone_No + "\u00A9" + type + "\u00A9" + column_Name + "\u00A9" + image_Path + "\u00A9" + mail_Id + "\u00A9" + encrypted_Mail_Password;
        }
    }

    public class Interview_And_Candidates_Count_Detail
    {
        public string interview_Name { private set; get; }
        public int feed_Count { private set; get; }
        public int filtered_Count { private set; get; }
        public int feeded_But_Not_Filtered_Count { private set; get; }
        public int qualified_Count { private set; get; }
        public int filtered_But_Not_Qualified_Count { private set; get; }
        public DateTime date{ private set; get; }
        public string coordinator{ private set; get; }
        public int attended_Count { private set; get; }
        public int qualified_But_Not_Attended_Count { private set; get; }
        public int selected_Count { private set; get; }
        public int attended_But_Not_Selected_Count { private set; get; }
        public int joined_Count { private set; get; }
        public int selected_But_Not_Joined_Count { private set; get; }
        public decimal payment { private set; get; }
        public bool settled { private set; get; }
        public string venue { private set; get; }
        
        public Interview_And_Candidates_Count_Detail(string given_Interview_Name,int given_Feed_Count, int given_Filtered_Count,int given_Feeded_But_Not_Filtered_Count,int given_Qualified_Count,int given_Filtered_But_Not_Qualified_Count,DateTime given_Date,string given_Coordinator,string given_Venue,int given_Attended_Count,int given_Qualified_But_Not_Attended_Count,int given_Selected_Count,int given_Attended_But_Not_Selected_Count,int given_Joined_Count,int given_Selected_But_Not_Joined_Count,decimal given_Payment,bool given_Settled)
        {
            interview_Name = given_Interview_Name;
            feed_Count = given_Feed_Count;
            filtered_Count = given_Filtered_Count;
            feeded_But_Not_Filtered_Count = given_Feeded_But_Not_Filtered_Count;
            qualified_Count = given_Qualified_Count;
            filtered_But_Not_Qualified_Count = given_Filtered_But_Not_Qualified_Count;
            date = given_Date;
            coordinator = given_Coordinator;
            venue = given_Venue;
            attended_Count = given_Attended_Count;
            qualified_But_Not_Attended_Count = given_Qualified_But_Not_Attended_Count;
            selected_Count = given_Selected_Count;
            attended_But_Not_Selected_Count = given_Attended_But_Not_Selected_Count;
            joined_Count = given_Joined_Count;
            selected_But_Not_Joined_Count = given_Selected_But_Not_Joined_Count;
            payment = given_Payment;
            settled = given_Settled;
        }

        public Interview_And_Candidates_Count_Detail(string given_Interview_Name,DateTime given_Date, string given_Coordinator,string given_Venue)
        {
            interview_Name = given_Interview_Name;
            feed_Count = 0;
            filtered_Count = 0;
            feeded_But_Not_Filtered_Count = 0;
            qualified_Count = 0;
            filtered_But_Not_Qualified_Count = 0;
            date = given_Date;
            coordinator = given_Coordinator;
            venue = given_Venue;
            attended_Count = 0;
            qualified_But_Not_Attended_Count = 0;
            selected_Count = 0;
            attended_But_Not_Selected_Count = 0;
            joined_Count = 0;
            selected_But_Not_Joined_Count = 0;
            payment = 0;
            settled = false;
        }

        public Interview_And_Candidates_Count_Detail(string given_Interview_Name, DateTime given_Date, string given_Coordinator,string given_Venue,decimal given_Payment,bool given_Settled)
        {
            interview_Name = given_Interview_Name;
            feed_Count = 0;
            filtered_Count = 0;
            feeded_But_Not_Filtered_Count = 0;
            qualified_Count = 0;
            filtered_But_Not_Qualified_Count = 0;
            date = given_Date;
            coordinator = given_Coordinator;
            venue = given_Venue;
            attended_Count = 0;
            qualified_But_Not_Attended_Count = 0;
            selected_Count = 0;
            attended_But_Not_Selected_Count = 0;
            joined_Count = 0;
            selected_But_Not_Joined_Count = 0;
            payment = given_Payment;
            settled = given_Settled;
        }

        public Interview_And_Candidates_Count_Detail(string content_String)
        {
            string[] temp = content_String.Split("\u00A9".ToCharArray());

            interview_Name = (temp[0]);
            feed_Count = Convert.ToInt32(temp[1]);
            filtered_Count = Convert.ToInt32(temp[2]);
            feeded_But_Not_Filtered_Count = Convert.ToInt32(temp[3]);
            qualified_Count = Convert.ToInt32(temp[4]);
            filtered_But_Not_Qualified_Count = Convert.ToInt32(temp[5]);
            DateTime temp_Date;
            DateTime.TryParse(temp[6],out temp_Date);
            date = temp_Date;
            coordinator = (temp[7]);
            venue = (temp[8]);
            attended_Count = Convert.ToInt32(temp[9]);
            qualified_But_Not_Attended_Count = Convert.ToInt32(temp[10]);
            selected_Count = Convert.ToInt32(temp[11]);
            attended_But_Not_Selected_Count = Convert.ToInt32(temp[12]);
            joined_Count = Convert.ToInt32(temp[13]);
            selected_But_Not_Joined_Count = Convert.ToInt32(temp[14]);
            payment = Convert.ToDecimal(temp[15]);
            settled = Convert.ToBoolean(temp[16]);
        }

        public string get_As_String()
        {
            return interview_Name + "\u00A9" + feed_Count.ToString() + "\u00A9" + filtered_Count.ToString() + "\u00A9" + feeded_But_Not_Filtered_Count.ToString() + "\u00A9" + qualified_Count.ToString() + "\u00A9" + filtered_But_Not_Qualified_Count.ToString() + "\u00A9" + date.ToString("yyyy-MM-dd HH:mm:ss") + "\u00A9" + coordinator + "\u00A9" + venue + "\u00A9" + attended_Count.ToString() + "\u00A9" + qualified_But_Not_Attended_Count.ToString() + "\u00A9" + selected_Count.ToString() + "\u00A9" + attended_But_Not_Selected_Count.ToString() + "\u00A9" + joined_Count.ToString() + "\u00A9" + selected_But_Not_Joined_Count.ToString() + "\u00A9" + payment.ToString() + "\u00A9" + settled.ToString();
        }
    }

    public class Job
    {
        public enum Status
        {
            Start,
            Middle,
            End,
            Beyond_The_Limit
        };

        public enum Time_Gap_Type
        {
            Hour,
            Minute
        }

        public enum Reminder_Type
        {
            SMS,
            Mail,
            Computer
        }

        public string image_Path { get; private set; }
        public string company_Name { get; private set; }
        public List<string> job_Doiers_Names { get; private set; }
        public List<string> job_Doiers_Mail_Ids { get; private set; }
        public List<string> job_Doiers_Mobile_Nos { get; private set; }
        public string creator_Name { get; private set; }
        public string subject { get; private set; }
        public string body { get; private set; }
        public string notes { get; private set; }
        public int time_Gap_Of_Minutes_Or_Hours { get; private set; }
        public Time_Gap_Type time_Gap_Type { get; private set; }
        public List<Reminder_Type> reminder_Types;
        public DateTime create_Time { get; private set; }
        public DateTime start_Time { get; private set; }
        public DateTime end_Time { get; private set; }
        public string id { get; private set; }
        public DateTime last_Reminded_Time { get; private set; }
        public TimeZoneInfo creators_Time_Zone { get; private set; }
        public Job(string given_Image_Path,
                                string given_Company_Name,
                                List<string> given_Job_Doiers_Names,
                                List<string> given_Job_Doiers_Mail_Ids,
                                List<string> given_Job_Doiers_Mobile_Nos,
                                string given_Creator_Name,
                                string given_Subject,
                                string given_Body,
                                string given_Notes,
                                int given_Time_Gap_Of_Minutes_Or_Hours,
                                Time_Gap_Type given_Time_Gap_Type,
                                List<Reminder_Type> given_Reminder_Types,
                                DateTime given_Create_Time,
                                DateTime given_Start_Time,
                                DateTime given_End_Time,
                                string given_Id,
                                DateTime given_Last_Reminded_Time,
                                TimeZoneInfo given_Creators_Time_Zone)
        {
            image_Path = given_Image_Path;
            company_Name = given_Company_Name;
            job_Doiers_Names = given_Job_Doiers_Names;
            job_Doiers_Mail_Ids = given_Job_Doiers_Mail_Ids;
            job_Doiers_Mobile_Nos = given_Job_Doiers_Mobile_Nos;
            creator_Name = given_Creator_Name;
            subject = given_Subject;
            body = given_Body;
            notes = given_Notes;
            time_Gap_Of_Minutes_Or_Hours = given_Time_Gap_Of_Minutes_Or_Hours;
            time_Gap_Type = given_Time_Gap_Type;
            reminder_Types = given_Reminder_Types;
            create_Time = given_Create_Time;
            start_Time = given_Start_Time;
            end_Time = given_End_Time;
            id = given_Id;
            last_Reminded_Time = given_Last_Reminded_Time;
            creators_Time_Zone = given_Creators_Time_Zone;
        }

        void change_Image_Path(string given_Image_Path)
        {
            image_Path = given_Image_Path;
        }

        void add_Job_Doier(string job_Doier_Name, string job_Doier_Mail_Id, string job_Doier_Mobile_No)
        {
            job_Doiers_Names.Add(job_Doier_Name);
            job_Doiers_Mail_Ids.Add(job_Doier_Mail_Id);
            job_Doiers_Mobile_Nos.Add(job_Doier_Mobile_No);
        }

        public Status get_Status(DateTime given_Time)
        {
            TimeSpan reference_TimeSpan = end_Time - given_Time;
            double a = (end_Time - given_Time).TotalMilliseconds/ 3;
            TimeSpan time_Part_1 = TimeSpan.FromMilliseconds(a);
            TimeSpan time_Part_2 = TimeSpan.FromMilliseconds((time_Part_1.TotalMilliseconds) * 2);
            TimeSpan temp_TimeSpan = DateTime.Now.ToUniversalTime() - given_Time;

            if(temp_TimeSpan.TotalMilliseconds > reference_TimeSpan.TotalMilliseconds)
                return Status.Beyond_The_Limit;
            else
            {
                if (temp_TimeSpan <= time_Part_1)
                    return Status.Start;
                else if (temp_TimeSpan <= time_Part_2)
                    return Status.Middle;
                else
                    return Status.End;
            }
        }
    }

    public class Company_Info
    {
        public string company_Name { get; private set; }
        public string webPage_Base_Address { get; private set; }
        public string ftp_Home_Folder_Path { get; private set; }
        public DateTime expiry_Date { get; private set; }
        public string message { get; private set; }
        public string message_Image_Path { get; private set; }
        public int sms_Count { get; private set; }
        public Company_Info(string given_Company_Name,string given_WebPage_Base_Address,
            string given_Ftp_Home_Folder_Path,DateTime given_Expiry_Date,string given_Message,
            string given_Message_Image_Path,int given_Sms_Count)
        {
            company_Name = given_Company_Name;
            webPage_Base_Address = given_WebPage_Base_Address;
            ftp_Home_Folder_Path = given_Ftp_Home_Folder_Path;
            expiry_Date = given_Expiry_Date;
            message = given_Message;
            message_Image_Path = given_Message_Image_Path;
            sms_Count = given_Sms_Count;
        }
    }
}

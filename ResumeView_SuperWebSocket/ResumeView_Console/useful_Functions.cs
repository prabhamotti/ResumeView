using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;

namespace Useful_Functions
{
    class Useful_Functions
    {
        public static string get_Guid_String()
        {
            Guid guid = System.Guid.NewGuid();
            return "rv_" + guid.ToString("N");
        }

        public static Image get_Image_From_This_Http_Address(string http_Path)
        {
            Image temp_Image = null;
            string temp_File_Path = Path.GetTempFileName();
            WebClient web_Client = new WebClient();
            try
            {
                web_Client.DownloadFile(http_Path, temp_File_Path);
                temp_Image = Image.FromFile(temp_File_Path);
            }
            catch (WebException excp)
            {
                return temp_Image;
            }
            return temp_Image;
        }

        public static string Surround_It_With_Double_Quote(string given_String)
        {
            return ("\"" + given_String + "\"");
        }

        //The below code got from here http://jprajavel.blogspot.in/2009/09/simple-code-to-get-weeks-in-month-using.html
        public static int GetWeekInMonth(DateTime date)
        {
            DateTime tempdate = date.AddDays(-date.Day + 1);

            System.Globalization.CultureInfo ciCurr = System.Globalization.CultureInfo.CurrentCulture;
            int weekNumStart = ciCurr.Calendar.GetWeekOfYear(tempdate, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int weekNum = ciCurr.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            return weekNum - weekNumStart + 1;
        }

        //----------------------------------------------------------------------------//
        // Got the below code from here http://stackoverflow.com/questions/516788/getting-current-gmt-time
        //Get a NTP time from NIST
        //do not request a nist date more than once every 4 seconds, or the connection will be refused.
        //more servers at tf.nist.goc/tf-cgi/servers.cgi
        public static DateTime GetDummyDate()
        {
            return new DateTime(1000, 1, 1); //to check if we have an online date or not.
        }

        public static DateTime GetNISTDate()
        {
            System.Random ran = new System.Random(DateTime.Now.Millisecond);
            DateTime date = GetDummyDate();
            string serverResponse = string.Empty;

            // Represents the list of NIST servers
            string[] servers = new string[] {
                         //"nist1-ny.ustiming.org",
                         //"time-a.nist.gov",
                         //"nist1-chi.ustiming.org",
                         "time.nist.gov",
                         "ntp-nist.ldsbc.edu",
                         "nist1-la.ustiming.org",

                         "nist1-ny.ustiming.org",
                         "nist1-nj.ustiming.org",
                         "nist1-pa.ustiming.org",
                         "time-a.nist.gov",
                         "time-b.nist.gov",
                         "nist1.aol-va.symmetricom.com",
                         "nist1.columbiacountyga.gov",
                         "nist1-atl.ustiming.org",
                         "nist1-chi.ustiming.org",
                         "nist-chicago (No DNS)",
                         "nist.expertsmi.com",
                         "nist.netservicesgroup.com",
                         "nisttime.carsoncity.k12.mi.us",
                         "nist1-lnk.binary.net",
                         "wwv.nist.gov",
                         "time-a.timefreq.bldrdoc.gov",
                         "time-b.timefreq.bldrdoc.gov",
                         "time-c.timefreq.bldrdoc.gov",
                         "time.nist.gov",
                         "utcnist.colorado.edu",
                         "utcnist2.colorado.edu",
                         "nist.colorado-networks.com",
                         "ntp-nist.ldsbc.edu",
                         "nist1-lv.ustiming.org",
                         "time-nw.nist.gov",
                         "nist-time-server.eoni.com",
                         "nist1.aol-ca.symmetricom.com",
                         "nist1.symmetricom.com",
                         "nist1-sj.ustiming.org",
                         "nist1-la.ustiming.org "
                };

            // Try each server in random order to avoid blocked requests due to too frequent request
            int i = 0;
            while (true)
            {
                try
                {
                    // Open a StreamReader to a random time server
                    //System.IO.StreamReader reader = new System.IO.StreamReader(new System.Net.Sockets.TcpClient(servers[ran.Next(0, servers.Length)], 13).GetStream());
                    System.IO.StreamReader reader = new System.IO.StreamReader(new System.Net.Sockets.TcpClient(servers[i], 13).GetStream());
                    serverResponse = reader.ReadToEnd();
                    reader.Close();

                    // Check to see that the signature is there
                    if (serverResponse.Length > 47 && serverResponse.Substring(38, 9).Equals("UTC(NIST)"))
                    {
                        // Parse the date
                        int jd = int.Parse(serverResponse.Substring(1, 5));
                        int yr = int.Parse(serverResponse.Substring(7, 2));
                        int mo = int.Parse(serverResponse.Substring(10, 2));
                        int dy = int.Parse(serverResponse.Substring(13, 2));
                        int hr = int.Parse(serverResponse.Substring(16, 2));
                        int mm = int.Parse(serverResponse.Substring(19, 2));
                        int sc = int.Parse(serverResponse.Substring(22, 2));

                        if (jd > 51544)
                            yr += 2000;
                        else
                            yr += 1999;

                        date = new DateTime(yr, mo, dy, hr, mm, sc);

                        // Exit the loop
                        break;
                    }

                }
                catch (Exception ex)
                {
                    if (i < servers.Length)
                        ++i;
                    else
                        break;
                    /* Do Nothing...try the next server */
                }
            }

            return date;
        }
        //----------------------------------------------------------------------------//

        

    }

    //I got the below code from this page http://stackoverflow.com/questions/165808/simple-2-way-encryption-for-c-sharp/212707|^|212707
    public class SimpleAES
    {
        // Change these keys
        private byte[] Key = { 123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209 };
        private byte[] Vector = { 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 250, 112, 79, 32, 114, 156 };


        private ICryptoTransform EncryptorTransform, DecryptorTransform;
        private System.Text.UTF8Encoding UTFEncoder;

        public SimpleAES()
        {
            //This is our encryption method
            RijndaelManaged rm = new RijndaelManaged();

            //Create an encryptor and a decryptor using our encryption method, key, and vector.
            EncryptorTransform = rm.CreateEncryptor(this.Key, this.Vector);
            DecryptorTransform = rm.CreateDecryptor(this.Key, this.Vector);

            //Used to translate bytes to text and vice versa
            UTFEncoder = new System.Text.UTF8Encoding();
        }

        /// -------------- Two Utility Methods (not used but may be useful) -----------
        /// Generates an encryption key.
        static public byte[] GenerateEncryptionKey()
        {
            //Generate a Key.
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateKey();
            return rm.Key;
        }

        /// Generates a unique encryption vector
        static public byte[] GenerateEncryptionVector()
        {
            //Generate a Vector
            RijndaelManaged rm = new RijndaelManaged();
            rm.GenerateIV();
            return rm.IV;
        }


        /// ----------- The commonly used methods ------------------------------    
        /// Encrypt some text and return a string suitable for passing in a URL.
        public string EncryptToString(string TextValue)
        {
            return ByteArrToString(Encrypt(TextValue));
        }

        /// Encrypt some text and return an encrypted byte array.
        public byte[] Encrypt(string TextValue)
        {
            //Translates our text value into a byte array.
            Byte[] bytes = UTFEncoder.GetBytes(TextValue);

            //Used to stream the data in and out of the CryptoStream.
            MemoryStream memoryStream = new MemoryStream();

            /*
             * We will have to write the unencrypted bytes to the stream,
             * then read the encrypted result back from the stream.
             */
            #region Write the decrypted value to the encryption stream
            CryptoStream cs = new CryptoStream(memoryStream, EncryptorTransform, CryptoStreamMode.Write);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            #endregion

            #region Read encrypted value back out of the stream
            memoryStream.Position = 0;
            byte[] encrypted = new byte[memoryStream.Length];
            memoryStream.Read(encrypted, 0, encrypted.Length);
            #endregion

            //Clean up.
            cs.Close();
            memoryStream.Close();

            return encrypted;
        }

        /// The other side: Decryption methods
        public string DecryptString(string EncryptedString)
        {
            return Decrypt(StrToByteArray(EncryptedString));
        }

        /// Decryption when working with byte arrays.    
        public string Decrypt(byte[] EncryptedValue)
        {
            #region Write the encrypted value to the decryption stream
            MemoryStream encryptedStream = new MemoryStream();
            CryptoStream decryptStream = new CryptoStream(encryptedStream, DecryptorTransform, CryptoStreamMode.Write);
            decryptStream.Write(EncryptedValue, 0, EncryptedValue.Length);
            decryptStream.FlushFinalBlock();
            #endregion

            #region Read the decrypted value from the stream.
            encryptedStream.Position = 0;
            Byte[] decryptedBytes = new Byte[encryptedStream.Length];
            encryptedStream.Read(decryptedBytes, 0, decryptedBytes.Length);
            encryptedStream.Close();
            #endregion
            return UTFEncoder.GetString(decryptedBytes);
        }

        /// Convert a string to a byte array.  NOTE: Normally we'd create a Byte Array from a string using an ASCII encoding (like so).
        //      System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        //      return encoding.GetBytes(str);
        // However, this results in character values that cannot be passed in a URL.  So, instead, I just
        // lay out all of the byte values in a long string of numbers (three per - must pad numbers less than 100).
        public byte[] StrToByteArray(string str)
        {
            if (str.Length == 0)
                throw new Exception("Invalid string value in StrToByteArray");

            byte val;
            byte[] byteArr = new byte[str.Length / 3];
            int i = 0;
            int j = 0;
            do
            {
                val = byte.Parse(str.Substring(i, 3));
                byteArr[j++] = val;
                i += 3;
            }
            while (i < str.Length);
            return byteArr;
        }

        // Same comment as above.  Normally the conversion would use an ASCII encoding in the other direction:
        //      System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        //      return enc.GetString(byteArr);    
        public string ByteArrToString(byte[] byteArr)
        {
            byte val;
            string tempStr = "";
            for (int i = 0; i <= byteArr.GetUpperBound(0); i++)
            {
                val = byteArr[i];
                if (val < (byte)10)
                    tempStr += "00" + val.ToString();
                else if (val < (byte)100)
                    tempStr += "0" + val.ToString();
                else
                    tempStr += val.ToString();
            }
            return tempStr;
        }

        
    }
}

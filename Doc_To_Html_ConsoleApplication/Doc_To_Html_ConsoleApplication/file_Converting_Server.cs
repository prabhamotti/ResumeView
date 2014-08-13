using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SuperWebSocket;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;

using HtmlAgilityPack;

namespace Doc_To_Html_ConsoleApplication
{
    class file_Converting_Server:WebSocketServer
    {
        static long word_Application_Count;
        static System.Collections.Concurrent.ConcurrentQueue<string> file_Paths;
        System.Collections.Generic.List<System.Threading.Thread> threads;
        static Microsoft.Office.Interop.Word._Application app;
        //WebSocketSession resumeview_Server_Session;
        public file_Converting_Server()
        {
            word_Application_Count = 5;
            System.IO.StreamReader reader = new System.IO.StreamReader("settings.txt");
            int port_Int = 2012;
            int given_Max_Connection = 100;

            while (!reader.EndOfStream)
            {
                string temp_SingleLine = reader.ReadLine();
                if (temp_SingleLine.Contains("Port"))
                {
                    string port_String = temp_SingleLine.Substring(temp_SingleLine.IndexOf(":") + 1);
                    int.TryParse(port_String, out port_Int);
                }
                if (temp_SingleLine.Contains("Max_Connection"))
                {
                    string max_Connection_String = temp_SingleLine.Substring(temp_SingleLine.IndexOf(":") + 1);
                    int.TryParse(max_Connection_String, out given_Max_Connection);
                }
            }
            reader.Close();

            this.Setup(new RootConfig(), new ServerConfig
            {
                Port = port_Int,
                Ip = "Any",
                MaxConnectionNumber = given_Max_Connection,
                MaxCommandLength = 100000,
                Mode = SuperSocket.SocketBase.SocketMode.Async,
                Name = "SuperWebSocket Server"
            }, SocketServerFactory.Instance);

            file_Paths = new System.Collections.Concurrent.ConcurrentQueue<string>();

            threads = new List<System.Threading.Thread>();
            for (int i = 0; i < word_Application_Count; ++i)
            {
                System.Threading.Thread temp_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(start_Operation));
                temp_Thread.IsBackground = true;
                threads.Add(temp_Thread);
            }

            assign_The_Function_For_Events();
            app = new Microsoft.Office.Interop.Word.Application();
        }

        public override bool Start()
        {
            try
            {
                foreach (System.Threading.Thread temp_Thread in threads)
                    temp_Thread.Start();
                base.Start();
                return true;
            }
            catch(Exception excp)
            {
                return false;
            }
        }

        private void assign_The_Function_For_Events()
        {
            this.NewSessionConnected += new SessionEventHandler<WebSocketSession>(this.WebSocketServer_NewSessionConnected);
            this.NewDataReceived += new SessionEventHandler<WebSocketSession, byte[]>(this.WebSocketServer_NewDataReceived);
            this.NewMessageReceived += new SessionEventHandler<WebSocketSession, string>(this.WebSocketServer_NewMessageReceived);
            this.SessionClosed += new SessionEventHandler<WebSocketSession, SuperSocket.SocketBase.CloseReason>(this.WebSocketServer_SessionClosed);
        }

        static private void start_Operation()
        {
            while (true)
            {
                if (file_Paths.Count == 0)
                    System.Threading.Thread.Sleep(1000);
                else
                {
                    try
                    {
                        //System.Threading.Interlocked.Increment(ref word_Application_Count);
                        
                        while (file_Paths.Count > 0)
                        {
                            string temp_String = "";
                            if (file_Paths.TryDequeue(out temp_String))
                            {
                                string[] temp_Collection = temp_String.Split(new string[] {"|^|"},StringSplitOptions.None);
                                string file_Path = temp_Collection[0];
                                string web_Page_Base_Address = temp_Collection[1];
                                string folder_Path = file_Path.Substring(0, file_Path.LastIndexOf("\\"));
                                string file_Name = file_Path.Substring(file_Path.LastIndexOf("\\") + 1);
                                if (!System.IO.File.Exists(file_Path))
                                    return;// continue;
                                string file_Name_Htm = folder_Path + "\\HTML\\" + System.IO.Path.ChangeExtension(file_Name, "htm");
                                if (System.IO.File.Exists(file_Name_Htm))
                                    return;// continue;
                                Microsoft.Office.Interop.Word._Document doc = app.Documents.Open(file_Path, Visible: false);
                                //Console.WriteLine(file_Name_Htm);
                                doc.SaveAs(FileName: file_Name_Htm, FileFormat: Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML);
                                System.Threading.ThreadPool.QueueUserWorkItem((state) => {
                                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

                                    // There are various options, set as needed
                                    htmlDoc.OptionFixNestedTags = true;

                                    // filePath is a path to a file containing the html
                                    //string filePath = @"C:\Documents and Settings\prabhakaran\Desktop\rv_3b72167e18a54fe28a330c5fbecc222f.htm";
                                    string filePath = file_Name_Htm;
                                    while (true)
                                    {
                                        try
                                        {
                                            htmlDoc.Load(filePath);
                                            break;
                                        }
                                        catch (Exception excp)
                                        {

                                        }
                                    }

                                    // Use:  htmlDoc.LoadXML(xmlString);  to load from a string

                                    // ParseErrors is an ArrayList containing any errors from the Load statement
                                    if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
                                    {
                                        // Handle any parse errors as required

                                    }
                                    else
                                    {
                                        if (htmlDoc.DocumentNode != null)
                                        {
                                            HtmlAgilityPack.HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//body");

                                            if (bodyNode != null)
                                            {
                                                bodyNode.SetAttributeValue("onload", "var temp_Element_Pdf = document.getElementById('PDF Link');temp_Element_Pdf.style.width=screen.width/2;temp_Element_Pdf.style.height='50';var temp_Element_Doc = document.getElementById('DOC Link');temp_Element_Doc.style.width=screen.width/2;temp_Element_Doc.style.height='50';temp_Element_Doc.style.position='absolute';");

                                                HtmlNode pdf_Link = htmlDoc.CreateElement("button"); //new HtmlNode(HtmlNodeType.Element, htmlDoc, 0);
                                                pdf_Link.SetAttributeValue("value", "PDF");
                                                pdf_Link.SetAttributeValue("id", "PDF Link");
                                                string pdf_Path = web_Page_Base_Address + "Local_Database/PDF/" + System.IO.Path.ChangeExtension(file_Name, "pdf");
                                                pdf_Link.SetAttributeValue("onclick", "window.open('"+pdf_Path+"','_blank','location=no,menubar=no,status=no,toolbar=no');");
                                                pdf_Link.InnerHtml = "PDF";
                                                pdf_Link.SetAttributeValue("style", "background-color:#262626;color:#747474;border-style:groove;border-color:#121212;-moz-appearance:none;");
                                                pdf_Link.SetAttributeValue("onmouseover", "this.style.backgroundColor='#262626';this.style.color='#adbdde';this.style.fontWeight='bold';this.style.fontSize='normal';this.style.borderColor='#afafaf';");  //#afafaf
                                                pdf_Link.SetAttributeValue("onmouseout", "this.style.backgroundColor='#262626';this.style.color='#747474';this.style.fontWeight='normal';this.style.fontSize='normal';this.style.borderColor='#121212';");
                                                bodyNode.ChildNodes.Add(pdf_Link);

                                                HtmlNode doc_Link = htmlDoc.CreateElement("button"); //new HtmlNode(HtmlNodeType.Element, htmlDoc, 0);
                                                doc_Link.SetAttributeValue("value", "DOC");
                                                doc_Link.SetAttributeValue("id", "DOC Link");
                                                string doc_Path = web_Page_Base_Address + "Local_Database/DOC/" + System.IO.Path.ChangeExtension(file_Name, "doc");
                                                doc_Link.SetAttributeValue("onclick", "window.open('"+doc_Path+"','_blank','location=no,menubar=no,status=no,toolbar=no');");
                                                doc_Link.InnerHtml = "DOC";
                                                doc_Link.SetAttributeValue("style", "background-color:#262626;color:#747474;border-style:groove;border-color:#121212;-moz-appearance:none;");
                                                doc_Link.SetAttributeValue("onmouseover", "this.style.backgroundColor='#262626';this.style.color='#adbdde';this.style.fontWeight='bold';this.style.fontSize='normal';this.style.borderColor='#afafaf';");  //#afafaf
                                                doc_Link.SetAttributeValue("onmouseout", "this.style.backgroundColor='#262626';this.style.color='#747474';this.style.fontWeight='normal';this.style.fontSize='normal';this.style.borderColor='#121212';");
                                                bodyNode.ChildNodes.Add(doc_Link);

                                                //htmlDoc.Save(@"C:\Documents and Settings\prabhakaran\Desktop\new.html");
                                                htmlDoc.Save(file_Name_Htm);

                                                // Do something with bodyNode
                                            }
                                        }
                                    }// ***

                                });


                                string file_Name_Pdf = folder_Path + "\\PDF\\" + System.IO.Path.ChangeExtension(file_Name, "pdf");
                                //Console.WriteLine(file_Name_Pdf);
                                doc.SaveAs(FileName: file_Name_Pdf, FileFormat: Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF);
                                //Console.WriteLine(folder_Path + "\\DOC\\" + file_Name);
                                ((Microsoft.Office.Interop.Word._Document)doc).Close(SaveChanges: Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges);
                                while (true)
                                {
                                    try
                                    {
                                        System.IO.File.Move(file_Path, folder_Path + "\\DOC\\" + file_Name);
                                        break;
                                    }
                                    catch (Exception excp)
                                    {
                                    }
                                }
                            }
                        }
                        //System.Threading.Interlocked.Decrement(ref word_Application_Count);
                    }
                    catch (Exception excp)
                    {
                        Console.WriteLine(excp.Message);
                    }
                }
            }
            
        }

        public void WebSocketServer_NewSessionConnected(WebSocketSession session){}
        public void WebSocketServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason close_Reason) { }
        public void WebSocketServer_NewDataReceived(WebSocketSession session, byte[] e) { }
        public void WebSocketServer_NewMessageReceived(WebSocketSession session, string e)
        {
            string file_Path = e;
            //if (!file_Paths.Contains(file_Path))
            file_Paths.Enqueue(file_Path);
        }
    }
}
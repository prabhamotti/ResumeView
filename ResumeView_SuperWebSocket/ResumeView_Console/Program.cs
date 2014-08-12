#define LOG
#undef LOG

using SuperWebSocket;
using SuperWebSocket.Config;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine;
using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using MySql_CSharp_Console;
using System.Collections.Generic;
using WebSocket4Net;

//using HtmlAgilityPack;
using mshtml;

using Common_Classes;

namespace temp_Check
{
    public class Main_Class
    {
        public static void Main()
        {
            #if LOG
            log_Queue = new System.Collections.Concurrent.ConcurrentQueue<string>();
            log_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(log_Start));
            log_Thread.Start();
            #endif

            #if DEBUG
            Console.WriteLine("Initiating starting phase");
            #endif
           
            Server server = new Server();
            #if DEBUG
            Console.WriteLine("Entering " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
            #endif

            server.start();
            #if DEBUG
            Console.WriteLine("Server is up and running");
            #endif
           
            while (server.IsRunning)
                System.Threading.Thread.Sleep(1000);
            #if DEBUG
            Console.WriteLine("Exiting " + System.Reflection.MethodBase.GetCurrentMethod().ToString());
            #endif
        }
    }
}
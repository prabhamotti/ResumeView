using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/*
namespace ResumeView_Console
{
    class Client
    {
        TcpClient tcp_Client;
        public Client(TcpClient tcp_Client)
        {
            this.tcp_Client = tcp_Client;

        }
    }
    class Server
    {
        public static string get_Reply(string client_Sent_Key)
        {
            String guid_String = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            String final_Key = "";
            byte[] combined_Key = Encoding.ASCII.GetBytes(client_Sent_Key + guid_String);
            System.Security.Cryptography.SHA1 sha1_Temp = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            final_Key = Convert.ToBase64String(sha1_Temp.ComputeHash(combined_Key));

            string accepted_Reply = "HTTP/1.1 101 Web Socket Protocol Handshake\n";
                accepted_Reply += "Upgrade: WebSocket\n";
                accepted_Reply += "Connection: Upgrade\n";
                accepted_Reply += "WebSocket-Origin: http://localhost:8080\n";
                accepted_Reply += "WebSocket-Location: ws://localhost:8181/websession\n";
                accepted_Reply += ("Sec-WebSocket-Accept: " + final_Key +"\n");

                return accepted_Reply;
        }

        static void Main(string[] args)
        {
            IPAddress ipAddress = IPAddress.Loopback;
            int port_No = 8181; 
            TcpListener listener = new TcpListener(ipAddress, port_No);
            
            listener.Start();

            TcpClient tcp_Client;
            while (true)
            {
                tcp_Client = listener.AcceptTcpClient();
                NetworkStream stream = tcp_Client.GetStream();
                System.IO.StreamReader reader = new System.IO.StreamReader(stream);

                String reply;
                

                String client_Sent_Key = "";
                do
                {
                    reply = reader.ReadLine();
                    Console.WriteLine(reply);
                    String key_Word = "Sec-WebSocket-Key";
                    if (reply.Contains(key_Word))
                    {
                        int position = reply.IndexOf(key_Word);
                        client_Sent_Key = reply.Substring(position + key_Word.Length + 2);
                        break;
                    }
                } while (reply != "");


                System.IO.StreamWriter writer = new System.IO.StreamWriter(stream);

                writer.WriteLine(get_Reply(client_Sent_Key));
                writer.WriteLine("");
                writer.Flush();

                reply = reader.ReadLine();
                Console.WriteLine(reply);
            }
        }
    }
}
*/

using Alchemy;
using Alchemy.Classes;

class temp
{
    static void Main(string[] args)
    {
        var aServer = new WebSocketServer(81, IPAddress.Any)
        {
            OnReceive = OnReceive,
            OnSend = OnSend,
            OnConnect = OnConnected,
            OnConnected = OnConnect,
            OnDisconnect = OnDisconnect,
            TimeOut = new TimeSpan(0, 5, 0)
        };

        aServer.Start();
    }

    static void OnConnected(UserContext context)
    {
        Console.WriteLine("Client Connection From : " +
        aContext.ClientAddress.ToString());
    }
}
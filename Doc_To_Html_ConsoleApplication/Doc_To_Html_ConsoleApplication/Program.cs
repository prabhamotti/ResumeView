using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Doc_To_Html_ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            file_Converting_Server server = new file_Converting_Server();

            server.Start();
            while (server.IsRunning)
                System.Threading.Thread.Sleep(1000);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reminder_Server_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Reminder_Server server = new Reminder_Server();
            server.start();
            while (server.IsRunning)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using temp_Check;

namespace ResumeView_Service
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
            this.ServiceName = "ResumeView Server";
        }

        protected override void OnStart(string[] args)
        {
            //Server server = new Server();
            //server.start();
            //while (server.IsRunning)
            //    System.Threading.Thread.Sleep(1000);
            //while (true)
            //    System.Threading.Thread.Sleep(1000);
        }

        protected override void OnStop()
        {
        }
    }
}

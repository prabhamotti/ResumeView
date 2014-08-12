using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;

namespace ResumeView_Service
{
    [RunInstaller(true)]
    public class MyWindowsServiceInstaller : System.Configuration.Install.Installer
    {
        public MyWindowsServiceInstaller()
        {
            var processInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            var serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            //set the privileges
            processInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = "ResumeView Server";
            serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Manual;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = "ResumeView Server";

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}

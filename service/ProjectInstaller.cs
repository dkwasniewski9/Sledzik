using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using static SledzikLibrary.AppLibrary;

namespace SledzikService
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.ServiceName = ServiceName;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
}
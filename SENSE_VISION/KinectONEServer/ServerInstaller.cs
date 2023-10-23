﻿using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

[RunInstaller(true)]
public class ServerInstaller : Installer
{
    private ServiceProcessInstaller processInstaller;
    private ServiceInstaller serviceInstaller;

    /// <summary>
    /// The installer hook for installing this service.  
    /// 
    /// To install and uninstall this service into Windows, use:
    /// (http://msdn.microsoft.com/en-us/library/aa984379%28v=vs.71%29.aspx)
    /// 
    /// InstallUtil /LogToConsole=true Kinect2Server.exe
    /// InstallUtil /u Kinect2Server.exe
    /// 
    /// </summary>
    public ServerInstaller()
    {
        processInstaller = new ServiceProcessInstaller();
        serviceInstaller = new ServiceInstaller();

        processInstaller.Account = ServiceAccount.LocalSystem;
        serviceInstaller.StartType = ServiceStartMode.Manual;
        serviceInstaller.ServiceName = "KinectONEServer"; 

        Installers.Add(serviceInstaller);
        Installers.Add(processInstaller);
    }
}  

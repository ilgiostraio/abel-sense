using System;
using System.Windows.Forms;

namespace Sense.Vision.KinectONEServer
{
    static class Launcher
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (Environment.UserInteractive)
            {
                Application.Run(new ServerTray());
            }
            else
            {
                System.ServiceProcess.ServiceBase.Run(new Service());
            }
        }
    }
}

using System;
using System.Reflection;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sense.Vision.KinectONEServer
{ 
    public class ServerTray : Form
    { 
        private NotifyIcon trayIcon = new NotifyIcon();
        private ContextMenu trayMenu = new ContextMenu();
        private Service service = new Service();

        Process KinectViewModule = new Process();
        public ServerTray()
        {
            var dllDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            var fileStream = new FileStream("config.txt", FileMode.Open, FileAccess.Read);
            List<string> line = new List<string>();

            using (var streaming = new StreamReader(fileStream, Encoding.UTF8))
            {
                string linea ="";
                while ((linea = streaming.ReadLine())!=null)
                {
                    line.Add(linea.Split(' ')[0]);
                }
            }

            //Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDirectory + "/ROS");
            Environment.SetEnvironmentVariable("ROS_HOSTNAME", line[0], EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("ROS_MASTER_URI", line[1], EnvironmentVariableTarget.User);

            // Register for connection events.
            service.IsConnectedChanged += OnConnectionChanged;

            // Create a simple tray menu with only one item.
            trayMenu.MenuItems.Add(GetPublicIP());
            trayMenu.MenuItems.Add("-");

            trayMenu.MenuItems.Add(GetIP4Address());
            trayMenu.MenuItems.Add("View", viewKinect);
            trayMenu.MenuItems.Add("Exit", OnExit);

            
            // Change the icon and text of the tray icon.
            trayIcon.Text = "Kinect ONE Server";
            trayIcon.Icon = (service.IsConnected)
                ? KinectONEServer.Properties.Resources.KinectGreen
                : KinectONEServer.Properties.Resources.KinectRed;
 
            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        private void OnConnectionChanged(object sender, IsConnectedChangedEventArgs e)
        {
            trayIcon.Icon = (e.IsConnected)
                ? KinectONEServer.Properties.Resources.KinectGreen
                : KinectONEServer.Properties.Resources.KinectRed;
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.
 
            // Start the Kinect server.
            typeof(Service)
                .GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(service, new object[] { null });

            // Load the placeholder form.
            base.OnLoad(e);
        }
 
        private void OnExit(object sender, EventArgs e)
        {
            
            // Shutdown the entire application.
            Application.Exit();
        }

        private void viewKinect(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("KinectView").Length == 0)
            {
               
                KinectViewModule.StartInfo.FileName = Application.StartupPath.ToString() + "/KinectView.exe";
                KinectViewModule.Start();
            }
            else
                KinectViewModule = Process.GetProcessesByName("KinectView")[0];
           
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the tray resources.
                trayIcon.Dispose();
                trayMenu.Dispose();
            }

            // Stop the Kinect server.
            typeof(Service)
                .GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(service, null);

            // Remove the placeholder form.
            base.Dispose(isDisposing);
        }

        public static string GetIP4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }

        public string GetPublicIP()
        {
            string externalIP = "";
            try
            {
                externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();
            }
            catch
            {
                externalIP = "No Public IP";
            }
                return externalIP;
        }
    }
}

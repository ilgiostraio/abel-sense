using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Globalization;

using Microsoft.Kinect;

using Sense.Lib.KinectONE;
using ShoreNet;
using Sense.Lib.FACELibrary;

using YarpManagerCS;


using Uml.Robotics.Ros;
using std_msgs = Messages.std_msgs;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Sense.Vision.ShoreModule
{
    public partial class ShoreModule : Form
    {
        #region Shore engine parameters

        private static ShoreNetEngine CreateFaceEngine(float timeBase, bool updateTimeBase, uint threadCount, string model,
                                                        float imageScale, float minFaceSize, int minFaceScore, float idMemoryLength,
                                                        string idMemoryType, bool trackFaces, string phantomTrap,
                                                        bool searchEyes, bool searchNose, bool searchMouth,
                                                        bool analyzeEyes, bool analyzeMouth, bool analyzeGender, bool analyzeAge,
                                                        bool analyzeHappy, bool analyzeSad, bool analyzeSurprised, bool analyzeAngry)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Console.WriteLine(executingAssembly.GetName().Name);
            string setupData = new StreamReader(executingAssembly.GetManifestResourceStream(executingAssembly.GetName().Name + ".Resources.FaceSetupData.txt")).ReadToEnd();
            string setupCall = String.Format(CultureInfo.InvariantCulture, "CreateFaceEngine({0},{1},{2},'{3}',{4},{5},{6},{7},'{8}',{9},'{10}',{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21})",
                 timeBase, updateTimeBase.ToString().ToLower(), threadCount, model,
                 imageScale, minFaceSize, minFaceScore,
                 idMemoryLength, idMemoryType, trackFaces.ToString().ToLower(), phantomTrap,
                 searchEyes.ToString().ToLower(), searchNose.ToString().ToLower(), searchMouth.ToString().ToLower(),
                 analyzeEyes.ToString().ToLower(), analyzeMouth.ToString().ToLower(), analyzeGender.ToString().ToLower(), analyzeAge.ToString().ToLower(),
                 analyzeHappy.ToString().ToLower(), analyzeSad.ToString().ToLower(), analyzeSurprised.ToString().ToLower(), analyzeAngry.ToString().ToLower());

            return new ShoreNetEngine(setupData, setupCall);
        }

        float timeBase = 0.03f;        // 0 = Use single image mode
        bool updateTimeBase = true;    // Not used in video mode
        uint threadCount = 2u;         // Let's take one thread only
        string model = "Face.Front";   // Search frontal faces
        float imageScale = 1.0f;       // Scale the images
        float minFaceSize = 0.0f;      // Find small faces too
        int minFaceScore = 9;          // That's the default value
        float idMemoryLength = 90.0f;
        string idMemoryType = "Spatial";
        bool trackFaces = true;
        string phantomTrap = "Off";
        bool searchEyes = false;
        bool searchNose = false;
        bool searchMouth = false;
        bool analyzeEyes = false;
        bool analyzeMouth = false;
        bool analyzeGender = true;
        bool analyzeAge = true;
        bool analyzeHappy = true;
        bool analyzeSad = true;
        bool analyzeSurprised = true;
        bool analyzeAngry = true;


        Int32 lineFeed = 0;
        //private Int32 pixelFeed = 0;
        ShoreNetEngine engine;
        System.Threading.Thread engineLoop; // Thread manages the shore engine work


        #endregion

        KinectOne kinect;
        ColorFrameReference colorRef = null;

        

        YarpPort yarpPort;
        string portName = ConfigurationManager.AppSettings["YarpPort"].ToString();
        string ShoreData = null;

        private System.Object lockThis = new System.Object();

       

        Publisher<std_msgs.String> Talker = null;
        SingleThreadSpinner spinner = new SingleThreadSpinner();

        LimitedConcurrencyLevelTaskScheduler tsk = new LimitedConcurrencyLevelTaskScheduler();

        string comunication = "YARP"; 
        string view = "view";
        string ROS_HOSTNAME = "";
        string ROS_MASTER_URI = "";
        ViewShore vShore;
       
        Stopwatch watch =  System.Diagnostics.Stopwatch.StartNew();
        

        public ShoreModule(string[] args)
        {

            if (args.Length >= 1)
                comunication = args[0];

            if (args.Length >= 2)
                ROS_HOSTNAME = args[1];

            if (args.Length >= 3)
                ROS_MASTER_URI = args[2];

            if (args.Length >= 4)
                view = args[3];

            var dllDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDirectory+"");

            //Directory.SetCurrentDirectory(dllDirectory);

            InitializeComponent();


            if (File.Exists(dllDirectory + "\\Shore140.dll"))
            {
                
                kinect = new KinectOne();

                if (view == "view")
                {
                    vShore = new ViewShore();
                    vShore.Show();

                    kinect.InitializeCamera(vShore.CameraImage);
                    kinect.OnColorFrameArrived();
                }
                else
                    kinect.InitializeCamera();

               
                kinect.OnColorFrameArrived(OnColorFrameArrived);

                if (comunication == "YARP")
                {
                    yarpPort = new YarpPort();
                    yarpPort.openSender(portName);

                    if (yarpPort != null && yarpPort.NetworkExists())
                    {
                        lblYarpServer.Text = "YarpServer: runnig...";
                        lblYarpPort.Text = "YarPort: " + portName;
                    }

                    lblStatus.Text = "running...";

                }
                else if (comunication == "ROS")
                {
                   
                   Environment.SetEnvironmentVariable("ROS_HOSTNAME", ROS_HOSTNAME, EnvironmentVariableTarget.User);
                    Environment.SetEnvironmentVariable("ROS_MASTER_URI", ROS_MASTER_URI, EnvironmentVariableTarget.User);

                    Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
                    ROS.Init(null, "ShoreModule");

                    NodeHandle node = new NodeHandle();
                    Talker = node.Advertise<std_msgs.String>("/shore", 10);

                    lblStatus.Text = "running...";
                    lblYarpServer.Text = "ROS: runnig...";
                }


                InitShoreEngine();
            }
            else
            {
                lblStatus.Text = "No Library SHORE";
                lblYarpServer.Text = "YarpServer:";
            }
        }


        private void ShoreModule_Load(object sender, EventArgs e)
        {
            var secondaryScreen = System.Windows.Forms.Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

            if (secondaryScreen != null)
            {

                var workingArea = secondaryScreen.WorkingArea;
                this.Left = workingArea.Left + 900;
                this.Top = workingArea.Top;
                //this.Width = workingArea.Width;
                //this.Height = workingArea.Height;
                // If window isn't loaded then maxmizing will result in the window displaying on the primary monitor
                //if (this.IsLoaded)
                //    this.WindowState = WindowState.Maximized;
            }
        }

        public void InitShoreEngine()
        {

            engine = CreateFaceEngine(timeBase, updateTimeBase, threadCount, model,
                           imageScale, minFaceSize, minFaceScore,
                           idMemoryLength, idMemoryType, trackFaces, phantomTrap,
                           searchEyes, searchNose, searchMouth,
                           analyzeEyes, analyzeMouth, analyzeGender, analyzeAge,
                           analyzeHappy, analyzeSad, analyzeSurprised, analyzeAngry);

            if (engineLoop != null)
                StopShore();

            //engineLoop = new System.Threading.Thread(RunEngine);
            //engineLoop.Start();

        }

        public void OnColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            // Get the reference to the color frame
            colorRef = e.FrameReference;

            if (colorRef == null) return;

       
            ColorFrame frame = colorRef.AcquireFrame();
            // It's possible that we skipped a frame or it is already gone
            if (frame == null) return;

            RunAsyncTask(frame, taskAfter);
            
            kinect.Camera.FrameReady.Set();
           
        }

        Task taskAfter = null;

        public void RunAsyncTask(ColorFrame frame, Task t)
        {

            taskAfter = tsk.SingleFactory.StartNew(() => RunEngine(frame, t));
            
        }

        // The engineLoop thread executes RunEngine() function. This thread waits until a signal is received
        // (frameReady.WaitOne()). In this case, the signal means that the copy of frame bytes is ready.
        // Since the engineLoop thread is not the UI thread (the thread which manages the interface), 
        // it cannot modify objects belonging to the interface. A Dispatcher object (the last line of the function) 
        // is necessary for updating the interface.
        private void RunEngine(ColorFrame frame, Task t)
        {
            List<Shore> ShoreList = new List<Shore>();
            uint size = Convert.ToUInt32(frame.FrameDescription.Height * frame.FrameDescription.Width * 4);
            IntPtr PinnedImageBuffer = Marshal.AllocHGlobal(Convert.ToInt32(size));
            ShoreNetContent content = null;

            frame.CopyConvertedFrameDataToIntPtr(PinnedImageBuffer, size, ColorImageFormat.Bgra);
            frame.Dispose();

            lock (lockThis)
            {

                try
                {
               
                    content = engine.Process(PinnedImageBuffer + 1, 1920, 1080, 1U, 4, 1920 * 4, 0, "GRAYSCALE");

                    if (content == null) return;

                    uint n = content.GetObjectCount();
                    Parallel.For(0, n, i =>
                    {

                        try
                        {
                            ShoreNetObject sObj = content.GetObject(Convert.ToUInt32(i));
                            if (sObj.GetShoreType() == "Face")
                            {
                                Shore shore = new Shore();

                                shore.Eyes.Left.X = sObj.GetMarkerOf("LeftEye").GetX();
                                shore.Eyes.Left.Y = sObj.GetMarkerOf("LeftEye").GetY();
                                shore.Eyes.Right.X = sObj.GetMarkerOf("RightEye").GetX();
                                shore.Eyes.Right.Y = sObj.GetMarkerOf("RightEye").GetY();

                                shore.Gender = (sObj.GetAttributeOf("Gender") == "Female") ? "Female" : "Male";
                                shore.Age = (int)sObj.GetRatingOf("Age");
                                shore.Happiness_ratio = sObj.GetRatingOf("Happy");
                                shore.Surprise_ratio = sObj.GetRatingOf("Surprised");
                                shore.Anger_ratio = sObj.GetRatingOf("Angry");
                                shore.Sadness_ratio = sObj.GetRatingOf("Sad");
                                shore.Uptime = sObj.GetRatingOf("Uptime");
                                shore.Age_deviation = sObj.GetRatingOf("AgeDeviation");


                                shore.Region_face.Left = sObj.GetRegion().GetLeft();
                                shore.Region_face.Top = sObj.GetRegion().GetTop();
                                shore.Region_face.Bottom = sObj.GetRegion().GetBottom();
                                shore.Region_face.Right = sObj.GetRegion().GetRight();


                                ShoreList.Add(shore);

                            }

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("error shore" + e.Message);

                        }

                    });
                }
                finally
                {
                    Marshal.FreeHGlobal(PinnedImageBuffer);
                }
            }

            if (t != null)
                t.Wait();


            //Console.WriteLine(frame.RelativeTime);
            //frame.Dispose();

            if (ShoreList.Count() != 0)
            {
                if (view == "view")
                    vShore.draw(ShoreList);


                if (ROS.OK)
                {
                    sendData(JsonConvert.SerializeObject(ShoreList));
 
                }
                else if (yarpPort != null)
                {
                    sendData(ComUtils.XmlUtils.Serialize<List<Shore>>(ShoreList));
                }
            }
            else
            {
                if (view == "view")
                    vShore.Canvas_Shore.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                       new Action(delegate ()
                       {
                           vShore.Canvas_Shore.Children.Clear();
                       }));

            }




            lblFaceDet.Invoke(new Action(() =>
            {
                lblFaceDet.Text = ShoreList.Count().ToString();
            }));

            return;
        }

   
        private void sendData(string msg)
        {
            if (ROS.OK)
            {

                var elapsedMs = watch.ElapsedMilliseconds;
                watch.Restart();
                lblYarpPort.Invoke(new Action(() =>
                {
                    lblYarpPort.Text = "Speed: " + elapsedMs.ToString()+" - "+tsk.getNumTaskRunning()+" "+ tsk.getNumQueueTask();
                }));


                Console.WriteLine("publishing message");

                Messages.std_msgs.String pow = new Messages.std_msgs.String(msg);
                Talker.Publish(pow);

              
                //spinner.SpinOnce();
            }
            else if (yarpPort!=null)
            {
                yarpPort.sendData(msg);
            }
        }
        /// <summary>
        /// Process the infrared frames and update UI
        /// </summary>

        void StopShore()
        {
            if (engineLoop != null)
                engineLoop.Abort();
        }

        private void StopYarp()
        {
            //if (senderYarp != null)
            //    senderYarp.Abort();

            if (yarpPort != null)
                yarpPort.Close();


        }


        private void ShoreModule_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopYarp();
            StopShore();
            ROS.Shutdown();
        }


    }
}

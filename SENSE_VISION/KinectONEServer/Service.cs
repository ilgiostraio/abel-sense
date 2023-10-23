
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;


using Uml.Robotics.Ros;
using std_msgs = Messages.std_msgs;
using Image = Messages.sensor_msgs.Image;
using Audio = Messages.audio_common_msgs.AudioData;
using System.Threading.Tasks;

namespace Sense.Vision.KinectONEServer
{
    /// <summary>
    /// A service that publishes data from the Kinect2 over ROS.
    /// </summary>
    /// See: http://msdn.microsoft.com/en-us/library/system.serviceprocess.servicebase(v=vs.110).aspx
    /// 
    public class Service : ServiceBase
    {
        KinectSensor kinect;
        MultiSourceFrameReader reader;
        AudioSource audioSource;
        AudioBeamFrameReader audioReader;
        HighDefinitionFaceFrameSource[] faceSources;
        HighDefinitionFaceFrameReader[] faceReaders;

      
        private readonly int _bytePerPixel = (PixelFormats.Bgr32.BitsPerPixel + 7) / 8;
        private WriteableBitmap _colorBitmap = null;

        int colorArraySize = 0;
        byte[] colorBuffer = null;
        Rectangle colorRect;

        string formatImage = "yuv";
        
        // Wrap RGB frames into bitmap buffers.
        Bitmap frame32bpp = null;
        Bitmap frame24bpp = null;


        Publisher<Image> Color = null;
        Publisher<Image> Depth = null;
        Publisher<Image> Ir = null;
        Publisher<std_msgs.String> Body = null;
        Publisher<Audio> Audio = null;
        Publisher<std_msgs.String> AudioInfo = null;
        Publisher<std_msgs.String> Face = null;

        SingleThreadSpinner spinner = new SingleThreadSpinner();


        /// <summary>
        /// Minimum energy of audio to display (a negative number in dB value, where 0 dB is full scale)
        /// </summary>
        private const int MinEnergy = -90;

        public Service()
        {
            this.ServiceName = "KinectONEService";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        /// <summary>
        /// Property that indicates whether the Kinect Server is connected to a sensor.
        /// </summary>
        public bool IsConnected { get { return (this.kinect != null) && kinect.IsAvailable; } }

        /// <summary>
        /// Event that triggers when the server detects a Kinect connection or disconnecting.
        /// </summary>
        public event EventHandler<IsConnectedChangedEventArgs> IsConnectedChanged;

        protected override void OnStart(string[] args) 
        {
            
            // Try to open the first available Kinect sensor.
            this.kinect = KinectSensor.GetDefault();
            if (this.kinect == null )
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "KinectONEService";
                    eventLog.WriteEntry("No Kinect device was detected.");
                }
                //EventLog.WriteEntry("No Kinect device was detected.");  
                ExitCode = -1;
                throw new KinectException("No kinect device was detected.");
            }
            else
            {
                this.kinect.Open();
                this.kinect.IsAvailableChanged += this.OnAvailableChanged;
                InitializeCamera();
            }

            // Register as a handler for the image data being returned by the Kinect.
            this.reader = this.kinect.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
            if (this.reader == null)
            {
                EventLog.WriteEntry("Unable to connect to Kinect data stream.");
                ExitCode = -2;
                throw new KinectException("Unable to connect to Kinect data stream.");
            }
            else
            {
                this.reader.MultiSourceFrameArrived += this.OnFrameArrived;
            }

            // Register as a handler for the audio source data being returned by the Kinect.
            this.audioSource = this.kinect.AudioSource;
            if (this.audioSource == null)
            {
                EventLog.WriteEntry("Unable to connect to Kinect audio source.");
                ExitCode = -3;
                throw new KinectException("Unable to connect to Kinect audio source.");
            }

            // Register as a handler for the audio reader data being returned by the Kinect.
            this.audioReader = this.audioSource.OpenReader();
            if (this.audioReader == null)
            {
                EventLog.WriteEntry("Unable to create reader for Kinect audio source.");
                ExitCode = -4;
                throw new KinectException("Unable to create reader for Kinect audio source.");
            }
            else
            {
                this.audioReader.FrameArrived += this.onAudioFrameArrived;
            }

            // Create an array of face sources/readers for each possible body.
            // These will be activated on demand as the corresponding bodies are tracked.
            this.faceSources = new HighDefinitionFaceFrameSource[this.kinect.BodyFrameSource.BodyCount];
            this.faceReaders = new HighDefinitionFaceFrameReader[this.kinect.BodyFrameSource.BodyCount];

            for (var i = 0; i < faceSources.Length; ++i)
            {
                // Register as a handler for the face source data being returned by the Kinect.
                this.faceSources[i] = new HighDefinitionFaceFrameSource(this.kinect);
                if (this.faceSources[i] == null)
                {
                    EventLog.WriteEntry("Unable to create Kinect face source [" + i + "].");
                    ExitCode = -5;
                    throw new KinectException("Unable to create Kinect face source [" + i + "].");
                }

                // Register as a handler for the face reader data being returned by the Kinect.
                this.faceReaders[i] = this.faceSources[i].OpenReader();
                if (this.faceReaders[i] == null)
                {
                    EventLog.WriteEntry("Unable to create reader for Kinect face source [" + i + "].");
                    ExitCode = -6;
                    throw new KinectException("Unable to create reader for Kinect face source [" + i + "].");
                }
                else
                {
                    this.faceReaders[i].FrameArrived += this.onFaceFrameArrived;
                }
            }



            try
            {

                ROS.Init(null, "KinectONEServer");

                Color = new NodeHandle().Advertise<Image>("/kinect/color", 10);
                Depth = new NodeHandle().Advertise<Image>("/kinect/depth", 10);
                Ir = new NodeHandle().Advertise<Image>("/kinect/ir", 10);
                Body = new NodeHandle().Advertise<std_msgs.String>("/kinect/bodies", 10);
                Audio = new NodeHandle().Advertise<Audio>("/kinect/audio/streaming", 10);
                AudioInfo = new NodeHandle().Advertise<std_msgs.String>("/kinect/audio/info", 10);
                Face = new NodeHandle().Advertise<std_msgs.String>("/kinect/face", 10);
            }
            catch(Exception ex)
            {

            }

        }

        protected override void OnStop()
        {
            this.kinect.Close();
         

            this.reader.Dispose();
            this.audioReader.Dispose();
         
        }


        public void InitializeCamera()
        {
            if (kinect == null) return;

           
            FrameDescription desc = kinect.ColorFrameSource.FrameDescription;

          
            _colorBitmap = new WriteableBitmap(desc.Width, desc.Height, 96, 96, PixelFormats.Bgr32, null);

            if (formatImage=="yuv")
                 colorArraySize = kinect.ColorFrameSource.FrameDescription.Height * kinect.ColorFrameSource.FrameDescription.Width * 2; 
            else
                colorArraySize = kinect.ColorFrameSource.FrameDescription.Height * kinect.ColorFrameSource.FrameDescription.Width * 3; 

            colorBuffer = new byte[colorArraySize];

            colorRect = new Rectangle(0, 0,
                                          kinect.ColorFrameSource.FrameDescription.Width,
                                          kinect.ColorFrameSource.FrameDescription.Height);

         
            if (formatImage == "yuv")
                frame32bpp = new Bitmap(kinect.ColorFrameSource.FrameDescription.Width,
                                   kinect.ColorFrameSource.FrameDescription.Height,
                                    kinect.ColorFrameSource.FrameDescription.Width * 2,
                                   System.Drawing.Imaging.PixelFormat.Format16bppRgb555,
                                   Marshal.UnsafeAddrOfPinnedArrayElement(colorBuffer, 0));
            else
            {
                frame32bpp = new Bitmap(kinect.ColorFrameSource.FrameDescription.Width,
                                   kinect.ColorFrameSource.FrameDescription.Height,
                                   System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                frame24bpp = new Bitmap(frame32bpp.Width, frame32bpp.Height, frame32bpp.Width * 3,
                                       System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                                       Marshal.UnsafeAddrOfPinnedArrayElement(colorBuffer, 0));
            }
        }

        private void OnFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            // Acquire current Kinect frame reference.
            MultiSourceFrame multiSourceFrame = e.FrameReference.AcquireFrame();

            // Record the current Unix epoch timestamp and convert it to a byte array for serialization.
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            byte[] timestampBytes = BitConverter.GetBytes(timestamp);

            // If clients exist, convert the RGB frame to a byte array and send it followed by a timestamp.
            #region color
            using (ColorFrame colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                   // Stopwatch sw = new Stopwatch();
                    //sw.Start();

                    var description = colorFrame.ColorFrameSource.FrameDescription;

                    System.Drawing.Imaging.BitmapData bmpData;

                    // Lock the bitmap's bits.
                    if (formatImage == "yuv")
                        bmpData = frame32bpp.LockBits(colorRect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
                    else
                        bmpData = frame32bpp.LockBits(colorRect, System.Drawing.Imaging.ImageLockMode.ReadWrite, frame32bpp.PixelFormat);

                    IntPtr bmpPtr = bmpData.Scan0;

                    if (formatImage == "yuv")
                        //Converts the raw format into the requested format and copies the data into the memory location provided.
                        colorFrame.CopyConvertedFrameDataToIntPtr(bmpPtr, (uint)(bmpData.Width * bmpData.Height * 2), ColorImageFormat.Yuv);
                    else
                        colorFrame.CopyConvertedFrameDataToIntPtr(bmpPtr, (uint)(bmpData.Width * bmpData.Height * 4), ColorImageFormat.Bgra);

                    frame32bpp.UnlockBits(bmpData);


                    // Convert from 32bpp to 24bpp using System.Drawing
                    if (formatImage != "yuv")
                        using (Graphics gr = Graphics.FromImage(frame24bpp))
                        {
                            gr.DrawImage(frame32bpp, new Rectangle(0, 0, frame32bpp.Width, frame32bpp.Height));
                        }

                    Image img = new Image();
                    img.header = new std_msgs.Header();
                    img.header.seq = 1;
                    img.header.stamp = new std_msgs.Time();
                    img.header.frame_id = "/jkfd";

                    if (formatImage == "yuv")
                    {
                        img.width = (uint)frame32bpp.Width;
                        img.height = (uint)frame32bpp.Height;
                        img.encoding = "yuv422"; //img.encoding="brga"
                        img.step = img.width * 1 * 2; // img.step = img.width * 1 * 3;
                    }
                    else
                    {
                        img.width = (uint)frame32bpp.Width;
                        img.height = (uint)frame32bpp.Height;
                        img.encoding = "bgr8";
                        img.step = img.width * 1 * 3;
                    }

                    img.is_bigendian = 0;
                    img.data = colorBuffer;
                    
                    // Transmit the byte buffer to color clients.
                    Color.Publish(img);
                    //sw.Stop();
                    //System.Console.WriteLine(sw.ElapsedMilliseconds + " fps " + colorBuffer.Length);
                   

                }
            }
            #endregion

            // If clients exist, convert the RGB frame to a byte array and send it followed by a timestamp.
            #region depth
            using (DepthFrame depthFrame = multiSourceFrame.DepthFrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    FrameDescription frameDesc = depthFrame.FrameDescription;

                    // Allocate a new byte buffer to store this depth frame and timestamp.
                    var depthArraySize = depthFrame.DepthFrameSource.FrameDescription.Height *
                                            depthFrame.DepthFrameSource.FrameDescription.Width *
                                            depthFrame.DepthFrameSource.FrameDescription.BytesPerPixel;
                    var depthBuffer = new byte[depthArraySize];

                    // Convert the depth frame into the byte buffer.
                    using (var depthFrameBuffer = depthFrame.LockImageBuffer())
                    {
                        Marshal.Copy(depthFrameBuffer.UnderlyingBuffer, depthBuffer, 0, (int)depthFrameBuffer.Size);
                    }

              
                    Image img = new Image();
                    img.header = new std_msgs.Header();
                    img.header.seq = 1;
                    img.header.stamp = new std_msgs.Time();
                    img.header.frame_id = "/jkfd";

                  
                    img.width = (uint)frameDesc.Width;
                    img.height = (uint)frameDesc.Height;
                    img.encoding = "bgr8";
                    img.step = img.width * 1 * 3;
                    

                    img.is_bigendian = 0;
                    img.data = depthBuffer;

                    // Transmit the byte buffer to color clients.
                    Depth.Publish(img);
                }
            }
            #endregion

            // If clients exist, convert the IR frame to a byte array and send it followed by a timestamp.
            #region ir
            using (InfraredFrame irFrame = multiSourceFrame.InfraredFrameReference.AcquireFrame())
            {
                if (irFrame != null)
                {
                    // Allocate a new byte buffer to store this IR frame and timestamp.
                    //var irArraySize = irFrame.InfraredFrameSource.FrameDescription.Height *
                    //                    irFrame.InfraredFrameSource.FrameDescription.Width *
                    //                    irFrame.InfraredFrameSource.FrameDescription.BytesPerPixel;
                    //var irBuffer = new byte[irArraySize + sizeof(long)];

                    //// Convert the IR frame into the byte buffer.
                    //using (var irFrameBuffer = irFrame.LockImageBuffer())
                    //{
                    //    Marshal.Copy(irFrameBuffer.UnderlyingBuffer, irBuffer, 0, (int)irFrameBuffer.Size);
                    //}

                    //// Append the system timestamp to the end of the buffer.
                    //Buffer.BlockCopy(timestampBytes, 0, irBuffer, (int)irArraySize, sizeof(long));
                    ////Image img = new Image();
                    //img.header = new std_msgs.Header();
                    //img.header.seq = 1;
                    //img.header.stamp = new std_msgs.Time();
                    //img.header.frame_id = "/jkfd";


                    //img.width = (uint)frame32bpp.Width;
                    //img.height = (uint)frame32bpp.Height;
                    //img.encoding = "bgr8";
                    //img.step = img.width * 1 * 3;


                    //img.is_bigendian = 0;
                    //img.data = irBuffer;

                    //// Transmit the byte buffer to color clients.
                    //Ir.Publish(img);
                }
            }
            #endregion

            // If clients exist, convert the tracked skeletons to a JSON array and send it with a timestamp.
           
            using (BodyFrame bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    var bodyArray = new Body[this.kinect.BodyFrameSource.BodyCount];
                    bodyFrame.GetAndRefreshBodyData(bodyArray);

                    // Configure tracking IDs for bodies that have been added.
                    for (var i = 0; i < bodyArray.Length; ++i)
                    {
                        // Only process the actively tracked bodies.
                        Body body = bodyArray[i];
                        if (!body.IsTracked) continue;

                        // Activate the corresponding face tracker using this body's tracking ID.
                        faceSources[i].TrackingId = body.TrackingId;
                    }
                  

                    // Serialize body tracking information to clients.
                    
                    // Iterate through the full list of bodies (which might not all be tracked).
                    List<Body> bodyList = new List<Body>();
                    for(var i = 0; i < bodyArray.Length; ++i)
                    {
                        // Only process the actively tracked bodies.
                        Body body = bodyArray[i];
                        if (!body.IsTracked) continue;

                        // Add this body to the list of bodies that are serialized to clients.
                        bodyList.Add(body);
                    }


                    if (bodyList.Count > 0)
                    {
                        string json = JsonConvert.SerializeObject(bodyList,
                            new JsonSerializerSettings { ContractResolver = new BodyContractResolver() }) + "\n";

                        Messages.std_msgs.String pow = new Messages.std_msgs.String(json);
                        Body.Publish(pow);
                    }

                }
            }
        }

        private void onAudioFrameArrived(object sender, AudioBeamFrameArrivedEventArgs e)
        {
           

            // Create an audio container representing Kinect audio buffer data.
            var audioContainer = new AudioInfo();
            audioContainer.samplingFrequency = 16000;
            audioContainer.frameLifeTime = 0.016;
            audioContainer.numSamplesPerFrame = (int)(audioContainer.samplingFrequency * audioContainer.frameLifeTime);
            audioContainer.numBytesPerSample = sizeof(float);
       
            // Retrieve audio beams for current frame.
            AudioBeamFrameList frameList = e.FrameReference.AcquireBeamFrames();
            if (frameList == null) return;

            // Serialize all of the subframes and send as a JSON message.
            using (frameList)
            {
                // Only one audio beam is supported. Get the subframe list for the one beam.
                IReadOnlyList<AudioBeamSubFrame> subFrameList = frameList[0].SubFrames;

                float[] audioStream = new float[256];

                // Consolidate the beam subframes into a single JSON message.
                foreach (AudioBeamSubFrame subFrame in subFrameList)
                {
                    using (subFrame)
                    {
                        audioContainer.beamAngle = subFrame.BeamAngle;
                        audioContainer.beamAngleConfidence = subFrame.BeamAngleConfidence;

                        byte[] array = new byte[subFrame.FrameLengthInBytes];
                        subFrame.CopyFrameDataToArray(array);

                        float AccumulatedSquareSum =0;

                        for (int i = 0; i < array.Length; i += sizeof(float))
                        {
                            audioStream[(int)(i / sizeof(float))] = BitConverter.ToSingle(array, i);
                            AccumulatedSquareSum += audioStream[(int)(i / sizeof(float))] * audioStream[(int)(i / sizeof(float))];
                        }

                     
                       
                     

                        float meanSquare = AccumulatedSquareSum / audioStream.Length;

                        if (meanSquare > 1.0f)
                        {
                            // A loud audio source right next to the sensor may result in mean square values
                            // greater than 1.0. Cap it at 1.0f for display purposes.
                            meanSquare = 1.0f;
                        }

                        // Calculate energy in dB, in the range [MinEnergy, 0], where MinEnergy < 0
                        float energy = MinEnergy;

                        if (meanSquare > 0)
                        {
                            energy = (float)(10.0 * Math.Log10(meanSquare));
                        }

                        audioContainer.soundDecibel = energy;

                    

                        // Send audio data to clients.
                        string json = JsonConvert.SerializeObject(audioContainer,
                            new JsonSerializerSettings { ContractResolver = new AudioResolver() }) + "\n";
                      
                        Audio pow = new Audio();
                        pow.data = audioStream;
                        Audio.Publish(pow);

                        Messages.std_msgs.String inf = new Messages.std_msgs.String(json);
                        AudioInfo.Publish(inf);
                    }
                }
            }
        }


        private void onFaceFrameArrived(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
           

            // Retrieve face data for current frame.
            var frame = e.FrameReference.AcquireFrame();
            if (frame == null) return;

            using (frame)
            {
                // Ignore untracked faces.
                if (!frame.IsTrackingIdValid) return;
                if (!frame.IsFaceTracked) return;

                // Record the current Unix epoch timestamp and convert it to a byte array for serialization.
                long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

                // Retrieve face alignment data.
                var faceAlignment = new FaceAlignment();
                frame.GetAndRefreshFaceAlignmentResult(faceAlignment);

                // Combine the body array with a timestamp.
                Dictionary<string, object> faceJson = new Dictionary<string, object>{
                    {"TrackingId", frame.HighDefinitionFaceFrameSource.TrackingId},
                    {"Alignment", faceAlignment},
                };

                // Send face data to clients.
                string json = JsonConvert.SerializeObject(faceJson,
                    new JsonSerializerSettings { ContractResolver = new FaceContractResolver() }) + "\n";
   

             
                Face.Publish(new Messages.std_msgs.String(json));
               
            }
        }

        protected void OnAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            this.IsConnectedChanged(this, new IsConnectedChangedEventArgs(e.IsAvailable));
        }
    }

    /// <summary>
    /// An exception indicating that a Kinect was not detected.
    /// </summary>
    [Serializable]
    public class KinectException : Exception
    {
        public KinectException()
        {
        }

        public KinectException(string message)
            : base(message)
        {
        }

        public KinectException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// Event triggered where the server connects or disconnects from a Kinect.
    /// </summary>
    public class IsConnectedChangedEventArgs : EventArgs
    {
        bool isConnected;

        public IsConnectedChangedEventArgs(bool isConnected)
        {
            this.isConnected = isConnected;
        }

        public bool IsConnected { get { return isConnected; } }
    }
}

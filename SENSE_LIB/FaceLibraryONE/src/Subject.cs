using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sense.Lib.FACELibrary
{
   [Serializable()]
    public class Limb
    {
        private Position Left;
        private Position Right;

        /// <summary>
        /// 
        /// </summary>
        public Position left
        {
            get {return Left;}
            set {Left= value;}
        }
        /// <summary>
        /// 
        /// </summary>
        public Position right
        {
            get { return Right; }
            set { Right = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="leftPos"></param>
        /// <param name="rightPos"></param>
        public Limb(Position leftPos, Position rightPos)
        {
            this.left = leftPos;
            this.right = rightPos;
        }

        public Limb() { }

        public string toStringClips(String name)
        {
            StringBuilder s = new StringBuilder();

            s.AppendFormat("({0}Left {1})\n", name, left.ToString());
            s.AppendFormat("({0}Right {1})\n", name, right.ToString());

            return s.ToString();

        }



    }

    [Serializable()]
    public class Position
    {
        private float x;
        private float y;
        private float z;

        /// <summary>
        ///   The X coordinate of the point
        /// </summary>
        /// 
        public float X 
        {
            get { return x; }
            set { x= value; }
        }

        /// <summary>
        ///   The Y coordinate of the point
        /// </summary>
        public float Y
        {
            get { return y; }
            set { y = value; }
        }
        
        /// <summary>
        ///   The Z coordinate of the point
        /// </summary>
       public float Z
       {
            get { return z; }
            set { z = value; }
        }

       public Position() { }
        public Position(float x1, float y1, float z1) 
        {
            this.X = x1;
            this.Y = y1;
            this.Z = z1;
        }
        override
        public string ToString()
        {
            return X.ToString("F3") + " " + Y.ToString("F3") + " " + Z.ToString("F3");
        }
    }


 
    public class Subject
    {
        private int IdKinect;
        private int Id = 0;
        private bool TrackedState = false;
        private List<string> Name;
        private string Gender = "unknown";
        private int Age;
        private float Speak_prob;
        private int Gesture = 0;
 //     private int Pose = 0;
        private float Uptime;
        private float Angle;
        private float Happiness_ratio;
        private float Anger_ratio;
        private float Sadness_ratio;
        private float Surprise_ratio;

        private Position SpineBase;
        private Position SpineMid;
        private Position Neck;
        private Position Head;
        private Position SpineShoulder;


        private Limb Shoulder = new Limb();
        private Limb Elbow = new Limb();
        private Limb Wrist = new Limb();
        private Limb Hand = new Limb();
        private Limb Hip = new Limb();
        private Limb Knee = new Limb();
        private Limb Ankle = new Limb();
        private Limb Foot = new Limb();
        private Limb HandTip = new Limb();
        private Limb Thumb = new Limb();

        private string leftEyeClosed = "Unknown";
        private string rightEyeClosed = "Unknown";
        private string lookingAway = "Unknown";
        private string wearingGlasses = "Unknown";
        private string mouthOpen = "Unknown";
        private string mouthMoved = "Unknown";
        private string engaged = "Unknown";

        /// <summary>
        /// Indicates whether the kinect has generated the skeleton of the subject
        /// </summary>
        //[XmlIgnoreAttribute()]
        public int idKinect
        {
            get { return IdKinect; }
            set { IdKinect = value; }
        }

        /// <summary>
        /// Subject id
        /// </summary>
        public int id
        {
            get { return Id; }
            set { Id = value; }
        }
     
       /// <summary>
       /// Indicates subject
       /// </summary>
        [XmlIgnoreAttribute()]
        public bool trackedState
        {
            get { return TrackedState; }
            set { TrackedState = value; }
        }
     
        /// <summary>
        /// 
        /// </summary>
        public List<string> name
        {
            get { return Name; }
            set { Name = value; }
        }

        /// <summary>
        /// Set or get the sex of the subject
        /// </summary>
        public string gender
        {
            get { return Gender; }
            set { Gender = value; }
        }
       
        /// <summary>
        /// Set or get the age of the subject
        /// </summary>
        public int age
        {
            get { return Age; }
            set { Age = value; }
        }
       
        /// <summary>
        /// Set or get the probability that the subject is speaking
        /// </summary>
        public float speak_prob
        {
            get { return Speak_prob; }
            set { Speak_prob = value; }
        }

        /// <summary>
        /// Set or get the gesture of the subject (no gesture == 0, hand raised==1)
        /// </summary>
        public int gesture
        {
            get { return Gesture; }
            set { Gesture = value; }
        }

        /// <summary>
        /// Set or get the pose of the subject (up == 0, Sit==1)
        /// </summary>
        //public int pose
        //{
        //    get { return Pose; }
        //    set { Pose = value; }
        //}
        /// <summary>
        /// Set or get how long the subject is present
        /// </summary>
        public float uptime
        {
            get { return Uptime; }
            set { Uptime = value; }
        }

        /// <summary>
        /// Set or get the angle of the subject in the scene
        /// </summary>
        public float angle
        {
            get { return Angle; }
            set { Angle = value; }
        }

        /// <summary>
        /// Set or get the probability of happiness a subject, expressed in percentage
        /// </summary>
        public float happiness_ratio
        {
            get { return Happiness_ratio; }
            set { Happiness_ratio = value; }
        }

        /// <summary>
        /// Set or get the probability that the subject is angry, expressed in percentage
        /// </summary>
        public float anger_ratio
        {
            get { return Anger_ratio; }
            set { Anger_ratio = value; }
        }
      
        /// <summary>
        /// Set or get the probability that the subject is sad, expressed as a percentage
        /// </summary>
        public float sadness_ratio
        {
            get { return Sadness_ratio; }
            set { Sadness_ratio = value; }
        }
       
        /// <summary>
        /// Set or get the probability that the subject is surprised, expressed as a percentage
        /// </summary>
        public float surprise_ratio
        {
            get { return Surprise_ratio; }
            set { Surprise_ratio = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the spin Base of the subject
        /// </summary>
        public Position spineBase 
        {
            get { return SpineBase; }
            set { SpineBase = value; }
        }

        /// <summary>
        ///  Set or get the position (x,y,z) of the spin Mid of the subject
        /// </summary>
        public Position spineMid
        {
            get { return SpineMid; }
            set { SpineMid = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the Neck of the subject
        /// </summary>
        public Position neck
        {
            get { return Neck; }
            set { Neck = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the Head of the subject
        /// </summary>
        public Position head
        {
            get { return Head; }
            set { Head = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the spine Shoulder of the subject
        /// </summary>
        public Position spineShoulder
        {
            get { return SpineShoulder; }
            set { SpineShoulder = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the shoulder (left,right) of the subject
        /// </summary>
        public Limb shoulder
        {
            get { return Shoulder; }
            set { Shoulder = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the Elbow (left,right) of the subject
        /// </summary>
        public Limb elbow
        {
            get { return Elbow; }
            set { Elbow = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the wrist (left,right) of the subject
        /// </summary>
        public Limb wrist
        {
            get { return Wrist; }
            set { Wrist = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the Hand (left,right) of the subject
        /// </summary>
        public Limb hand
        {
            get { return Hand; }
            set { Hand = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the Hip (left,right) of the subject
        /// </summary>
        public Limb hip
        {
            get { return Hip; }
            set { Hip = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the Knee (left,right) of the subject
        /// </summary>
        public Limb knee
        {
            get { return Knee; }
            set { Knee = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the Ankle (left,right) of the subject
        /// </summary>
        public Limb ankle
        {
            get { return Ankle; }
            set { Ankle = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the Foot (left,right) of the subject
        /// </summary>
        public Limb foot
        {
            get { return Foot; }
            set { Foot = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the HandTip (left,right) of the subject
        /// </summary>
        public Limb handTip
        {
            get { return HandTip; }
            set { HandTip = value; }
        }

        /// <summary>
        /// Set or get the position (x,y,z) of the Thumb (left,right) of the subject
        /// </summary>
        public Limb thumb
        {
            get { return Thumb; }
            set { Thumb = value; }
        }


        public string LeftEyeClosed
        {
            get { return leftEyeClosed; }
            set { leftEyeClosed = value; }
        }

        public string RightEyeClosed
        {
            get { return rightEyeClosed; }
            set { rightEyeClosed = value; }
        }


        public string LookingAway
        {
            get { return lookingAway; }
            set { lookingAway = value; }
        }


        public string WearingGlasses
        {
            get { return wearingGlasses; }
            set { wearingGlasses = value; }
        }

        public string MouthOpen
        {
            get { return mouthOpen; }
            set { mouthOpen = value; }
        }
        public string MouthMoved
        {
            get { return mouthMoved; }
            set { mouthMoved = value; }
        }
        public string Engaged
        {
            get { return engaged; }
            set { engaged = value; }
        }
     
        public Subject() { }

        public string ToStringClips() 
        {
            StringBuilder s = new StringBuilder();

            s.Append("(subject \n");
            s.AppendFormat("(IdKinect {0})\n", IdKinect.ToString());
            s.AppendFormat("(Id {0})\n", Id.ToString());
            s.AppendFormat("(TrackedState {0})\n", TrackedState.ToString());

            s.AppendFormat("(Name");
            foreach (object elem in Name)
            {
                s.AppendFormat(" {0} ", elem.ToString());
            }
            s.AppendFormat(")\n");

            s.AppendFormat("(Gender {0})\n", Gender.ToString());
            s.AppendFormat("(Age {0})\n", Age.ToString());
            s.AppendFormat("(Speak_prob {0})\n", Speak_prob.ToString());
            s.AppendFormat("(Gesture {0})\n", Gesture.ToString());
            s.AppendFormat("(Uptime {0})\n", Uptime.ToString());

            s.AppendFormat("(Angle {0})\n", Angle.ToString());
            s.AppendFormat("(Happiness_ratio {0})\n", Happiness_ratio.ToString());
            s.AppendFormat("(Anger_ratio {0})\n", Anger_ratio.ToString());
            s.AppendFormat("(Sadness_ratio {0})\n", Sadness_ratio.ToString());
            s.AppendFormat("(Surprise_ratio {0})\n", Surprise_ratio.ToString());
            s.AppendFormat("(SpineBase {0})\n", SpineBase.ToString());
            s.AppendFormat("(SpineMid {0})\n", SpineMid.ToString());
            s.AppendFormat("(Neck {0})\n", Neck.ToString());
            s.AppendFormat("(Head {0})\n", Head.ToString());
            s.AppendFormat("(SpineShoulder {0})\n", SpineShoulder.ToString());

            s.AppendFormat("({0})\n", Shoulder.toStringClips("Shoulder"));
            s.AppendFormat("({0})\n", Elbow.toStringClips("Elbow"));
            s.AppendFormat("({0})\n", Wrist.toStringClips("Wrist"));
            s.AppendFormat("({0})\n", Hand.toStringClips("Hand"));
            s.AppendFormat("({0})\n", Hip.toStringClips("Hip"));
            s.AppendFormat("({0})\n", Knee.toStringClips("Knee"));
            s.AppendFormat("({0})\n", Ankle.toStringClips("Ankle"));
            s.AppendFormat("({0})\n", Foot.toStringClips("Foot"));
            s.AppendFormat("({0})\n", HandTip.toStringClips("HandTip"));
            s.AppendFormat("({0})\n", Thumb.toStringClips("Thumb"));

            s.AppendFormat("(leftEyeClosed {0})\n", leftEyeClosed);

            s.AppendFormat("(rightEyeClosed {0})\n", rightEyeClosed);
            s.AppendFormat("(lookingAway {0})\n", lookingAway);
            s.AppendFormat("(wearingGlasses {0})\n", wearingGlasses);
            s.AppendFormat("(mouthOpen {0})\n", mouthOpen);
            s.AppendFormat("(mouthMoved {0})\n", mouthMoved);
            s.AppendFormat("(engaged {0})\n", engaged);

            s.AppendFormat(")");


            return s.ToString();




        }

        //public Subject(int idSub, List<string> nameSub, string genderSub, int ageSub,
        //    float speakingProbabilitySub, int gestureSub, float uptimeSub, float angleSub,
        //    float happinessRatioSub, float angerRatioSub, float sadnessRatioSub, float surpriseRatioSub,
        //    List<float> headSub, List<float> spincenterSub, List<float> normalizedSpinCenterSub, List<float> righthandSub, List<float> lefthandSub)
        //{
        //    Id = idSub;
        //    Name = nameSub;            
        //    Gender = genderSub;
        //    Age = ageSub;
        //    Speak_prob = speakingProbabilitySub;
        //    Gesture = gestureSub;
        //    Uptime = uptimeSub;
        //    Angle = angleSub;
        //    Happiness_ratio = happinessRatioSub;
        //    Anger_ratio = angerRatioSub;
        //    Sadness_ratio = sadnessRatioSub;
        //    Surprise_ratio = surpriseRatioSub;
        //    Head_xyz = headSub;
        //    Spincenter_xyz = spincenterSub;
        //    NormalizedSpinCenter_xy = normalizedSpinCenterSub;
        //    Righthand_xyz = righthandSub;
        //    Lefthand_xyz = lefthandSub;
        //}




        //private string leftEyeClosed = "Unknown";
        //private string rightEyeClosed = "Unknown";
        //private string lookingAway = "Unknown";
        //private string wearingGlasses = "Unknown";
        //private string mouthOpen = "Unknown";
        //private string mouthMoved = "Unknown";
        //private string engaged = "Unknown";


    }
}
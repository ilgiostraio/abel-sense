using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sense.Lib.FACELibrary
{

    [Serializable()]
    public class SOME
    {
        private int Mic;
        private int Lux;
        private int ir;
        private bool Touch;
        private int Temp;

        public int mic
        {
            get { return Mic; }
            set { Mic = value; }
        }

        public int lux
        {
            get { return Lux; }
            set { Lux = value; }
        }

        public int IR
        {
            get { return ir; }
            set { ir = value; }
        }

        public bool touch
        {
            get { return Touch; }
            set { Touch = value; }
        }

        public int temp
        {
            get { return Temp; }
            set { Temp = value; }
        }

        public SOME() { }
    }

    [Serializable()]
    public class Saliency
    {
        private List<float> Position;
        private float SaliencyWeight = (float)1;

        /// <summary>
        /// Set or get the position (x,y) of the saliency
        /// </summary>
        public List<float> position
        {
            get { return Position; }
            set { Position = value; }
        }

        /// <summary>
        /// Ser or get value of the saliency (default 1)
        /// </summary>
        public float saliencyWeight
        {
            get { return SaliencyWeight; }
            set { SaliencyWeight = value; }
        }

        public Saliency() { }

        public Saliency(List<float> pos)
        {
            position = pos;
        }
        public Saliency(float x, float y)
        {
            List<float> pos = new List<float>() { x, y };

            position = pos;
        }

    }

    [Serializable()]
    public class Ambience
    {

        public Ambience() { }
    }

    [Serializable()]
    public class Resolution
    {
        private double width;
        private double height;

        public double Width
        {
            get { return width; }
            set { width = value; }
        }

        public double Height
        {
            get { return height; }
            set { height = value; }
        }

        public Resolution() { }

        public Resolution(double h, double w)
        {
            height = h;
            width = w;
        }
    }

    [Serializable()]
    public class Surroundings
    {
        private float SoundAngle;
        private float SoundEstimatedX;
        private string RecognizedWord = "";
        private int NumberSubject = 0;
        private float SoundAverageNorm = 0;
        private float SoundDecibelNorm = 0;

        private SOME Toi = new SOME();
        private Saliency Saliency = new Saliency();
        private Ambience Ambience = new Ambience();
        private Resolution Resolution = new Resolution();

        /// <summary>
        /// Set or get angle to the where comes the sound
        /// </summary>
        public float soundAngle
        {
            get { return SoundAngle; }
            set { SoundAngle = value; }
        }

        /// <summary>
        /// Set or get the estimated position (x) of the sound source (Math.Tan(Math.PI * soundAngle / 180))
        /// </summary>
        public float soundEstimatedX
        {
            get { return SoundEstimatedX; }
            set { SoundEstimatedX = value; }
        }

        public float soundAverageNorm
        {
            get { return SoundAverageNorm; }
            set { SoundAverageNorm = value; }
        }

        public float soundDecibelNorm
        {
            get { return SoundDecibelNorm; }
            set { SoundDecibelNorm = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string recognizedWord
        {
            get { return RecognizedWord; }
            set { RecognizedWord = value; }
        }

        /// <summary>
        /// Set or get the Saliency 
        /// </summary>
        public Saliency saliency
        {
            get { return Saliency; }
            set { Saliency = value; }
        }

        /// <summary>
        /// Set or get TOI-SOME
        /// </summary>
        public SOME toi
        {
            get { return Toi; }
            set { Toi = value; }
        }

        public Ambience ambience
        {
            get { return Ambience; }
            set { Ambience = value; }
        }

        public Resolution resolution
        {
            get { return Resolution; }
            set { Resolution = value; }
        }

        /// <summary>
        /// Set or get the number of subject in the scene
        /// </summary>
        public int numberSubject
        {
            get { return NumberSubject; }
            set { NumberSubject = value; }
        }

        public Surroundings() { }

        //public Surroundings(float angle, float x, string word, List<float> saliency_xy, float saliencyScore, int number)
        //{
        //    soundAngle = angle;
        //    soundEstimatedX = x;
        //    RecognizedWord = word;
        //    VirtualSaliency_xy = saliency_xy;
        //    saliency = saliencyScore;
        //    numberSubject = number;
        //}

    }





}
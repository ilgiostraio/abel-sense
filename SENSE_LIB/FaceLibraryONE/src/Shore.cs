using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sense.Lib.FACELibrary
{
    public class RegionFace 
    {
        float left;
        float top;
        float right;
        float bottom;

        public float Left
        {
            get {return left;}
            set {left = value;}
        }

        public float Top
        {
            get { return top; }
            set { top = value; }
        }

        public float Right
        {
            get { return right; }
            set { right = value; }
        }

        public float Bottom
        {
            get { return bottom; }
            set { bottom = value; }
        }
    }

    public class Eyes 
    {
        Position left = new Position();
        Position right = new Position();

       

        public Position Left
        {
            get { return left; }
            set { left = value; }
        }

        public Position Right
        {
            get { return right; }
            set { right = value; }
        }
    }

    public class Shore
    {
       private Eyes eyes = new Eyes();
       private string gender;
       private int age;

       private float happiness_ratio;
       private float anger_ratio;
       private float sadness_ratio;
       private float surprise_ratio;

       private float uptime;
       private float ageDeviation;
       private RegionFace region =new RegionFace();

       public Eyes Eyes
       {
           get { return eyes;}
           set { eyes = value; }
       }

       /// <summary>
       /// Set or get the sex of the subject
       /// </summary>
       public string Gender
       {
           get { return gender; }
           set { gender = value; }
       }

       /// <summary>
       /// Set or get the age of the subject
       /// </summary>
       public int Age
       {
           get { return age; }
           set { age = value; }
       }

       /// <summary>
       /// Set or get how long the subject is present
       /// </summary>
       public float Uptime
       {
           get { return uptime; }
           set { uptime = value; }
       }


       /// <summary>
       /// Set or get the probability of happiness a subject, expressed in percentage
       /// </summary>
       public float Happiness_ratio
       {
           get { return happiness_ratio; }
           set { happiness_ratio = value; }
       }

       /// <summary>
       /// Set or get the probability that the subject is angry, expressed in percentage
       /// </summary>
       public float Anger_ratio
       {
           get { return anger_ratio; }
           set { anger_ratio = value; }
       }

       /// <summary>
       /// Set or get the probability that the subject is sad, expressed as a percentage
       /// </summary>
       public float Sadness_ratio
       {
           get { return sadness_ratio; }
           set { sadness_ratio = value; }
       }

       /// <summary>
       /// Set or get the probability that the subject is surprised, expressed as a percentage
       /// </summary>
       public float Surprise_ratio
       {
           get { return surprise_ratio; }
           set { surprise_ratio = value; }
       }

       public float Age_deviation
       {
           get { return ageDeviation; }
           set { ageDeviation = value; }
       }

       /// <summary>
       /// 
       /// </summary>
       public RegionFace Region_face
       {
           get { return region; }
           set { region = value; }
       }
    }
}

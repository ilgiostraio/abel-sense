using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sense.Lib.FACELibrary
{
    [Serializable()]
    public class Winner : ICloneable
    {
        private int Id;
        public int id
        {
            get { return Id; }
            set { Id = value; }
        }

        private float SpinX;
        public float spinX
        {
            get { return SpinX; }
            set { SpinX = value; }
        }

        private float SpinY;
        public float spinY
        {
            get { return SpinY; }
            set { SpinY = value; }
        }

        private float SpinZ;
        public float spinZ
        {
            get { return SpinZ; }
            set { SpinZ = value; }
        }

        public Winner() { }

        public Winner(int idWin, float SpinXWin, float SpinYWin, float SpinZWin)
        {
            id = idWin;
            spinX = SpinXWin;
            spinY = SpinYWin;
            spinZ = SpinZWin;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }
}
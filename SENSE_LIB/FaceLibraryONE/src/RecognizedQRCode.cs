using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sense.Lib.FACELibrary
{
    public class ResolutionCam
    {
        private string width;
        private string height;

        public string Width 
        {
            get { return width; }
            set { width = value; } 
        }
        public string Height 
        {
            get { return height; }
            set { height = value; }    
        }
    }

    public class Pos
    {
        private string x;
        private string y;

        public string X 
        {
            get { return x; }
            set { x = value; }    
        }
        public string Y 
        {
            get { return y; }
            set { y = value; }    
        }
    }

    public class InfoQRCode
    {
        private string Msg;
        private List<Pos> pos = new List<Pos>();

        public string Message
        {
            get { return Msg; }
            set { Msg = value; }
        }
        public List<Pos> Positions
        {
            get { return pos; }
            set { pos = value; }
        }
    }

    public class RecognizedQRCode
    {
        private ResolutionCam resolutionCam;
        private List<InfoQRCode> infoQRCode = new List<InfoQRCode>();

        public ResolutionCam ResolutionCam
        {
            get { return resolutionCam; }
            set { resolutionCam = value; }
        }
        
        public List<InfoQRCode> InfoQRCode
        {
            get { return infoQRCode; }
            set { infoQRCode = value; }
        }
    }
   
}

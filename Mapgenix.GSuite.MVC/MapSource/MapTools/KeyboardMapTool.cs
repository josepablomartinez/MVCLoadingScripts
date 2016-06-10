using System;
using System.Runtime.Serialization;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    [DataContract]
    public class KeyboardMapTool : BaseMapTool
    {
        private int _panPixels;

        public KeyboardMapTool()
        {
            this._panPixels = 75;
        }

        [JsonMember(MemberName = "slideFactor")]
        public int PanPixels
        {
            get 
            { 
                return _panPixels; 
            }
            set 
            { 
                _panPixels = value; 
            }
        }
    }
}

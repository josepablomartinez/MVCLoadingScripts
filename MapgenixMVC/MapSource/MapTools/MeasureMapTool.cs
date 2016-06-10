using System;

namespace Mapgenix.GSuite.Mvc
{

    [Serializable]
    public class MeasureMapTool : BaseMapTool {
        private MeasureType _measureType;
        private MeasureUnitType _measureUnitType;
        private bool _geodesic;

        
        public MeasureMapTool()
            : base(false) {
            _measureType = MeasureType.Line;
            _measureUnitType = MeasureUnitType.Metric;
        }

       
        [JsonMember(MemberName = "measureType")]
        public MeasureType MeasureType {
            get {
                return _measureType;
            }
            set {
                if (value != MeasureType.None) {
                    Enabled = true;
                }
                else {
                    Enabled = false;
                }
                _measureType = value;
            }
        }

       
        [JsonMember(MemberName = "displaySystem")]
        public MeasureUnitType MeasureUnitType {
            get { return _measureUnitType; }
            set { _measureUnitType = value; }
        }

       
        [JsonMember(MemberName = "geodesic")]
        public bool Geodesic {
            get {
                return _geodesic;
            }
            set {
                _geodesic = value;
            }
        }
    }
}

using System.Runtime.Serialization;

namespace Mapgenix.GSuite.Mvc
{
    [DataContract]
    internal class JsonFeatureOverlayStyle
    {
        [DataMember(Name = "fillColor")]
        internal string FillColor;

        [DataMember(Name = "strokeColor")]
        internal string StrokeColor;

        [DataMember(Name = "fillOpacity")]
        internal double FillOpacity;

        [DataMember(Name = "strokeOpacity")]
        internal double StrokeOpacity;

        [DataMember(Name = "strokeWidth")]
        internal int StrokeWidth;
    }
}

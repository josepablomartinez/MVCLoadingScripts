using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Mapgenix.GSuite.Mvc
{
    [DataContract]
    internal class JsonMarker
    {
        internal JsonMarker()
        { }

        [DataMember(Name = "id")]
        internal string Id { get; set; }

        [DataMember(Name = "x")]
        internal double X { get; set; }

        [DataMember(Name = "y")]
        internal double Y { get; set; }
    }

    [DataContract]
    internal class JsonSimpleMarkerOverlay
    {
        internal JsonSimpleMarkerOverlay()
        { }

        [DataMember(Name = "id")]
        internal string Id { get; set; }

        [DataMember(Name = "markers")]
        internal Collection<JsonMarker> JsonMarkers { get; set; }
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [DataContract]
    internal class JsonFeature
    {
        [DataMember(Name = "id")]
        internal string Id;

        [DataMember(Name = "wkt")]
        internal string Wkt;

        [DataMember(Name = "fieldValues")]
        internal Dictionary<string, string> FieldValues;

        internal JsonFeature(string id, string wkt, Dictionary<string, string> fieldValues)
        {
            this.Id = id;
            this.Wkt = wkt;
            this.FieldValues = fieldValues;
        }
    }

    [DataContract]
    internal class JsonFeatureCollection
    {
        [DataMember(Name = "Name")]
        public string Name
        { get; set; }

        [DataMember(Name = "features")]
        internal Collection<JsonFeature> JsonFeatures;

        public JsonFeatureCollection(Collection<JsonFeature> jsonFeatures)
        {
            this.JsonFeatures = jsonFeatures;
        }

        internal static string ConvertFeaturesToJson(IEnumerable<Feature> features)
        {
            Collection<JsonFeature> jsonFeatures = new Collection<JsonFeature>();
            foreach (Feature feature in features)
            {
                Dictionary<string, string> jsonFields = new Dictionary<string, string>();
                foreach (string fieldKey in feature.ColumnValues.Keys)
                {
                    jsonFields.Add(fieldKey, feature.ColumnValues[fieldKey]);
                }

                jsonFeatures.Add(new JsonFeature(feature.Id, feature.GetWellKnownText(), jsonFields));
            }

            return JsonConverter.ConvertObjectToStringUsingWcf(new JsonFeatureCollection(jsonFeatures));
        }
    }
}

using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class EditSettings : IJsonSerializable
    {
       
        public EditSettings()
        { }

       
        [JsonMember(MemberName = "drag")]
        public bool IsDraggable { get; set; }

       
        [JsonMember(MemberName = "reshape")]
        public bool IsReshapable { get; set; }

       
        [JsonMember(MemberName = "resize")]
        public bool IsResizable { get; set; }

        
        [JsonMember(MemberName = "rotate")]
        public bool IsRotatable { get; set; }

        #region IJsonSerializable Members

        /// <summary>
        /// Returns a JSON string that contains the information for setting the edit mode at
        /// client side.
        /// </summary>
        /// <returns>A JSON string that is used to set the edit mode at client side.</returns>
        public string ToJson()
        {
            return JsonConverter.ConvertObjectToJson(this);
        }

        #endregion
    }
}

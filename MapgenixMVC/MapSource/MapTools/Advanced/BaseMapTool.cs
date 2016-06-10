using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public abstract class BaseMapTool : IJsonSerializable
    {
        private bool _enabled = false;

        protected BaseMapTool():this(false)
        { }

        protected BaseMapTool(bool enabled)
        {
            this._enabled = enabled;
        }

        [JsonMember(MemberName = "enabled")]
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        #region IJsonSerializable Members

       
        public virtual string ToJson()
        {
            return JsonConverter.ConvertObjectToJson(this);
        }

        #endregion
    }
}

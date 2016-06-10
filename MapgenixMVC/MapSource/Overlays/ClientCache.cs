using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class ClientCache : IJsonSerializable
    {
        private string _cacheId;
        private TimeSpan _duration;

        
        public ClientCache()
            : this(TimeSpan.FromDays(7), string.Empty)
        { }

        public ClientCache(TimeSpan duration)
            : this(duration, string.Empty)
        { }

        public ClientCache(TimeSpan duration, string cacheId)
        {
            this._duration = duration;
            this._cacheId = cacheId;
        }

        public TimeSpan Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        [JsonMember(MemberName = "cacheId")]
        public string CacheId
        {
            get { return _cacheId; }
            set { _cacheId = value; }
        }

       

        #region IJsonSerializable Members

        public virtual string ToJson()
        {
            return JsonConverter.ConvertObjectToJson(this);
        }

        #endregion
    }
}

using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class ServerCache
    {
        private string _cacheDirectory;
        private string _cacheId;

        public ServerCache()
            : this(string.Empty)
        { }

        public ServerCache(string cacheDirectory)
        {
            this._cacheDirectory = cacheDirectory;
        }

        public string CacheDirectory
        {
            get { return _cacheDirectory; }
            set { _cacheDirectory = value; }
        }

        public string CacheId
        {
            get { return _cacheId; }
            set { _cacheId = value; }
        }

     }
}

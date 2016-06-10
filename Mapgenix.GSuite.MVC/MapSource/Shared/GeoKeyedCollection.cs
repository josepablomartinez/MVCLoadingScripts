using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class GeoKeyedCollection<T> : KeyedCollection<string, T>, IJsonSerializable where T : IRequireId
    {
        public GeoKeyedCollection()
        { }

        protected override string GetKeyForItem(T item)
        {
            return item.Id;
        }

        public string ToJson()
        {
            StringBuilder json = new StringBuilder();
            JsonConverter.WriteJsonCollectionStart(json);
            foreach (T item in this)
            {
                IJsonSerializable tempItem = item as IJsonSerializable;
                if (tempItem == null)
                {
                    JsonConverter.WriteJsonItem(json, item.ToString(), true);
                }
                else
                {
                    JsonConverter.WriteJsonItem(json, tempItem);
                }
            }
            JsonConverter.WriteJsonCollectionEnd(json);

            return json.ToString();
        }
    }
}

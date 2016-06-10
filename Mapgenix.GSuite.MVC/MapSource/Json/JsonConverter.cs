using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using Mapgenix.Shapes;
using Mapgenix.Canvas;

namespace Mapgenix.GSuite.Mvc
{
    internal static class JsonConverter
    {
        internal static string ConvertObjectToStringUsingWcf(object jsonObject)
        {
            string resultString;

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(jsonObject.GetType());
            MemoryStream jsonStream = new MemoryStream();
            jsonSerializer.WriteObject(jsonStream, jsonObject);
            resultString = Encoding.Default.GetString(jsonStream.GetBuffer());
            jsonStream.Close();

            return resultString.Trim('\0');
        }

        internal static object ConvertStringToObjectUsingWcf(string jsonString, Type type)
        {
            object resultObject;
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(type);
            MemoryStream jsonStream = new MemoryStream(Encoding.Default.GetBytes(jsonString));
            resultObject = jsonSerializer.ReadObject(jsonStream);
            jsonStream.Close();

            return resultObject;
        }

        #region Convert object to json

        private static string ConvertShapeToJson(BaseShape shape)
        {
            PointShape point = shape as PointShape;
            RectangleShape rectangle = shape as RectangleShape;

            if (point != null)
            {
                return ConvertShapeToJson(point);
            }
            else if (rectangle != null)
            {
                return ConvertShapeToJson(rectangle);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private static string ConvertShapeToJson(RectangleShape rectangleShape)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonStart(json);
            WriteJsonContent(json, "left", rectangleShape.LowerLeftPoint.X.ToString(CultureInfo.InvariantCulture));
            WriteJsonContent(json, "bottom", rectangleShape.LowerLeftPoint.Y.ToString(CultureInfo.InvariantCulture));
            WriteJsonContent(json, "right", rectangleShape.UpperRightPoint.X.ToString(CultureInfo.InvariantCulture));
            WriteJsonContent(json, "top", rectangleShape.UpperRightPoint.Y.ToString(CultureInfo.InvariantCulture));
            WriteJsonEnd(json);
            return json.ToString();
        }

        private static string ConvertShapeToJson(PointShape pointShape)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonStart(json);
            WriteJsonContent(json, "x", pointShape.X.ToString(CultureInfo.InvariantCulture));
            WriteJsonContent(json, "y", pointShape.Y.ToString(CultureInfo.InvariantCulture));
            WriteJsonEnd(json);
            return json.ToString();
        }

        internal static string ConvertJsonCollectionToJson(ICollection<IJsonSerializable> items)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonCollectionStart(json);
            foreach (IJsonSerializable item in items)
            {
                WriteJsonItem(json, item);
            }
            WriteJsonCollectionEnd(json);

            return json.ToString();
        }

        internal static string ConvertDoubleCollectionToJson(ICollection<double> items)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonCollectionStart(json);
            foreach (double item in items)
            {
                WriteJsonItem(json, item.ToString("R", CultureInfo.InvariantCulture));
            }
            WriteJsonCollectionEnd(json);

            return json.ToString();
        }

        private static string ConvertUriCollectionToJson(ICollection<Uri> items)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonCollectionStart(json);
            foreach (Uri item in items)
            {
                WriteJsonItem(json, item.ToString(), true);
            }
            WriteJsonCollectionEnd(json);

            return json.ToString();
        }

        private static string ConvertDictionaryToJson(IDictionary<string, IJsonSerializable> values)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonStart(json);
            foreach (string key in values.Keys)
            {
                WriteJsonContent(json, key, ((IJsonSerializable)values[key]).ToJson());
            }
            WriteJsonEnd(json);

            return json.ToString();
        }

        private static string ConvertDictionaryToJson(IDictionary<string, string> values, bool withQuot)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonStart(json);
            foreach (string key in values.Keys)
            {
                WriteJsonContent(json, key, values[key].ToString(), withQuot);
            }
            WriteJsonEnd(json);

            return json.ToString();
        }

        private static string ConvertDictionaryToJson(IDictionary<string, Feature> values)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonStart(json);
            foreach (string key in values.Keys)
            {
                WriteJsonContent(json, key, ConvertFeatureToJson(values[key]));
            }
            WriteJsonEnd(json);

            return json.ToString();
        }

        private static string ConvertFeatureToJson(Feature feature)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonStart(json);
            WriteJsonContent(json, "id", feature.Id, true);
            WriteJsonContent(json, "wkt", feature.GetWellKnownText(), true);
            WriteJsonContent(json, "values", ConvertDictionaryToJson(feature.ColumnValues, true));
            WriteJsonEnd(json);
            return json.ToString();
        }

        private static string ConvertFeatureCollectionToJson(ICollection<Feature> features)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonCollectionStart(json);
            foreach (Feature feature in features)
            {
                WriteJsonItem(json, ConvertFeatureToJson(feature));
            }
            WriteJsonCollectionEnd(json);

            return json.ToString();
        }

        private static string ConvertStringCollectionToJson(ICollection<string> items)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonCollectionStart(json);
            foreach (string item in items)
            {
                WriteJsonItem(json, item, true);
            }
            WriteJsonCollectionEnd(json);
            return json.ToString();
        }

        private static string ConvertContextMenuItemCollectionToJson(ICollection<ContextMenuItem> items)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonCollectionStart(json);
            foreach (ContextMenuItem item in items)
            {
                WriteJsonItem(json, ConvertObjectToJson(item));
            }
            WriteJsonCollectionEnd(json);
            return json.ToString();
        }
        #endregion

        #region Json Writer
        private static void WriteJsonStart(StringBuilder json)
        {
            json.Append("{");
        }

        private static void WriteJsonEnd(StringBuilder json)
        {
            if (json.ToString().EndsWith(",", StringComparison.OrdinalIgnoreCase))
            {
                json.Remove(json.Length - 1, 1);
            }
            json.Append("}");
        }

        internal static void WriteJsonCollectionStart(StringBuilder json)
        {
            json.Append("[");
        }

        internal static void WriteJsonCollectionEnd(StringBuilder json)
        {
            if (json.ToString().EndsWith(",", StringComparison.OrdinalIgnoreCase))
            {
                json.Remove(json.Length - 1, 1);
            }
            json.Append("]");
        }

        private static void WriteJsonContent(StringBuilder json, string key, string value)
        {
            WriteJsonContent(json, key, value, false, new string[] { });
        }

        private static void WriteJsonContent(StringBuilder json, string key, string value, bool withQuot)
        {
            WriteJsonContent(json, key, value, withQuot, new string[] { });
        }

        private static void WriteJsonContent(StringBuilder json, string key, string value, params string[] parameters)
        {
            WriteJsonContent(json, key, value, false, parameters);
        }

        private static void WriteJsonContent(StringBuilder json, string key, string value, bool withQuot, params string[] parameters)
        {
            if (parameters.Length != 0)
            {
                value = String.Format(CultureInfo.InvariantCulture, value, parameters);
            }

            if (withQuot)
            {
                json.AppendFormat(CultureInfo.InvariantCulture, "\"{0}\":\"{1}\",", key, value);
            }
            else
            {
                json.AppendFormat(CultureInfo.InvariantCulture, "\"{0}\":{1},", key, value);
            }
        }

        internal static void WriteJsonItem(StringBuilder json, IJsonSerializable jsonItem)
        {
            WriteJsonItem(json, jsonItem.ToJson(), false);
        }

        internal static void WriteJsonItem(StringBuilder json, string jsonItem)
        {
            WriteJsonItem(json, jsonItem, false);
        }

        internal static void WriteJsonItem(StringBuilder json, string jsonItem, bool withQuot)
        {
            if (withQuot)
            {
                json.AppendFormat(CultureInfo.InvariantCulture, "\"{0}\"", jsonItem.ToString());
            }
            else
            {
                json.Append(jsonItem);
            }
            json.Append(",");
        }
        #endregion

        internal static string ConvertObjectToJson(Object jsonObject)
        {
            StringBuilder json = new StringBuilder();
            WriteJsonStart(json);

            PropertyInfo[] properties = jsonObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                JsonMemberAttribute jsonAttribute = (JsonMemberAttribute)Attribute.GetCustomAttribute(property, typeof(JsonMemberAttribute));
                if (jsonAttribute != null)
                {
                    string memberName = jsonAttribute.MemberName;
                    string valuePropertyName = jsonAttribute.ValuePropertyName;
                    if (String.IsNullOrEmpty(memberName))
                    {
                        memberName = property.Name;
                    }

                    object value = property.GetValue(jsonObject, null);
                    if (value == null)
                    {
                        continue;
                    }

                    Type valueType = value.GetType();

                    if (!String.IsNullOrEmpty(valuePropertyName))
                    {
                        value = valueType.GetProperty(valuePropertyName).GetValue(value, null);
                        valueType = value.GetType();
                    }

                    if (valueType.IsArray)
                    {
                        WriteJsonContent(json, memberName, JsonConverter.ConvertJsonCollectionToJson((Collection<IJsonSerializable>)value));
                    }
                    else if (value is IJsonSerializable)
                    {
                        WriteJsonContent(json, memberName, ((IJsonSerializable)value).ToJson());
                    }
                    else if (valueType.Equals(typeof(string)) || valueType.Equals(typeof(Uri)))
                    {
                        WriteJsonContent(json, memberName, value.ToString(), true);
                    }
                    else if (value is BaseShape)
                    {
                        WriteJsonContent(json, memberName, JsonConverter.ConvertShapeToJson((BaseShape)value));
                    }
                    else if (value is Boolean)
                    {
                        WriteJsonContent(json, memberName, value.ToString().ToLowerInvariant());
                    }
                    else if (value is GeoColor)
                    {
                        WriteJsonContent(json, memberName, GeoColor.ToHtml((GeoColor)value), true);
                    }
                    else if (value is IDictionary<string, string>)
                    {
                        WriteJsonContent(json, memberName, ConvertDictionaryToJson((IDictionary<string, string>)value, true));
                    }
                    else if (value is IDictionary<string, Feature>)
                    {
                        WriteJsonContent(json, memberName, ConvertDictionaryToJson((IDictionary<string, Feature>)value));
                    }
                    else if (value is ICollection<Uri>)
                    {
                        WriteJsonContent(json, memberName, ConvertUriCollectionToJson((Collection<Uri>)value));
                    }
                    else if (value is ICollection<Feature>)
                    {
                        WriteJsonContent(json, memberName, ConvertFeatureCollectionToJson((Collection<Feature>)value));
                    }
                    else if (value is ICollection<double>)
                    {
                        WriteJsonContent(json, memberName, ConvertDoubleCollectionToJson((Collection<double>)value));
                    }
                    else if (value is ICollection<string>)
                    {
                        WriteJsonContent(json, memberName, ConvertStringCollectionToJson((Collection<string>)value));
                    }
                    else if (value is ICollection<ContextMenuItem>)
                    {
                        WriteJsonContent(json, memberName, ConvertContextMenuItemCollectionToJson((Collection<ContextMenuItem>)value));
                    }
                    else if (valueType.IsEnum)
                    {
                        string enumValue = value.ToString();
                        JsonMemberAttribute[] jsonMemberAttributes = (JsonMemberAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(JsonMemberAttribute), false);
                        if (jsonMemberAttributes != null && jsonMemberAttributes.Length != 0)
                        {
                            if (!String.IsNullOrEmpty(jsonMemberAttributes[0].MemberName))
                            {
                                enumValue = jsonMemberAttributes[0].MemberName;
                            }
                        }
                        WriteJsonContent(json, memberName, enumValue.ToString(), true);
                    }
                    else
                    {
                        WriteJsonContent(json, memberName, Convert.ToString(value, CultureInfo.InvariantCulture));
                    }
                }
            }
            WriteJsonEnd(json);
            return json.ToString();
        }
    }
}

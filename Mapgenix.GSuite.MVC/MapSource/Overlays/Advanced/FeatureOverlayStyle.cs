using System;
using Mapgenix.Canvas;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class FeatureOverlayStyle : IJsonSerializable
    {
        private int _outlineWidth;

        public FeatureOverlayStyle()
            : this(GeoColor.StandardColors.Transparent, GeoColor.StandardColors.Transparent, 1)
        {
        }

        public FeatureOverlayStyle(GeoColor fillColor, GeoColor outlineColor, int outlineWidth)
        {
            Validators.CheckValueIsGreaterOrEqualToZero(outlineWidth, "OutlineWidth");
            FillColor = fillColor;
            OutlineColor = outlineColor;
            this._outlineWidth = outlineWidth;
        }

        
        public GeoColor FillColor { get; set; }

       
        public GeoColor OutlineColor { get; set; }

       
        public int OutlineWidth
        {
            get 
            { 
                return _outlineWidth;
            }
            set 
            {
                Validators.CheckValueIsGreaterOrEqualToZero(value, "OutlineWidth");
                _outlineWidth = value; 
            }
        }

        #region IJsonSerializable Members

       
        public string ToJson()
        {
            JsonFeatureOverlayStyle jsonStyle = new JsonFeatureOverlayStyle();
            jsonStyle.FillColor = GeoColor.ToHtml(FillColor);
            jsonStyle.FillOpacity = Convert.ToDouble(FillColor.AlphaComponent) / 255.0;
            jsonStyle.StrokeColor = GeoColor.ToHtml(OutlineColor);
            jsonStyle.StrokeOpacity = Convert.ToDouble(OutlineColor.AlphaComponent) / 255.0;
            jsonStyle.StrokeWidth = OutlineWidth;
            return JsonConverter.ConvertObjectToStringUsingWcf(jsonStyle);
        }

        #endregion
    }
}

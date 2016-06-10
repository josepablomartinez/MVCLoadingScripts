using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class ValueMarkerStyle : BaseMarkerStyle
    {
        private Collection<MarkerValueItem> _valueItems;
        private string _columnName;

        public ValueMarkerStyle()
            : this(String.Empty, new Collection<MarkerValueItem>())
        { }

       
        public ValueMarkerStyle(string columnName)
            : this(columnName, new Collection<MarkerValueItem>())
        { }

      
        public ValueMarkerStyle(string columnName, Collection<MarkerValueItem> valueItems)
        {
            //Validators.CheckParameterIsNotNullOrEmpty(columnName, "columnName");
            //Validators.CheckParameterIsNotNull(valueItems, "valueItems");

            this._columnName = columnName;
            this._valueItems = valueItems;
        }

        public Collection<MarkerValueItem> ValueItems
        {
            get { return _valueItems; }
        }

        public string ColumnName
        {
            get 
            { 
                return _columnName;
            }
            set
            {
                //Validators.CheckParameterIsNotNullOrEmpty(value, "ColumnName");
                _columnName = value;
            }
        }

        private MarkerValueItem GetValueItem(string itemValue)
        {
            MarkerValueItem returnValueItem = null;

            foreach (MarkerValueItem valueItem in _valueItems)
            {
                if (valueItem.Value == itemValue)
                {
                    returnValueItem = valueItem;
                    break;
                }
            }

            return returnValueItem;
        }

        protected override GeoKeyedCollection<ContextMenu> GetContextMenusCore()
        {
            GeoKeyedCollection<ContextMenu> contextMenus = new GeoKeyedCollection<ContextMenu>();
            foreach (MarkerValueItem valueItem in ValueItems)
            {
                if (valueItem.CustomMarkerStyle != null)
                {
                    foreach (ContextMenu contextMenu in valueItem.CustomMarkerStyle.GetContextMenus())
                    {
                        if (!contextMenus.Contains(contextMenu.Id))
                            contextMenus.Add(contextMenu);
                    }
                }

                if (valueItem.DefaultMarkerStyle.ContextMenu != null)
                {
                    if (!contextMenus.Contains(valueItem.DefaultMarkerStyle.ContextMenu.Id))
                        contextMenus.Add(valueItem.DefaultMarkerStyle.ContextMenu);
                }
            }
            return contextMenus;
        }

     
        public override Collection<Marker> GetMarkers(IEnumerable<Feature> features)
        {
            //Validators.CheckParameterIsNotNull(features, "features");
            //Validators.CheckColumnNameIsInFeature(_columnName, features);

            Collection<Marker> returnMarkers = new Collection<Marker>();

            foreach (Feature feature in features)
            {
                string columnValue = feature.ColumnValues[ColumnName];
                MarkerValueItem valueItem = GetValueItem(columnValue);
                
                if(valueItem != null)
                {
                    if (valueItem.CustomMarkerStyle != null)
                    {
                        Collection<Marker> tempMarkers = valueItem.CustomMarkerStyle.GetMarkers(new Collection<Feature>() { feature });
                        foreach (Marker marker in tempMarkers)
                        {
                            returnMarkers.Add(marker);
                        }
                    }
                    else
                    {
                        Collection<Marker> tempMarkers = valueItem.DefaultMarkerStyle.GetMarkers(new Collection<Feature>() { feature });
                        foreach (Marker marker in tempMarkers)
                        {
                            returnMarkers.Add(marker);
                        }
                    }
                }
            }

            return returnMarkers;
        }
    }
}

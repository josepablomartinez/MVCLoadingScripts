using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Mapgenix.Shapes;
using Mapgenix.Styles;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class ClassBreakMarkerStyle : BaseMarkerStyle
    {
        private string _columnName;
        private BreakValueInclusion _breakValueInclusion;
        private Collection<MarkerClassBreak> _classBreaks;

        public ClassBreakMarkerStyle()
            : this(String.Empty, BreakValueInclusion.IncludeValue, new Collection<MarkerClassBreak>())
        { }

        public ClassBreakMarkerStyle(string columnName)
            : this(columnName, BreakValueInclusion.IncludeValue, new Collection<MarkerClassBreak>())
        { }

       
        public ClassBreakMarkerStyle(string columnName, BreakValueInclusion breakValueInclusion)
            : this(columnName, breakValueInclusion, new Collection<MarkerClassBreak>())
        { }

        public ClassBreakMarkerStyle(string columnName, BreakValueInclusion breakValueInclusion, Collection<MarkerClassBreak> classBreaks)
        {
            //Validators.CheckParameterIsNotNullOrEmpty(columnName, "columnName");
            //Validators.CheckBreakValueInclusionIsValid(breakValueInclusion);
            //Validators.CheckParameterIsNotNull(classBreaks, "classBreaks");

            this._columnName = columnName;
            this._breakValueInclusion = breakValueInclusion;
            this._classBreaks = classBreaks;
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

        public BreakValueInclusion BreakValueInclusion
        {
            get { return _breakValueInclusion; }
            set 
            {
                //Validators.CheckBreakValueInclusionIsValid(value);
                _breakValueInclusion = value; 
            }
        }

        public Collection<MarkerClassBreak> ClassBreaks
        {
            get { return _classBreaks; }
        }

       
        protected override GeoKeyedCollection<ContextMenu> GetContextMenusCore()
        {
            GeoKeyedCollection<ContextMenu> contextMenus = new GeoKeyedCollection<ContextMenu>();
            foreach (MarkerClassBreak classBreak in ClassBreaks)
            {
               
                if (classBreak.CustomMarkerStyle != null)
                {
                    foreach (ContextMenu contextMenu in classBreak.CustomMarkerStyle.GetContextMenus())
                    {
                        if (!contextMenus.Contains(contextMenu.Id))
                            contextMenus.Add(contextMenu);
                    }
                }

                if (classBreak.DefaultMarkerStyle.ContextMenu != null)
                {
                    if (!contextMenus.Contains(classBreak.DefaultMarkerStyle.ContextMenu.Id))
                        contextMenus.Add(classBreak.DefaultMarkerStyle.ContextMenu);
                }
            }
            return contextMenus;
        }

        public override Collection<Marker> GetMarkers(IEnumerable<Feature> features)
        {
            //Validators.CheckParameterIsNotNull(features, "features");
            //Validators.CheckColumnNameIsInFeature(_columnName, features);
            //Validators.CheckClassBreaksAreValid(_classBreaks);

            Collection<Marker> returnMarkers = new Collection<Marker>();

            foreach (Feature feature in features)
            {
                double columnValue = double.Parse(feature.ColumnValues[ColumnName].Trim(), CultureInfo.InvariantCulture);
                MarkerClassBreak classBreak = GetClassBreak(columnValue);
                if (classBreak != null)
                {
                    if (classBreak.CustomMarkerStyle != null)
                    {
                        Collection<Marker> tempMarkers = classBreak.CustomMarkerStyle.GetMarkers(new Collection<Feature>() { feature });
                        foreach (Marker marker in tempMarkers)
                        {
                            returnMarkers.Add(marker);
                        }
                    }
                    else
                    {
                        Collection<Marker> tempMarkers = classBreak.DefaultMarkerStyle.GetMarkers(new Collection<Feature>() { feature });
                        foreach (Marker marker in tempMarkers)
                        {
                            returnMarkers.Add(marker);
                        }

                    }
                }
            }

            return returnMarkers;
        }

        private MarkerClassBreak GetClassBreak(double breakValue)
        {
            MarkerClassBreak result = _classBreaks[_classBreaks.Count - 1];

            if (_breakValueInclusion == BreakValueInclusion.IncludeValue)
            {
                if (breakValue <= _classBreaks[0].Value)
                {
                    return null;
                }

                for (int i = 0; i < _classBreaks.Count; i++)
                {
                    if (breakValue < _classBreaks[i].Value)
                    {
                        result = _classBreaks[i - 1];
                        break;
                    }
                }
            }
            else
            {
                if (breakValue < _classBreaks[0].Value)
                {
                    return null;
                }

                for (int i = 1; i < _classBreaks.Count; i++)
                {
                    if (breakValue <= _classBreaks[i].Value)
                    {
                        result = _classBreaks[i - 1];
                        break;
                    }
                }
            }

            return result;
        }
    }
}

using System;
using System.Collections.ObjectModel;
using Mapgenix.Canvas;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class MarkerZoomLevelSet
    {
        private MarkerZoomLevel[] _defaultZoomLevels;
        private Collection<MarkerZoomLevel> _customZoomLevels;

        
        public MarkerZoomLevelSet()
        {
            _defaultZoomLevels = new MarkerZoomLevel[20];
        }

        
        public MarkerZoomLevel ZoomLevel01
        {
            get
            {
                if (_defaultZoomLevels[0] == null)
                {
                    _defaultZoomLevels[0] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[0];
            }
        }

        
        public MarkerZoomLevel ZoomLevel02
        {
            get
            {
                if (_defaultZoomLevels[1] == null)
                {
                    _defaultZoomLevels[1] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[1];
            }
        }

        
        public MarkerZoomLevel ZoomLevel03
        {
            get
            {
                if (_defaultZoomLevels[2] == null)
                {
                    _defaultZoomLevels[2] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[2];
            }
        }

        
        public MarkerZoomLevel ZoomLevel04
        {
            get
            {
                if (_defaultZoomLevels[3] == null)
                {
                    _defaultZoomLevels[3] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[3];
            }
        }

       
        public MarkerZoomLevel ZoomLevel05
        {
            get
            {
                if (_defaultZoomLevels[4] == null)
                {
                    _defaultZoomLevels[4] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[4];
            }
        }

        
        public MarkerZoomLevel ZoomLevel06
        {
            get
            {
                if (_defaultZoomLevels[5] == null)
                {
                    _defaultZoomLevels[5] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[5];
            }
        }

        
        public MarkerZoomLevel ZoomLevel07
        {
            get
            {
                if (_defaultZoomLevels[6] == null)
                {
                    _defaultZoomLevels[6] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[6];
            }
        }

        
        public MarkerZoomLevel ZoomLevel08
        {
            get
            {
                if (_defaultZoomLevels[7] == null)
                {
                    _defaultZoomLevels[7] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[7];
            }
        }

        
        public MarkerZoomLevel ZoomLevel09
        {
            get
            {
                if (_defaultZoomLevels[8] == null)
                {
                    _defaultZoomLevels[8] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[8];
            }
        }

        
        public MarkerZoomLevel ZoomLevel10
        {
            get
            {
                if (_defaultZoomLevels[9] == null)
                {
                    _defaultZoomLevels[9] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[9];
            }
        }

        
        public MarkerZoomLevel ZoomLevel11
        {
            get
            {
                if (_defaultZoomLevels[10] == null)
                {
                    _defaultZoomLevels[10] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[10];
            }
        }

        
        public MarkerZoomLevel ZoomLevel12
        {
            get
            {
                if (_defaultZoomLevels[11] == null)
                {
                    _defaultZoomLevels[11] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[11];
            }
        }

        
        public MarkerZoomLevel ZoomLevel13
        {
            get
            {
                if (_defaultZoomLevels[12] == null)
                {
                    _defaultZoomLevels[12] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[12];
            }
        }

        public MarkerZoomLevel ZoomLevel14
        {
            get
            {
                if (_defaultZoomLevels[13] == null)
                {
                    _defaultZoomLevels[13] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[13];
            }
        }

        public MarkerZoomLevel ZoomLevel15
        {
            get
            {
                if (_defaultZoomLevels[14] == null)
                {
                    _defaultZoomLevels[14] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[14];
            }
        }

        public MarkerZoomLevel ZoomLevel16
        {
            get
            {
                if (_defaultZoomLevels[15] == null)
                {
                    _defaultZoomLevels[15] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[15];
            }
        }

        public MarkerZoomLevel ZoomLevel17
        {
            get
            {
                if (_defaultZoomLevels[16] == null)
                {
                    _defaultZoomLevels[16] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[16];
            }
        }

        public MarkerZoomLevel ZoomLevel18
        {
            get
            {
                if (_defaultZoomLevels[17] == null)
                {
                    _defaultZoomLevels[17] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[17];
            }
        }

        public MarkerZoomLevel ZoomLevel19
        {
            get
            {
                if (_defaultZoomLevels[18] == null)
                {
                    _defaultZoomLevels[18] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[18];
            }
        }

        public MarkerZoomLevel ZoomLevel20
        {
            get
            {
                if (_defaultZoomLevels[19] == null)
                {
                    _defaultZoomLevels[19] = new MarkerZoomLevel();
                }
                return _defaultZoomLevels[19];
            }
        }

        public Collection<MarkerZoomLevel> CustomZoomLevels
        {
            get
            {
                if (_customZoomLevels == null)
                {
                    _customZoomLevels = new Collection<MarkerZoomLevel>();
                }
                return _customZoomLevels;
            }
        }

        internal MarkerZoomLevel SelectZoomLevelById(int id)
        {
            if (CustomZoomLevels.Count > 0)
            {
                if (id > 0 && id <= CustomZoomLevels.Count)
                {
                    return CustomZoomLevels[id - 1];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (id > 0 && id <= 20)
                {
                    return _defaultZoomLevels[id - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        public MarkerZoomLevel GetZoomLevelForDrawing(int currentZoomLevelId)
        {
            MarkerZoomLevel returnZoomLevel = null;
            if (CustomZoomLevels.Count > 0)
            {
                returnZoomLevel = GetZoomLevelFromCustomZoomLevels(currentZoomLevelId);
            }
            else
            {
                returnZoomLevel = GetZoomLevelFromDefaultZoomLevels(currentZoomLevelId);
            }
            return returnZoomLevel;
        }

        private MarkerZoomLevel GetZoomLevelFromCustomZoomLevels(int currentZoomLevelId)
        {
            MarkerZoomLevel returnZoomLevel = null;
            MarkerZoomLevel selectedZoomLevel = SelectZoomLevelById(currentZoomLevelId);
            if (selectedZoomLevel != null)
            {
                if (selectedZoomLevel.HasStyleDefined)
                {
                    returnZoomLevel = selectedZoomLevel;
                }
                else
                {
                    for (int i = 1; i <= CustomZoomLevels.Count; i++)
                    {
                        MarkerZoomLevel zoomLevel = CustomZoomLevels[i - 1];
                        if (zoomLevel.HasStyleDefined && zoomLevel.ApplyUntilZoomLevel != ApplyUntilZoomLevel.None)
                        {
                            int upperZoomLevelId = i;
                            int lowerZoomLevelId = (int)zoomLevel.ApplyUntilZoomLevel;

                            if (lowerZoomLevelId < upperZoomLevelId)
                            {
                                upperZoomLevelId = lowerZoomLevelId;
                                lowerZoomLevelId = i;
                            }

                            if (currentZoomLevelId >= upperZoomLevelId && currentZoomLevelId <= lowerZoomLevelId)
                            {
                                returnZoomLevel = zoomLevel;
                                break;
                            }
                        }
                    }
                }
            }
            return returnZoomLevel;
        }

        private MarkerZoomLevel GetZoomLevelFromDefaultZoomLevels(int currentZoomLevelId)
        {
            MarkerZoomLevel returnZoomLevel = null;
            MarkerZoomLevel selectedZoomLevel = SelectZoomLevelById(currentZoomLevelId);
            if (selectedZoomLevel != null && selectedZoomLevel.HasStyleDefined)
            {
                returnZoomLevel = selectedZoomLevel;
            }
            else
            {
                for (int i = 1; i <= 20; i++)
                {
                    MarkerZoomLevel zoomLevel = SelectZoomLevelById(i);
                    if (zoomLevel != null && zoomLevel.HasStyleDefined && zoomLevel.ApplyUntilZoomLevel != ApplyUntilZoomLevel.None)
                    {
                        int upperZoomLevelId = i;
                        int lowerZoomLevelId = (int)zoomLevel.ApplyUntilZoomLevel;

                        if (lowerZoomLevelId < upperZoomLevelId)
                        {
                            upperZoomLevelId = lowerZoomLevelId;
                            lowerZoomLevelId = i;
                        }

                        if (currentZoomLevelId >= upperZoomLevelId && currentZoomLevelId <= lowerZoomLevelId)
                        {
                            returnZoomLevel = zoomLevel;
                            break;
                        }
                    }
                }
            }
            return returnZoomLevel;
        }

        internal void DetachContextMenuClickEvents()
        {
            foreach (MarkerZoomLevel zoomlevel in CustomZoomLevels)
            {
                if (zoomlevel != null)
                {
                    GeoKeyedCollection<ContextMenu> contextMenus = zoomlevel.GetContextMenus();
                    foreach (ContextMenu contextMenu in contextMenus)
                    {
                        contextMenu.DetachItemsClickEvent();
                    }
                }
            }

            foreach (MarkerZoomLevel zoomlevel in _defaultZoomLevels)
            {
                if (zoomlevel != null)
                {
                    GeoKeyedCollection<ContextMenu> contextMenus = zoomlevel.GetContextMenus();
                    foreach (ContextMenu contextMenu in contextMenus)
                    {
                        contextMenu.DetachItemsClickEvent();
                    }
                }
            }
        }

        internal void RewireContextMenuClickEvents(object target)
        {
            foreach (MarkerZoomLevel zoomlevel in CustomZoomLevels)
            {
                if (zoomlevel != null)
                {
                    GeoKeyedCollection<ContextMenu> contextMenus = zoomlevel.GetContextMenus();
                    foreach (ContextMenu contextMenu in contextMenus)
                    {
                        contextMenu.RewireItemsClickEvent(target);
                    }
                }
            }

            foreach (MarkerZoomLevel zoomlevel in _defaultZoomLevels)
            {
                if (zoomlevel != null)
                {
                    GeoKeyedCollection<ContextMenu> contextMenus = zoomlevel.GetContextMenus();
                    foreach (ContextMenu contextMenu in contextMenus)
                    {
                        contextMenu.RewireItemsClickEvent(target);
                    }
                }
            }
        }
    }
}

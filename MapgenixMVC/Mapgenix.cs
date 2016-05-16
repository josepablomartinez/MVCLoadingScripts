
using Mapgenix.Canvas;
using Mapgenix.GSuite.Web;
using Mapgenix.Layers;
using Mapgenix.Shapes;
using Mapgenix.Styles;
using System.Web.Mvc;

namespace Mapgenix
{

    public class Mapgenix
    {
        private HtmlHelper htmlHelper;
        SimpleScriptManager.SimpleScriptManager scriptmanager;
        Map gsuitemap;

        private System.Drawing.Color _backgroundColor = System.Drawing.Color.Blue;
        private int _height = 600;
        private int _weight = 800;

        public Mapgenix(HtmlHelper helper)
        {
            this.htmlHelper = helper;
            //add scripts 
            scriptmanager = new SimpleScriptManager.SimpleScriptManager(helper);
            scriptmanager.ScriptInclude("Script2", "Scripts/Script2.js");
            scriptmanager.ScriptInclude("Script1", "Scripts/Script1.js");
        }

        public Map Map(int height, int width)
        {
            gsuitemap = new Map();
            //gsuitemap.red = height;
            //gsuitemap.Width = width;
            return gsuitemap;
        }


        public Map Map(int height, int width, System.Drawing.Color background)
        {
             gsuitemap = new Map();
            //gsuitemap.HeightInPixels = height;
            //gsuitemap.Width = width;
            return gsuitemap;
        }


        public Map Map()
        {
            try
            {
                gsuitemap = new Map();
                //gsuitemap.BackColor = _backgroundColor;
                //gsuitemap.Height = _height;
                //gsuitemap.Width = _weight;
                gsuitemap.MapUnit = GeographyUnit.DecimalDegree;
                gsuitemap.MapBackground.BackgroundBrush = new GeoSolidBrush(GeoColor.StandardColors.White);

                string districtPath = "C:\\SampleData\\Panama\\panama_districts.shp";
                string roadPath = "C:\\SampleData\\Panama\\Panama_Roads.shp";
                string locationPath = "C:\\SampleData\\Panama\\panama_locations.shp";

                ShapeFileFeatureLayer districtLayer = FeatureLayerFactory.CreateShapeFileFeatureLayer(districtPath);
                districtLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.GetRandomGeoColor(RandomColorType.Pastel), GeoColor.StandardColors.Black);
                districtLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                //create layer for road
                ShapeFileFeatureLayer roadLayer = FeatureLayerFactory.CreateShapeFileFeatureLayer(roadPath);
                roadLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.GetRandomGeoColor(RandomColorType.Pastel), 2, true);
                roadLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                //create layer for location
                ShapeFileFeatureLayer locationLayer = FeatureLayerFactory.CreateShapeFileFeatureLayer(locationPath);
                locationLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.GetRandomGeoColor(RandomColorType.Pastel), 8, GeoColor.StandardColors.Black);
                locationLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                //add layers
                LayerOverlay staticOverlay = new LayerOverlay();
                staticOverlay.Layers.Add("District", districtLayer);
                staticOverlay.Layers.Add("Road", roadLayer);
                staticOverlay.Layers.Add("Location", locationLayer);

                gsuitemap.CustomOverlays.Add(staticOverlay);



                gsuitemap.MapTools.ScaleLine.Enabled = true;
                gsuitemap.MapTools.MouseCoordinate.Enabled = true;

                //open the district layer
                districtLayer.Open();
                gsuitemap.CurrentExtent = districtLayer.GetBoundingBox();
                districtLayer.Close();

                return gsuitemap;

            }
            catch (System.Exception)
            {

                throw;
            }

            
        }

        public void Render()
        {
            scriptmanager.Render();            
        }
    }
}

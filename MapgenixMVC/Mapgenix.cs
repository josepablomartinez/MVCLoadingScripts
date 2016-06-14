
using Mapgenix.Canvas;
using Mapgenix.Layers;
using Mapgenix.Shapes;
using Mapgenix.Styles;
using System;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;



namespace Mapgenix.GSuite.Mvc
{
    
    public class MapgenixTest
    {
        private HtmlHelper htmlHelper;
        //SimpleScriptManager scriptmanager;
        Map gsuitemap;

        private System.Drawing.Color _backgroundColor = System.Drawing.Color.Blue;
        private int _height = 600;
        private int _weight = 800;

        public MapgenixTest(HtmlHelper helper)
        {
            //this.htmlHelper = helper;
            //add scripts 
            helper.SimpleScriptManager().ScriptInclude<Mapgenix.GSuite.Mvc.MapgenixTest>("Script1", "Mapgenix.GSuite.Mvc.Scripts.1.js");
            helper.SimpleScriptManager().ScriptInclude<Mapgenix.GSuite.Mvc.MapgenixTest>("Script2", "Mapgenix.GSuite.Mvc.Scripts.2.js");
            //scriptmanager.ScriptInclude<Mapgenix>("Script1", " Mapgenix.GSuite.Mvc.Scripts.2.js");
            helper.SimpleScriptManager().Render();
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
       


        public void Map()
        {
            try
            {
                //this.htmlHelper = helper;
                gsuitemap = new Map(Guid.NewGuid().ToString(), 600,600);
                //gsuitemap.BackColor = _backgroundColor;
                //gsuitemap.Height = _height;
                //gsuitemap.Width = _weight;
                gsuitemap.MapUnit = GeographyUnit.DecimalDegree;
                gsuitemap.MapBackground.BackgroundBrush = new GeoSolidBrush(GeoColor.StandardColors.White);

                string districtPath = "C:\\SampleData\\Vector\\shapefiles\\Panama\\panama_districts.shp";
                string roadPath = "C:\\SampleData\\Vector\\shapefiles\\Panama\\Panama_Roads.shp";
                string locationPath = "C:\\SampleData\\Vector\\shapefiles\\Panama\\panama_locations.shp";

                ShapeFileFeatureLayer districtLayer = FeatureLayerFactory.CreateShapeFileFeatureLayer(districtPath);
                //AreaStyle aStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.Black);
                AreaStyles.CreateSimpleAreaStyle(GeoColor.GetRandomGeoColor(RandomColorType.Pastel), GeoColor.StandardColors.Black);
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

                //return gsuitemap;

                var writer = this.htmlHelper.ViewContext.HttpContext.Response.Output;

                using (HtmlTextWriter HTMLwriter = new HtmlTextWriter(writer))
                {
                    gsuitemap.RenderControl(HTMLwriter);
                }

            }
            catch (System.Exception)
            {

                throw;
            }

            
        }

        public void Render()
        {
            //scriptmanager.Render();

            //var writer = this.htmlHelper.ViewContext.HttpContext.Response.Output;

            //using (HtmlTextWriter HTMLwriter = new HtmlTextWriter(writer))
            //{
            //    gsuitemap.RenderControl(HTMLwriter);
            //}

        }
    }
}

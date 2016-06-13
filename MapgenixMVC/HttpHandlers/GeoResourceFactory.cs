using System;
using System.Web;

namespace Mapgenix.GSuite.Mvc
{
    public class GeoResourceFactory : IHttpHandlerFactory
    {
        private const string OpenlayersLocalFileName = "Mapgenix.GSuite.Mvc.JavaScripts.OpenLayers.js";
        private const string OpenlayersLocalExtensionFileName = "Mapgenix.GSuite.Mvc.JavaScripts.OpenLayersExtension.js";
        private const string ContextMenuLocalFileName = "Mapgenix.GSuite.Mvc.JavaScripts.ContextMenu.js";
        private const string GeoFunctionLocalFileName = "Mapgenix.GSuite.Mvc.JavaScripts.GeoFunction.js";
        private const string GeoHelperLocalFileName = "Mapgenix.GSuite.Mvc.JavaScripts.MapHelper.js";
        internal const string PrintTemplateFileName = "Mapgenix.GSuite.Mvc.JavaScripts.PrintTemplate.htm";
        internal const string GeoParserLocalFileName = "Mapgenix.GSuite.Mvc.JavaScripts.MapParser.js";
        internal const string TestJavascriptFiles1 = "Mapgenix.GSuite.Mvc.1.js";
        internal const string TestJavascriptFiles2 = "Mapgenix.GSuite.Mvc.2.js";

        private const string Openlayers = "opl";
        private const string Tile = "tile";
        private const string Singletile = "singletile";
        private const string Menu = "cm";
        private const string Function = "func";
        private const string Parser = "parser";
        private const string Extension = "extension";
        private const string Print = "print";
        private const string Icon = "icon";
        private const string Background = "bg";
        private const string Session = "session";
        private const string Logo = "logo";
        private const string Helper = "helper";
        private const string Markers = "markers";
        private const string Split = "split";
        private const string Popup = "popup";

        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            return GetHandlerCore(context);
            //return new InstanceHttpHandler();
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
            /* */
        }

        protected virtual IHttpHandler GetHandlerCore(HttpContext context)
        {
            IHttpHandler returnHandler = null;
            string requestName = GetRequestNameFromRequestPath(context.Request.Path);

            //switch (requestName)
            //{
            //case Openlayers:
            //    return new ScriptResource(OpenlayersLocalFileName);

            //case Tile:
            //    return new MultiThreadTileResource();

            //case Singletile:
            //    return new SingleThreadTileResource();

            //case Menu:
            //    return new ScriptResource(ContextMenuLocalFileName);

            //case Function:
            //    return new ScriptResource(GeoFunctionLocalFileName);

            //case Parser:
            //    return new ScriptResource(GeoParserLocalFileName);

            //case Helper:
            //    return new ScriptResource(GeoHelperLocalFileName);

            //case Print:
            //    return new ScriptResource(PrintTemplateFileName);

            //case Extension:
            //    return new ScriptResource(OpenlayersLocalExtensionFileName);

            //case Icon:
            //    return new ImageResource();

            //case Background:
            //    return new BackgroundAndLogoResource(Background);

            //case Session:
            //    return new SessionRefreshHandler();

            //case Logo:
            //    return new BackgroundAndLogoResource(Logo);

            //case Markers:
            //    return new MarkerResource();

            //case Popup:
            //    return new MarkerPopupResource();

            //    default: break;
            //}

            return null;
        }

        private static string GetRequestNameFromRequestPath(string requestPath)
        {
            if (requestPath.IndexOf("/", StringComparison.Ordinal) != -1)
            {
                requestPath = requestPath.Substring(requestPath.LastIndexOf("/", StringComparison.Ordinal) + 1);
            }

            return requestPath.Substring(0, requestPath.LastIndexOf("_", StringComparison.Ordinal));
        }
    }
}

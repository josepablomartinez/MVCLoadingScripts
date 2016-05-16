using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mapgenix
{
    public static class MapgenixExtension
    {
        private static readonly string mapGenixKey = "Mapgenix";

        public static Mapgenix Mapgenix(this HtmlHelper helper)
        {
            // Get SimpleScriptManager from HttpContext.Items
            // This allows for a single SimpleScriptManager to be created and used per HTTP request.
            var mapgenixManager = helper.ViewContext.HttpContext.Items[mapGenixKey] as Mapgenix;
            if (mapgenixManager == null)
            {
                // If SimpleScriptManager hasn't been initialized yet, then initialize it.
                mapgenixManager = new Mapgenix(helper);
                // Store it in HttpContext.Items for subsequent requests during this HTTP request.
                helper.ViewContext.HttpContext.Items[mapGenixKey] = mapgenixManager;
            }
            // Return the SimpleScriptManager
            return mapgenixManager;
        }
    }
}
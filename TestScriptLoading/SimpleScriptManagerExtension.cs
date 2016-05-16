using System.Web.Mvc;

namespace SimpleScriptManager
{
    public static class SimpleScriptManagerExtensions
    {
        private static readonly string simpleScriptManagerKey = "SimpleScriptManager";

        public static SimpleScriptManager SimpleScriptManager(this HtmlHelper helper)
        {
            // Get SimpleScriptManager from HttpContext.Items
            // This allows for a single SimpleScriptManager to be created and used per HTTP request.
            var scriptmanager = helper.ViewContext.HttpContext.Items[simpleScriptManagerKey] as SimpleScriptManager;
            if (scriptmanager == null)
            {
                // If SimpleScriptManager hasn't been initialized yet, then initialize it.
                scriptmanager = new SimpleScriptManager(helper);
                // Store it in HttpContext.Items for subsequent requests during this HTTP request.
                helper.ViewContext.HttpContext.Items[simpleScriptManagerKey] = scriptmanager;
            }
            // Return the SimpleScriptManager
            return scriptmanager;
        }
    }
}
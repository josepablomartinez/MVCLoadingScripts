using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;

namespace Mapgenix.GSuite.Mvc
{
    internal class ScriptResource : IHttpHandler
    {
        private string _resourceName;

        public ScriptResource()
            : this(String.Empty)
        {
        }

        public ScriptResource(string resourceName)
        {
            this._resourceName = resourceName;
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.Cache.SetExpires(DateTime.Now.AddDays(1.0));
            context.Response.Cache.SetCacheability(HttpCacheability.Public);

            if (_resourceName.Equals(GeoResourceFactory.PrintTemplateFileName))
            {
                context.Response.Write(GetPrintTemplateScript(context));
            }
            else if (_resourceName.Equals(GeoResourceFactory.GeoParserLocalFileName))
            {
                string codeBase = GetType().Assembly.CodeBase;
                string filePath = context.Server.UrlDecode(new Uri(Path.GetDirectoryName(codeBase) + "\\MapParser.js").AbsolutePath);
                if (File.Exists(filePath))
                {
                    context.Response.Write(MapResourceHelper.GetFileScript(filePath));
                }
                else
                {
                    context.Response.Write(MapResourceHelper.GetResourceScript(_resourceName, GetType()));
                }
            }
            else if (!String.IsNullOrEmpty(_resourceName))
            {
                context.Response.Write(MapResourceHelper.GetResourceScript(_resourceName, GetType()));
            }

            context.ApplicationInstance.CompleteRequest();
        }

        private string GetPrintTemplateScript(HttpContext context)
        {
            string html = MapResourceHelper.GetResourceScript(GeoResourceFactory.PrintTemplateFileName, GetType());
            string containerId = context.Request.QueryString["containerid"];
            html = html.Replace("#ID#", containerId);

            string[] controls = context.Request.QueryString["controls"].Split(',');
            StringBuilder checkBoxForControlsHtml = new StringBuilder();

            foreach (string control in controls)
            {
                if (control != "MouseDefaults")
                {
                    checkBoxForControlsHtml.AppendFormat(CultureInfo.InvariantCulture, "<input type='checkbox' value='{0}' onclick='setVisible(this)' /> {0} ", control);
                }
            }

            html = html.Replace("#CONTROLSET#", checkBoxForControlsHtml.ToString());

            return html;
        }
    }
}

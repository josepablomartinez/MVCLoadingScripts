using SimpleScriptManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MapgenixMVC
{
    public class CustomViewClass : ViewUserControl
    {
        public override void RenderControl(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderControl(writer);
            this.Html.SimpleScriptManager().ScriptInclude<CustomViewClass>(
                "Script1",
                "MapgenixMVC.Scripts.1.js");

            this.Html.SimpleScriptManager().Render();
        }
    }
}

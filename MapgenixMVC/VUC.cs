using Mapgenix.GSuite.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MapgenixMVC
{
    public class VUC : ViewUserControl
    {
        public string Message { get; set; }

        public override void RenderControl(System.Web.UI.HtmlTextWriter writer)
        {
            base.RenderControl(writer);

            // By specifying a Key when adding the ScriptInclude below, we are ensuring that the script only gets included
            // within the Page once, no matter how many instances of this control are renderd to the Page.
            this.Html.SimpleScriptManager().ScriptInclude<VUC>(
                 "Script1",
                 "MapgenixMVC.Script1.js");

            this.Html.SimpleScriptManager().Render();
        }
    }
}

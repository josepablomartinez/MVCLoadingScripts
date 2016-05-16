using Mapgenix.GSuite.Web;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            gsuitemap.BackColor = _backgroundColor;
            gsuitemap.Height = height;
            gsuitemap.Width = width;
            return gsuitemap;
        }


        public Map Map(int height, int width, System.Drawing.Color background)
        {
             gsuitemap = new Map();
            gsuitemap.BackColor = background;
            gsuitemap.Height = height;
            gsuitemap.Width = width;
            return gsuitemap;
        }


        public Map Map()
        {
             gsuitemap = new Map();
            gsuitemap.BackColor = _backgroundColor;
            gsuitemap.Height = _height;
            gsuitemap.Width = _weight;
            return gsuitemap;
        }

        public void Render()
        {
            scriptmanager.Render();            
        }
    }
}
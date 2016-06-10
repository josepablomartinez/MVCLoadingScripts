using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapgenix.GSuite.Mvc;

namespace Mapgenix.GSuite.Mvc
{
    public static class MapHtmlHelper
    {
        /// <summary>
        /// Generates a Mapgenix map
        /// </summary>
        public static Map Map(this MapHtmlHelper html,
            object htmlAttributes = null
            )
        {
            return new Map();
        }

        /// <summary>
        /// Generates a Mapgenix map
        /// </summary>
        public static Map Map(this HtmlHelper html,
            string id, int width, int height,
            object htmlAttributes = null
            )
        {
            return new Map(id, width, height, htmlAttributes);
        }

        public static Map Map(this HtmlHelper html,
            Map map
            )
        {
            return map;
        }
    }
}

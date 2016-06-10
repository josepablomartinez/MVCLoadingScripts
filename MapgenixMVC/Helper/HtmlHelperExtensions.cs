using Mapgenix.GSuite.Mvc;
using System;
using System.Collections.Generic;
using System.Web;

namespace Mapgenix.GSuite.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString MapControl(this MapHtmlHelper htmlHelper, object htmlAttributes = null)
        {
            Map map = new Map(htmlAttributes);
            return MapControl(htmlHelper, map);
        }

        public static IHtmlString MapControl(this HtmlHelper htmlHelper, string id, int width, int height, object htmlAttributes = null)
        {
            Map map = new Map(id, width, height, htmlAttributes);
            return MapControl(htmlHelper, map);
        }

        public static IHtmlString MapControl(this HtmlHelper htmlHelper, BaseControl baseControl)
        {
            if (baseControl == null)
            {
                throw new ArgumentNullException("control");
            }

            return new HtmlString(baseControl.Html(htmlHelper.ViewContext));
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

namespace Mapgenix.GSuite.Mvc
{
    public abstract class BaseControl
    {
        // These attributes are merged in GetTagBuilder() after HtmlAttributes.
        // This means that any custom settings are preserved.
        protected IDictionary<string, string> Attributes { get; private set; }

        public string Class
        {
            set { AddClass(value); }
        }

        public virtual string ID
        {
            get { return Attributes.GetValue("id"); }
            set { Attributes.Merge("id", value); }
        }

        protected string InnerHtml { get; set; }

        public object HtmlAttributes { get; set; }

        public string Style
        {
            set { Attributes.Merge("style", value); }
        }

        private string TagName { get; set; }

        private TagRenderMode TagRenderMode { get; set; }

        public string Title
        {
            set { Attributes.Merge("title", value); }
        }

        //TODO: Revisar
        protected ViewContext _viewContext = null;

        public BaseControl(string tagName)
            : this(tagName, TagRenderMode.Normal) { }

        public BaseControl(string tagName, TagRenderMode tagRenderMode)
        {
            Attributes = new SortedDictionary<string, string>(StringComparer.Ordinal);
            TagName = tagName;
            TagRenderMode = tagRenderMode;
        }

        public void AddClass(string className)
        {
            if (string.IsNullOrEmpty(className))
            {
                className = className.Trim();
            }

            string currentClassName;

            if (Attributes.TryGetValue("class", out currentClassName))
            {
                currentClassName = currentClassName.Trim();

                Attributes["class"] = currentClassName + " " + className;
            }
            else
            {
                Attributes["class"] = className;
            }
        }

        public void AddEventScript(string eventKey, string script)
        {
            string newScript = script;

            if (string.IsNullOrEmpty(newScript))
            {
                newScript = newScript.Trim();

                if (!newScript.EndsWith("}")
                    && !newScript.EndsWith(";"))
                {
                    newScript += ";";
                }
            }

            string currentScript;

            if (Attributes.TryGetValue(eventKey, out currentScript))
            {
                currentScript = currentScript.Trim();

                if (!currentScript.EndsWith("}")
                    && !currentScript.EndsWith(";"))
                {
                    currentScript += ";";
                }

                Attributes[eventKey] = currentScript + " " + newScript;
            }
            else
            {
                Attributes[eventKey] = newScript;
            }
        }

        private TagBuilder GetTagBuilder()
        {
            TagBuilder tagBuilder = new TagBuilder(TagName);
            tagBuilder.MergeAttributes(new RouteValueDictionary(HtmlAttributes));
            tagBuilder.MergeAttributes(Attributes);
            tagBuilder.InnerHtml = InnerHtml;

            return tagBuilder;
        }

        public string Html(ViewContext viewContext)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException("viewContext");
            }

            //TODO: Revisar
            _viewContext = viewContext;

            StringBuilder html = new StringBuilder();

            Initialise(viewContext);

            TagBuilder tagBuilder = GetTagBuilder();

            using (StringWriter writer = new StringWriter(html))
            {
                writer.Write(tagBuilder.ToString(TagRenderMode));
                using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter(writer)) 
                    RenderHtml(htmlTextWriter);
            }

            return html.ToString();
        }

        // Override if the control needs access to the ViewContext.
        protected virtual void Initialise(ViewContext viewContext) { }

        // Override if the control needs to render some custom HTML (e.g. MvcCheckBox).
        protected virtual void RenderHtml(HtmlTextWriter writer) { }

        protected void SetInnerText(object innerText)
        {
            if (innerText == null)
            {
                SetInnerText(null);
            }

            SetInnerText(innerText.ToString());
        }

        protected void SetInnerText(string innerText)
        {
            InnerHtml = System.Web.HttpUtility.HtmlEncode(innerText);
        }
    }
}

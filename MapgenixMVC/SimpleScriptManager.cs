﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Mapgenix.GSuite.Mvc
{
    public class SimpleScriptManager
    {
        private HtmlHelper htmlHelper;

        private Dictionary<string, string> scriptIncludes = new Dictionary<string, string>();

        private Dictionary<string, string> scripts = new Dictionary<string, string>();
        private Dictionary<string, Action> scriptsActions = new Dictionary<string, Action>();

        /// <summary>
        /// SimpleScriptManager Constructor
        /// </summary>
        /// <param name="helper">The HtmlHelper that this SimpleScriptManager will use to render to.</param>
        public SimpleScriptManager(HtmlHelper helper)
        {
            // Store reference to the HtmlHelper object this SimpleScriptManager is tied to.
            this.htmlHelper = helper;
        }

        /// <summary>
        /// Adds a script file reference to the page.
        /// </summary>
        /// <param name="scriptPath">The URL of the script file.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager ScriptInclude(string scriptPath)
        {
            return this.ScriptInclude(Guid.NewGuid().ToString(), scriptPath);
        }

        /// <summary>
        /// Adds a script file reference to the page.
        /// </summary>
        /// <param name="key">A unique identifier for the script file.</param>
        /// <param name="scriptPath">The URL of the script file.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager ScriptInclude(string key, string scriptPath)
        {
            if (!this.scriptIncludes.ContainsKey(key))
            {
                // Check if the scriptPath is a Virtual Path
                if (scriptPath.StartsWith("~/"))
                {
                    // Convert the Virtual Path to an Application Absolute Path
                    scriptPath = VirtualPathUtility.ToAbsolute(scriptPath);
                }
                this.scriptIncludes.Add(key, scriptPath);
            }
            return this;
        }

        /// <summary>
        /// Adds a script file reference to the page for an Embedded Web Resource.
        /// </summary>
        /// <typeparam name="T">The Type whos Assembly contains the Web Resource.</typeparam>
        /// <param name="key">A unique identifier for the script file.</param>
        /// <param name="resourceName">The name of the Web Resource.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager ScriptInclude<T>(string key, string resourceName)
        {
            return this.ScriptInclude(key, getWebResourceUrl<T>(resourceName));
        }

        /// <summary>
        /// Adds a script file reference to the page for an Embedded Web Resource.
        /// </summary>
        /// <typeparam name="T">The Type whos Assembly contains the Web Resource.</typeparam>
        /// <param name="resourceName">The name of the Web Resource.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager ScriptInclude<T>(string resourceName)
        {
            return this.ScriptInclude(getWebResourceUrl<T>(resourceName));
        }

        /// <summary>
        /// Adds a script block to the page.
        /// </summary>
        /// <param name="javascript">The JavaScript code to include in the Page.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager Script(string javascript)
        {
            return this.Script(Guid.NewGuid().ToString(), javascript);
        }

        /// <summary>
        /// Adds a script block to the page.
        /// </summary>
        /// <param name="key">A unique identifier for the script.</param>
        /// <param name="javascript">The JavaScript code to include in the Page.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager Script(string key, string javascript)
        {
            if (!this.scripts.ContainsKey(key) && !this.scriptsActions.ContainsKey(key))
            {
                this.scripts.Add(key, javascript);
            }
            return this;
        }

        /// <summary>
        /// Adds a script block to the page.
        /// </summary>
        /// <param name="javascript">The JavaScript code to include in the Page.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager Script(Action javascript)
        {
            return this.Script(Guid.NewGuid().ToString(), javascript);
        }

        /// <summary>
        /// Adds a script block to the page.
        /// </summary>
        /// <param name="key">A unique identifier for the script.</param>
        /// <param name="javascript">The JavaScript code to include in the Page.</param>
        /// <returns>Returns the SimpleScriptManager</returns>
        public SimpleScriptManager Script(string key, Action javascript)
        {
            if (!this.scripts.ContainsKey(key) && !this.scriptsActions.ContainsKey(key))
            {
                this.scriptsActions.Add(key, javascript);
            }
            return this;
        }

        /// <summary>
        /// Renders the SimpleScriptManager to the Page
        /// </summary>
        public void 
            Render()
        {
             var writer = this.htmlHelper.ViewContext.HttpContext.Response.Output;

            // Render All Script Includes to the Page
            foreach (var scriptInclude in this.scriptIncludes)
            {
                writer.WriteLine(String.Format("<script type='text/javascript' src='{0}'></script>", scriptInclude.Value));
            }

            // Render All other scripts to the Page
            if (this.scripts.Count > 0 || this.scriptsActions.Count > 0)
            {
                writer.WriteLine("<script type='text/javascript'>");

                if (this.scripts.Count > 0)
                {
                    foreach (var script in this.scripts)
                    {
                        writer.WriteLine(script.Value);
                    }
                }

                if (this.scriptsActions.Count > 0)
                {
                    foreach (var script in this.scriptsActions)
                    {
                        script.Value();
                    }
                }

                writer.WriteLine("</script>");
            }
        }


        private static MethodInfo _getWebResourceUrlMethod;
        private static MethodInfo[] _getWebResourceUrlMethods;
        private static object _getWebResourceUrlLock = new object();

        private static string getWebResourceUrl<T>(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException("resourceName");
            }


            if (_getWebResourceUrlMethod == null)
            {
                lock (_getWebResourceUrlLock)
                {
                    if (_getWebResourceUrlMethod == null)
                    {                        
                        _getWebResourceUrlMethods = typeof(System.Web.Handlers.AssemblyResourceLoader).GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
                        foreach (MethodInfo mi in _getWebResourceUrlMethods)
                        {
                            if (mi.Name.Equals("GetWebResourceUrlInternal") && mi.GetParameters().Length == 5) { 
                                _getWebResourceUrlMethod = mi;
                                break;
                            }
                        }
                    }
                }
            }
            //IScriptManager 
            return  (string)_getWebResourceUrlMethod.Invoke(null,
                new object[] { Assembly.GetAssembly(typeof(T)), resourceName, false, false, null});
        }

    }
}
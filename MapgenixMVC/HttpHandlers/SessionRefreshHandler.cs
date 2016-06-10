using System;
using System.Web;

namespace Mapgenix.GSuite.Mvc
{
    internal class SessionRefreshHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Write(DateTime.Now.Ticks);
        }
    }
}

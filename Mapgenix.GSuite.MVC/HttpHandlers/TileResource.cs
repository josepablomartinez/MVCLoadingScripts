using System;
using System.Web;
using System.Web.SessionState;

namespace Mapgenix.GSuite.Mvc
{
    internal delegate void ProcessRequestDelegate(HttpContext context);

    public class MultiThreadTileResource : IHttpAsyncHandler, IReadOnlySessionState
    {
        public MultiThreadTileResource()
        { }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            return BeginProcessRequestCore(context, cb, extraData);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            EndProcessRequestCore(result);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequestCore(context);
        }

        protected virtual void ProcessRequestCore(HttpContext context)
        {
        }

        protected virtual IAsyncResult BeginProcessRequestCore(HttpContext context, AsyncCallback cb, object extraData)
        {
            ProcessRequestDelegate processRequestMethod = new ProcessRequestDelegate(new SingleThreadTileResource().ProcessRequest);
            return processRequestMethod.BeginInvoke(context, cb, extraData);
        }

        protected virtual void EndProcessRequestCore(IAsyncResult result)
        {
        }
    }
}

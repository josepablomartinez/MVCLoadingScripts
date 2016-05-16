using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestScriptLoading
{
    public class ClientScriptManager
    {
        public string AddScriptResourse()
        {
            string returnResource = string.Empty;
            string resources1 = String.Format("<script src=\"{0}\" type=\"text/javascript\" ></script>", "script2.js");
            returnResource += resources1; 
            string resources = String.Format("<script src=\"{0}\" type=\"text/javascript\" ></script>", "script2.js");
            returnResource += resources;

            return returnResource;
        }
    }
}
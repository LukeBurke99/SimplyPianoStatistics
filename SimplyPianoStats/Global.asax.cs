using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace SimplyPianoStats
{
	public class WebApiApplication : HttpApplication
	{
		protected void Application_Start()
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			if (HttpContext.Current.Request.HttpMethod == "OPTIONS") {
				HttpContext.Current.Response.Flush();
			}
		}
	}
}

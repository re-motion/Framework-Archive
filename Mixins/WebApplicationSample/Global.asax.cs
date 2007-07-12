using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
using System.Web.SessionState;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Context;
using Rubicon.Mixins;
using System.Diagnostics;
using Rubicon.Mixins.Definitions;
using System.Reflection;

namespace WebApplicationSample
{
  public class Global : System.Web.HttpApplication
  {

    protected void Application_Start (object sender, EventArgs e)
    {
    }

    protected void Application_End (object sender, EventArgs e)
    {
    }
  }
}
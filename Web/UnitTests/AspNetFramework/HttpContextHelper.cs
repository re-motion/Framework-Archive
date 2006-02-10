using System;
using System.Web.Hosting;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using Rubicon.Utilities;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Web.UnitTests.AspNetFramework
{

/// <summary> 
///   Provides helper methods for initalizing an <see cref="HttpContext"/> object when simulating ASP.NET request
///   cycles. 
/// </summary>
public class HttpContextHelper
{
  public const string c_appVirtualDir = "/";
  public const string c_appPhysicalDir = @"C:\";
  public const string c_serverPath = "http://127.0.0.1";

  public static void SetCurrent (HttpContext context)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    HttpContext.Current = context;
  }

	public static HttpContext CreateHttpContext (string httpMethod, string page, string query)
	{
    ArgumentUtility.CheckNotNullOrEmpty ("httpMethod", httpMethod);
    ArgumentUtility.CheckNotNullOrEmpty ("page", page);


    SimpleWorkerRequest workerRequest = 
        new SimpleWorkerRequest (c_appVirtualDir, c_appPhysicalDir, page, query, new System.IO.StringWriter());

    object httpRuntime = PrivateInvoke.GetNonPublicStaticField (typeof (HttpRuntime), "_theRuntime");
    PrivateInvoke.SetNonPublicField (httpRuntime, "_appDomainAppPath", c_appPhysicalDir);
#if NET11
    PrivateInvoke.SetNonPublicField (httpRuntime, "_appDomainAppVPath", c_appVirtualDir);
#else
    string assemblyName = typeof (HttpApplication).Assembly.FullName;
    Type virtualPathType = Type.GetType ("System.Web.VirtualPath, " + assemblyName, true);
    object virtualPath = PrivateInvoke.InvokePublicStaticMethod (virtualPathType, "Create", c_appVirtualDir);
    PrivateInvoke.SetNonPublicField (httpRuntime, "_appDomainAppVPath", virtualPath);
    PrivateInvoke.SetNonPublicField (httpRuntime, "_appDomainAppId", "Rubicon.Web.UnitTests");
#endif
    HttpContext context = new HttpContext (workerRequest);
    PrivateInvoke.SetNonPublicField (context.Request, "_httpMethod", httpMethod);

    HttpSessionState sessionState = CreateSession();
    SetSession (context, sessionState);
    return context;
	}

  public static void SetQueryString (HttpContext context, NameValueCollection queryString)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    ArgumentUtility.CheckNotNull ("queryString", queryString);

    PrivateInvoke.InvokeNonPublicMethod (context.Request.QueryString, "MakeReadWrite", new object[0]);
    context.Request.QueryString.Clear();
    foreach (string key in queryString)
      context.Request.QueryString.Set (key, queryString[key]);
    PrivateInvoke.InvokeNonPublicMethod (context.Request.QueryString, "MakeReadOnly", new object[0]);

    PrivateInvoke.SetNonPublicField (context.Request, "_params", null);
  }

  public static void SetForm (HttpContext context, NameValueCollection form)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    ArgumentUtility.CheckNotNull ("form", form);

    PrivateInvoke.InvokeNonPublicMethod (context.Request.Form, "MakeReadWrite", new object[0]);
    context.Request.Form.Clear();
    foreach (string key in form)
      context.Request.Form.Set (key, form[key]);
    PrivateInvoke.InvokeNonPublicMethod (context.Request.Form, "MakeReadOnly", new object[0]);

    PrivateInvoke.SetNonPublicField (context.Request, "_params", null);
  }
 
  protected static HttpSessionState CreateSession ()
  {
    HttpSessionState sessionState;
    string id = Guid.NewGuid().ToString();
    HttpStaticObjectsCollection staticObjects = new HttpStaticObjectsCollection();
    int timeout = 20;
    bool newSession = true;
    SessionStateMode mode = SessionStateMode.InProc;
    bool isReadOnly = false;
#if NET11
    string assemblyName = typeof (HttpApplication).Assembly.FullName;
    Type sessionDictionaryType =  Type.GetType ("System.Web.SessionState.SessionDictionary, " + assemblyName, true);
    
    object sessionDictionary = PrivateInvoke.CreateInstanceNonPublicCtor (sessionDictionaryType, new object[0]);

    bool cookieless = true;
    sessionState = (HttpSessionState) PrivateInvoke.CreateInstanceNonPublicCtor (
        typeof (HttpSessionState), 
        new object[] {id, sessionDictionary, staticObjects, timeout, newSession, cookieless, mode, isReadOnly});
#else
    SessionStateItemCollection sessionItems = new SessionStateItemCollection ();
    HttpSessionStateContainer httpSessionStateContainer = new HttpSessionStateContainer (
      id, sessionItems, staticObjects, timeout, newSession, HttpCookieMode.UseUri, mode, isReadOnly);

    sessionState = (HttpSessionState) PrivateInvoke.CreateInstanceNonPublicCtor (
        typeof (HttpSessionState), httpSessionStateContainer);
#endif
    return sessionState;
  }

  protected static void SetSession (HttpContext context, HttpSessionState sessionState)
  {
    context.Items["AspSession"] = sessionState;
  }

  private HttpContextHelper()
  {
  }
}

}

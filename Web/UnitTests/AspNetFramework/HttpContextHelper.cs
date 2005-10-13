using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Reflection;
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
  public static void SetCurrent (HttpContext context)
  {
    ArgumentUtility.CheckNotNull ("context", context);
    System.Runtime.Remoting.Messaging.CallContext.SetData("HtCt", context);
  }

	public static HttpContext CreateHttpContext (string requestFilename, string requestUrl, string requestQueryString)
	{
    ArgumentUtility.CheckNotNullOrEmpty ("requestFilename", requestFilename);
    ArgumentUtility.CheckNotNullOrEmpty ("requestUrl", requestUrl);

    HttpRequest request = new HttpRequest (requestFilename, requestUrl, StringUtility.NullToEmpty (requestQueryString));
    HttpResponse response = new HttpResponse (new StringWriter());
    return Init (request, response);     
	}

	public static HttpContext CreateHttpContext (HttpRequest request, HttpResponse response)
  {
    ArgumentUtility.CheckNotNull ("request", request);
    ArgumentUtility.CheckNotNull ("response", response);
    return Init (request, response);
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

  protected static HttpContext Init (HttpRequest request, HttpResponse response)
  {
    ArgumentUtility.CheckNotNull ("request", request);
    ArgumentUtility.CheckNotNull ("response", response);

    HttpContext context = new HttpContext (request, response);
    HttpSessionState sessionState = CreateSession();
    SetSession (context, sessionState);
    
    return context;
  }
 
  protected static HttpSessionState CreateSession ()
  {
    string assemblyName = typeof (HttpApplication).Assembly.FullName;
    Type sessionDictionaryType =  Type.GetType ("System.Web.SessionState.SessionDictionary, " + assemblyName, true);
    
    string id = Guid.NewGuid().ToString();
    object sessionDictionary = PrivateInvoke.CreateInstanceNonPublicCtor (sessionDictionaryType, new object[0]);
    HttpStaticObjectsCollection staticObjects = new HttpStaticObjectsCollection();
    int timeout = 20;
    bool newSession = true;
    bool cookieless = true;
    SessionStateMode mode = SessionStateMode.InProc;
    bool isReadOnly = false;

    HttpSessionState sessionState = (HttpSessionState) PrivateInvoke.CreateInstanceNonPublicCtor (
        typeof (HttpSessionState), 
        new object[] {id, sessionDictionary, staticObjects, timeout, newSession, cookieless, mode, isReadOnly});

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

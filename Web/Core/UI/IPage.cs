using System;
using System.Web;
using System.Web.UI;
using System.Web.Caching;
using System.Web.SessionState;
using System.Security.Principal;

namespace Rubicon.Web.UI.Controls
{

public interface ITemplateControl: IControl
{
  event EventHandler AbortTransaction;
  event EventHandler CommitTransaction;
  event EventHandler Error;

  Control LoadControl(string virtualPath);
  ITemplate LoadTemplate(string virtualPath);
  Control ParseControl(string content);
}

/// <summary>
/// </summary>
public interface IPage: ITemplateControl, IHttpHandler
{
  void DesignerInitialize();
  string GetPostBackClientEvent(Control control, string argument);
  string GetPostBackClientHyperlink(Control control, string argument);
  string GetPostBackEventReference(Control control);
  string GetPostBackEventReference(Control control, string argument);
  int GetTypeHashCode();
  bool IsClientScriptBlockRegistered(string key);
  bool IsStartupScriptRegistered(string key);
  string MapPath(string virtualPath);
  void RegisterArrayDeclaration(string arrayName, string arrayValue);
  void RegisterClientScriptBlock(string key, string script);
  void RegisterHiddenField(string hiddenFieldName, string hiddenFieldInitialValue);
  void RegisterOnSubmitStatement(string key, string script);
  void RegisterRequiresPostBack(Control control);
  void RegisterRequiresRaiseEvent(IPostBackEventHandler control);
  void RegisterStartupScript(string key, string script);
  void RegisterViewStateHandler();
  void Validate();
  void VerifyRenderingInServerForm(Control control);

  HttpApplicationState Application { get; }
  Cache Cache { get; }
  string ClientTarget { get; set; }
  string ErrorPage { get; set; }
  bool IsPostBack { get; }
  bool IsValid { get; }
  HttpRequest Request { get; }
  HttpResponse Response { get; }
  HttpServerUtility Server { get; }
  HttpSessionState Session { get; }
  bool SmartNavigation { get; set; }
  TraceContext Trace { get; }
  IPrincipal User { get; }
  ValidatorCollection Validators { get; }
  string ViewStateUserKey { get; set; }
}

}


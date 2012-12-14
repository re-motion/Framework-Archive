using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.ExecutionEngine
{
  public class WxePageInfo: WxeTemplateControlInfo, IDisposable
  {
    private IWxePage _page;
    private WxeForm _form;
    private bool _postbackCollectionInitialized = false;
    private NameValueCollection _postbackCollection = null;
    private IHttpHandler _originalWxeHandler;
    private bool _executeNextStep = false;
    private HttpResponse _response; // used for cleanup in Dispose

    public WxePageInfo (IWxePage page)
    {
      _page = page;
    }

    public void Initialize (HttpContext context)
    {
      base.OnInit(_page, context);

      if (! ControlHelper.IsDesignMode (_page, context))
      {
        //  if (_page.HtmlForm == null)
        //    throw new HttpException (_page.GetType().FullName + " does not initialize field 'Form'.");
        _form = WxeForm.Replace (_page.HtmlForm);
        _page.HtmlForm = _form;
        _originalWxeHandler = context.Handler;
        context.Handler = _page;
      }

      HtmlInputHidden wxePageTokenField = new HtmlInputHidden();
      wxePageTokenField.ID = "wxePageToken";
      if (_page.CurrentStep != null)
        wxePageTokenField.Value = _page.CurrentStep.PageToken;
      _form.Controls.Add (wxePageTokenField);

      _page.Load += new EventHandler (Page_Load);
    }

    private void Page_Load (object sender, EventArgs e)
    {
      PageUtility.RegisterClientScriptBlock ((Page)_page, "wxeDoSubmit",
                                             "function wxeDoSubmit (button, pageToken) { \n"
                                             + "  var theForm = document." + _form.ClientID + "; \n"
                                             + "  theForm.returningToken.value = pageToken; \n"
                                             + "  document.getElementById(button).click(); \n"
                                             + "}");

      NameValueCollection postBackCollection = _page.GetPostBackCollection();
      if (postBackCollection != null)
      {
        string returningToken = _form.ReturningToken;
        // string returningToken = postBackCollection["returningToken"];
        if (! StringUtility.IsNullOrEmpty (returningToken))
        {
          WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
          WxeFunctionState functionState = functionStates.GetItem (returningToken);
          if (functionState != null)
          {
            WxeContext.Current.ReturningFunction = functionState.Function;
            WxeContext.Current.IsReturningPostBack = true;
          }
        }
      }

      _form.ReturningToken = string.Empty;    
    }

    public NameValueCollection DeterminePostBackMode (HttpContext context)
    {
      if (! _postbackCollectionInitialized)
      {
        if (WxeContext.Current == null)
        {
          _postbackCollection = null;
        }
        else if (! WxeContext.Current.IsPostBack)
        {
          _postbackCollection = null;
        }
        else if (WxeContext.Current.PostBackCollection != null)
        {
          _postbackCollection = WxeContext.Current.PostBackCollection;
        }
        else if (context.Request == null)
        {
          _postbackCollection = null;
        }
        else
        {
          NameValueCollection collection;
          if (0 == string.Compare (context.Request.HttpMethod, "POST", false, CultureInfo.InvariantCulture))
            collection = context.Request.Form;
          else
            collection = context.Request.QueryString;

          if ((collection["__VIEWSTATE"] == null) && (collection["__EVENTTARGET"] == null))
            _postbackCollection = null;
          else
            _postbackCollection = collection;
        }

        _postbackCollectionInitialized = true;
      }
      return _postbackCollection;
    }  

    public void ExecuteNextStep ()
    {
      _executeNextStep = true;
      _response = _page.Response;
      _page.Visible = false; // suppress prerender and render events
    }

    public void ExecuteFunction (WxeFunction function, string target, Control sender, bool returningPostback)
    {
      WxeFunctionState functionState = new WxeFunctionState (function, 20);
      WxeFunctionStateCollection functionStates = WxeFunctionStateCollection.Instance;
      functionStates.Add (functionState);

      string href = _page.Request.Path + "?WxeFunctionToken=" + functionState.FunctionToken;
      string openScript = string.Format (@"window.open(""{0}"", ""{1}"");", href, target);
      PageUtility.RegisterStartupScriptBlock ((Page)_page, "WxeExecuteFunction", openScript);

      string returnScript;
      if (! returningPostback)
      {
        returnScript = "window.close();";
      }
      else if (UsesEventTarget)
      {
        string eventtarget = _page.GetPostBackCollection()["__EVENTTARGET"];
        string eventargument = _page.GetPostBackCollection()["__EVENTARGUMENT"];
        returnScript = string.Format (
            "if (window.opener && window.opener.__doPostBack && window.opener.document.getElementById(\"wxePageToken\") && window.opener.document.getElementById(\"wxePageToken\").value == \"{0}\") \n"
            + "  window.opener.__doPostBack(\"{1}\", \"{2}\"); \n"
            + "window.close();", 
            _page.CurrentStep.PageToken,
            eventtarget, 
            eventargument);
      }
      else
      {
        returnScript = string.Format (
            "if (window.opener && window.opener.wxeDoSubmit && window.opener.document.getElementById(\"wxePageToken\") && window.opener.document.getElementById(\"wxePageToken\").value == \"{0}\") \n"
            + "  window.opener.wxeDoSubmit(\"{1}\", \"{2}\"); \n"
            + "window.close();", 
            _page.CurrentStep.PageToken,
            sender.ClientID, 
            functionState.FunctionToken);
      }
      function.ReturnUrl = "javascript:" + returnScript;
    }

    public bool UsesEventTarget
    {
      get { return ! StringUtility.IsNullOrEmpty (_page.GetPostBackCollection()["__EVENTTARGET"]); }
    }

    public void Dispose ()
    {
      RestoreOriginalWxeHandler();
      if (_executeNextStep)
      {
        _response.Clear(); // throw away page trace output
        throw new WxeExecuteNextStepException();
      }
    }

    public void RestoreOriginalWxeHandler ()
    {
      HttpContext.Current.Handler = _originalWxeHandler;
    }

    public WxeForm Form
    {
      get { return _form; }
    }

    private FieldInfo _htmlFormField = null;
    private bool _htmlFormFieldInitialized = false;

    private void EnsureHtmlFormFieldInitialized()
    {
      if (! _htmlFormFieldInitialized)
      {
        MemberInfo[] fields = _page.GetType().FindMembers (
            MemberTypes.Field, 
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
            new MemberFilter (FindHtmlFormControlFilter), null);
        if (fields.Length < 1)
          throw new ApplicationException ("Page class " + _page.GetType().FullName + " has no field of type HtmlForm. Please add a field or override property IWxePage.HtmlForm.");
        else if (fields.Length > 1)
          throw new ApplicationException ("Page class " + _page.GetType().FullName + " has more than one field of type HtmlForm. Please remove excessive fields or override property IWxePage.HtmlForm.");
        _htmlFormField = (FieldInfo) fields[0];
        _htmlFormFieldInitialized = true;
      }
    }

    private bool FindHtmlFormControlFilter (MemberInfo member, object filterCriteria)
    {
      return (member is FieldInfo && ((FieldInfo)member).FieldType == typeof (HtmlForm));
    }

    public HtmlForm HtmlFormDefaultImplementation
    {
      get
      {
        EnsureHtmlFormFieldInitialized();
        return (HtmlForm) _htmlFormField.GetValue (_page);
      }
      set 
      {
        EnsureHtmlFormFieldInitialized();
        _htmlFormField.SetValue (_page, value);
      }
    }
  }
}
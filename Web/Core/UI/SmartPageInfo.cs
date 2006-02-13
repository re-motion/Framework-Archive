using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Rubicon.Collections;
using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Globalization;
using Rubicon.Web.Utilities;

namespace Rubicon.Web.UI
{

public class SmartPageInfo
{
  /// <summary> A list of resources. </summary>
  /// <remarks> 
  ///   Resources will be accessed using 
  ///   <see cref="M:Rubicon.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
  ///   See the documentation of <b>GetString</b> for further details.
  /// </remarks>
  [ResourceIdentifiers]
  [MultiLingualResources ("Rubicon.Web.Globalization.SmartPageInfo")]
  protected enum ResourceIdentifier
  {
    /// <summary> Displayed when the user attempts to leave the page. </summary>
    AbortMessage,
    /// <summary> Displayed when the user attempts to submit while the page is already submitting. </summary>
    StatusIsSubmittingMessage
  }

  public static readonly string CacheDetectionID = "SmartPage_CacheDetectionField";
  private const string c_smartScrollingID = "smartScrolling";
  private const string c_smartFocusID = "smartFocus";
  private const string c_scriptFileUrl = "SmartPage.js";
  private const string c_styleFileUrl = "SmartPage.css";
  private const string c_smartNavigationScriptFileUrl = "SmartNavigation.js";

  private static readonly string s_scriptFileKey = typeof (SmartPageInfo).FullName + "_Script";
  private static readonly string s_styleFileKey = typeof (SmartPageInfo).FullName + "_Style";
  private static readonly string s_smartNavigationScriptKey = typeof (SmartPageInfo).FullName+ "_SmartNavigation";

  private ISmartPage _page;

  private bool _isSmartNavigationDataDisacarded = false;
  private string _smartFocusID = null;
  private string _abortMessage;
  private string _statusIsSubmittingMessage = string.Empty;

  private bool _isPreRendering = false;
  private AutoInitHashtable _clientSideEventHandlers = new AutoInitHashtable (typeof (NameValueCollection));
  private string _checkFormStateFunction;
  private Hashtable _trackedControls = new Hashtable();
  private StringCollection _trackedControlsByID = new StringCollection();
  private Hashtable _navigationControls = new Hashtable();

  private ResourceManagerSet _cachedResourceManager;

  private FieldInfo _htmlFormField = null;
  private bool _htmlFormFieldInitialized = false;

	public SmartPageInfo (ISmartPage page)
	{
    ArgumentUtility.CheckNotNullAndType ("page", page, typeof (Page));
    _page = page;
    _page.Init += new EventHandler (Page_Init);
    _page.PreRender +=new EventHandler(Page_PreRender);
	}

  /// <summary> Implements <see cref="ISmartPage.RegisterClientSidePageEventHandler">ISmartPage.RegisterClientSidePageEventHandler</see>. </summary>
  public void RegisterClientSidePageEventHandler (SmartPageEvents pageEvent, string key, string function)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("key", key);
    ArgumentUtility.CheckNotNullOrEmpty ("function", function);
    if (! System.Text.RegularExpressions.Regex.IsMatch (function, @"^([a-zA-Z_][a-zA-Z0-9_]*)$"))
      throw new ArgumentException ("Invalid function name: '" + function + "'.", "function");

    if (_isPreRendering)
      throw new InvalidOperationException ("RegisterClientSidePageEventHandler must not be called after the PreRender method of the System.Web.UI.Page has been invoked.");

    NameValueCollection eventHandlers = (NameValueCollection) _clientSideEventHandlers[pageEvent];
    eventHandlers[key] = function;
  }


  /// <summary> Implements <see cref="ISmartPage.RegisterControlForDirtyStateTracking">ISmartPage.RegisterClientSidePageEventHandler</see>. </summary>
  public void RegisterControlForDirtyStateTracking (IModifiableControl control)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    _trackedControls[control] = control;
  }

  /// <summary> Implements <see cref="ISmartPage.RegisterControlForClientSideDirtyStateTracking">ISmartPage.RegisterControlForClientSideDirtyStateTracking</see>. </summary>
  public void RegisterControlForDirtyStateTracking (string clientID)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("clientID", clientID);
    if (! _trackedControlsByID.Contains (clientID))
      _trackedControlsByID.Add (clientID);
  }

  /// <summary> Implements <see cref="ISmartPage.EvaluateDirtyState">ISmartPage.EvaluateDirtyState</see>. </summary>
  public bool EvaluateDirtyState()
  {
    foreach (IModifiableControl control in _trackedControls.Values)
    {
      if (control.IsDirty)
        return true;
    }
    return false;
  }


  public string CheckFormStateFunction
  {
    get { return _checkFormStateFunction; }
    set { _checkFormStateFunction = StringUtility.EmptyToNull (value); }
  }


  /// <summary> Find the <see cref="IResourceManager"/> for this SmartPageInfo. </summary>
  protected virtual IResourceManager GetResourceManager()
  {
    return GetResourceManager (typeof (ResourceIdentifier));
  }

  /// <summary> Find the <see cref="IResourceManager"/> for this control info. </summary>
  /// <param name="localResourcesType"> 
  ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it.
  ///   Typically an <b>enum</b> or the derived class itself.
  /// </param>
  protected IResourceManager GetResourceManager (Type localResourcesType)
  {
    Rubicon.Utilities.ArgumentUtility.CheckNotNull ("localResourcesType", localResourcesType);

    //  Provider has already been identified.
    if (_cachedResourceManager != null)
        return _cachedResourceManager;

    //  Get the resource managers

    IResourceManager localResourceManager = 
        MultiLingualResourcesAttribute.GetResourceManager (localResourcesType, true);
    IResourceManager pageResourceManager = ResourceManagerUtility.GetResourceManager ((Page) _page, true);

    if (pageResourceManager == null)
      _cachedResourceManager = new ResourceManagerSet (localResourceManager);
    else
      _cachedResourceManager = new ResourceManagerSet (localResourceManager, pageResourceManager);

    return _cachedResourceManager;
  }


  private void EnsureHtmlFormFieldInitialized()
  {
    if (! _htmlFormFieldInitialized)
    {
      bool isDesignMode = ControlHelper.IsDesignMode (_page);

      Control page = (Page) _page;
#if ! NET11      
      if (((Page) page).Master != null)
        page = ((Page) page).Master;
#endif

      MemberInfo[] fields = page.GetType().FindMembers (
            MemberTypes.Field, 
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
            new MemberFilter (FindHtmlFormControlFilter), null);
      if (fields.Length < 1 && ! isDesignMode)
        throw new ApplicationException ("Page class " + page.GetType().FullName + " has no field of type HtmlForm. Please add a field or override property IWxePage.HtmlForm.");
      else if (fields.Length > 1)
        throw new ApplicationException ("Page class " + page.GetType().FullName + " has more than one field of type HtmlForm. Please remove excessive fields or override property IWxePage.HtmlForm.");
      if (fields.Length > 0) // Can only be null without an exception during design mode
      {
        _htmlFormField = (FieldInfo) fields[0];
        _htmlFormFieldInitialized = true;
      }
    }
  }

  private bool FindHtmlFormControlFilter (MemberInfo member, object filterCriteria)
  {
    return (member is FieldInfo && ((FieldInfo)member).FieldType == typeof (HtmlForm));
  }

  /// <summary> 
  ///   Implements <see cref="ISmartPage.HtmlForm">IWxePage.HtmlForm</see>.
  /// </summary>
  public HtmlForm HtmlForm
  {
    get
    {
      EnsureHtmlFormFieldInitialized();

      Control page = (Page) _page;
#if ! NET11      
      if (((Page) page).Master != null)
        page = ((Page) page).Master;
#endif

      if (_htmlFormField != null) // Can only be null without an exception during design mode
        return (HtmlForm) _htmlFormField.GetValue (page);
      else
        return null;
    }
    set
    {
      EnsureHtmlFormFieldInitialized();

      Control page = (Page) _page;
#if ! NET11      
      if (((Page) page).Master != null)
        page = ((Page) page).Master;
#endif

      if (_htmlFormField != null) // Can only be null without an exception during design mode
        _htmlFormField.SetValue (page, value);
    }
  }


  private void Page_Init(object sender, EventArgs e)
  {
#if ! NET11
    if (((Page) _page).Header != null)
    {
      bool hasHeadContents = false;
      foreach (Control control in ((Page) _page).Header.Controls)
      {
        if (control is HtmlHeadContents)
        {
          hasHeadContents = true;
          break;
        }
      }
      if (! hasHeadContents)
        ((Page) _page).Header.Controls.AddAt (0, new HtmlHeadContents());
    }
#endif
  }

  private void Page_PreRender (object sender, EventArgs e)
  {
    PreRenderSmartPage();
    PreRenderSmartNavigation();
    
    _isPreRendering = true;
  }

  private void PreRenderSmartPage()
  {    
    _page.RegisterHiddenField (SmartPageInfo.CacheDetectionID, null);

    HtmlHeadAppender.Current.RegisterUtilitiesJavaScriptInclude ((Page) _page);
    string url = ResourceUrlResolver.GetResourceUrl (
        (Page) _page, typeof (SmartPageInfo), ResourceType.Html, c_scriptFileUrl);
    HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_scriptFileKey, url);

    if (! ControlHelper.IsDesignMode (_page, HttpContext.Current))
    {
      url = ResourceUrlResolver.GetResourceUrl (
          (Page) _page, typeof (SmartPageInfo), ResourceType.Html, c_styleFileUrl);
      HtmlHeadAppender.Current.RegisterStylesheetLink (s_styleFileKey, url, HtmlHeadAppender.Priority.Library);
    }
  
    RegisterSmartPageInitializationScript(); 
  }

  private void RegisterSmartPageInitializationScript()
  {
    string abortMessage = GetAbortMessage();
    string statusIsSubmittingMessage = GetStatusIsSubmittingMessage ();

    string checkFormStateFunction = "null";
    if (! StringUtility.IsNullOrEmpty (_checkFormStateFunction))
      checkFormStateFunction = "'" + _checkFormStateFunction + "'";

    string smartScrollingFieldID = "null";
    string smartFocusFieldID = "null";

    ISmartNavigablePage smartNavigablePage = _page as ISmartNavigablePage;
    if (smartNavigablePage != null)
    {
      if (smartNavigablePage.IsSmartScrollingEnabled)
        smartScrollingFieldID = "'" + c_smartScrollingID + "'";
      if (smartNavigablePage.IsSmartFocusingEnabled)
        smartFocusFieldID = "'" + c_smartFocusID + "'";
    }
  
    string isDirtyStateTrackingEnabled = "false";
    string isDirty = "false";

    StringBuilder initScript = new StringBuilder (500);

    const string eventHandlersArray = "_smartPage_eventHandlers";
    initScript.Append ("var ").Append (eventHandlersArray).Append (" = new Array(); \r\n");
    FormatPopulateEventHandlersArrayClientScript (initScript, eventHandlersArray);
    initScript.Append ("\r\n");

    const string trackedControlsArray = "_smartPage_trackedControls";
    initScript.Append ("var ").Append (trackedControlsArray).Append (" = new Array(); \r\n");
    if (_page.IsDirtyStateTrackingEnabled)
    {
      isDirtyStateTrackingEnabled = "true";
      if (_page.EvaluateDirtyState())
        isDirty = "true";
      else
        FormatPopulateTrackedControlsArrayClientScript (initScript, trackedControlsArray);
    }
    initScript.Append ("\r\n");

    initScript.Append ("SmartPage_Context.Instance = new SmartPage_Context (\r\n");
    initScript.Append ("    '").Append (_page.HtmlForm.ClientID).Append ("',\r\n");
    initScript.Append ("    ").Append (isDirtyStateTrackingEnabled).Append (",\r\n");
    initScript.Append ("    ").Append (isDirty).Append (",\r\n");
    initScript.Append ("    ").Append (abortMessage).Append (",\r\n");
    initScript.Append ("    ").Append (statusIsSubmittingMessage).Append (",\r\n");
    initScript.Append ("    ").Append (smartScrollingFieldID).Append (",\r\n");
    initScript.Append ("    ").Append (smartFocusFieldID).Append (",\r\n");
    initScript.Append ("    ").Append (checkFormStateFunction).Append (",\r\n");
    initScript.Append ("    ").Append (eventHandlersArray).Append (",\r\n");
    initScript.Append ("    ").Append (trackedControlsArray).Append ("); \r\n");

    initScript.Append ("\r\n");
    initScript.Append (eventHandlersArray).Append (" = null; \r\n");
    initScript.Append (trackedControlsArray).Append (" = null; \r\n");
    initScript.Append ("delete ").Append (eventHandlersArray).Append ("; \r\n");
    initScript.Append ("delete ").Append (trackedControlsArray).Append (";");

    ScriptUtility.RegisterClientScriptBlock ((Page) _page, "smartPageInitialize", initScript.ToString());
    ScriptUtility.RegisterStartupScriptBlock ((Page) _page, "smartPageStartUp", "SmartPage_OnStartUp();");
  }

  private string GetAbortMessage ()
  {
    string abortMessage = "null";
    IResourceManager resourceManager = GetResourceManager ();

    if (_page.IsAbortConfirmationEnabled)
    {
      string temp;
      if (StringUtility.IsNullOrEmpty (_page.AbortMessage))
        temp = resourceManager.GetString (ResourceIdentifier.AbortMessage);
      else
        temp = _page.AbortMessage;
      abortMessage = "'" + ScriptUtility.EscapeClientScript (temp) + "'";
    }

    return abortMessage;
  }

  private string GetStatusIsSubmittingMessage ()
  {
    string statusIsSubmittingMessage = "null";
    IResourceManager resourceManager = GetResourceManager ();

    if (_page.IsStatusIsSubmittingMessageEnabled)
    {
      string temp;
      if (StringUtility.IsNullOrEmpty (_page.StatusIsSubmittingMessage))
        temp = resourceManager.GetString (ResourceIdentifier.StatusIsSubmittingMessage);
      else
        temp = _page.StatusIsSubmittingMessage;
      statusIsSubmittingMessage = "'" + ScriptUtility.EscapeClientScript (temp) + "'";
    }

    return statusIsSubmittingMessage;
  }

  private void FormatPopulateTrackedControlsArrayClientScript (StringBuilder script, string trackedControlsArray)
  {
    foreach (IModifiableControl control in _trackedControls.Values)
    {
      if (control.Visible)
      {
        string[] trackedIDs = control.GetTrackedClientIDs ();
        for (int i = 0; i < trackedIDs.Length; i++)
        {
          // IE 5.0.1 does not understand push
          script.Append (trackedControlsArray).Append ("[").Append (trackedControlsArray).Append (".length] = '");
          script.Append (trackedIDs[i]);
          script.Append ("'; \r\n");
        }
      }
    }

    foreach (string trackedID in _trackedControlsByID)
    {
      // IE 5.0.1 does not understand push
      script.Append (trackedControlsArray).Append ("[").Append (trackedControlsArray).Append (".length] = '");
      script.Append (trackedID);
      script.Append ("'; \r\n");
    }
  }

  private void FormatPopulateEventHandlersArrayClientScript (StringBuilder script, string eventHandlersArray)
  {
    const string eventHandlersByEventArray = "_smartPage_eventHandlersByEvent";

    script.Append ("var ").Append (eventHandlersByEventArray).Append (" = null; \r\n");
    script.Append ("\r\n");
    foreach (SmartPageEvents pageEvent in _clientSideEventHandlers.Keys)
    {
      NameValueCollection eventHandlers = (NameValueCollection) _clientSideEventHandlers[pageEvent];

      script.Append (eventHandlersByEventArray).Append (" = new Array(); \r\n");

      for (int i = 0; i < eventHandlers.Keys.Count; i++)
      {
        // IE 5.0.1 does not understand push
        script.Append (eventHandlersByEventArray).Append ("[").Append (eventHandlersByEventArray).Append (".length] = '");
        script.Append (eventHandlers.Get (i));
        script.Append ("'; \r\n");
      }

      script.Append (eventHandlersArray).Append ("['");
      script.Append (pageEvent.ToString ().ToLower ());
      script.Append ("'] = ").Append (eventHandlersByEventArray).Append ("; \r\n");
      script.Append ("\r\n");
    }
    script.Append (eventHandlersByEventArray).Append (" = null; \r\n");
    script.Append ("delete ").Append (eventHandlersByEventArray).Append ("; \r\n");
  }

  private void PreRenderSmartNavigation()
  {
    ISmartNavigablePage smartNavigablePage = _page as ISmartNavigablePage;
    if (smartNavigablePage == null)
      return;

    NameValueCollection postBackCollection = _page.GetPostBackCollection();
    Page page = (Page) _page;

    if (smartNavigablePage.IsSmartScrollingEnabled || smartNavigablePage.IsSmartFocusingEnabled)
    {
      string url = ResourceUrlResolver.GetResourceUrl (
          page, typeof (SmartPageInfo), ResourceType.Html, c_smartNavigationScriptFileUrl);
      HtmlHeadAppender.Current.RegisterJavaScriptInclude (s_smartNavigationScriptKey, url);
    }

    if (smartNavigablePage.IsSmartScrollingEnabled)
    {
      string smartScrollingValue = null;
      if (postBackCollection != null && ! _isSmartNavigationDataDisacarded)
        smartScrollingValue = postBackCollection[c_smartScrollingID];
      page.RegisterHiddenField (c_smartScrollingID, smartScrollingValue);
    }

    if (smartNavigablePage.IsSmartFocusingEnabled)
    {
      string smartFocusValue = null;
      if (postBackCollection != null && ! _isSmartNavigationDataDisacarded)
        smartFocusValue = postBackCollection[c_smartFocusID];
      if (! StringUtility.IsNullOrEmpty (_smartFocusID))
        smartFocusValue = _smartFocusID;
      page.RegisterHiddenField (c_smartFocusID, smartFocusValue);
    }
  }


  /// <summary>
  ///   Implements <see cref="ISmartPage.StatusIsSubmittingMessage">ISmartPage.StatusIsSubmittingMessage</see>.
  /// </summary>
  public string StatusIsSubmittingMessage
  {
    get { return _statusIsSubmittingMessage; }
    set { _statusIsSubmittingMessage = StringUtility.NullToEmpty (value); }
  }

  /// <summary>
  ///   Implements <see cref="ISmartPage.AbortMessage">ISmartPage.AbortMessage</see>.
  /// </summary>
  public string AbortMessage 
  {
    get { return _abortMessage; }
    set { _abortMessage = StringUtility.NullToEmpty (value); }
  }

  /// <summary>
  ///   Implements <see cref="ISmartNavigablePage.DiscardSmartNavigationData">ISmartNavigablePage.DiscardSmartNavigationData</see>.
  /// </summary>
  public void DiscardSmartNavigationData ()
  {
    _isSmartNavigationDataDisacarded = true;
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.UI.ISmartNavigablePage.SetFocus(Rubicon.Web.UI.Controls.IFocusableControl)">ISmartNavigablePage.SetFocus(IFocusableControl)</see>.
  /// </summary>
  public void SetFocus (IFocusableControl control)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    if (StringUtility.IsNullOrEmpty (control.FocusID))
      return;
    SetFocus (control.FocusID);
  }

  /// <summary>
  ///   Implements <see cref="M:Rubicon.Web.UI.ISmartNavigablePage.SetFocus(System.String)">ISmartNavigablePage.SetFocus(String)</see>.
  /// </summary>
  public void SetFocus (string id)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("id", id);
    _smartFocusID = id;
  }
  public void RegisterNavigationControl (INavigationControl control)
  {
    ArgumentUtility.CheckNotNull ("control", control);
    _navigationControls[control] = control;
  }

  public string AppendNavigationUrlParameters (string url)
  {
    NameValueCollection urlParameters = GetNavigationUrlParameters();
    return UrlUtility.AddParameters (url, urlParameters);
  }

  public NameValueCollection GetNavigationUrlParameters()
  {
    NameValueCollection urlParameters = new NameValueCollection();
    foreach (INavigationControl control in _navigationControls.Values)
      urlParameters.Add (control.GetNavigationUrlParameters());
    
    return urlParameters;
  }
}

}

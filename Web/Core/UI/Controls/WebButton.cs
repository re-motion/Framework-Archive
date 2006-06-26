using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using Rubicon.NullableValueTypes;
using Rubicon.Web.UI.Globalization;
using Rubicon.Globalization;
using Rubicon.Security;

namespace Rubicon.Web.UI.Controls
{

  /// <summary> A <c>Button</c> using <c>&amp;</c> as access key prefix in <see cref="Button.Text"/>. </summary>
  /// <include file='doc\include\UI\Controls\WebButton.xml' path='WebButton/Class/*' />
  [ToolboxData ("<{0}:WebButton runat=server></{0}:WebButton>")]
  public class WebButton :
      Button,
    // Required because Page.ProcessPostData always registers the last IPostBackEventHandler in the controls 
    // collection for controls (buttons) having PostData but no IPostBackDataHandler. 
      IPostBackDataHandler
  {
    private static readonly object s_clickEvent = new object ();

    private IconInfo _icon;
#if NET11
  private bool _useSubmitBehavior = true;
#else
    PostBackOptions _options;
#endif

    private NaBooleanEnum _useLegacyButton = NaBooleanEnum.Undefined;

    private ISecurableObject _securableObject;
    private MissingPermissionBehavior _missingPermissionBehavior = MissingPermissionBehavior.Invisible;

    public WebButton ()
    {
      _icon = new IconInfo ();
    }

    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
    }

    /// <remarks>
    ///   This method is never called if the button is rendered as a legacy button.
    /// </remarks>
    bool IPostBackDataHandler.LoadPostData (string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
    {
      ArgumentUtility.CheckNotNull ("postCollection", postCollection);

      string eventTarget = postCollection[ControlHelper.PostEventSourceID];
      bool isScriptedPostBack = !StringUtility.IsNullOrEmpty (eventTarget);
      if (!isScriptedPostBack && IsLegacyButtonEnabled)
      {
        // The button can only fire a click event if client script is active or the button is used in legacy mode
        // A more general fallback is not possible becasue of compatibility issues with ExecuteFunctionNoRepost
        bool isSuccessfulControl = !StringUtility.IsNullOrEmpty (postCollection[postDataKey]);
        if (isSuccessfulControl)
          Page.RegisterRequiresRaiseEvent (this);
      }
      return false;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (this, true);
      LoadResources (resourceManager);
    }

    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      string accessKey;
      string text = StringUtility.NullToEmpty (Text);
      text = SmartLabel.FormatLabelText (text, !IsLegacyButtonEnabled, out accessKey);

      if (StringUtility.IsNullOrEmpty (AccessKey))
        writer.AddAttribute (HtmlTextWriterAttribute.Accesskey, accessKey);

      string tempText = Text;
      bool hasIcon = _icon != null && !StringUtility.IsNullOrEmpty (_icon.Url);
      bool hasText = !StringUtility.IsNullOrEmpty (text);
      if (IsLegacyButtonEnabled)
      {
        if (hasText)
          Text = text;
        else if (hasIcon)
          Text = _icon.AlternateText;
      }
      else
      {
        Text = text;
      }

#if NET11
    AddAttributesToRender_net11 (writer);
#else
      AddAttributesToRender_net20 (writer);
#endif

      Text = tempText;

      if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
        writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

#if NET11
  /// <summary> Method to be executed when compiled for .net 1.1. </summary>
  private void AddAttributesToRender_net11 (HtmlTextWriter writer)
  {
    if (Page != null)
      Page.VerifyRenderingInServerForm(this);

    if (! IsUseSubmitBehaviorEnabled) // System.Web.UI.WebControls.Button already adds a type=submit
      writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");

    if (Page != null)
    {
      string onClick = string.Empty;
      if (CausesValidation && Page.Validators.Count > 0)
      {
        const string clientValidation = "if (typeof(Page_ClientValidate) == 'function') Page_ClientValidate(); ";
        onClick += clientValidation;
        if (Attributes.Count > 0)
        {
          string baseOnClick = Attributes["onclick"];
          if (baseOnClick != null)
          {
            onClick += baseOnClick;
            Attributes.Remove("onclick");
          }
        }
      }
      onClick += "this.disabled = (typeof (Page_IsValid) == 'undefined' || Page_IsValid == null || Page_IsValid == true);";
      string postBackEventReference = Page.GetPostBackEventReference (this);
      if (CausesValidation)
        onClick += " if (typeof (Page_IsValid) == 'undefined' || Page_IsValid == null || Page_IsValid == true) " + postBackEventReference + "; ";
      else
        onClick += postBackEventReference + "; ";
      onClick += "return false;";
      writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
    }
    bool causesValidationTemp = CausesValidation;
    CausesValidation = false;
    base.AddAttributesToRender (writer);
    CausesValidation = causesValidationTemp;
  }
#endif

#if ! NET11
    /// <summary> Method to be executed when compiled for .net 2.0. </summary>
    private void AddAttributesToRender_net20 (HtmlTextWriter writer)
    {
      if (this.Page != null)
        Page.VerifyRenderingInServerForm (this);

      if (IsEnabled)
      {
        string onClick = EnsureEndWithSemiColon (OnClientClick);
        if (HasAttributes)
        {
          string onClickAttribute = Attributes["onclick"];
          if (onClickAttribute != null)
          {
            onClick = onClick + EnsureEndWithSemiColon (onClickAttribute);
            Attributes.Remove ("onclick");
          }
        }

        if (Page != null)
        {
          PostBackOptions options = GetPostBackOptions ();
          options.ClientSubmit = true;

          string postBackScript;
          if (options.PerformValidation)
            postBackScript = "this.disabled = true;";
          else
            postBackScript = "this.disabled = (typeof (Page_IsValid) == 'undefined' || Page_IsValid == null || Page_IsValid == true);";

          string postBackEventReference = Page.ClientScript.GetPostBackEventReference (options, false);
          if (StringUtility.IsNullOrEmpty (postBackEventReference))
            postBackEventReference = Page.ClientScript.GetPostBackEventReference (this, null);
          postBackScript += EnsureEndWithSemiColon (postBackEventReference);

          if (options.PerformValidation)
            postBackScript += "this.disabled = (typeof (Page_IsValid) == 'undefined' || Page_IsValid == null || Page_IsValid == true);";
          postBackScript += "return false;";

          if (postBackScript != null)
            onClick = MergeScript (onClick, postBackScript);
        }

        if (!StringUtility.IsNullOrEmpty (onClick))
          writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClick);
      }


      _options = base.GetPostBackOptions ();
      _options.ClientSubmit = false;
      _options.PerformValidation = false;
      _options.AutoPostBack = false;

      string backUpOnClientClick = OnClientClick;
      OnClientClick = null;

      base.AddAttributesToRender (writer);

      OnClientClick = backUpOnClientClick;

      _options = null;
    }

    protected override PostBackOptions GetPostBackOptions ()
    {
      if (_options == null)
        return base.GetPostBackOptions ();
      else
        return _options;
    }
#endif

    protected override HtmlTextWriterTag TagKey
    {
      get
      {
        if (IsLegacyButtonEnabled)
          return HtmlTextWriterTag.Input;
        else
          return HtmlTextWriterTag.Button;
      }
    }

    /// <summary> Checks whether the control conforms to the required WAI level. </summary>
    /// <exception cref="WcagException"> Thrown if the control does not conform to the required WAI level. </exception>
    protected virtual void EvaluateWaiConformity ()
    {
      if (WcagHelper.Instance.IsWcagDebuggingEnabled () && WcagHelper.Instance.IsWaiConformanceLevelARequired ())
      {
        if (_useLegacyButton != NaBooleanEnum.True)
          WcagHelper.Instance.HandleError (1, this, "UseLegacyButton");
#if NET11
      if (! _useSubmitBehavior)
        WcagHelper.Instance.HandleError (1, this, "UseSubmitBehavior");
#endif

      }
    }

    protected override void RenderContents (HtmlTextWriter writer)
    {
      EvaluateWaiConformity ();

      if (IsLegacyButtonEnabled)
        return;

      string text = StringUtility.NullToEmpty (Text);
      text = SmartLabel.FormatLabelText (text, true);

      if (HasControls ())
      {
        base.RenderContents (writer);
      }
      else
      {
        bool hasIcon = _icon != null && !StringUtility.IsNullOrEmpty (_icon.Url);
        bool hasText = !StringUtility.IsNullOrEmpty (text);
        if (hasIcon)
        {
          writer.AddAttribute (HtmlTextWriterAttribute.Src, _icon.Url);
          if (!_icon.Height.IsEmpty)
            writer.AddAttribute (HtmlTextWriterAttribute.Height, _icon.Height.ToString ());
          if (!_icon.Width.IsEmpty)
            writer.AddAttribute (HtmlTextWriterAttribute.Width, _icon.Width.ToString ());
          writer.AddStyleAttribute ("vertical-align", "middle");
          writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
          writer.AddAttribute (HtmlTextWriterAttribute.Alt, _icon.AlternateText);
          writer.RenderBeginTag (HtmlTextWriterTag.Img);
          writer.RenderEndTag ();
        }
        if (hasIcon && hasText)
          writer.Write ("&nbsp;");
        if (hasText)
          writer.Write (text); // Do not HTML enocde
      }
    }

    /// <summary> Gets or sets the icon displayed in this menu item. </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The icon displayed.")]
    [NotifyParentProperty (true)]
    public IconInfo Icon
    {
      get { return _icon; }
      set { _icon = value; }
    }

#if NET11
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category("Behavior")]
  [DefaultValue(true)]
  [Description ("Indicates whether the button renders as a submit button.")]
  public virtual bool UseSubmitBehavior
  {
    get { return _useSubmitBehavior; }
    set { _useSubmitBehavior = value; }
  } 

  protected bool IsUseSubmitBehaviorEnabled
  {
    get { return WcagHelper.Instance.IsWaiConformanceLevelARequired() || _useSubmitBehavior;}
  }
#endif

    /// <summary> Loads the resources into the control's properties. </summary>
    protected virtual void LoadResources (IResourceManager resourceManager)
    {
      if (resourceManager == null)
        return;

      if (ControlHelper.IsDesignMode (this))
        return;

      //  Dispatch simple properties
      string key;
      key = ResourceManagerUtility.GetGlobalResourceKey (Text);
      if (!StringUtility.IsNullOrEmpty (key))
        Text = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (AccessKey);
      if (!StringUtility.IsNullOrEmpty (key))
        AccessKey = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (ToolTip);
      if (!StringUtility.IsNullOrEmpty (key))
        ToolTip = resourceManager.GetString (key);

      if (Icon != null)
        Icon.LoadResources (resourceManager);
    }

    /// <summary> 
    ///   Gets or sets the flag that determines whether to use a legacy (i.e. input) element for the button or the modern 
    ///   form (i.e. button). 
    /// </summary>
    /// <value> 
    ///   <see cref="NaBooleanEnum.True"/> to enable the legacy version. 
    ///   Defaults to <see cref="NaBooleanEnum.Undefined"/>, which is interpreted as <see cref="NaBooleanEnum.False"/>.
    /// </value>
    [Description ("Determines whether to use a legacy (i.e. input) element for the button or the modern form (i.e. button).")]
    [Category ("Behavior")]
    [DefaultValue (NaBooleanEnum.Undefined)]
    public NaBooleanEnum UseLegacyButton
    {
      get { return _useLegacyButton; }
      set { _useLegacyButton = value; }
    }

    protected bool IsLegacyButtonEnabled
    {
      get { return WcagHelper.Instance.IsWaiConformanceLevelARequired () || _useLegacyButton == NaBooleanEnum.True; }
    }

    private string EnsureEndWithSemiColon (string value)
    {
      if (!StringUtility.IsNullOrEmpty (value))
      {
        value = value.Trim ();

        if (!value.EndsWith (";"))
          value += ";";
      }

      return value;
    }

    private string MergeScript (string firstScript, string secondScript)
    {
      if (!StringUtility.IsNullOrEmpty (firstScript))
        return (firstScript + secondScript);
      if (secondScript.TrimStart (new char[0]).StartsWith ("javascript:"))
        return secondScript;
      return ("javascript:" + secondScript);
    }

    protected bool HasAccess ()
    {
      IWebSecurityProvider securityProvider = SecurityProviderRegistry.Instance.GetProvider<IWebSecurityProvider> ();
      if (securityProvider == null)
        return true;

      EventHandler clickHandler = (EventHandler) Events[s_clickEvent];
      if (clickHandler == null)
        return true;
      
      return securityProvider.HasAccess (_securableObject, (EventHandler) Events[s_clickEvent]);
    }

    public override bool Visible
    {
      get
      {
        if (_missingPermissionBehavior == MissingPermissionBehavior.Invisible)
        {
          if (base.Visible)
            return HasAccess ();
          return false;
        }

        return base.Visible;
      }
      set
      {
        base.Visible = value;
      }
    }

    public override bool Enabled
    {
      get
      {
        if (_missingPermissionBehavior == MissingPermissionBehavior.Disabled)
        {
          if (base.Enabled)
            return HasAccess ();
          return false;
        }

        return base.Enabled;
      }
      set
      {
        base.Enabled = value;
      }
    }

    [Category ("Action")]
    public new event EventHandler Click
    {
      add
      {
        base.Events.AddHandler (s_clickEvent, value);
      }
      remove
      {
        base.Events.RemoveHandler (s_clickEvent, value);
      }
    }

    protected override void OnClick (EventArgs e)
    {
      base.OnClick (e);

      EventHandler clickHandler = (EventHandler) Events[s_clickEvent];
      if (clickHandler != null)
        clickHandler (this, e);
    }

    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public ISecurableObject SecurableObject
    {
      get { return _securableObject; }
      set { _securableObject = value; }
    }

    [Category ("Behavior")]
    [DefaultValue (MissingPermissionBehavior.Invisible)]
    public MissingPermissionBehavior MissingPermissionBehavior
    {
      get { return _missingPermissionBehavior; }
      set { _missingPermissionBehavior = value; }
    }
	
    #region protected virtual string CssClass...
    /// <summary> Gets the CSS-Class applied to the <see cref="WebButton"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>webButton</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBase
    { get { return "webButton"; } }
    #endregion
  }

}

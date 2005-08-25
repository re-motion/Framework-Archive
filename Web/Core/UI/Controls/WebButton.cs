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

namespace Rubicon.Web.UI.Controls
{

/// <summary> A <c>Button</c> using <c>&amp;</c> as access key prefix in <see cref="Button.Text"/>. </summary>
/// <include file='doc\include\UI\Controls\WebButton.xml' path='WebButton/Class/*' />
[ToolboxData("<{0}:WebButton runat=server></{0}:WebButton>")]
public class WebButton : 
    Button
#if ! net20
    // Required because Page.ProcessPostData always registers the last IPostBackEventHandler in the controls 
    // collection for controls (buttons) having PostData but no IPostBackDataHandler. 
    // .net 2.0 resolves this issue for controls using a javascript induced postback event.
    , IPostBackDataHandler
#endif
{
  private IconInfo _icon;
#if ! net20
  private bool _useSubmitBehavior = true;
#endif

  private NaBooleanEnum _useLegacyButton = NaBooleanEnum.Undefined;

  public WebButton()
  {
    _icon = new IconInfo();
  }

#if ! net20
  void IPostBackDataHandler.RaisePostDataChangedEvent()
  {
  }

  bool IPostBackDataHandler.LoadPostData (string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
  {
    bool isScriptedPostBack = StringUtility.IsNullOrEmpty (postCollection[ControlHelper.PostEventSourceID]);
    if (isScriptedPostBack)
    {
      bool isSuccessfulControl = ! StringUtility.IsNullOrEmpty (postCollection[postDataKey]);
      if (isSuccessfulControl)
        Page.RegisterRequiresRaiseEvent (this);
    }
    return false;
  }
#endif

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);

    IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager (this, true);
    LoadResources (resourceManager);
  }

  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    string accessKey;
    string text = StringUtility.NullToEmpty (Text);
    text = SmartLabel.FormatLabelText (text, !IsLegacyButtonEnabled, out accessKey);

    if (StringUtility.IsNullOrEmpty (AccessKey))
      writer.AddAttribute (HtmlTextWriterAttribute.Accesskey, accessKey);
    
    string tempText = Text;
    bool hasIcon = _icon != null && ! StringUtility.IsNullOrEmpty (_icon.Url);
    bool hasText = ! StringUtility.IsNullOrEmpty (text);
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

#if ! net20
    AddAttributesToRender_net11 (writer);
#else
    AddAttributesToRender_net20 (writer);
#endif

    Text = tempText;
  
    if (StringUtility.IsNullOrEmpty (CssClass) && StringUtility.IsNullOrEmpty (Attributes["class"]))
      writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
  }

  /// <summary> Method to be executed when compiled for .net 1.1. </summary>
  private void AddAttributesToRender_net11 (HtmlTextWriter writer)
  {
#if ! net20
    if (Page != null)
      Page.VerifyRenderingInServerForm(this);

    if (! _useSubmitBehavior) // System.Web.UI.WebControls.Button already adds a type=submit
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
      onClick += "this.disabled=true; ";
      onClick += Page.GetPostBackEventReference (this) + "; ";
      onClick += "return false;";
      writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
    }
    bool causesValidationTemp = CausesValidation;
    CausesValidation = false;
    base.AddAttributesToRender (writer);
    CausesValidation = causesValidationTemp;
#endif
  }

  /// <summary> Method to be executed when compiled for .net 2.0. </summary>
  private void AddAttributesToRender_net20 (HtmlTextWriter writer)
  {
#if net20
//    if (base.IsEnabled)
//    {
//      string tempOnClientClick = OnClientClick;
//      OnClientClick = null;
//      string onClick = EnsureEndWithSemiColon (tempOnClientClick);
//      if (HasAttributes)
//      {
//        string onClickAttribute = Attributes["onclick"];
//        if (onClickAttribute != null)
//        {
//          onClick = onClick + EnsureEndWithSemiColon (onClickAttribute);
//          Attributes.Remove ("onclick");
//        }
//      }
//      if (Page != null)
//      {
//        PostBackOptions options = GetPostBackOptions();
//        bool tempClientSubmit = options.ClientSubmit;
//        options.ClientSubmit = false;
//        string postBackScript = Page.ClientScript.GetPostBackEventReference (options);
//        options.ClientSubmit = tempClientSubmit;
//        postBackScript = EnsureEndWithSemiColon (postBackScript);
//        postBackScript += "this.disabled=true; ";
//        postBackScript += Page.ClientScript.GetPostBackEventReference (this, null) + "; ";
//        postBackScript += "return false;";
//
//        if (postBackScript != null)
//          onClick = MergeScript(onClick, postBackScript);
//      }
//      if (! StringUtility.IsNullOrEmpty (onClick))
//      {
//        writer.AddAttribute(HtmlTextWriterAttribute.Onclick, onClick);
//      }
//      OnClientClick = tempOnClientClick;
//    }
    base.AddAttributesToRender (writer);
#endif
  }

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
    if (WcagUtility.IsWcagDebuggingEnabled() && WcagUtility.IsWaiConformanceLevelARequired())
    {
      if (_useLegacyButton == NaBooleanEnum.True)
        throw new WcagException (1, this, "UseLegacyButton");
    }
  }

  protected override void RenderContents(HtmlTextWriter writer)
  {
    if (WcagUtility.IsWaiConformanceLevelARequired())
      EvaluateWaiConformity ();

    if (IsLegacyButtonEnabled)
      return;

    string text = StringUtility.NullToEmpty (Text);
    text = SmartLabel.FormatLabelText (text, true);

    if (HasControls())
    {
      base.RenderContents (writer);
    }
    else
    {
      bool hasIcon = _icon != null && ! StringUtility.IsNullOrEmpty (_icon.Url);
      bool hasText = ! StringUtility.IsNullOrEmpty (text);
      if (hasIcon)
      {
        writer.AddAttribute (HtmlTextWriterAttribute.Src, _icon.Url);
        if (! _icon.Height.IsEmpty)
          writer.AddAttribute (HtmlTextWriterAttribute.Height, _icon.Height.ToString());
        if (! _icon.Width.IsEmpty)
          writer.AddAttribute (HtmlTextWriterAttribute.Width, _icon.Width.ToString());
        writer.AddStyleAttribute ("vertical-align", "middle");
        writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
        writer.AddAttribute (HtmlTextWriterAttribute.Alt, _icon.AlternateText);
        writer.RenderBeginTag (HtmlTextWriterTag.Img);
        writer.RenderEndTag();
      }
      if (hasIcon && hasText)
        writer.Write ("&nbsp;");
      if (hasText)
        writer.Write (text);
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

#if ! net20
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category("Behavior")]
  [DefaultValue(true)]
  [Description ("Indicates whether the button renders as a submit button.")]
  public virtual bool UseSubmitBehavior
  {
    get { return _useSubmitBehavior; }
    set { _useSubmitBehavior = value; }
  } 
#endif

  /// <summary> Loads the resources into the control's properties. </summary>
  protected virtual void LoadResources (IResourceManager resourceManager)
  {
    ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
    if (ControlHelper.IsDesignMode (this))
      return;

    //  Dispatch simple properties
    string key;
    key = ResourceManagerUtility.GetGlobalResourceKey (Text);
    if (! StringUtility.IsNullOrEmpty (key))
      Text = resourceManager.GetString (key);

    key = ResourceManagerUtility.GetGlobalResourceKey (AccessKey);
    if (! StringUtility.IsNullOrEmpty (key))
      AccessKey = resourceManager.GetString (key);

    key = ResourceManagerUtility.GetGlobalResourceKey (ToolTip);
    if (! StringUtility.IsNullOrEmpty (key))
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
  [Description("Determines whether to use a legacy (i.e. input) element for the button or the modern form (i.e. button).")]
  [Category ("Behavior")]
  [DefaultValue (NaBooleanEnum.Undefined)]
  public NaBooleanEnum UseLegacyButton
  {
    get { return _useLegacyButton; }
    set { _useLegacyButton = value; }
  }

  protected bool IsLegacyButtonEnabled
  {
    get { return ! WcagUtility.IsWaiConformanceLevelARequired() && _useLegacyButton == NaBooleanEnum.True;}
  }

  private string EnsureEndWithSemiColon (string value)
  {
    if (! StringUtility.IsNullOrEmpty (value))
    {
      value = value.Trim ();

      if (!value.EndsWith (";"))
        value += ";";
    }

    return value;
  }

  private string MergeScript(string firstScript, string secondScript)
  {
    if (! StringUtility.IsNullOrEmpty (firstScript))
      return (firstScript + secondScript);
    if (secondScript.TrimStart(new char[0]).StartsWith("javascript:"))
      return secondScript;
    return ("javascript:" + secondScript);
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

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Collections;
using Rubicon.Web.UI.Globalization;
using Rubicon.Globalization;

namespace Rubicon.Web.UI.Controls
{

//  TODO: Command: Move long comment blocks to xml-file
/// <summary> An <see cref="Command"/> defines an action the user can invoke. </summary>
[TypeConverter (typeof (ExpandableObjectConverter))]
public class Command: IControlItem
{
  /// <summary> Wraps the properties required for rendering a hyperlink. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class HrefCommandInfo
  {
    private string _href = string.Empty;
    private string _target = string.Empty;

    /// <summary> Simple constructor. </summary>
    public HrefCommandInfo()
    {
    }

    /// <summary> Returns a string representation of this <see cref="HrefCommandInfo"/>. </summary>
    /// <remarks> Foramt: Href, Target </remarks>
    /// <returns> A <see cref="string"/>. </returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder (50);

      if (_href == string.Empty || _target == string.Empty)
        return _href;
      else
        return _href + ", " + _target;
    }

    public virtual string FormatHref (params string[] parameters)
    {
      string[] encodedParameters = new string[parameters.Length];
      for (int i = 0; i < parameters.Length; i++)
      {
        if (HttpContext.Current != null)
          encodedParameters[i] = HttpUtility.UrlEncode (parameters[i], HttpContext.Current.Response.ContentEncoding);
        else
          encodedParameters[i] = "";
      }
      return string.Format (Href, encodedParameters);
    }

    /// <summary> Gets or sets the URL to link to when the rendered command is clicked. </summary>
    /// <value> 
    ///   The URL to link to when the rendered command is clicked. The default value is 
    ///   <see cref="String.Empty"/>. 
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The hyperlink reference of the command.")]
    [DefaultValue("")]
    [NotifyParentProperty (true)]
    public virtual string Href 
    {
      get
      {
        return _href; 
      }
      set
      {
        _href = StringUtility.NullToEmpty (value); 
        _href = _href.Trim();
      }
    }

    /// <summary> 
    ///   Gets or sets the target window or frame to display the web page specified by <see cref="Href"/> 
    ///   when  the rendered command is clicked.
    /// </summary>
    /// <value> 
    ///   The target window or frame to load the web page specified by <see cref="Href"/> when the rendered command
    ///   is clicked.  The default value is <see cref="String.Empty"/>. 
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The target window or frame of the command. Leave it blank for no target.")]
    [DefaultValue("")]
    [NotifyParentProperty (true)]
    public virtual string Target 
    { 
      get
      { 
        return _target; 
      }
      set
      {
         _target = StringUtility.NullToEmpty (value); 
         _target = _target.Trim();
      }
    }
  }

  /// <summary> Wraps the properties required for calling a WxeFunction. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class WxeFunctionCommandInfo
  {
    private string _typeName = string.Empty;
    private string _parameters = string.Empty;
    private string _target = string.Empty;
    private bool _createPermanentUrlForExternalFunction = false;

    /// <summary> Simple constructor. </summary>
    public WxeFunctionCommandInfo()
    {
    }

    /// <summary>
    ///   Returns a string representation of this <see cref="WxeFunctionCommandInfo"/>.
    /// </summary>
    /// <remarks> Foramt: Href, Target </remarks>
    /// <returns> A <see cref="string"/>. </returns>
    public override string ToString()
    {
      if (_typeName == string.Empty)
        return string.Empty;
      else
        return _typeName + " (" + _parameters + ")";
    }

    /// <summary> 
    ///   Gets or sets the complete type name of the WxeFunction to call when the rendered 
    ///   command is clicked. The type name is requried.
    /// </summary>
    /// <value> 
    ///   The complete type name of the WxeFunction to call when the rendered command is clicked. 
    ///   The default value is <see cref="String.Empty"/>. 
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The complete type name (type, assembly) of the WxeFunction used for the command.")]
    [DefaultValue("")]
    [NotifyParentProperty (true)]
    public virtual string TypeName
    {
      get
      {
        return _typeName; 
      }
      set
      {
        _typeName = StringUtility.NullToEmpty (value); 
        _typeName = _typeName.Trim();
      }
    }

    /// <summary> 
    ///   Gets or sets the comma separated list of parameters passed to the WxeFunction when the rendered 
    ///   command is clicked.
    /// </summary>
    /// <value> 
    ///   The comma separated list of parameters passed to the WxeFunction when the rendered 
    ///   command is clicked. The default value is <see cref="String.Empty"/>. 
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("A comma separated list of parameters for the command.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public virtual string Parameters
    {
      get
      {
        return _parameters;
      }
      set
      {
        _parameters = StringUtility.NullToEmpty (value);
        _parameters = _parameters.Trim();
      }
    }

    /// <summary> 
    ///   Gets or sets the target window or frame to open the Wxe Function when the rendered command is clicked.
    /// </summary>
    /// <value> 
    ///   The target window or frame to open the Wxe Function when the rendered command is clicked. 
    ///   The default value is <see cref="String.Empty"/>. 
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behaviour")]
    [Description ("The target window or frame of the command. Leave it blank for no target.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string Target
    {
      get { return _target; }
      set { _target = value; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether a hyperlink will be created that executes the function as an
    ///   extneral function.
    /// </summary>
    /// <value> A boolean value. The default value is <see langword="true"/>. </value>
    /// <remarks>
    ///   If <see langword="true"/>, the function's required parameters must be provided via the 
    ///   <see cref="Parameters"/> string.
    /// </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behaviour")]
    [Description ("If set, the function is executed on the client-side as an external function without requiring a round-trip to the server.")]
    [DefaultValue (false)]
    [NotifyParentProperty (true)]
    public virtual bool CreatePermanentUrlForExternalFunction
    {
      get { return _createPermanentUrlForExternalFunction; }
      set { _createPermanentUrlForExternalFunction = value; }
    }
  }

  private string _toolTip;
  private CommandType _type;
  private CommandType _defaultType = CommandType.None;
  private CommandShow _show = CommandShow.Always;
  private HrefCommandInfo _hrefCommand = new HrefCommandInfo();
  private bool _hasClickFired = false;

  /// <summary>
  ///   The <see cref="WxeFunctionCommandInfo"/> used when rendering the command as a <see cref="WxeFunction"/>.
  /// </summary>
  private WxeFunctionCommandInfo _wxeFunctionCommand = new WxeFunctionCommandInfo();

  //private ScriptCommandInfo _scriptCommand = null;

  private Control _ownerControl = null;
  
  [Browsable (false)]
  public CommandClickEventHandler Click;

  public Command ()
    : this (CommandType.None)
  {
  }

  public Command (CommandType defaultType)
  {
    _defaultType = defaultType;
    _type = _defaultType;
  }

  /// <summary> Fires the <see cref="Click"/> event. </summary>
  public virtual void OnClick ()
  {
    if (_hasClickFired)
      return;
    _hasClickFired = true;
    if (Click != null)
      Click (OwnerControl, new CommandClickEventArgs (this));
  }

  /// <summary> Renders the opening tag for the command. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
  /// <param name="postBackEvent">
  ///   The string executed upon the click on a command of types
  ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
  ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
  ///   to force a post back.
  /// </param>
  /// <param name="onClick"> 
  ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
  /// </param>
  /// <param name="parameters">
  ///   The strings inserted into the href attribute using <c>string.Format</c>.
  /// </param>
  public virtual void RenderBegin (
      HtmlTextWriter writer, 
      string postBackEvent,
      string onClick,
      params string[] parameters)
  {
    switch (_type)
    {
      case CommandType.Href:
      {
        AddAttributesToRenderForHrefCommand (writer, parameters, onClick);
        break;
      }
      case CommandType.Event:
      {
        AddAttributesToRenderForEventCommand (writer, postBackEvent, onClick);
        break;
      }
      case CommandType.WxeFunction:
      {
        AddAttributesToRenderForWxeFunctionCommand (writer, postBackEvent, onClick);
        break;
      }
      case CommandType.None:
      {
        break;
      }
    }
    writer.RenderBeginTag (HtmlTextWriterTag.A);
  }

  /// <summary> Adds the attributes for the Href command to the anchor tag. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
  /// <param name="parameters">
  ///   The strings inserted into the href attribute using <c>string.Format</c>.
  /// </param>
  /// <param name="onClick"> 
  ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.Href"/>.
  /// </exception> 
  protected virtual void AddAttributesToRenderForHrefCommand (
      HtmlTextWriter writer,
      string[] parameters, 
      string onClick)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("parameters", parameters);  
    if (Type != CommandType.Href)
      throw new InvalidOperationException ("Call to AddAttributesToRenderForHrefCommand not allowed unless Type is set to CommandType.Href.");
      
    string href = HrefCommand.FormatHref (parameters);
    writer.AddAttribute (HtmlTextWriterAttribute.Href, href);
    if (! StringUtility.IsNullOrEmpty (HrefCommand.Target))
      writer.AddAttribute (HtmlTextWriterAttribute.Target, HrefCommand.Target);
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClick);
    if (! StringUtility.IsNullOrEmpty (_toolTip))
      writer.AddAttribute (HtmlTextWriterAttribute.Title, _toolTip);
  }

  /// <summary> Adds the attributes for the Event command to the anchor tag. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
  /// <param name="postBackEvent">
  ///   The string executed upon the click on a command of types
  ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
  ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
  ///   to force a post back. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="onClick"> 
  ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.Event"/>.
  /// </exception> 
  protected virtual void AddAttributesToRenderForEventCommand (
      HtmlTextWriter writer, 
      string postBackEvent,
      string onClick)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("postBackEvent", postBackEvent);     
    if (Type != CommandType.Event)
      throw new InvalidOperationException ("Call to AddAttributesToRenderForEventCommand not allowed unless Type is set to CommandType.Event.");
   
    writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent + StringUtility.NullToEmpty (onClick));
    if (! StringUtility.IsNullOrEmpty (_toolTip))
      writer.AddAttribute (HtmlTextWriterAttribute.Title, _toolTip);
  }

  /// <summary> Adds the attributes for the Wxe Function command to the anchor tag. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. Must not be <see langword="null"/>. </param>
  /// <param name="postBackEvent">
  ///   The string executed upon the click on a command of types
  ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
  ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
  ///   to force a post back. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="onClick"> 
  ///   The string always rendered in the <c>onClick</c> tag of the anchor element. 
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  /// </exception> 
  protected virtual void AddAttributesToRenderForWxeFunctionCommand (
      HtmlTextWriter writer, 
      string postBackEvent,
      string onClick)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    ArgumentUtility.CheckNotNull ("postBackEvent", postBackEvent); 
    if (Type != CommandType.WxeFunction)
      throw new InvalidOperationException ("Call to AddAttributesToRenderForWxeFunctionCommand not allowed unless Type is set to CommandType.WxeFunction.");
       
    if (WxeFunctionCommand.CreatePermanentUrlForExternalFunction)
    {
      string href = GetWxeFunctionPermanentUrl();
      writer.AddAttribute (HtmlTextWriterAttribute.Href, href);
      if (! StringUtility.IsNullOrEmpty (WxeFunctionCommand.Target))
        writer.AddAttribute (HtmlTextWriterAttribute.Target, WxeFunctionCommand.Target);
      if (! StringUtility.IsNullOrEmpty (onClick))
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClick);
    }
    else
    {
      writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent + StringUtility.NullToEmpty (onClick));
    }
    if (! StringUtility.IsNullOrEmpty (_toolTip))
      writer.AddAttribute (HtmlTextWriterAttribute.Title, _toolTip);
  }

  /// <summary> Renders the closing tag for the command. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. </param>
  public virtual void RenderEnd (HtmlTextWriter writer)
  {
    ArgumentUtility.CheckNotNull ("writer", writer);
    writer.RenderEndTag();
  }

  /// <summary>
  ///   Returns a string representation of this <see cref="Command"/>.
  /// </summary>
  /// <remarks>
  ///   <list type="table">
  ///     <listheader>
  ///     <term>Type</term> 
  ///     <description>Format</description>
  ///     </listheader>
  ///     <item>
  ///       <term>Href</term>
  ///       <description> Href: &lt;HrefCommand.ToString()&gt; </description>
  ///     </item>
  ///     <item>
  ///       <term>WxeFunction</term>
  ///       <description> WxeFunction: &lt;WxeFunctionCommand.ToString()&gt; </description>
  ///     </item>
  ///   </list>
  /// </remarks>
  /// <returns> A <see cref="string"/>. </returns>
  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder (50);

    stringBuilder.Append (Type.ToString());

    switch (Type)
    {
      case CommandType.Href:
      {
        if (HrefCommand != null)
          stringBuilder.AppendFormat (": {0}", HrefCommand.ToString());
        break;
      }
      case CommandType.WxeFunction:
      {
        if (WxeFunctionCommand != null)
          stringBuilder.AppendFormat (": {0}", WxeFunctionCommand.ToString());
        break;
      }
      default:
      {
        break;
      }
    }

    return stringBuilder.ToString();
  }

  /// <summary> Executes the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommandInfo"/>. </summary>
  /// <param name="wxePage"> 
  ///   The <see cref="IWxePage"/> where this command is rendered on. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="additionalParameters"> 
  ///   The parameters passed to the <see cref="WxeFunction"/> in addition to the executing function's variables.
  ///   Use <see langword="null"/> or an empty collection if all parameters are supplied by the 
  ///   <see cref="WxeFunctionCommandInfo.Parameters"/> string and the function stack.
  /// </param>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  /// </exception> 
  public virtual void ExecuteWxeFunction (IWxePage wxePage, NameObjectCollection additionalParameters)
  {
    ArgumentUtility.CheckNotNull ("wxePage", wxePage);

    if (Type != CommandType.WxeFunction)
      throw new InvalidOperationException ("Call to ExecuteWxeFunction not allowed unless Type is set to CommandType.WxeFunction.");

    if (! wxePage.IsReturningPostBack)
    {
      string target = WxeFunctionCommand.Target;
      bool hasTarget = ! StringUtility.IsNullOrEmpty (target);
      Type functionType = TypeUtility.GetType (WxeFunctionCommand.TypeName, true, false);
      WxeFunction function = (WxeFunction) Activator.CreateInstance (functionType);

      function.InitializeParameters (WxeFunctionCommand.Parameters, additionalParameters);

      if (hasTarget)
        wxePage.ExecuteFunctionExternal (function, target, null, false);
      else
        wxePage.ExecuteFunction (function);
    }
  }

  /// <summary> 
  ///   Gets the permanent URL for the <see cref="WxeFunction"/> defined by the <see cref="WxeFunctionCommandInfo"/>. 
  /// </summary>
  /// <exception cref="InvalidOperationException">
  ///   If called while the <see cref="Type"/> is not set to <see cref="CommandType.WxeFunction"/>.
  /// </exception> 
  public virtual string GetWxeFunctionPermanentUrl()
  {
    if (Type != CommandType.WxeFunction)
      throw new InvalidOperationException ("Call to ExecuteWxeFunction not allowed unless Type is set to CommandType.WxeFunction.");

    Type functionType = TypeUtility.GetType (WxeFunctionCommand.TypeName, true, false);
    WxeParameterDeclaration[] parameterDeclarations = WxeFunction.GetParamaterDeclarations (functionType);
    object[] parameterValues = WxeFunction.ParseActualParameters (
        parameterDeclarations, WxeFunctionCommand.Parameters, System.Globalization.CultureInfo.InvariantCulture);
    NameValueCollection queryString = 
        WxeFunction.SerializeParametersForQueryString (parameterDeclarations, parameterValues);
    return WxeContext.GetPermanentUrl (HttpContext.Current, functionType, queryString);
  }

  /// <summary> The <see cref="CommandType"/> represented by this instance of <see cref="Command"/>. </summary>
  /// <value> One of the <see cref="CommandType"/> enumeration values. The default is <see cref="CommandType.None"/>. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The type of command generated.")]
  [NotifyParentProperty (true)]
  public CommandType Type
  {
    get { return _type; }
    set { _type = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The ToolTip/Title rendered in the anchor tag.")]
  [NotifyParentProperty (true)]
  public string ToolTip
  {
    get { return _toolTip; }
    set { _toolTip = value; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public bool IsDefaultType
  {
    get { return _type == _defaultType; }
  }

  /// <summary> Controls the persisting of the <see cref="Type"/>. </summary>
  protected bool ShouldSerializeType()
  {
    return ! IsDefaultType;
  }

  /// <summary> Sets the <see cref="Type"/> to its default value. </summary>
  protected void ResetType()
  {
    _type = _defaultType;
  }

  /// <summary>
  ///   Determines when the item command is shown to the user in regard of the parent control's read-only setting.
  /// </summary>
  /// <value> 
  ///   One of the <see cref="CommandShow"/> enumeration values. The default is <see cref="CommandShow.Always"/>.
  /// </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("Determines when to show the item command to the user in regard to the parent control's read-only setting.")]
  [DefaultValue (CommandShow.Always)]
  [NotifyParentProperty (true)]
  public CommandShow Show
  {
    get { return _show; }
    set { _show = value; }
  }

  /// <summary>
  ///   The <see cref="HrefCommandInfo"/> used when rendering the command as a hyperlink.
  /// </summary>
  /// <remarks> 
  ///   Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.Href"/>.
  /// </remarks>
  /// <value> A <see cref="HrefCommandInfo"/> object. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The properties of the hyperlink. Interpreted if Type is set to Href.")]
  [DefaultValue ((string)null)]
  [NotifyParentProperty (true)]
  public virtual HrefCommandInfo HrefCommand
  {
    get { return _hrefCommand; }
    set { _hrefCommand = value; }
  }

  /// <summary>
  ///   The <see cref="WxeFunctionCommandInfo"/> used when rendering the command as a <see cref="WxeFunction"/>.
  /// </summary>
  /// <remarks> 
  ///   Only interpreted if <see cref="Type"/> is set to <see cref="CommandType.WxeFunction"/>.
  /// </remarks>
  /// <value> A <see cref="WxeFunctionCommandInfo"/> object. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The properties of the WxeFunction. Interpreted if Type is set to WxeFunction.")]
  [DefaultValue ((string)null)]
  [NotifyParentProperty (true)]
  public virtual WxeFunctionCommandInfo WxeFunctionCommand
  {
    get { return _wxeFunctionCommand; }
    set { _wxeFunctionCommand = value; }
  }

  /// <summary> Gets or sets the control to which this object belongs. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public Control OwnerControl
  {
    get { return OwnerControlImplementation;  }
    set { OwnerControlImplementation = value; }
  }

  protected virtual Control OwnerControlImplementation
  {
    get { return _ownerControl;  }
    set
    { 
      if (_ownerControl != value)
        _ownerControl = value;
    }
  }

  string IControlItem.ItemID
  {
    get { return null; }
  }

  public virtual void LoadResources (IResourceManager resourceManager)
  {
    if (resourceManager == null)
      return;

    string key = ResourceManagerUtility.GetGlobalResourceKey (ToolTip);
    if (! StringUtility.IsNullOrEmpty (key))
      ToolTip = resourceManager.GetString (key);
  }
}

/// <summary> The possible command types of a <see cref="Command"/>. </summary>
public enum CommandType
{
  /// <summary> No command will be generated. </summary>
  None,
  /// <summary> A server side event will be raised upon a command click. </summary>
  Event,
  /// <summary> A hyperlink will be rendered on the page. </summary>
  Href,
  /// <summary> A <see cref="WxeFunction"/> will be called upon a command click. </summary>
  WxeFunction
}

/// <summary> Defines when the command will be active on the page. </summary>
public enum CommandShow
{
  /// <summary> The command is always active. </summary>
  Always,
  /// <summary> The command is only active if the containing element is read-only. </summary>
  ReadOnly,
  /// <summary> The command is only active if the containing element is in edit-mode. </summary>
  EditMode
}

/// <summary>
///   Represents the method that handles the <see cref="Command.Click"/> event
///   raised when clicking on a <see cref="Command"/> of type <see cref="CommandType.Event"/>.
/// </summary>
public delegate void CommandClickEventHandler (object sender, CommandClickEventArgs e);

/// <summary> Provides data for the <see cref="Command.Click"/> event. </summary>
public class CommandClickEventArgs: EventArgs
{
  private Command _command;

  /// <summary> Initializes a new instance. </summary>
  public CommandClickEventArgs (Command command)
  {
    ArgumentUtility.CheckNotNull ("command", command);
    _command = command;
  }

  /// <summary> The <see cref="Command"/> that caused the event. </summary>
  public Command Command
  {
    get { return _command; }
  }
}

}

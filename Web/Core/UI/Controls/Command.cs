using System;
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

namespace Rubicon.Web.UI.Controls
{

//  TODO: Command: Move long comment blocks to xml-file
/// <summary> An <see cref="Command"/> defines an action the user can invoke. </summary>
[TypeConverter (typeof (ExpandableObjectConverter))]
public class Command
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
        encodedParameters[i] = HttpUtility.UrlEncode (parameters[i], HttpContext.Current.Response.ContentEncoding);
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
    ///   Gets or sets the target window or frame to display the Web page content linked to when 
    ///   the rendered command is clicked.
    /// </summary>
    /// <value> 
    ///   The target window or frame to load the Web page linked to when the rendered command
    ///   is clicked.  The default value is <see cref="String.Empty"/>. 
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The target frame of the command. Leave it blank for no target.")]
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
  }

  /// <summary> Wraps the properties required for rendering a script command. </summary>
  public class ScriptCommandInfo
  {
    //  Add enum value Script to BocCommandType
    //  Add property ScriptCommand to BocCommand
  }

  /// <summary>
  ///   The <see cref="CommandType"/> represented by this instance of <see cref="Command"/>.
  /// </summary>
  private CommandType _type = CommandType.None;
  /// <summary>
  ///   Determines when the item command is shown to the user in regard of the parent control's read-only setting.
  /// </summary>
  private CommandShow _show = CommandShow.Always;
  /// <summary>
  ///   The <see cref="HrefCommandInfo"/> used when rendering the command as a hyperlink.
  /// </summary>
  private HrefCommandInfo _hrefCommand = new HrefCommandInfo();
  /// <summary>
  ///   The <see cref="WxeFunctionCommandInfo"/> used when rendering the command as a <see cref="WxeFunction"/>.
  /// </summary>
  private WxeFunctionCommandInfo _wxeFunctionCommand = new WxeFunctionCommandInfo();

  //private ScriptCommandInfo _scriptCommand = null;

  private IControl _ownerControl = null;

  /// <summary> Renders the opening tag for the command. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. </param>
  /// <param name="postBackLink">
  ///   The string rendered in the <c>href</c> tag of the anchor element when the command type is
  ///   <see cref="CommandType.Event"/> or <see cref="CommandType.WxeFunction"/>.
  ///   This string is usually the call to the <c>__doPostBack</c> script function used by ASP.net
  ///   to force a post back.
  /// </param>
  /// <param name="onClick"> 
  ///   The string rendered in the <c>onClick</c> tag of the anchor element. 
  /// </param>
  /// <param name="parameters">
  ///   The strings inserted into the href attribute using <c>string.Formar</c>.
  /// </param>
  public virtual void RenderBegin (
      HtmlTextWriter writer, 
      string postBackLink,
      string onClick,
      params string[] parameters)
  {
    switch (_type)
    {
      case CommandType.Href:
      {
        ArgumentUtility.CheckNotNull ("parameters", postBackLink);        
        writer.AddAttribute (HtmlTextWriterAttribute.Href, HrefCommand.FormatHref (parameters));
        if (HrefCommand.Target != null) 
          writer.AddAttribute (HtmlTextWriterAttribute.Target, HrefCommand.Target);
        break;
      }
      case CommandType.Event:
      {
        ArgumentUtility.CheckNotNull ("postBackLink", postBackLink);        
        writer.AddAttribute (HtmlTextWriterAttribute.Href, postBackLink);
        break;
      }
      case CommandType.WxeFunction:
      {
        ArgumentUtility.CheckNotNull ("postBackLink", postBackLink);        
        writer.AddAttribute (HtmlTextWriterAttribute.Href, postBackLink);

        break;
      }
      default:
      {
        break;
      }
    }
    writer.AddAttribute (HtmlTextWriterAttribute.Onclick, onClick);
    writer.RenderBeginTag (HtmlTextWriterTag.A);
  }

  /// <summary> Renders the closing tag for the command. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. </param>
  public virtual void RenderEnd (HtmlTextWriter writer)
  {
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

  /// <summary>
  ///   Executes the <see cref="WxeFunction"/> defined by the 
  ///   <see cref="WxeFunctionCommandInfo"/>.
  /// </summary>
  /// <param name="wxePage"> The <see cref="IWxePage"/> where this command is rendered on. Must not be null</param>
  /// <param name="parameters"> The parameters passed to the <see cref="WxeFunction"/>. </param>
  public virtual void ExecuteWxeFunction (IWxePage wxePage, NameObjectCollection parameters)
  {
    ArgumentUtility.CheckNotNull ("wxePage", wxePage);

    if (Type != CommandType.WxeFunction)
      throw new InvalidOperationException ("Call to ExecuteWxeFunction not allowed unless Type is set to CommandType.WxeFunction.");

    if (! WxeContext.Current.IsReturningPostBack)
    {
      if (parameters != null)
      {
        foreach (string key in parameters.Keys)
          wxePage.CurrentFunction.Variables[key] = parameters[key];
      }

      Type functionType = TypeUtility.GetType (WxeFunctionCommand.TypeName, true, false);

      object [] actualParameters = WxeParameterDeclaration.ParseActualParameters (
          functionType,
          WxeFunctionCommand.Parameters,
          null);

      WxeFunction function = (WxeFunction) Activator.CreateInstance (functionType, actualParameters);
      
      wxePage.CurrentStep.ExecuteFunction (wxePage, function);
    }
  }

  /// <summary>
  ///   The <see cref="CommandType"/> represented by this instance of 
  ///   <see cref="Command"/>.
  /// </summary>
  /// <value> 
  ///   One of the <see cref="CommandType"/> enumeration values. 
  ///   The default is <see cref="CommandType.None"/>.
  /// </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The type of command generated.")]
  [DefaultValue (CommandType.None)]
  [NotifyParentProperty (true)]
  public CommandType Type
  {
    get
    {
      return _type; 
    }
    set 
    {
      _type = value; 
    }
  }

  /// <summary>
  ///   Determines when the item command is shown to the user in regard of the parent control's 
  ///   read-only setting.
  /// </summary>
  /// <value> 
  ///   One of the <see cref="CommandShow"/> enumeration values. 
  ///   The default is <see cref="CommandShow.Always"/>.
  /// </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("Determines when to show the item command to the user in regard to the parent controls read-only setting.")]
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
    get
    {
      return _hrefCommand; 
    }
    set
    { 
      _hrefCommand = value;
    }
  }

  /// <summary>
  ///   The <see cref="WxeFunctionCommandInfo"/> used when rendering the command as a 
  ///   <see cref="WxeFunction"/>.
  /// </summary>
  /// <remarks> 
  ///   Only interpreted if <see cref="Type"/> is set to 
  ///   <see cref="CommandType.WxeFunction"/>.
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
    get 
    {
      return _wxeFunctionCommand; 
    }
    set 
    {
      _wxeFunctionCommand = value; 
    }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IControl"/> to which this object belongs. 
  /// </summary>
  protected internal IControl OwnerControl
  {
    get { return OwnerControlImplementation;  }
    set { OwnerControlImplementation = value; }
  }

  protected virtual IControl OwnerControlImplementation
  {
    get { return _ownerControl;  }
    set
    { 
      if (_ownerControl != value)
        _ownerControl = value;
    }
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

}

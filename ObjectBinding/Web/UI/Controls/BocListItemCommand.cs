using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.ObjectBinding.Web.Controls
{

//  TODO: BocItemCommand: Move long comment blocks to xml-file
/// <summary> A <see cref="BocItemCommand"/> defines an action the user can invoke on a datarow. </summary>
[TypeConverter (typeof (ExpandableObjectConverter))]
public class BocItemCommand
{
  /// <summary> Wraps the properties required for rendering a hyperlink. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class HrefCommandInfo
  {
    private string _href = string.Empty;
    private string _target = string.Empty;

    /// <summary> Simple constructor. </summary>
    public HrefCommandInfo()
    {}

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

    /// <summary> Gets or sets the URL to link to when the rendered command is clicked. </summary>
    /// <value> 
    ///   The URL to link to when the rendered command is clicked. The default value is 
    ///   <see cref="String.Empty"/>. 
    /// </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The hyperlink reference of the command. Use {0} to insert the Busines Object's index in the list and {1} to insert the Business Object's ID.")]
    [DefaultValue("")]
    [NotifyParentProperty (true)]
    public string Href 
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
    public string Target 
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
    {}

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
    public string TypeName
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
    public string Parameters
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
    //  TODO: BocItemCommand: Script
    //  Add enum value Script to BocItemCommandType
    //  Add property ScriptCommand to BocItemCommand
  }

  /// <summary>
  ///   The <see cref="BocItemCommandType"/> represented by this instance of 
  ///   <see cref="BocItemCommand"/>.
  /// </summary>
  private BocItemCommandType _type = BocItemCommandType.None;
  /// <summary>
  ///   Determines when the item command is shown to the user in regard of the parent control's 
  ///   read-only setting.
  /// </summary>
  private BocItemCommandShow _show = BocItemCommandShow.Always;
  /// <summary>
  ///   The <see cref="HrefCommandInfo"/> used when rendering the command as a hyperlink.
  /// </summary>
  private HrefCommandInfo _hrefCommand = new HrefCommandInfo();
  /// <summary>
  ///   The <see cref="WxeFunctionCommandInfo"/> used when rendering the command as a 
  ///   <see cref="WxeFunction"/>.
  /// </summary>
  private WxeFunctionCommandInfo _wxeFunctionCommand = new WxeFunctionCommandInfo();
  //private ScriptCommandInfo _scriptCommand = null;

  /// <summary> Simple Constructor. </summary>
  public BocItemCommand()
  {}

  /// <summary> Renders the opening tag for the command. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. </param>
  /// <param name="listIndex">
  ///   An index that indentifies the <see cref="IBusinessObject"/> on which the rendered command 
  ///   is applied on.
  /// </param>
  /// <param name="businessObjectID">
  ///   An identifier for the <see cref="IBusinessObject"/> to which the rendered command is 
  ///   applied.
  /// </param>
  /// <param name="postBackLink">
  ///   The string rendered in the "href" tag of the anchor element when the command type is
  ///   <see cref="BocItemCommandType.Event"/> or <see cref="BocItemCommandType.WxeFunction"/>.
  ///   This string is usually the call to the "__doPostBack" script function used by ASP.net
  ///   to force a post back.
  /// </param>
  public virtual void RenderBegin (
      HtmlTextWriter writer, 
      int listIndex, 
      string businessObjectID,
      string postBackLink)
  {
    switch (_type)
    {
      case BocItemCommandType.Href:
      {
        string encodedBusinessObjectID = 
            HttpUtility.UrlEncode (businessObjectID, HttpContext.Current.Response.ContentEncoding);
        string href = string.Format (HrefCommand.Href, listIndex, encodedBusinessObjectID);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, href);
        if (HrefCommand.Target != null) 
          writer.AddAttribute (HtmlTextWriterAttribute.Target, HrefCommand.Target);
        break;
      }
      case BocItemCommandType.Event:
      {
        ArgumentUtility.CheckNotNull ("postBackLink", postBackLink);        
        writer.AddAttribute (HtmlTextWriterAttribute.Href, postBackLink);
        break;
      }
      case BocItemCommandType.WxeFunction:
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
    writer.RenderBeginTag (HtmlTextWriterTag.A);
  }

  /// <summary> Renders the closing tag for the command. </summary>
  /// <param name="writer"> The <see cref="HtmlTextWriter"/> object to use. </param>
  public virtual void RenderEnd (HtmlTextWriter writer)
  {
    writer.RenderEndTag();
  }

  /// <summary>
  ///   Returns a string representation of this <see cref="BocItemCommand"/>.
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
      case BocItemCommandType.Href:
      {
        if (HrefCommand != null)
          stringBuilder.AppendFormat (": {0}", HrefCommand.ToString());
        break;
      }
      case BocItemCommandType.WxeFunction:
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
  /// <param name="wxePage"> The <see cref="IWxePage"/> where this command is rendered on. </param>
  /// <param name="listIndex"> 
  ///   The index of the <see cref="IBusinessObject"/> in the row where the command was clicked.
  /// </param>
  /// <param name="businessObjectID"> 
  ///   The <see cref="IBusinessObjectWithIdentity.UniqueIdentifier"/>, if the 
  ///   <see cref="IBusinessObject"/> in the row where the command was clicked 
  ///   is an <see cref="IBusinessObjectWithIdentity"/>.
  /// </param>
  public void ExecuteWxeFunction (IWxePage wxePage, int listIndex, string businessObjectID)
  {
    ArgumentUtility.CheckNotNull ("wxePage", wxePage);

    if (Type != BocItemCommandType.WxeFunction)
      throw new InvalidOperationException ("Call to ExecuteWxeFunction not allowed unless Type is set to BocItemCommandType.WxeFunction.");

    if (! WxeContext.Current.IsReturningPostBack)
    {
      Type functionType = System.Type.GetType (WxeFunctionCommand.TypeName); 

      throw new NotImplementedException ("WxeFunctionParameterDeclaration parsing logic not completed.");

//      WxeParameterDeclaration[] parameterDeclarations = null;
//      object [] parameters = WxeParameterDeclaration.ParseActualParameters (
//          parameterDeclarations,
//          WxeFunctionCommand.Parameters,
//          null);
//
//      WxeFunction function = (WxeFunction) Activator.CreateInstance (functionType, parameters);
//      
//      wxePage.CurrentStep.ExecuteFunction (wxePage, function);
    }
  }

  /// <summary>
  ///   The <see cref="BocItemCommandType"/> represented by this instance of 
  ///   <see cref="BocItemCommand"/>.
  /// </summary>
  /// <value> 
  ///   One of the <see cref="BocItemCommandType"/> enumeration values. 
  ///   The default is <see cref="BocItemCommandType.None"/>.
  /// </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The type of command generated.")]
  [DefaultValue (BocItemCommandType.None)]
  [NotifyParentProperty (true)]
  public BocItemCommandType Type
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
  ///   One of the <see cref="BocItemCommandShow"/> enumeration values. 
  ///   The default is <see cref="BocItemCommandShow.Always"/>.
  /// </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("Determines when to show the item command to the user in regard to the parent controls read-only setting.")]
  [DefaultValue (BocItemCommandShow.Always)]
  [NotifyParentProperty (true)]
  public BocItemCommandShow Show
  {
    get { return _show; }
    set { _show = value; }
  }

  /// <summary>
  ///   The <see cref="HrefCommandInfo"/> used when rendering the command as a hyperlink.
  /// </summary>
  /// <remarks> 
  ///   Only interpreted if <see cref="Type"/> is set to <see cref="BocItemCommandType.Href"/>.
  /// </remarks>
  /// <value> A <see cref="HrefCommandInfo"/> object. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The properties of the hyperlink. Interpreted if Type is set to Href.")]
  [DefaultValue ((string)null)]
  [NotifyParentProperty (true)]
  public HrefCommandInfo HrefCommand
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
  ///   <see cref="BocItemCommandType.WxeFunction"/>.
  /// </remarks>
  /// <value> A <see cref="WxeFunctionCommandInfo"/> object. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The properties of the WxeFunction. Interpreted if Type is set to WxeFunction.")]
  [DefaultValue ((string)null)]
  [NotifyParentProperty (true)]
  public WxeFunctionCommandInfo WxeFunctionCommand
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
}

/// <summary> The possible command types of a <see cref="BocItemCommand"/>. </summary>
public enum BocItemCommandType
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
public enum BocItemCommandShow
{
  /// <summary> The command is always active. </summary>
  Always,
  /// <summary> The command is only active if the <see cref="BocList"/> is read-only. </summary>
  ReadOnly,
  /// <summary> The command is only active if the <see cref="BocList"/> is in edit-mode. </summary>
  EditMode
}

}

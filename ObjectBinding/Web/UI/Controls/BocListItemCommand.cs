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

/// <summary>
///   A <see cref="BocItemCommand"/> defines an action the user can invoke on a datarow.
/// </summary>
//  TODO: BocItemCommand: Event
//  TODO: BocItemCommand: Script
//  TODO: Place Commands into Expandable SubTypes
//  TODO: WireUp Event
//[TypeConverter (typeof (BocItemCommandConverter))]
public class BocItemCommand
{
  public class HrefCommandParameters
  {
    private string _href;
    private string _target;

    /// <summary> The hyperlink reference; used for <see cref="BocItemCommandType.Href"/>. </summary>
    /// <value> A <see cref="string"/> representing the hyperlink reference. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Type: Href")]
    [Description ("The hyperlink reference of the command. Use {0} for the index and {1} for the ID.")]
    [DefaultValue("")]
    [NotifyParentProperty (true)]
    public string Href 
    {
      get
      {
        return StringUtility.NullToEmpty (_href); 
      }
      set
      {
        _href = value; 
      }
    }

    /// <summary> The hyperlink target; used for <see cref="BocItemCommandType.Href"/>. </summary>
    /// <value> A <see cref="string"/> representing the hyperlink target. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Type: Href")]
    [Description ("The target frame of the command. Leave it blank for no target.")]
    [DefaultValue("")]
    [NotifyParentProperty (true)]
    public string Target 
    { 
      get
      { 
        return StringUtility.NullToEmpty (_target); 
      }
      set
      {
        _target = value; 
      }
    }
  }

  public class WxeFunctionCommandParameters
  {
    private string _typeName;
    private string[] _parameters;

    public WxeFunctionCommandParameters()
    {
      ParameterList = string.Empty;
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Type: WxeFunction")]
    [Description ("The type name of the WxeFunction used for this command.")]
    [DefaultValue("")]
    [NotifyParentProperty (true)]
    public string TypeName
    {
      get
      {
        return StringUtility.NullToEmpty (_typeName); 
      }
      set 
      {
        _typeName = value; 
      }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Category ("Type: WxeFunction")]
    [Description ("A comma seperated list of parameters for this command.")]
    [DefaultValue((string) null)]
    [NotifyParentProperty (true)]
    public string[] Parameters
    {
      get
      {
        if (_parameters == null)
            return new string[0];
        return _parameters; 
      }
      set { _parameters = value; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [DefaultValue ("")]
    [Browsable (false)]
    public string ParameterList
    {
      get
      {
        if (Parameters == null)
          return "";
        return StringUtility.ConcatWithSeperator (Parameters, ", ");
      }
      set
      {
        value = StringUtility.NullToEmpty (value);
        value = value.Trim();
        if (value.Length == 0)
        {
          _parameters = new string[0];
        }
        else
        {
          string[] paramters = value.Split (',');   
          _parameters = new string[paramters.Length];
          for (int i = 0; i < paramters.Length; i++)
          {
            paramters[i] = paramters[i].Trim();
            if (paramters[i].Length == 0)
              throw new ArgumentEmptyException ("Parameter name missing for parameter No. " + i + " in ParameterList + '" + value + "'.");
            _parameters[i] = paramters[i];
          }
        }
      }
    }
  }

  public class EventCommandParameters
  {
  }

  public class ScriptCommandParameters
  {
  }

  private BocItemCommandType _type = BocItemCommandType.Href;
  private BocItemCommandShow _show = BocItemCommandShow.Always;

  private HrefCommandParameters _hrefCommand = new HrefCommandParameters();
  private WxeFunctionCommandParameters _wxeFunctionCommand = new WxeFunctionCommandParameters();
  private EventCommandParameters _eventCommand = new EventCommandParameters();
  private ScriptCommandParameters _scriptCommand = new ScriptCommandParameters();

  /// <summary> Simple Constructor. </summary>
  public BocItemCommand()
  {
  }

  /// <summary>
  ///   Instantiates a <see cref="BocItemCommand"/> of type <see cref="BocItemCommandType.Href"/>.
  /// </summary>
  /// <param name="href"> 
  ///   The hyperlink reference of the command. Use {0} for the index and {1} for the ID.
  /// </param>
  /// <param name="target"> The targetframe of the <see cref="BocItemCommand"/>. </param>
  /// <returns> A <see cref="BocItemCommand"/>. </returns>
  public static BocItemCommand CreateHrefItemCommand (string href, string target)
  {
    BocItemCommand command = new BocItemCommand();
    command.Type = BocItemCommandType.Href;
    command.HrefCommand.Href = href;
    command.HrefCommand.Target = target;
    return command;
  }

  /// <summary>
  ///   Instantiates a <see cref="BocItemCommand"/> of type <see cref="BocItemCommandType.Href"/>.
  /// </summary>
  /// <param name="href"> 
  ///   The hyperlink reference of the command. Use {0} for the index and {1} for the ID.
  /// </param>
  /// <returns> A <see cref="BocItemCommand"/>. </returns>
  public static BocItemCommand CreateHrefItemCommand (string href)
  {
    return BocItemCommand.CreateHrefItemCommand (href, null);
  }

  /// <summary> Renders the opening tag for the command. </summary>
  /// <param name="writer"></param>
  /// <param name="listIndex">
  ///   An index that indtifies the <see cref="IBusinessObject"/> on which the rendered command is 
  ///   applied on.
  /// </param>
  /// <param name="businessObjectID">
  ///   An identifier for the <see cref="IBusinessObject"/> on which the rendered command is 
  ///   applied on.
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
        string href = string.Format (HrefCommand.Href, listIndex, businessObjectID);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, href);
        if (HrefCommand.Target != null) 
          writer.AddAttribute (HtmlTextWriterAttribute.Target, HrefCommand.Target);
        writer.RenderBeginTag (HtmlTextWriterTag.A);    
        break;
      }
      case BocItemCommandType.WxeFunction:
      {
        ArgumentUtility.CheckNotNull ("postBackLink", postBackLink);        
        writer.AddAttribute (HtmlTextWriterAttribute.Href, postBackLink);
        writer.RenderBeginTag (HtmlTextWriterTag.A);

        break;
      }
      default:
      {
        break;
      }
    }
  }

  /// <summary> Renders the closing tag for the command. </summary>
  /// <param name="writer"></param>
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
  ///       <description>
  ///         <para>Href: Href, Target</para>
  ///         <para>Href: Href</para>
  ///       </description>
  ///     </item>
  ///   </list>
  /// </remarks>
  /// <returns> A <see cref="string"/>. </returns>
//  public override string ToString()
//  {
//    StringBuilder stringBuilder = new StringBuilder (50);
//
//    stringBuilder.Append (_type.ToString());
//
//    switch (_type)
//    {
//      case BocItemCommandType.Href:
//      {
//        if (! StringUtility.IsNullOrEmpty (HrefCommand.Href))
//        {
//          stringBuilder.AppendFormat (": {0}", HrefCommand.Href);
//          if (! StringUtility.IsNullOrEmpty (HrefCommand.Target))
//            stringBuilder.AppendFormat (", {0}", HrefCommand.Target);
//        }
//        break;
//      }
//      case BocItemCommandType.WxeFunction:
//      {
//        if (! StringUtility.IsNullOrEmpty (WxeFunctionCommand.TypeName))
//        {
//          stringBuilder.AppendFormat (": {0}", WxeFunctionCommand.TypeName);
//          if (WxeFunctionCommand.Parameters != null)
//          {
//            stringBuilder.AppendFormat (
//              " ({0})", 
//              StringUtility.ConcatWithSeperator (WxeFunctionCommand.Parameters, ", "));
//          }
//        }
//        break;
//      }
//      default:
//      {
//        break;
//      }
//    }
//
//    return stringBuilder.ToString();
//  }

  public void ExecuteWxeFunction (IWxePage wxePage, int index, string id)
  {
    ArgumentUtility.CheckNotNull ("wxePage", wxePage);

    if (Type != BocItemCommandType.WxeFunction)
      throw new InvalidOperationException ("Call to ExecuteWxeFunction not allowed unless Type is set to BocItemCommandType.WxeFunction.");

    if (! WxeContext.Current.IsReturningPostBack)
    {
      Type functionType = System.Type.GetType (WxeFunctionCommand.TypeName); 

      object [] arguments = new object[WxeFunctionCommand.Parameters.Length];
      for (int i = 0; i < WxeFunctionCommand.Parameters.Length; i++)
      {
        //  TODO: BocItemCommand: Interpret BocItemCommand Parameters
        if (WxeFunctionCommand.Parameters[i] == "%ID")
          arguments[i] = id;
        else if (WxeFunctionCommand.Parameters[i] == "%Index")
          arguments[i] = index;
        else
          arguments[i] = WxeFunctionCommand.Parameters[i];
      }

      WxeFunction function = (WxeFunction) Activator.CreateInstance (functionType, arguments);
      
      wxePage.CurrentStep.ExecuteFunction (wxePage, function);
    }
  }

  /// <summary>
  ///   The <see cref="BocItemCommandType"/> represented by this instance of 
  ///   <see cref="BocItemCommand"/>.
  /// </summary>
  /// <value> 
  ///   One of the <see cref="BocItemCommandType"/> enumeration values. 
  /// </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The type of the command.")]
  //  No default value
  [NotifyParentProperty (true)]
  public BocItemCommandType Type
  {
    get { return _type; }
    set { _type = value; }
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
  [Description ("Determines when to show the item command to the user in regard to the parent controls read-only setting.")]
  [Category ("Behavior")]
  [DefaultValue (BocItemCommandShow.Always)]
  [NotifyParentProperty (true)]
  public BocItemCommandShow Show
  {
    get { return _show; }
    set { _show = value; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Editor (typeof (ExpandableObjectConverter), typeof (UITypeEditor))]
  [Description ("")]
  [Category ("Behavior")]
  [DefaultValue ((string)null)]
  [NotifyParentProperty (true)]
  public HrefCommandParameters HrefCommand
  {
    get
    {
      if (_type != BocItemCommandType.Href)
        return new HrefCommandParameters();
      return _hrefCommand; 
    }
    set
    { 
      _hrefCommand = value;
    }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Editor (typeof (ExpandableObjectConverter), typeof (UITypeEditor))]
  [Description ("")]
  [Category ("Behavior")]
  [DefaultValue ((string)null)]
  [NotifyParentProperty (true)]
  public WxeFunctionCommandParameters WxeFunctionCommand
  {
    get 
    {
      if (_type != BocItemCommandType.WxeFunction)
        return new WxeFunctionCommandParameters();
      return _wxeFunctionCommand; 
    }
    set 
    {
      _wxeFunctionCommand = value; 
    }
  }
}

// TODO: BocItemCommandType: Documentation
public enum BocItemCommandType
{
  Href,
  WxeFunction
}

// TODO: BocItemCommandShow: Documentation
public enum BocItemCommandShow
{
  Always,
  ReadOnly,
  EditMode
}


}

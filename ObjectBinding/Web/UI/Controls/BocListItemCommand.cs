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
//  TODO: BocItemCommand: WebExecutionEngine
[TypeConverter (typeof (BocItemCommandConverter))]
public class BocItemCommand: Control, IPostBackEventHandler
{
  private string _id;

  private BocItemCommandType _type = BocItemCommandType.Href;
  private BocItemCommandShow _show = BocItemCommandShow.Always;

  private string _href;
  private string _target;

  private string _functionAssemblyName;
  private string _functionTypeName;
  private string[] _functionParameters;

  public event BocItemCommandClickEventHandler Click;

  /// <summary> Simple Constructor. </summary>
  public BocItemCommand()
  {
    FunctionParameterList = " ";
    FunctionParameterList = "";
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
    command.Href = href;
    command.Target = target;
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
  /// <param name="index">
  ///   An index that indtifies the <see cref="IBusinessObject"/> on which the rendered command is 
  ///   applied on.
  /// </param>
  /// <param name="id">
  ///   An identifier for the <see cref="IBusinessObject"/> on which the rendered command is 
  ///   applied on.
  /// </param>
  public virtual void RenderBegin (HtmlTextWriter writer, int index, string id)
  {
    switch (_type)
    {
      case BocItemCommandType.Href:
      {
        string href = string.Format (Href, index, id);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, href);
        if (Target != null) 
          writer.AddAttribute (HtmlTextWriterAttribute.Target, Target);
        writer.RenderBeginTag (HtmlTextWriterTag.A);    
        break;
      }
      case BocItemCommandType.WxeFunction:
      {
        if (Page == null)
          throw new InvalidOperationException ("'" + typeof (BocItemCommand).FullName + "' can only be rendered when it is part of a '" + typeof (Page).FullName + "'.");
                
        string argument = index.ToString();
        if (! StringUtility.IsNullOrEmpty (id))
          argument += "," + id;
        
        writer.AddAttribute (
          HtmlTextWriterAttribute.Href, 
          Page.GetPostBackClientHyperlink (this, argument));
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
  public override string ToString()
  {
    StringBuilder stringBuilder = new StringBuilder (50);

    stringBuilder.Append (_type.ToString());

    switch (_type)
    {
      case BocItemCommandType.Href:
      {
        if (! StringUtility.IsNullOrEmpty (Href))
        {
          stringBuilder.AppendFormat (": {0}", Href);
          if (! StringUtility.IsNullOrEmpty (Target))
            stringBuilder.AppendFormat (", {0}", Target);
        }
        break;
      }
      case BocItemCommandType.WxeFunction:
      {
        if (! StringUtility.IsNullOrEmpty (FunctionTypeName))
        {
          stringBuilder.AppendFormat (": {0}, {1}", FunctionAssemblyName, FunctionTypeName);
          if (FunctionParameters != null)
          {
            stringBuilder.AppendFormat (
              " ({0})", 
              StringUtility.ConcatWithSeperator (FunctionParameters, ", "));
          }
        }
        break;
      }
      default:
      {
        break;
      }
    }

    return stringBuilder.ToString();
  }

  public void RaisePostBackEvent (string eventArgument)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("eventArgument", eventArgument);

    string[] eventArgumentParts = eventArgument.Split (new char[] {','}, 2);

    int index;
    string id = null;
    
    eventArgumentParts[0] = eventArgumentParts[0].Trim();
    if (eventArgumentParts[0].Length == 0)
      throw new ArgumentException ("First part of argument 'eventArgument' must be an integer. Expected format: 'Int32' or 'Int32,String'.");
    try 
    {
      index = int.Parse (eventArgumentParts[0]);
    }
    catch (FormatException)
    {
      throw new ArgumentException ("First part of argument 'eventArgument' must be an integer. Expected format: 'Int32' or 'Int32,String'.");
    }
    
    if (eventArgumentParts.Length > 1)
      id = eventArgumentParts[1].Trim();

    BocItemCommandClickEventArgs e = new BocItemCommandClickEventArgs (index, id);
    OnClick (e);
  }

  protected void OnClick (BocItemCommandClickEventArgs e)
  {
    if (Click != null)
      Click (this, e);
  }

  /// <summary> The ID of this command. </summary>
  /// <value> A <see cref="string"/> providing an identifier for this command. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The ID of this command.")]
  [Category ("Misc")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string ID
  {
    get { return _id; }
    set { _id = value; }
  }

  [Browsable (false)]
  public override bool Visible
  {
    get { return base.Visible; }
    set { base.Visible = value; }
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
      if (_type != BocItemCommandType.Href)
        return null;

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
      if (_type != BocItemCommandType.Href)
        return null;
      return StringUtility.NullToEmpty (_target); 
    }
    set
    {
      _target = value; 
    }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Type: WxeFunction")]
  [Description ("The assembly containing the WxeFunction used for this command.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string FunctionAssemblyName
  {
    get
    {
      if (_type != BocItemCommandType.WxeFunction)
        return null;
      return StringUtility.NullToEmpty (_functionAssemblyName); 
    }
    set 
    {
      _functionAssemblyName = value; 
    }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Type: WxeFunction")]
  [Description ("The type name of the WxeFunction used for this command.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string FunctionTypeName
  {
    get
    {
      if (_type != BocItemCommandType.WxeFunction)
        return null;
      return StringUtility.NullToEmpty (_functionTypeName); 
    }
    set 
    {
      _functionTypeName = value; 
    }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Type: WxeFunction")]
  [Description ("A comma seperated list of parameters for this command.")]
  [DefaultValue((string) null)]
  [NotifyParentProperty (true)]
  public string[] FunctionParameters
  {
    get
    {
      if (_type != BocItemCommandType.WxeFunction)
        return null;
      if (_functionParameters == null)
          return new string[0];
      return _functionParameters; 
    }
    set { _functionParameters = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue ("")]
  [Browsable (false)]
  public string FunctionParameterList
  {
    get
    {
      if (_type != BocItemCommandType.WxeFunction)
        return null;
      if (FunctionParameters == null)
        return "";
      return StringUtility.ConcatWithSeperator (FunctionParameters, ", ");
    }
    set
    {
      value = StringUtility.NullToEmpty (value);
      value = value.Trim();
      if (value.Length == 0)
      {
        _functionParameters = new string[0];
      }
      else
      {
        string[] functionParamters = value.Split (',');   
        _functionParameters = new string[functionParamters.Length];
        for (int i = 0; i < functionParamters.Length; i++)
        {
          functionParamters[i] = functionParamters[i].Trim();
          if (functionParamters[i].Length == 0)
            throw new ArgumentEmptyException ("Parameter name missing for parameter No. " + i + " in FunctionParameterList + '" + value + "'.");
          _functionParameters[i] = functionParamters[i];
        }
      }
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
  [Description ("Determines when to show the item command to the user in regard to the parent controls read-only setting.")]
  [Category ("Behavior")]
  [DefaultValue (BocItemCommandShow.Always)]
  [NotifyParentProperty (true)]
  public BocItemCommandShow Show
  {
    get { return _show; }
    set { _show = value; }
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

public delegate void BocItemCommandClickEventHandler (object sender, BocItemCommandClickEventArgs e);

public class BocItemCommandClickEventArgs: EventArgs
{
  private int _index;
  private string _id;

  public BocItemCommandClickEventArgs (int index, string id)
  {
    _index = index;
    _id = StringUtility.EmptyToNull (id);
  }

  public int Index
  {
    get { return _index; }
  }

  public string ID
  {
    get { return _id; }
  }
}

}

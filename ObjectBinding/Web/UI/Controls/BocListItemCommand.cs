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
public class BocItemCommand
{
  private BocItemCommandType _type = BocItemCommandType.Href;
  private BocItemCommandShow _show = BocItemCommandShow.Always;

  private string _href;
  private string _target;

  private string _functionTypeName;

  private string[] _functionParameters;

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
        WxeFunction function = Function;

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
          stringBuilder.AppendFormat (": {0}", FunctionTypeName);
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

  [Browsable (false)]
  public WxeFunction Function
  {
    get
    {
      if (_type != BocItemCommandType.WxeFunction)
        return null;

      Type functionType = System.Type.GetType (FunctionTypeName); 


      return null;
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
}

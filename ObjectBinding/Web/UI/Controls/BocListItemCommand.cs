using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary>
///   A <see cref="BocItemCommand"/> defines an action the user can invoke on a datarow.
/// </summary>
[TypeConverter (typeof (BocItemCommandConverter))]
public class BocItemCommand
{
  private BocItemCommandType _type = BocItemCommandType.Href;
  private BocItemCommandShow _show = BocItemCommandShow.Always;

  private string _href;
  private string _target;

  public BocItemCommand()
  {}

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

  public static BocItemCommand CreateHrefItemCommand (string href)
  {
    return BocItemCommand.CreateHrefItemCommand (href, null);
  }

  /// <summary> Renders the opening tag for the command. </summary>
  /// <param name="writer"></param>
  /// <param name="index"></param>
  /// <param name="id"></param>
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
          stringBuilder.AppendFormat (": {0}", Href);
        if (! StringUtility.IsNullOrEmpty (Target))
          stringBuilder.AppendFormat (", {0}", Target);
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
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The type of the command.")]
  //  No default value
  public BocItemCommandType Type
  {
    get { return _type; }
    set { _type = value; }
  }

  /// <summary> The hyperlink reference; used for <see cref="BocItemCommandType.Href"/>. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The hyperlink reference of the command. Use {0} for the index and {1} for the ID.")]
  [Category ("Type: Href")]
  [DefaultValue("")]
  public string Href 
  {
    get
    {
      if (_type == BocItemCommandType.Href)
        return StringUtility.NullToEmpty (_href); 
      else
        return null;
    }
    set
    {
      _href = value; 
    }
  }

  /// <summary> The hyperlink target; used for <see cref="BocItemCommandType.Href"/>. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The target frame of the command. Leave it blank for no target.")]
  [Category ("Type: Href")]
  [DefaultValue("")]
  public string Target 
  { 
    get
    { 
      if (_type == BocItemCommandType.Href)
        return StringUtility.NullToEmpty (_target); 
      else
        return null;
    }
    set
    {
      _target = value; 
    }
  }

  /// <summary>
  ///   Determines when the item command is shown to the user in regard of the parent control's 
  ///   read-only setting.
  /// </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("Determines when to show the item command to the user in regard to the parent controls read-only setting.")]
  [Category ("Behavior")]
  [DefaultValue (BocItemCommandShow.Always)]
  public BocItemCommandShow Show
  {
    get { return _show; }
    set { _show = value; }
  }
}

public enum BocItemCommandType
{
  Href
}

public enum BocItemCommandShow
{
  Always,
  ReadOnly,
  EditMode
}
}

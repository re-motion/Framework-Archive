using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

[TypeConverter (typeof (BocItemCommandConverter))]
public class BocItemCommand
{
  private string _href;
  private string _target;
  private BocItemCommandType _type = BocItemCommandType.Href;

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

  public virtual void RenderEnd (HtmlTextWriter writer)
  {
    writer.RenderEndTag();
  }

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

  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The hyperlink reference of the command. Use {0} for the index and {1} for the ID.")]
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

  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The target frame of the command. Leave it blank for no target.")]
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

  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The type of the command.")]
  //  No default value
  public BocItemCommandType Type
  {
    get { return _type; }
    set { _type = value; }
  }
}

public enum BocItemCommandType
{
  Undefined,
  Href
}
}

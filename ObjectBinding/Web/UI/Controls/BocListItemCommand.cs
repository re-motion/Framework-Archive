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
//  TODO: BocItemCommand: Script
[TypeConverter (typeof (ExpandableObjectConverter))]
public class BocItemCommand
{
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class HrefCommandProperties
  {
    private string _href;
    private string _target;

    public HrefCommandProperties()
    {}

    /// <summary>
    ///   Returns a string representation of this <see cref="HrefCommandProperties"/>.
    /// </summary>
    /// <remarks> Foramt: Href, Target </remarks>
    /// <returns> A <see cref="string"/>. </returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder (50);

      if (! StringUtility.IsNullOrEmpty (Href))
      {
        stringBuilder.Append (Href);
        if (! StringUtility.IsNullOrEmpty (Target))
          stringBuilder.AppendFormat (", {0}", Target);
      }

      return stringBuilder.ToString();
    }

    [Obsolete ("Only user for HrefCommandPropertiesConverter.")]
    internal static HrefCommandProperties Parse (string value)
    {
      // parse the format "href" or "href, target"
      string[] properties = ((string) value).Split (',');

      if (properties.Length > 0 && properties.Length < 3) 
      {          
        BocItemCommand.HrefCommandProperties hrefProperties = 
          new BocItemCommand.HrefCommandProperties();

        hrefProperties.Href = properties[0].Trim();                
        if (StringUtility.IsNullOrEmpty (hrefProperties.Href))
          return null;

        hrefProperties.Target = string.Empty;
        if (properties.Length == 2)
          hrefProperties.Target  = properties[1].Trim();

        return hrefProperties;
      }

      throw new FormatException ("The string '" + value + "' could not be converted to type BocItemCommand.HrefCommandProperties. Expected format is 'href' or 'href, target'.");
    }

    /// <summary> The hyperlink reference. </summary>
    /// <value> A <see cref="string"/> representing the hyperlink reference. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The hyperlink reference of the command. Use {0} to insert the Busines Object's index in the list and {1} to insert the Business Object's ID.")]
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

    /// <summary> The hyperlink target. </summary>
    /// <value> A <see cref="string"/> representing the hyperlink target. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
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

  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class WxeFunctionCommandProperties
  {
    private string _typeName;
    private string[] _parameters;

    public WxeFunctionCommandProperties()
    {
      ParameterList = string.Empty;
    }

    /// <summary>
    ///   Returns a string representation of this <see cref="WxeFunctionCommandProperties"/>.
    /// </summary>
    /// <remarks> Foramt: Href, Target </remarks>
    /// <returns> A <see cref="string"/>. </returns>
    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder (50);

      if (! StringUtility.IsNullOrEmpty (TypeName))
      {
        stringBuilder.Append (TypeName);
        if (Parameters != null)
          stringBuilder.AppendFormat (" ({0})", StringUtility.ConcatWithSeperator (Parameters, ", "));
      }

      return stringBuilder.ToString();
    }

    [Obsolete ("Only used for WxeFunctionCommandPropertiesConverter.")]
    internal static WxeFunctionCommandProperties Parse (string value)
    {
      // parse the format "functionTypeName ([parameter1[, parameter2[, ...]]])"
      string[] properties = ((string) value).Split ('(');

      if (properties.Length == 2) 
      {
        BocItemCommand.WxeFunctionCommandProperties wxeFunctionProperties = 
          new BocItemCommand.WxeFunctionCommandProperties();

        wxeFunctionProperties.TypeName = properties[0].Trim();
        if (StringUtility.IsNullOrEmpty (wxeFunctionProperties.TypeName))
          return null;

        properties[1].Trim();
        if (properties[1].EndsWith (")"))
        {
          wxeFunctionProperties.ParameterList = 
            properties[1].Substring (0, properties[1].Length - 1);
        }

        return wxeFunctionProperties;
      }

      throw new FormatException ("The string '" + value + "' could not be converted to type BocItemCommand.WxeFunctionCommandProperties. Expected format is 'typename ([parameter1[, parameter2[, ...]]])'.");
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The complete type name (type, assembly) of the WxeFunction used for the command.")]
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
    [Browsable (false)]
    public string[] Parameters
    {
      get
      {
        if (_parameters == null)
            return new string[0];
        return _parameters; 
      }
      set
      {
        _parameters = value; 
      }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("A comma seperated list of parameters for the command. Use '%ID' to pass the BusinessObject's ID and '%Index' to pass the BusinessObject's index in the list.")]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
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

  public class ScriptCommandProperties
  {
  }

  private BocItemCommandType _type = BocItemCommandType.Event;
  private BocItemCommandShow _show = BocItemCommandShow.Always;

  private HrefCommandProperties _hrefCommand = new HrefCommandProperties();
  private WxeFunctionCommandProperties _wxeFunctionCommand = new WxeFunctionCommandProperties();
  //private ScriptCommandProperties _scriptCommand = null;

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
  [Description ("The type of command generated.")]
  //  No default value
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

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The parameters of the hyperlink. Interpreted if Type is set to Href.")]
  [DefaultValue ((string)null)]
  [NotifyParentProperty (true)]
  public HrefCommandProperties HrefCommand
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

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("The parameters of the WxeFunction. Interpreted if Type is set to WxeFunction.")]
  [DefaultValue ((string)null)]
  [NotifyParentProperty (true)]
  public WxeFunctionCommandProperties WxeFunctionCommand
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

// TODO: BocItemCommandType: Documentation
public enum BocItemCommandType
{
  Event,
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

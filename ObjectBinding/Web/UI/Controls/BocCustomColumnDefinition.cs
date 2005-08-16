using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using System.Collections.Specialized;
using System.Reflection;
using System.Globalization;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{
/// <summary> A column definition using <see cref="BocCustomColumnDefinitionCell"/> for rendering the data. </summary>
public class BocCustomColumnDefinition: BocColumnDefinition, IBusinessObjectClassSource
{
  private PropertyPathBinding _propertyPathBinding;
  private BocCustomColumnDefinitionCell _customCell;
  private string _customCellType;
  private string _customCellArgument;
  private bool _isSortable = false;

  /// <summary> Initializes a new instance of the <see cref="BocCustomColumnDefinition"/> class. </summary>
  public BocCustomColumnDefinition()
  {
    _propertyPathBinding = new PropertyPathBinding();
  }

  /// <summary> Passes the new OwnerControl to the <see cref="PropertyPathBindingCollection"/>. </summary>
  protected override void OnOwnerControlChanged()
  {
    _propertyPathBinding.OwnerControl = OwnerControl;
    base.OnOwnerControlChanged();
  }

  /// <summary> Gets or sets the <see cref="BocCustomColumnDefinitionCell"/> to be used for rendering. </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BocCustomColumnDefinitionCell CustomCell
  {
    get 
    {
      if (_customCell == null)
      {
        Type type = TypeUtility.GetType (_customCellType, true, false);
        _customCell = (BocCustomColumnDefinitionCell) Activator.CreateInstance (type);
      }
      return _customCell; 
    }
    set { _customCell = value; }
  }

  /// <summary> Gets or sets the type of the <see cref="BocCustomColumnDefinitionCell"/> to be used for rendering. </summary>
  /// <remarks>
  ///    Optionally uses the abbreviated type name as defined in <see cref="TypeUtility.ParseAbbreviatedTypeName"/>. 
  /// </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Format")]
  [Description ("The BocCustomColumnDefinitionCell to be used for rendering.")]
  //  No default value
  [NotifyParentProperty (true)]
  public string CustomCellType
  {
    get { return _customCellType; }
    set { _customCellType = value; }
  }

  /// <summary> 
  ///   Gets or sets the The name/value pairs to set the <see cref="BocCustomColumnDefinitionCell"/>'s properties. 
  /// </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Format")]
  [Description ("The name/value pairs to set the BocCustomColumnDefinitionCell's properties (property=value).")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string CustomCellArgument
  {
    get { return _customCellArgument; }
    set { _customCellArgument = value; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="BusinessObjectPropertyPath"/> used by to access the value of an 
  ///   <see cref="IBusinessObject"/>. Must not be <see langword="null"/>.
  /// </summary>
  /// <value> A <see cref="BusinessObjectPropertyPath"/>. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BusinessObjectPropertyPath PropertyPath 
  { 
    get { return _propertyPathBinding.PropertyPath; }
    set { _propertyPathBinding.PropertyPath = value; }
  }

  /// <summary>
  ///   Gets or sets the string representation of the <see cref="PropertyPath"/>. 
  ///   Must not be <see langword="null"/> or emtpy.
  /// </summary>
  /// <value> A <see cref="string"/> representing the <see cref="PropertyPath"/>. </value>
  [Editor (typeof (PropertyPathPickerEditor), typeof (UITypeEditor))]
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  [Description ("The string representation of the Property Path. Must not be emtpy.")]
  //  No default value
  [NotifyParentProperty (true)]
  public string PropertyPathIdentifier
  { 
    get { return _propertyPathBinding.PropertyPathIdentifier; }
    set { _propertyPathBinding.PropertyPathIdentifier = value; }
  }


  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Behavior")]
  [Description ("A flag determining whether to enable sorting for this columns.")]
  [DefaultValue (true)]
  [NotifyParentProperty (false)]
  public bool IsSortable
  {
    get { return _isSortable; }
    set { _isSortable = value; }
  }

  /// <summary> Gets the displayed value of the column title. </summary>
  /// <remarks> 
  ///   If <see cref="BocColumnDefinition.ColumnTitle"/> is empty or <see langowrd="null"/>, 
  ///   the <c>DisplayName</c> of the <see cref="IBusinessObjectProperty"/> is returned.
  /// </remarks>
  /// <value> A <see cref="string"/> representing this column's title row contents. </value>
  public override string ColumnTitleDisplayValue
  {
    get 
    {
      bool isTitleEmpty = StringUtility.IsNullOrEmpty(ColumnTitle);

      if (! isTitleEmpty)
        return ColumnTitle;
      else if (PropertyPath != null)
        return PropertyPath.LastProperty.DisplayName;  
      else
        return string.Empty;
    }
  }

  /// <summary> The human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "CustomColumnDefinition"; }
  }

  IBusinessObjectClass IBusinessObjectClassSource.BusinessObjectClass
  {
    get { return _propertyPathBinding.BusinessObjectClass; }
  } 
}


/// <summary> Represents the method that handles the <see cref="BocList.CustomCellClick"/> event. </summary>
public delegate void BocCustomCellClickEventHandler (object sender, BocCustomCellClickEventArgs e);

/// <summary> Provides data for the <see cref="BocList.CustomCellClick"/> event. </summary>
public class BocCustomCellClickEventArgs: EventArgs
{
  private IBusinessObject _businessObject;
  private BocCustomColumnDefinition _column;
  private string _argument;

  /// <summary> Initializes a new instance. </summary>
  public BocCustomCellClickEventArgs (
      BocCustomColumnDefinition column, 
      IBusinessObject businessObject,
      string argument)
  {
    _businessObject = businessObject;
    _column = column;
    _argument = argument;
  }

  /// <summary> The <see cref="IBusinessObject"/> on which the rendered command is applied on. </summary>
  public IBusinessObject BusinessObject
  {
    get { return _businessObject; }
  }

  /// <summary> The <see cref="BocCustomColumnDefinition"/> to which the command belongs. </summary>
  public BocCustomColumnDefinition Column
  {
    get { return _column; }
  }

  /// <summary> 
  ///   The argument generated by the <see cref="BocCustomColumnDefinitionCell.Render"/> method when registering
  ///   for a <c>click</c> event.
  /// </summary>
  public string Argument
  {
    get { return _argument; }
  }
}


/// <summary>
///   Derive custom cell renderers from this class. 
/// </summary>
public abstract class BocCustomColumnDefinitionCell
{
  private BocCustomCellArguments _arguments = null;

  /// <summary> Get the javascript code that invokes <see cref="OnClick"/> when called. </summary>
  /// <param name="eventArgument"> The event argument to be passed to <see cref="OnClick"/>. </param>
  /// <returns> The script invoking <see cref="OnClick"/> when called. </returns>
  protected string GetPostBackClientEvent (string eventArgument)
  {
    BocCustomCellRenderArguments renderArguments = _arguments as BocCustomCellRenderArguments;
    if (renderArguments == null)
      throw new InvalidOperationException ("GetPostBackClientEvent can only be called from DoRender method.");

    return renderArguments.List.GetCustomCellPostBackClientEvent (
        renderArguments.ColumnIndex, renderArguments.ListIndex, eventArgument) + renderArguments.OnClick;
  }

  public void Render (HtmlTextWriter writer, BocCustomCellRenderArguments arguments)
  {
    InitArguments (arguments);
    DoRender (writer, arguments);
  }

  /// <summary> 
  ///   Override this method to render a custom cell. 
  /// </summary>
  /// <remarks> 
  ///   Use <see cref="GetPostBackClientEvent"/> to render the code that invokes <see cref="OnClick"/>. 
  /// </remarks>
  protected abstract void DoRender (HtmlTextWriter writer, BocCustomCellRenderArguments arguments);

  public void OnClick (BocCustomCellArguments arguments, string eventArgument)
  {
    InitArguments (arguments);
    DoOnClick (arguments, eventArgument);
  }

  /// <summary>
  ///   Override this method to process an <c>OnClick</c> event generated by <see cref="GetPostBackClientEvent"/>.
  /// </summary>
  /// <param name="arguments"> The event arguments. </param>
  /// <param name="eventArgument"> The parameter passed to <see cref="GetPostBackClientEvent"/>. </param>
  protected virtual void DoOnClick (BocCustomCellArguments arguments, string eventArgument)
  {
  }

  private void InitArguments (BocCustomCellArguments arguments)
  {
    _arguments = arguments;
    string customArg = arguments.ColumnDefinition.CustomCellArgument;
    if (! StringUtility.IsNullOrEmpty (customArg))
    {
      NameValueCollection values = new NameValueCollection();
      StringUtility.ParsedItem[] items = StringUtility.ParseSeparatedList (customArg, ',');
      for (int i = 0; i < items.Length; i++)
      {
        string[] pair = items[i].Value.Split (new char[] {'='}, 2);
        if (pair.Length == 2)
        {
          string key = pair[0].Trim();
          string value = pair[1].Trim(' ', '\"');
          values.Add (key, value);
        }
      }
      PropertyInfo[] properties = this.GetType().GetProperties (BindingFlags.Public | BindingFlags.Instance);
      for (int i = 0; i < properties.Length; i++)
      {
        PropertyInfo property = properties[i];
        string strval = values[property.Name];
        if (strval != null)
        {
          try
          {
            if (strval.Length >= 2 && strval[0] == '\"' && strval[strval.Length-1] == '\"')
              strval = strval.Substring (1, strval.Length - 2);
            object value = StringUtility.Parse (property.PropertyType, strval, CultureInfo.InvariantCulture);
            property.SetValue (this, value, new object[0]);
          }
          catch (Exception e)
          {
            throw new ApplicationException ("Property " + property.Name + ": " + e.Message, e);
          }
        }
      }
    }
  }
}

public class BocCustomCellArguments
{
  private BocList _list;
  private IBusinessObject _businessObject;
  private BocCustomColumnDefinition _columnDefinition;

  public BocCustomCellArguments (
      BocList list,
      IBusinessObject businessObject, 
      BocCustomColumnDefinition columnDefiniton)
  {
    _list = list;
    _businessObject = businessObject;
    _columnDefinition = columnDefiniton;
  }

  /// <summary> The <see cref="BocList"/> containing the column. </summary>
  public BocList List
  {
    get { return _list; }
  }

  /// <summary> The <see cref="IBusinessObject"/> that should be rendered or that was clicked. </summary>
  public IBusinessObject BusinessObject
  {
    get { return _businessObject; }
  }

  /// <summary> The column definition of the column that should be rendered or that was clicked. </summary>
  public BocCustomColumnDefinition ColumnDefinition
  {
    get { return _columnDefinition; }
  }
}

public class BocCustomCellRenderArguments: BocCustomCellArguments
{
  private int _columnIndex;
  private int _listIndex;
  private string _onClick;

  public BocCustomCellRenderArguments (
      BocList list,
      IBusinessObject businessObject, 
      BocCustomColumnDefinition columnDefiniton,
      int columnIndex,
      int listIndex,
      string onClick)
    : base (list, businessObject, columnDefiniton)
  {
    _columnIndex = columnIndex;
    _listIndex = listIndex;
    _onClick = onClick;
  }

  /// <summary> The index of the rendered column. </summary>
  public int ColumnIndex
  {
    get { return _columnIndex; }
  }

  /// <summary> The list index of the current business object. </summary>
  public int ListIndex
  {
    get { return _listIndex; }
  }

  /// <summary> Client script code that prevents row selection. For internal use. </summary>
  /// <remarks>
  ///   A function to be appended to the client side <c>OnClick</c> event handler. The function tasked with
  ///   preventing the row from being selected/highlighted when clicking on the link itself instead of the row.
  ///   This string is inserted after the return value of <see cref="BocList.GetCustomCellPostBackClientEvent"/>.
  /// </remarks>
  public string OnClick
  {
    get { return _onClick; }
  }
}

}

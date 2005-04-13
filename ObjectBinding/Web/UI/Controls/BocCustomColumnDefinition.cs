using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using Rubicon.Utilities;
using Rubicon.Web.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{
/// <summary> A column definition using <see cref="IBocCustomColumnDefinitionCell"/> for rendering the data. </summary>
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

  /// <summary> The <see cref="IBocCustomColumnDefinitionCell"/> to be used for rendering. </summary>
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

  /// <summary> Gets or sets the type of the <see cref="IBocCustomColumnDefinitionCell"/> to be used for rendering. </summary>
  /// <remarks>
  ///    Optionally uses the abbreviated type name as defined in <see cref="TypeUtility.ParseAbbreviatedTypeName"/>. 
  /// </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Format")]
  [Description ("The IBocCustomColumnDefinitionCell to be used for rendering.")]
  //  No default value
  [NotifyParentProperty (true)]
  public string CustomCellType
  {
    get { return _customCellType; }
    set { _customCellType = value; }
  }

  /// <summary> Gets or sets the argument to be passed to the constructor of the <see cref="CustomCellType"/>. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Format")]
  [Description ("The argument to be passed to the constructor of the CustomCellType.")]
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
  ///   The argument generated by the <see cref="IBocCustomColumnDefinitionCell.Render"/> method when registering
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
  private BocCustomCellRenderArguments _arguments = null;

  /// <summary>
  ///   Get the javascript code that invokes OnClick when called. The 
  /// </summary>
  /// <param name="argument"></param>
  /// <returns></returns>
  protected string GetPostBackClientEvent (string eventArgument)
  {
    if (_arguments == null)
      throw new InvalidOperationException ("GetPostBackClientEvent can only be called from DoRender method.");
    return _arguments.List.GetCustomCellPostBackClientEvent (_arguments.ColumnIndex, _arguments.ListIndex, eventArgument) + _arguments.OnClick;
  }

  public void Render (HtmlTextWriter writer, BocCustomCellRenderArguments arguments)
  {
    _arguments = arguments;
    DoRender (writer, arguments);
  }

  /// <summary>
  ///   Override this method to render a custom cell.
  /// </summary>
  /// <remarks>
  ///   Use <see cref="GetPostBackClientEvent"/> to render the code that invokes <see cref="OnClick"/>.
  /// </remarks>
  protected abstract void DoRender (HtmlTextWriter writer, BocCustomCellRenderArguments arguments);

  /// <summary>
  ///   This method is called when a script created by <see cref="GetPostBackClientEvent"/> is executed.
  /// </summary>
  /// <param name="arguments"> The event arguments. </param>
  /// <param name="eventArgument"> The parameter passed to <see cref="GetPostBackClientEvent"/>. </param>
  public virtual void OnClick (BocCustomCellArguments arguments, string eventArgument)
  {
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

  /// <summary>
  ///   The <see cref="BocList"/> containing the column.
  /// </summary>
  public BocList List
  {
    get { return _list; }
  }

  /// <summary>
  ///   The <see cref="IBusinessObject"/> that should be rendered or that was clicked.
  /// </summary>
  public IBusinessObject BusinessObject
  {
    get { return _businessObject; }
  }

  /// <summary>
  ///   The column definition of the column that should be rendered or that was clicked.
  /// </summary>
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

  /// <summary>
  ///   The index of the rendered column.
  /// </summary>
  public int ColumnIndex
  {
    get { return _columnIndex; }
  }

  /// <summary>
  ///   The list index of the current business object.
  /// </summary>
  public int ListIndex
  {
    get { return _listIndex; }
  }

  /// <summary>
  ///  Client script code that prevents row selection. For internal use.
  /// </summary>
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

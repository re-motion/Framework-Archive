using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Globalization;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.Design;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{
/// <summary>
///   A BocColumnDefinition defines how to display a column of a list. 
/// </summary>
[Editor (typeof(ExpandableObjectConverter), typeof(UITypeEditor))]
public abstract class BocColumnDefinition
{
  /// <summary> The text displayed in the column header. </summary>
  private string _columnHeader;
  /// <summary> The width of the column. </summary>
  private Unit _width; 
  /// <summary>
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this column definition belongs to. 
  /// </summary>
  private IBusinessObjectBoundWebControl _ownerControl;

  public BocColumnDefinition (string columnHeader, Unit width)
  {
    _width = width;
    _columnHeader = StringUtility.NullToEmpty (columnHeader);
  }

  private BocColumnDefinition()
    : this (null, Unit.Empty)
  {}

  /// <summary> Is called when the value of <see cref="OwnerControl"/> was changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {}

  /// <summary>
  ///   Returns a <see cref="string"/> that represents this <see cref="BocColumnDefinition"/>.
  /// </summary>
  /// <returns>
  ///   Returns the class name of the instance, followed by the <see cref="ColumnHeader"/>.
  /// </returns>
  public override string ToString()
  {
    if (StringUtility.IsNullOrEmpty (ColumnHeader))
      return GetType().Name;
    else
      return string.Format ("{0} ({1})", GetType().Name, ColumnHeader);
  }

  /// <summary> The displayed value of the column header. </summary>
  /// <remarks> Override this property to change the way the column header text is generated. </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual string ColumnHeaderDisplayValue
  {
    get { return ColumnHeader; }
  }

  /// <summary> The text displayed in the column header. </summary>
  /// <remarks>
  ///   Override this property to add validity checks to the set accessor.
  ///   The get accessor should return the value verbatim.
  /// </remarks>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The assigned value of the column header, can be empty.")]
  [DefaultValue("")]
  public virtual string ColumnHeader
  {
    get { return _columnHeader; }
    set { _columnHeader = StringUtility.NullToEmpty (value); }
  }

  /// <summary> THe width of the column. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue(typeof (Unit), "")]
  public Unit Width 
  { 
    get { return _width; } 
    set { _width = value; }
  }

  /// <summary>
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this column definition belongs to. 
  /// </summary>
  protected internal IBusinessObjectBoundWebControl OwnerControl
  {
    get
    {
      return _ownerControl; 
    }
    set
    {
      if (_ownerControl != value)
      {
        _ownerControl = value;
        OnOwnerControlChanged();
      }
    }
  }
}

/// <summary>
///   A column definition containing no data, but a <see cref="BocItemCommand"/>.
/// </summary>
public class BocCommandColumnDefinition: BocColumnDefinition
{
  /// <summary> The <see cref="BocItemCommand"/> rendered in this column. </summary>
  private BocItemCommand _command;
  /// <summary> The text symbolizing the command in the rendered page. </summary>
  private object _label;
  /// <summary> The image symbolizing the command in the rendered page. </summary>
  private string _iconPath;

  public BocCommandColumnDefinition (
      BocItemCommand command, 
      object label, 
      string iconPath, 
      string columnHeader, 
      Unit width)
    : base (columnHeader, width)
  {
    ArgumentUtility.CheckNotNull ("command", command);

    _command = command;
    _iconPath = iconPath;
    _label = label;
  }

  public BocCommandColumnDefinition()
    : base (null, Unit.Empty)
  {
    _command = new BocItemCommand();
  }

  /// <summary>
  ///   Returns a <see cref="string"/> that represents this <see cref="BocColumnDefinition"/>.
  /// </summary>
  /// <returns> 
  ///   Returns the class name of the instance, followed by the <see cref="Label"/>. 
  /// </returns>
  public override string ToString()
  {
    if (StringUtility.IsNullOrEmpty (Label))
      return GetType().Name;
    else
      return string.Format ("{0} ({1})", GetType().Name, Label);
  }

  /// <summary> The <see cref="BocItemCommand"/> rendered in this column. </summary>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  public BocItemCommand Command
  {
    get { return _command; }
    set { _command = value; }
  }

  /// <summary> The text symbolizing the command in the rendered page. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  public string Label
  {
    get { return (_label != null) ? _label.ToString() : string.Empty; }
    set { _label = value; }
  }

  /// <summary> The image symbolizing the command in the rendered page. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  public string IconPath 
  {
    get { return _iconPath; }
    set { _iconPath = value; }
  }
}

/// <summary>
///   A column definition for displaying data.
/// </summary>
public abstract class BocValueColumnDefinition: BocColumnDefinition
{
  public BocValueColumnDefinition (string columnHeader, Unit width)
    : base (columnHeader, width)
  {}

  private BocValueColumnDefinition()
    : this (null, Unit.Empty)
  {}

  /// <summary> Creates a string representation of the data displayed in this column. </summary>
  /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
  /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
  public abstract string GetStringValue (IBusinessObject obj);
}

/// <summary>
///   A column definition for displaying a single property path.
/// </summary>
/// <remarks>
///   Note that using the methods of <see cref="BusinessObjectPropertyPath"/>, 
///   the original value of this property can be retreived or changed.
/// </remarks>
public class BocSimpleColumnDefinition: BocValueColumnDefinition
{
  /// <summary>
  ///   The <see cref="PropertyPathBinding"/> used to store the <see cref="PropertyPath"/> 
  ///   internally.
  /// </summary>
  private PropertyPathBinding _propertyPathBinding;

  public BocSimpleColumnDefinition (
      BusinessObjectPropertyPath propertyPath,
      string columnHeader, 
      Unit width)
    : base (columnHeader, width)
  {
    ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);
    _propertyPathBinding = new PropertyPathBinding (propertyPath);
  }

  public BocSimpleColumnDefinition (
      string propertyPathIdentifier,
      string columnHeader, 
      Unit width)
    : base (columnHeader, width)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);
    _propertyPathBinding = new PropertyPathBinding (propertyPathIdentifier);
  }

  public BocSimpleColumnDefinition ()
    : base (string.Empty, Unit.Empty)
  {
    _propertyPathBinding = new PropertyPathBinding();
  }

  /// <summary> Creates a string representation of the data displayed in this column. </summary>
  /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
  /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
  public override string GetStringValue (IBusinessObject obj)
  {
    if (PropertyPath != null)
      return PropertyPath.GetStringValue (obj);
    else
      return string.Empty;
  }

  /// <summary>
  ///   The <see cref="BusinessObjectPropertyPath"/> used by <see cref="GetStringValue"/> 
  ///   to access the value of an <see cref="IBusinessObject"/>.
  /// </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BusinessObjectPropertyPath PropertyPath 
  { 
    get
    {
      if (OwnerControl == null)
        throw new InvalidOperationException ("PropertyPath could not be resolved because the object is not part of an IBusinessObjectBoundWebControl.");
      
      if (! ControlHelper.IsDesignMode (OwnerControl))
      {
        _propertyPathBinding.DataSource = OwnerControl.DataSource;
        return _propertyPathBinding.PropertyPath;
      }
      else
        return null;
    }
    set
    {
      _propertyPathBinding.PropertyPath = value;
    }
  }

  /// <summary> The string representation of the <see cref="PropertyPath"/>. </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  public string PropertyPathIdentifier
  { 
    get
    { 
      return _propertyPathBinding.PropertyPathIdentifier; 
    }
    set
    { 
      _propertyPathBinding.PropertyPathIdentifier = value; 
    }
  }

  /// <summary> The displayed value of the column header. </summary>
  /// <remarks> 
  ///   If <see cref="ColumnHeader"/> is empty or <see langowrd="null"/>, the <c>DisplayName</c>
  ///   of the <see cref="BusinessObjectProperty"/> is returned.
  /// </remarks>
  public override string ColumnHeaderDisplayValue
  {
    get 
    {
      bool isHeaderEmpty = StringUtility.IsNullOrEmpty(ColumnHeader);

      if (! isHeaderEmpty)
        return ColumnHeader;
      else if (PropertyPath != null)
        return PropertyPath.LastProperty.DisplayName;  
      else
        return string.Empty;
    }
  }
}

/// <summary>
///   A column definition for displaying a string made up from different property paths.
/// </summary>
/// <remarks>
///   Note that values in these columnDefinitions can usually not be modified directly.
/// </remarks>
[ParseChildren (true, "PropertyPathBindings")]
public class BocCompoundColumnDefinition: BocValueColumnDefinition
{
  /// <summary>
  ///   A format string describing how the values access through the 
  ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
  /// </summary>
  private string _formatString;

  /// <summary>
  ///   The collection of <see cref="PropertyPathBinding"/> objects used by
  ///   <see cref="GetStringValue"/> to access the values of an <see cref="IBusinessObject"/>.
  /// </summary>
  private PropertyPathBindingCollection _propertyPathBindings;

  public BocCompoundColumnDefinition (
      string formatString,
      BusinessObjectPropertyPath[] propertyPaths, 
      string columnHeader, 
      Unit width)
    : base (columnHeader, width)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPaths", propertyPaths);
    ArgumentUtility.CheckNotNullOrEmpty ("columnHeader", columnHeader);

    ColumnHeader = columnHeader;
    _formatString = formatString;
    _propertyPathBindings = new PropertyPathBindingCollection (null);
    _propertyPathBindings.AddRange (propertyPaths);
  }

  public BocCompoundColumnDefinition (
      string formatString,
      string[] propertyPathIdentifiers, 
      string columnHeader, 
      Unit width)
    : base (columnHeader, width)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifiers", propertyPathIdentifiers);
    ArgumentUtility.CheckNotNullOrEmpty ("columnHeader", columnHeader);

    ColumnHeader = columnHeader;
    _formatString = formatString;
    _propertyPathBindings = new PropertyPathBindingCollection (null);
    _propertyPathBindings.AddRange (propertyPathIdentifiers);
  }

  public BocCompoundColumnDefinition()
    : base (string.Empty, Unit.Empty)
  {
    _propertyPathBindings = new PropertyPathBindingCollection (null);
    _formatString = string.Empty;
  }

  /// <summary> Creates a string representation of the data displayed in this column. </summary>
  /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
  /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
  public override string GetStringValue (IBusinessObject obj)
  {
    string[] strings = new string[_propertyPathBindings.Count];
    for (int i = 0; i < _propertyPathBindings.Count; ++i)
      strings[i] = _propertyPathBindings[i].PropertyPath.GetStringValue (obj);

    return string.Format (_formatString, strings);
  }

  /// <summary>
  ///   Passes the new <see cref="OwnerControl"/> to the <see cref="PropertyPathBindingCollection"/>.
  /// </summary>
  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged();

    _propertyPathBindings.OwnerControl = OwnerControl;
  }

  /// <summary>
  ///   A format string describing how the values access through the 
  ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
  /// </summary>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  public string FormatString
  {
    get { return _formatString; }
    set { _formatString = value; }
  }

  /// <summary>
  ///   The collection of <see cref="PropertyPathBinding"/> objects used by
  ///   <see cref="GetStringValue"/> to access the values of an <see cref="IBusinessObject"/>.
  /// </summary>
  [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
  [ListBindable (false)]
  [DefaultValue ((string) null)]
  public PropertyPathBindingCollection PropertyPathBindings
  {
    get 
    { 
      return _propertyPathBindings; 
    }
  }

  /// <summary>
  ///   The text displayed in the column header. Must not be empty or <see langword="null"/>.
  /// </summary>
  [Description ("The assigned value of the column header, must not be empty or null.")]
  public override string ColumnHeader
  {
    get { return base.ColumnHeader; }
    set 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("ColumnHeader", value);
      base.ColumnHeader = value;
    }
  }
}

}

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
/// <summary> A BocColumnDefinition defines how to display a column of a list. </summary>
[Editor (typeof(ExpandableObjectConverter), typeof(UITypeEditor))]
public abstract class BocColumnDefinition
{
  /// <summary> The ID of this column definition. </summary>
  private string _id;

  /// <summary> The text displayed in the column title. </summary>
  private string _columnTitle;
  /// <summary> The width of the column. </summary>
  private Unit _width; 
  /// <summary>
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this column definition belongs to. 
  /// </summary>
  private IBusinessObjectBoundWebControl _ownerControl;

  /// <summary> Simple Constructor. </summary>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  public BocColumnDefinition (string columnTitle, Unit width)
  {
    _width = width;
    _columnTitle = StringUtility.NullToEmpty (columnTitle);
  }

  /// <summary> Simple Constructor. </summary>
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
  ///   Returns the class name of the instance, followed by the <see cref="ColumnTitle"/>.
  /// </returns>
  public override string ToString()
  {
    if (StringUtility.IsNullOrEmpty (ColumnTitle))
      return GetType().Name;
    else
      return string.Format ("{0} ({1})", GetType().Name, ColumnTitle);
  }

  /// <summary> The ID of this column definition. </summary>
  /// <value> A <see cref="string"/> providing an identifier for this column. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The ID of this column definition.")]
  [Category ("Misc")]
  [DefaultValue("")]
  public string ID
  {
    get { return _id; }
    set { _id = value; }
  }

  /// <summary> The displayed value of the column title. </summary>
  /// <remarks> Override this property to change the way the column title text is generated. </remarks>
  /// <value> A <see cref="string"/> representing this column's title row contents. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual string ColumnTitleDisplayValue
  {
    get { return ColumnTitle; }
  }

  /// <summary> The text displayed in the column title. </summary>
  /// <remarks>
  ///   Override this property to add validity checks to the set accessor.
  ///   The get accessor should return the value verbatim.
  /// </remarks>
  /// <value> A <see cref="string"/> representing the externally set title of this column. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Description ("The assigned value of the column title, can be empty.")]
  [Category ("Appearance")]
  [DefaultValue("")]
  public virtual string ColumnTitle
  {
    get { return _columnTitle; }
    set { _columnTitle = StringUtility.NullToEmpty (value); }
  }

  /// <summary> The width of the column. </summary>
  /// <value> A <see cref="Unit"/> providing the width of this column when it is rendered. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Layout")]
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

/// <summary> A column definition containing no data, but a <see cref="BocItemCommand"/>. </summary>
public class BocCommandColumnDefinition: BocColumnDefinition
{
  /// <summary> The <see cref="BocItemCommand"/> rendered in this column. </summary>
  private BocItemCommand _command;
  /// <summary> The text symbolizing the command on the rendered page. </summary>
  private object _label;
  /// <summary> The image symbolizing the command on the rendered page. </summary>
  private string _iconPath;

  /// <summary> Simple Constructor. </summary>
  /// <param name="command"> The <see cref="BocItemCommand"/> rendered in this column. </param>
  /// <param name="label"> The text symbolizing the command on the rendered page. </param>
  /// <param name="iconPath"> The image symbolizing the command on the rendered page. </param>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  public BocCommandColumnDefinition (
      BocItemCommand command, 
      object label, 
      string iconPath, 
      string columnTitle, 
      Unit width)
    : base (columnTitle, width)
  {
    ArgumentUtility.CheckNotNull ("command", command);

    _command = command;
    _iconPath = iconPath;
    _label = label;
  }

  /// <summary> Simple Constructor. </summary>
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
  /// <value> A <see cref="BocItemCommand"/>. </value>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Action")]
  public BocItemCommand Command
  {
    get { return _command; }
    set { _command = value; }
  }

  /// <summary> The text symbolizing the command in the rendered page. </summary>
  /// <value> A <see cref="string"/> representing the command. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [DefaultValue("")]
  public string Label
  {
    get { return (_label != null) ? _label.ToString() : string.Empty; }
    set { _label = value; }
  }

  /// <summary> The image symbolizing the command in the rendered page. </summary>
  /// <value> An <see cref="image"/> representing the command. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [DefaultValue("")]
  public string IconPath 
  {
    get { return _iconPath; }
    set { _iconPath = value; }
  }
}

/// <summary> A column definition for displaying data. </summary>
public abstract class BocValueColumnDefinition: BocColumnDefinition
{
  /// <summary> Simple Constructor. </summary>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  public BocValueColumnDefinition (string columnTitle, Unit width)
    : base (columnTitle, width)
  {}

  /// <summary> Simple Constructor. </summary>
  private BocValueColumnDefinition()
    : this (null, Unit.Empty)
  {}

  /// <summary> Creates a string representation of the data displayed in this column. </summary>
  /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
  /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
  public abstract string GetStringValue (IBusinessObject obj);
}

/// <summary> A column definition for displaying a single property path. </summary>
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

  /// <summary> Simple Constructor. </summary>
  /// <param name="propertyPath">
  ///   The <see cref="BusinessObjectPropertyPath"/> used by <see cref="GetStringValue"/> 
  ///   to access the value of an <see cref="IBusinessObject"/>. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  public BocSimpleColumnDefinition (
      BusinessObjectPropertyPath propertyPath,
      string columnTitle, 
      Unit width)
    : base (columnTitle, width)
  {
    ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);
    _propertyPathBinding = new PropertyPathBinding (propertyPath);
  }

  /// <summary> Simple Constructor. </summary>
  /// <param name="propertyPathIdentifier">
  ///   The string representation of the <see cref="PropertyPath"/>. Must not be 
  ///   <see langword="null"/> or emtpy.
  /// </param>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  public BocSimpleColumnDefinition (
      string propertyPathIdentifier,
      string columnTitle, 
      Unit width)
    : base (columnTitle, width)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);
    _propertyPathBinding = new PropertyPathBinding (propertyPathIdentifier);
  }

  /// <summary> Simple Constructor. </summary>
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
  ///   to access the value of an <see cref="IBusinessObject"/>. Must not be <see langword="null"/>.
  /// </summary>
  /// <value> A <see cref="BusinessObjectPropertyPath"/>. </value>
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
      ArgumentUtility.CheckNotNull ("PropertyPath", value);
      _propertyPathBinding.PropertyPath = value;
    }
  }

  /// <summary>
  ///   The string representation of the <see cref="PropertyPath"/>. Must not be 
  ///   <see langword="null"/> or emtpy.
  /// </summary>
  /// <value> A <see cref="string"/> representing the <see cref="PropertyPath"/>. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Data")]
  //  No default value
  public string PropertyPathIdentifier
  { 
    get
    { 
      return _propertyPathBinding.PropertyPathIdentifier; 
    }
    set
    { 
     ArgumentUtility.CheckNotNullOrEmpty ("PropertyPathIdentifier", value);
     _propertyPathBinding.PropertyPathIdentifier = value; 
    }
  }

  /// <summary> The displayed value of the column title. </summary>
  /// <remarks> 
  ///   If <see cref="ColumnTitle"/> is empty or <see langowrd="null"/>, the <c>DisplayName</c>
  ///   of the <see cref="BusinessObjectProperty"/> is returned.
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

  /// <summary> Simple Constructor. </summary>
  /// <param name="formatString"> 
  ///   A format string describing how the values access through the 
  ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
  /// </param>
  /// <param name="propertyPaths">
  ///   The <see cref="BusinessObjectPropertyPath"/> objects used to access the values that will be
  ///   rendered in this column.
  /// </param>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  public BocCompoundColumnDefinition (
      string formatString,
      BusinessObjectPropertyPath[] propertyPaths, 
      string columnTitle, 
      Unit width)
    : base (columnTitle, width)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPaths", propertyPaths);
    ArgumentUtility.CheckNotNullOrEmpty ("columnTitle", columnTitle);

    ColumnTitle = columnTitle;
    _formatString = formatString;
    _propertyPathBindings = new PropertyPathBindingCollection (null);
    _propertyPathBindings.AddRange (propertyPaths);
  }

  /// <summary> Simple Constructor. </summary>
  /// <param name="formatString"> 
  ///   A format string describing how the values access through the 
  ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
  /// </param>
  /// <param name="propertyPathIdentifiers">
  ///   The strings identifying the <see cref="BusinessObjectPropertyPath"/> objects used to access
  ///   the values that will be rendered in this column.
  /// </param>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  public BocCompoundColumnDefinition (
      string formatString,
      string[] propertyPathIdentifiers, 
      string columnTitle, 
      Unit width)
    : base (columnTitle, width)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifiers", propertyPathIdentifiers);
    ArgumentUtility.CheckNotNullOrEmpty ("columnTitle", columnTitle);

    ColumnTitle = columnTitle;
    _formatString = formatString;
    _propertyPathBindings = new PropertyPathBindingCollection (null);
    _propertyPathBindings.AddRange (propertyPathIdentifiers);
  }

  /// <summary> Simple Constructor. </summary>
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
  /// <value> 
  ///   A <see cref="string"/> containing zero or more format items. The indices must match the 
  ///   order of the <see cref="BusinessObjectPropertyPath"/> objects to be formatted.
  /// </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Format")]
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
  /// <value> A collection of <see cref="PropertyPathBinding"/> objects. </value>
  [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
  [ListBindable (false)]
  [Category ("Data")]
  public PropertyPathBindingCollection PropertyPathBindings
  {
    get 
    { 
      return _propertyPathBindings; 
    }
  }

  /// <summary>
  ///   The text displayed in the column title. Must not be empty or <see langword="null"/>.
  /// </summary>
  /// <value> A <see cref="string"/> representing the title of this column. </value>
  [Description ("The assigned value of the column title, must not be empty or null.")]
  [DefaultValue ("")]
  public override string ColumnTitle
  {
    get { return base.ColumnTitle; }
    set 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("ColumnTitle", value);
      base.ColumnTitle = value;
    }
  }
}

}

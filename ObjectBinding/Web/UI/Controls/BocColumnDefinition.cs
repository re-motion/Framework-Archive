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
using Rubicon.ObjectBinding;
using Rubicon.ObjectBinding.Design;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> A BocColumnDefinition defines how to display a column of a list. </summary>
[Editor (typeof(ExpandableObjectConverter), typeof(UITypeEditor))]
public abstract class BocColumnDefinition
{
  private const string c_commandIDSuffix = "_Command";

  /// <summary> The programmatic name of the <see cref="BocColumnDefinition"/>. </summary>
  private string _columnID;
  /// <summary> The text displayed in the column title. </summary>
  private string _columnTitle;
  /// <summary> The width of the column. </summary>
  private Unit _width; 
  /// <summary> The <see cref="BocListItemCommand"/> rendered in this column. </summary>
  private BocListItemCommand _command;
  /// <summary>
  ///   The <see cref="IBusinessObjectBoundWebControl"/> to which this 
  ///   <see cref="BocColumnDefinition"/> belongs. 
  /// </summary>
  private IBusinessObjectBoundWebControl _ownerControl;

  /// <summary> 
  ///   Initializes a new instance of the <see cref="BocColumnDefinition"/> class with a 
  ///   title, a width, and the <see cref="BocListItemCommand"/>.
  /// </summary>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  /// <param name="command"> The <see cref="BocListItemCommand"/> rendered in this column. </param>
  public BocColumnDefinition (string columnTitle, Unit width, BocListItemCommand command)
  {
    _columnTitle = StringUtility.NullToEmpty (columnTitle);
    _width = width;
    _command = command;
  }

  /// <summary> 
  ///   Initializes a new instance of the <see cref="BocColumnDefinition"/> class with a 
  ///   title and a width.
  /// </summary>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  public BocColumnDefinition (string columnTitle, Unit width)
    : this (columnTitle, width, new BocListItemCommand())
  {
  }

  /// <summary> Initializes a new instance of the <see cref="BocColumnDefinition"/> class. </summary>
  public BocColumnDefinition()
    : this (null, Unit.Empty)
  {
  }

  /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
  protected virtual void OnOwnerControlChanged()
  {
  }

  /// <summary>
  ///   Returns a <see cref="string"/> that represents this <see cref="BocColumnDefinition"/>.
  /// </summary>
  /// <returns>
  ///   Returns the <see cref="ColumnTitle"/>, followed by the class name of the instance.
  /// </returns>
  public override string ToString()
  {
    string displayName = ColumnID;
    if (StringUtility.IsNullOrEmpty (displayName))
      displayName = ColumnTitle;
    if (StringUtility.IsNullOrEmpty (displayName))
      return DisplayedTypeName;
    else
      return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
  }

  /// <summary> Gets the programmatic name of the <see cref="BocColumnDefinition"/>. </summary>
  /// <value>
  ///   A <see cref="string"/> providing an identifier for the <see cref="BocColumnDefinition"/>. 
  /// </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Misc")]
  [Description ("The programmatic name of the column definition.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  [ParenthesizePropertyName (true)]
  public string ColumnID
  {
    get { return _columnID; }
    set { _columnID = value; }
  }

  /// <summary> Gets the displayed value of the column title. </summary>
  /// <remarks> Override this property to change the way the column title text is generated. </remarks>
  /// <value> A <see cref="string"/> representing this column's title row contents. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual string ColumnTitleDisplayValue
  {
    get { return ColumnTitle; }
  }

  /// <summary> Gets or sets the text displayed in the column title. </summary>
  /// <remarks>
  ///   Override this property to add validity checks to the set accessor.
  ///   The get accessor should return the value verbatim.
  /// </remarks>
  /// <value> A <see cref="string"/> representing the manually set title of this column. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The manually assigned value of the column title, can be empty.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public virtual string ColumnTitle
  {
    get { return _columnTitle; }
    set { _columnTitle = StringUtility.NullToEmpty (value); }
  }

  /// <summary> Gets or sets the width of the column definition. </summary>
  /// <value> A <see cref="Unit"/> providing the width of this column when it is rendered. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Layout")]
  [Description ("The width of the rendered column.")]
  [DefaultValue(typeof (Unit), "")]
  [NotifyParentProperty (true)]
  public Unit Width 
  { 
    get { return _width; } 
    set { _width = value; }
  }

  /// <summary> Gets or sets the <see cref="BocListItemCommand"/> rendered in this column. </summary>
  /// <value> A <see cref="BocListItemCommand"/>. </value>
  [PersistenceMode (PersistenceMode.InnerProperty)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Action")]
  [Description ("The command rendered in this column.")]
  [NotifyParentProperty (true)]
  public BocListItemCommand Command
  {
    get { return _command; }
    set 
    { 
      _command = value; 
      if (_ownerControl != null)
        _command.OwnerControl = _ownerControl;
    }
  }

  /// <summary> Controls the persisting of the <see cref="Command"/>. </summary>
  /// <remarks> 
  ///   Does not persist <see cref="BocListItemCommand"/> objects with a 
  ///   <c>Command.Type</c> set to <see cref="CommandType.None"/>.
  /// </remarks>
  /// <returns> 
  ///   <see langref="true"/> if the <see cref="BocListItemCommand"/> object's
  ///   <c>Command.Type</c> is set to <see cref="CommandType.None"/>.
  /// </returns>
  private bool ShouldSerializeCommand()
  {
    if (_command == null)
      return false;

    if (_command.Type == CommandType.None)
      return false;
    else
      return true;
  }

  /// <summary> Sets the <see cref="Command"/> to it's default value. </summary>
  /// <remarks> 
  ///   The defualt value is a <see cref="BocListItemCommand"/> object with a 
  ///   <c>Command.Type</c> set to <see cref="CommandType.None"/>.
  /// </remarks>
  private void ResetCommand()
  {
    _command = new BocListItemCommand();
    _command.Type = CommandType.None;
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected virtual string DisplayedTypeName
  {
    get { return "ColumnDefinition"; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectBoundWebControl"/> 
  ///   to which this <see cref="BocColumnDefinition"/> belongs. 
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
        if (_command != null)
          _command.OwnerControl = value;
        OnOwnerControlChanged();
      }
    }
  }
}

/// <summary> A column definition containing no data, only the <see cref="BocListItemCommand"/>. </summary>
public class BocCommandColumnDefinition: BocColumnDefinition
{
  /// <summary> The text representing the command on the rendered page. </summary>
  private object _label;
  /// <summary> The image representing the command on the rendered page. </summary>
  private string _iconPath;

  /// <summary> 
  ///   Initializes a new instance of the <see cref="BocCommandColumnDefinition"/> class with a
  ///   title, a width, a <see cref="BocListItemCommand"/>, and the label and icon representing 
  ///   the command on the rendered page.
  /// </summary>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  /// <param name="command"> The <see cref="BocListItemCommand"/> rendered in this column. </param>
  /// <param name="label"> The text representing the command on the rendered page. </param>
  /// <param name="iconPath"> The image representing the command on the rendered page. </param>
  public BocCommandColumnDefinition (
      string columnTitle, 
      Unit width, 
      BocListItemCommand command, 
      object label, 
      string iconPath)
    : base (columnTitle, width, command)
  {
    ArgumentUtility.CheckNotNull ("command", command);

    _iconPath = iconPath;
    _label = label;
  }

  /// <summary> Initializes a new instance of the <see cref="BocCommandColumnDefinition"/> class. </summary>
  public BocCommandColumnDefinition()
    : base ()
  {
  }

  /// <summary>
  ///   Returns a <see cref="string"/> that represents this <see cref="BocColumnDefinition"/>.
  /// </summary>
  /// <returns> Returns <see cref="Label"/>, followed by the the class name of the instance.  </returns>
  public override string ToString()
  {
    string displayName = ColumnID;
    if (StringUtility.IsNullOrEmpty (displayName))
      displayName = ColumnTitle;
    if (StringUtility.IsNullOrEmpty (displayName))
      displayName = Label;
    if (StringUtility.IsNullOrEmpty (displayName))
      return DisplayedTypeName;
    else
      return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
  }

  /// <summary> Gets or sets the text representing the command in the rendered page. </summary>
  /// <value> A <see cref="string"/> representing the command. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The text representing the command in the rendered page.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string Label
  {
    get { return (_label != null) ? _label.ToString() : string.Empty; }
    set { _label = value; }
  }

  /// <summary> Gets or sets the image representing the command in the rendered page. </summary>
  /// <value> An <see cref="Image"/> representing the command. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The relative url to image representing the command in the rendered page.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string IconPath 
  {
    get { return _iconPath; }
    set { _iconPath = value; }
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "CommandColumnDefinition"; }
  }
}

/// <summary> A column definition for displaying data. </summary>
public abstract class BocValueColumnDefinition: BocColumnDefinition
{
  /// <summary>
  ///   Initializes a new instance of the <see cref="BocValueColumnDefinition"/> class with a 
  ///   title, a width, and the <see cref="BocListItemCommand"/>.
  /// </summary>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  /// <param name="command"> The <see cref="BocListItemCommand"/> rendered in this column. </param>
  public BocValueColumnDefinition (string columnTitle, Unit width, BocListItemCommand command)
    : base (columnTitle, width, command)
  {
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="BocValueColumnDefinition"/> class with a 
  ///   title and a width.
  /// </summary>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  public BocValueColumnDefinition (string columnTitle, Unit width)
    : base (columnTitle, width)
  {
  }

  /// <summary> Initializes a new instance of the <see cref="BocValueColumnDefinition"/> class. </summary>
  private BocValueColumnDefinition()
    : base (null, Unit.Empty)
  {
  }

  /// <summary> Creates a string representation of the data displayed in this column. </summary>
  /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
  /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
  public abstract string GetStringValue (IBusinessObject obj);

  /// <summary> Gets the human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "ValueColumnDefinition"; }
  }
}

/// <summary> A column definition for displaying a single property path. </summary>
/// <remarks>
///   Note that using the methods of <see cref="BusinessObjectPropertyPath"/>, 
///   the original value of this property can be retreived or changed.
/// </remarks>
public class BocSimpleColumnDefinition: BocValueColumnDefinition, IBusinessObjectClassSource
{
  /// <summary>
  ///   A format string describing how the value accessed through the 
  ///   <see cref="BusinessObjectPropertyPath"/> object is formatted.
  /// </summary>
  private string _formatString;

  /// <summary>
  ///   The <see cref="PropertyPathBinding"/> used to store the <see cref="PropertyPath"/> 
  ///   internally.
  /// </summary>
  private PropertyPathBinding _propertyPathBinding;

  /// <summary>
  ///   Initializes a new instance of the <see cref="BocSimpleColumnDefinition"/> class with a 
  ///   title, a width, a <see cref="BocListItemCommand"/>, a format string describing how the
  ///   value shall be formatted and the <see cref="BusinessObjectPropertyPath"/> used to access
  ///   the value.
  /// </summary>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  /// <param name="command"> The <see cref="BocListItemCommand"/> rendered in this column. </param>
  /// <param name="formatString"> 
  ///   A format string describing how the value accessed through the 
  ///   <see cref="BusinessObjectPropertyPath"/> object is formatted.
  /// </param>
  /// <param name="propertyPath">
  ///   The <see cref="BusinessObjectPropertyPath"/> used by <see cref="GetStringValue"/> 
  ///   to access the value of an <see cref="IBusinessObject"/>. Must not be <see langword="null"/>.
  /// </param>
  public BocSimpleColumnDefinition (
      string columnTitle, 
      Unit width,
      BocListItemCommand command,
      string formatString,
      BusinessObjectPropertyPath propertyPath)
    : base (columnTitle, width, command)
  {
    ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);
    FormatString = formatString;
    _propertyPathBinding = new PropertyPathBinding (propertyPath);
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="BocSimpleColumnDefinition"/> class with a 
  ///   title, a width, a <see cref="BocListItemCommand"/>, a format string describing how the
  ///   value shall be formatted and the string representation of the
  ///   <see cref="BusinessObjectPropertyPath"/> used to access the value.
  /// </summary>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  /// <param name="command"> The <see cref="BocListItemCommand"/> rendered in this column. </param>
  /// <param name="formatString"> 
  ///   A format string describing how the value accessed through the 
  ///   <see cref="BusinessObjectPropertyPath"/> object is formatted.
  /// </param>
  /// <param name="propertyPathIdentifier">
  ///   The string representation of the <see cref="PropertyPath"/>. Must not be 
  ///   <see langword="null"/> or emtpy.
  /// </param>
  public BocSimpleColumnDefinition (
      string columnTitle, 
      Unit width,
      BocListItemCommand command, 
      string formatString,
      string propertyPathIdentifier)
    : base (columnTitle, width, command)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);
    FormatString = formatString;
    _propertyPathBinding = new PropertyPathBinding (propertyPathIdentifier);
  }

  /// <summary> Initializes a new instance of the <see cref="BocSimpleColumnDefinition"/> class. </summary>
  public BocSimpleColumnDefinition()
    : base (string.Empty, Unit.Empty)
  {
    _formatString = string.Empty;
    _propertyPathBinding = new PropertyPathBinding();
  }

  /// <summary>
  ///   Passes the new <see cref="BocColumnDefinition.OwnerControl"/> to the 
  ///   <see cref="PropertyPathBindingCollection"/>.
  /// </summary>
  protected override void OnOwnerControlChanged()
  {
    _propertyPathBinding.OwnerControl = OwnerControl;
    base.OnOwnerControlChanged();
  }

  /// <summary> Creates a string representation of the data displayed in this column. </summary>
  /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
  /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
  public override string GetStringValue (IBusinessObject obj)
  {
    if (PropertyPath == null)
      return string.Empty;

    string formatString = _formatString;
    if (StringUtility.IsNullOrEmpty (formatString))
    {
      IBusinessObjectProperty lastProperty = PropertyPath.LastProperty;
      bool isDate = lastProperty is IBusinessObjectDateProperty;
      bool isDateTime = lastProperty is IBusinessObjectDateTimeProperty;

      if (isDate)
        formatString = "d";
      else if (isDateTime)
        formatString = "g";
    }

    return string.Format ("{0:" + formatString + "}", PropertyPath.GetValue (obj));
  }

  /// <summary>
  ///   Gets or sets the format string describing how the value accessed through the 
  ///   <see cref="BusinessObjectPropertyPath"/> object is formatted.
  /// </summary>
  /// <value> 
  ///   A <see cref="string"/> representing a valid format string. 
  /// </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Format")]
  [Description ("A format string describing how the value accessed through the Property Path is formatted.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string FormatString
  {
    get
    {
      return _formatString;
    }
    set
    {
      _formatString = StringUtility.NullToEmpty (value); 
    }
  }

  /// <summary>
  ///   Gets or sets the <see cref="BusinessObjectPropertyPath"/> used by 
  ///   <see cref="GetStringValue"/> to access the value of an <see cref="IBusinessObject"/>. 
  ///   Must not be <see langword="null"/>.
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
        return _propertyPathBinding.PropertyPath;
      }
      else
      {
        if (OwnerControl.DataSource != null)
          return _propertyPathBinding.PropertyPath;
        else
          return null;
      }
    }
    set
    {
      ArgumentUtility.CheckNotNull ("PropertyPath", value);
      _propertyPathBinding.PropertyPath = value;
    }
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

  /// <summary> 
  ///   Gets or sets the <see cref="IBusinessObjectDataSource"/> used to evaluate the 
  ///   <see cref="PropertyPathIdentifier"/>. 
  /// </summary>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public IBusinessObjectDataSource DataSource
  {
    get
    {
      return _propertyPathBinding.DataSource; 
    }
    set 
    {
      _propertyPathBinding.DataSource = value; 
    }
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
    get { return "SimpleColumnDefinition"; }
  }

  IBusinessObjectClass IBusinessObjectClassSource.BusinessObjectClass
  {
    get 
    {
      if (OwnerControl != null)
      {
        if (OwnerControl.Property is IBusinessObjectReferenceProperty)
          return ((IBusinessObjectReferenceProperty)OwnerControl.Property).ReferenceClass;
        else if (OwnerControl.DataSource != null)
          return OwnerControl.DataSource.BusinessObjectClass;
      }
      return null; 
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
  ///   A format string describing how the values accessed through the 
  ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
  /// </summary>
  private string _formatString;

  /// <summary>
  ///   The collection of <see cref="PropertyPathBinding"/> objects used by
  ///   <see cref="GetStringValue"/> to access the values of an <see cref="IBusinessObject"/>.
  /// </summary>
  private PropertyPathBindingCollection _propertyPathBindings;

  /// <summary>
  ///   Initializes a new instance of the <see cref="BocCompoundColumnDefinition"/> class with a 
  ///   title, a width, a <see cref="BocListItemCommand"/>, a format string describing how the
  ///   values shall be merged and the <see cref="BusinessObjectPropertyPath"/> objects used 
  ///   to access the values.
  /// </summary>
  /// <param name="command"> The <see cref="BocListItemCommand"/> rendered in this column. </param>
  /// <param name="formatString"> 
  ///   A format string describing how the values accessed through the 
  ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
  /// </param>
  /// <param name="propertyPaths">
  ///   The <see cref="BusinessObjectPropertyPath"/> objects used to access the values that will be
  ///   rendered in this column.
  /// </param>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  public BocCompoundColumnDefinition (
      string columnTitle, 
      Unit width,
      BocListItemCommand command, 
      string formatString,
      BusinessObjectPropertyPath[] propertyPaths)
    : base (columnTitle, width, command)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPaths", propertyPaths);
    ArgumentUtility.CheckNotNullOrEmpty ("columnTitle", columnTitle);

    ColumnTitle = columnTitle;
    _formatString = formatString;
    _propertyPathBindings = new PropertyPathBindingCollection (null);
    _propertyPathBindings.AddRange (propertyPaths);
  }

  /// <summary>
  ///   Initializes a new instance of the <see cref="BocCompoundColumnDefinition"/> class with a 
  ///   title, a width, a <see cref="BocListItemCommand"/>, a format string describing how the
  ///   values shall be merged and the string representations of the
  ///   <see cref="BusinessObjectPropertyPath"/> objects used to access the values.
  /// </summary>
  /// <param name="columnTitle"> The text displayed in the title row. </param>
  /// <param name="width"> The width of the rendered column. </param>
  /// <param name="command"> The <see cref="BocListItemCommand"/> rendered in this column. </param>
  /// <param name="formatString"> 
  ///   A format string describing how the values accessed through the 
  ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
  /// </param>
  /// <param name="propertyPathIdentifiers">
  ///   The strings identifying the <see cref="BusinessObjectPropertyPath"/> objects used to access
  ///   the values that will be rendered in this column.
  /// </param>
  public BocCompoundColumnDefinition (
      string columnTitle, 
      Unit width,
      BocListItemCommand command, 
      string formatString,
      string[] propertyPathIdentifiers)
    : base (columnTitle, width, command)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifiers", propertyPathIdentifiers);
    ArgumentUtility.CheckNotNullOrEmpty ("columnTitle", columnTitle);

    ColumnTitle = columnTitle;
    FormatString = formatString;
    _propertyPathBindings = new PropertyPathBindingCollection (null);
    _propertyPathBindings.AddRange (propertyPathIdentifiers);
  }

  /// <summary> Initializes a new instance of the <see cref="BocCompoundColumnDefinition"/> class. </summary>
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
    object[] values = new object[_propertyPathBindings.Count];
    for (int i = 0; i < _propertyPathBindings.Count; ++i)
      values[i] = _propertyPathBindings[i].PropertyPath.GetValue (obj);

    return string.Format (_formatString, values);
  }

  /// <summary>
  ///   Passes the new <see cref="BocColumnDefinition.OwnerControl"/> to the 
  ///   <see cref="PropertyPathBindingCollection"/>.
  /// </summary>
  protected override void OnOwnerControlChanged()
  {
    _propertyPathBindings.OwnerControl = OwnerControl;
    base.OnOwnerControlChanged();
  }

  /// <summary>
  ///   Gets or sets the format string describing how the values accessed through the 
  ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
  /// </summary>
  /// <value> 
  ///   A <see cref="string"/> containing a format item for each 
  ///   <see cref="BusinessObjectPropertyPath"/> to be displayed. The indices must match the 
  ///   order of the <see cref="BusinessObjectPropertyPath"/> objects to be formatted.
  /// </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Format")]
  [Description ("A format string describing how the values accessed through the Property Path are merged.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string FormatString
  {
    get { return _formatString; }
    set { _formatString = StringUtility.NullToEmpty (value); }
  }

  /// <summary>
  ///   Gets the collection of <see cref="PropertyPathBinding"/> objects used by
  ///   <see cref="GetStringValue"/> to access the values of an <see cref="IBusinessObject"/>.
  /// </summary>
  /// <value> A collection of <see cref="PropertyPathBinding"/> objects. </value>
  [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
  [Category ("Data")]
  [Description ("The Property Paths used to access the values of Business Object.")]
  [NotifyParentProperty (true)]
  public PropertyPathBindingCollection PropertyPathBindings
  {
    get { return _propertyPathBindings; }
  }

  /// <summary>
  ///   Gets or sets the text displayed in the column title. 
  ///   Must not be empty or <see langword="null"/>.
  /// </summary>
  /// <value> A <see cref="string"/> representing the title of this column. </value>
  [Description ("The assigned value of the column title, must not be empty.")]
  [DefaultValue ("")]
  [NotifyParentProperty (true)]
  public override string ColumnTitle
  {
    get { return base.ColumnTitle; }
    set 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("ColumnTitle", value);
      base.ColumnTitle = value;
    }
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "CompoundColumnDefinition"; }
  }
}

}

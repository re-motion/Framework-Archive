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
public abstract class BocColumnDefinition: BusinessObjectControlItem
{
  private string _columnID;
  private string _columnTitle;
  /// <summary> The width of the column. </summary>
  private Unit _width; 

  /// <summary> Initializes a new instance of the <see cref="BocColumnDefinition"/> class. </summary>
  public BocColumnDefinition()
  {
    _columnTitle = string.Empty;
    _width = Unit.Empty;
  }

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
  /// <value> A <see cref="string"/> providing an identifier for the <see cref="BocColumnDefinition"/>. </value>
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

  /// <summary> Gets the human readable name of this type. </summary>
  protected virtual string DisplayedTypeName
  {
    get { return "ColumnDefinition"; }
  }
}

/// <summary> A column defintion with the possibility of rendering a command in the cell. </summary>
public abstract class BocCommandEnabledColumnDefinition: BocColumnDefinition
{
  /// <summary> The <see cref="BocListItemCommand"/> rendered in this column. </summary>
  private SingleControlItemCollection _command;

  /// <summary> Initializes a new instance of the <see cref="BocCommandEnabledColumnDefinition"/> class. </summary>
  public BocCommandEnabledColumnDefinition()
  {
    _command = new SingleControlItemCollection (new BocListItemCommand(), new Type[] {typeof (BocListItemCommand)});
  }

  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged();
    if (Command != null)
      Command.OwnerControl = OwnerControl;
  }

  /// <summary> Gets or sets the <see cref="BocListItemCommand"/> rendered in this column. </summary>
  /// <value> A <see cref="BocListItemCommand"/>. </value>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Category ("Behavior")]
  [Description ("The command rendered in this column.")]
  [NotifyParentProperty (true)]
  public BocListItemCommand Command
  {
    get { return (BocListItemCommand) _command.Item; }
    set 
    { 
      _command.Item = value; 
      if (OwnerControl != null)
        _command.Item.OwnerControl = (Control) OwnerControl;
    }
  }

  private bool ShouldSerializeCommand()
  {
    if (Command == null)
      return false;

    if (Command.Type == CommandType.None)
      return false;
    else
      return true;
  }

  /// <summary> Sets the <see cref="Command"/> to its default value. </summary>
  /// <remarks> 
  ///   The default value is a <see cref="BocListItemCommand"/> object with a <c>Command.Type</c> set to 
  ///   <see cref="CommandType.None"/>.
  /// </remarks>
  private void ResetCommand()
  {
    if (Command != null)
    {
      Command = (BocListItemCommand) Activator.CreateInstance (Command.GetType());
      Command.Type = CommandType.None;
    }
  }

  [PersistenceMode (PersistenceMode.InnerProperty)]
  [Browsable (false)]
  public SingleControlItemCollection PersistedCommand
  {
    get { return _command; }
  }

  /// <summary> Controls the persisting of the <see cref="Command"/>. </summary>
  /// <remarks> 
  ///   Does not persist <see cref="BocListItemCommand"/> objects with a <c>Command.Type</c> set to 
  ///   <see cref="CommandType.None"/>.
  /// </remarks>
  private bool ShouldSerializePersistedCommand()
  {
    return ShouldSerializeCommand();
  }
}

/// <summary> A column definition containing no data, only the <see cref="BocListItemCommand"/>. </summary>
public class BocCommandColumnDefinition: BocCommandEnabledColumnDefinition
{
  /// <summary> The text representing the command on the rendered page. </summary>
  private string _text;
  /// <summary> The image representing the command on the rendered page. </summary>
  private string _iconPath;

  /// <summary> Initializes a new instance of the <see cref="BocCommandColumnDefinition"/> class. </summary>
  public BocCommandColumnDefinition()
  {
  }

  /// <summary> Returns a <see cref="string"/> that represents this <see cref="BocColumnDefinition"/>. </summary>
  /// <returns> Returns <see cref="Text"/>, followed by the the class name of the instance.  </returns>
  public override string ToString()
  {
    string displayName = ColumnID;
    if (StringUtility.IsNullOrEmpty (displayName))
      displayName = ColumnTitle;
    if (StringUtility.IsNullOrEmpty (displayName))
      displayName = Text;
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
  public string Text
  {
    get { return StringUtility.NullToEmpty (_text); }
    set { _text = value; }
  }

  /// <summary> Depracated property. Only used for designer. Use <see cref="Text"/> instead. </summary>
  /// TODO: Remove in a few weeks. (End of April).
  /// Used right now only in demo applications
  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  [Browsable (false)]
  [Obsolete ("Not functional. Use 'Text' instead.")]
  public string Label
  {
    get { return ""; }
    set 
    {
      if (StringUtility.IsNullOrEmpty (Text))
        Text = value; 
    }
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

/// <summary> A column definition for displaying data and an optional command. </summary>
public abstract class BocValueColumnDefinition: BocCommandEnabledColumnDefinition
{
  /// <summary> Initializes a new instance of the <see cref="BocValueColumnDefinition"/> class. </summary>
  public BocValueColumnDefinition()
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

  /// <summary> Initializes a new instance of the <see cref="BocSimpleColumnDefinition"/> class. </summary>
  public BocSimpleColumnDefinition()
  {
    _formatString = string.Empty;
    _propertyPathBinding = new PropertyPathBinding();
  }

  /// <summary> Passes the new OwnerControl to the <see cref="PropertyPathBindingCollection"/>. </summary>
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
      if (PropertyPath.LastProperty is IBusinessObjectDateProperty)
        formatString = "d";
      else if (PropertyPath.LastProperty is IBusinessObjectDateTimeProperty)
        formatString = "g";
    }

    return PropertyPath.GetString (obj, StringUtility.EmptyToNull(formatString));
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
    get { return _formatString; }
    set { _formatString = StringUtility.NullToEmpty (value); }
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
    get { return _propertyPathBinding.BusinessObjectClass; }
  } 
}

/// <summary> A column definition for displaying a string made up from different property paths. </summary>
/// <remarks> Note that values in these columnDefinitions can usually not be modified directly. </remarks>
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

  /// <summary> Initializes a new instance of the <see cref="BocCompoundColumnDefinition"/> class. </summary>
  public BocCompoundColumnDefinition()
  {
    _propertyPathBindings = new PropertyPathBindingCollection (null);
    _formatString = string.Empty;
  }

  /// <summary> Creates a string representation of the data displayed in this column. </summary>
  /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
  /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
  public override string GetStringValue (IBusinessObject obj)
  {
    BusinessObjectPropertyPath.Formatter[] formatters = new BusinessObjectPropertyPath.Formatter[_propertyPathBindings.Count];
    for (int i = 0; i < _propertyPathBindings.Count; ++i)
      formatters[i] = new BusinessObjectPropertyPath.Formatter (obj, _propertyPathBindings[i].PropertyPath);

    return string.Format (_formatString, formatters);
  }

  /// <summary> Passes the new OwnerControl to the <see cref="PropertyPathBindingCollection"/>. </summary>
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
  [PersistenceMode(PersistenceMode.InnerProperty)]
  [Category ("Data")]
  [Description ("The Property Paths used to access the values of Business Object.")]
  [NotifyParentProperty (true)]
  public PropertyPathBindingCollection PropertyPathBindings
  {
    get { return _propertyPathBindings; }
  }

  /// <summary> Gets or sets the text displayed in the column title. Must not be empty or <see langword="null"/>. </summary>
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

/// <summary> A column definition used for switching between edit-mode and returning from it via save and cancel. </summary>
public class BocEditDetailsColumnDefinition: BocColumnDefinition
{
  private string _editText;
  private IconInfo _editIcon;
  private string _saveText;
  private IconInfo _saveIcon;
  private string _cancelText;
  private IconInfo _cancelIcon;

  /// <summary> Initializes a new instance of the <see cref="BocCommandColumnDefinition"/> class. </summary>
  public BocEditDetailsColumnDefinition()
  {
    _editIcon = new IconInfo();
    _saveIcon = new IconInfo();
    _cancelIcon = new IconInfo();
  }

  /// <summary> Gets or sets the text representing the edit command in the rendered page. </summary>
  /// <value> A <see cref="string"/> representing the edit command. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The text representing the edit command in the rendered page.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string EditText
  {
    get { return StringUtility.NullToEmpty (_editText); }
    set { _editText = value; }
  }

  /// <summary> Gets or sets the image representing the edit command in the rendered page. </summary>
  /// <value> An <see cref="IconInfo"/> representing the edit command. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Appearance")]
  [Description ("The image representing the edit command in the rendered page.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public IconInfo EditIcon
  {
    get { return _editIcon; }
    set { _editIcon = value; }
  }

  private bool ShouldSerializeEditIcon()
  {
    return IconInfo.ShouldSerialize (_editIcon);
  }

  private void ResetEditIcon()
  {
    _editIcon.Reset();
  }


  /// <summary> Gets or sets the text representing the save command in the rendered page. </summary>
  /// <value> A <see cref="string"/> representing the save command. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The text representing the save command in the rendered page.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string SaveText
  {
    get { return StringUtility.NullToEmpty (_saveText); }
    set { _saveText = value; }
  }

  /// <summary> Gets or sets the image representing the save command in the rendered page. </summary>
  /// <value> An <see cref="Image"/> representing the save command. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Appearance")]
  [Description ("The relative url to image representing the save command in the rendered page.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public IconInfo SaveIcon
  {
    get { return _saveIcon; }
    set { _saveIcon = value; }
  }

  private bool ShouldSerializeSaveIcon()
  {
    return IconInfo.ShouldSerialize (_saveIcon);
  }

  private void ResetSaveIcon()
  {
    _saveIcon.Reset();
  }

  /// <summary> Gets or sets the text representing the cancel command in the rendered page. </summary>
  /// <value> A <see cref="string"/> representing the cancel command. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [Category ("Appearance")]
  [Description ("The text representing the cancel command in the rendered page.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public string CancelText
  {
    get { return StringUtility.NullToEmpty (_cancelText); }
    set { _cancelText = value; }
  }

  /// <summary> Gets or sets the image representing the cancel command in the rendered page. </summary>
  /// <value> An <see cref="IconInfo"/> representing the cancel command. </value>
  [PersistenceMode (PersistenceMode.Attribute)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
  [Category ("Appearance")]
  [Description ("The image representing the cancel command in the rendered page.")]
  [DefaultValue("")]
  [NotifyParentProperty (true)]
  public IconInfo CancelIcon
  {
    get { return _cancelIcon; }
    set { _cancelIcon = value; }
  }

  private bool ShouldSerializeCancelIcon()
  {
    return IconInfo.ShouldSerialize (_cancelIcon);
  }

  private void ResetCancelIcon()
  {
    _cancelIcon.Reset();
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected override string DisplayedTypeName
  {
    get { return "EditDetailsColumnDefinition"; }
  }
}

/// <summary> A column definition using <see cref="IBocCustomColumnDefinitionCell"/> for rendering the data. </summary>
public class BocCustomColumnDefinition: BocColumnDefinition, IBusinessObjectClassSource
{
  private PropertyPathBinding _propertyPathBinding;
  private IBocCustomColumnDefinitionCell _customCell;
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
  public IBocCustomColumnDefinitionCell CustomCell
  {
    get 
    {
      if (_customCell == null)
      {
        Type type = TypeUtility.GetType (_customCellType, true, false);
        object[] arguments = null;
        if (! StringUtility.IsNullOrEmpty (_customCellArgument))
          arguments = new object[] {_customCellArgument};
        _customCell = (IBocCustomColumnDefinitionCell) Activator.CreateInstance (type, arguments);
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

/// <summary> 
///   This interface allows for customized rendering of a column's contents and registering for post backs.
/// </summary>
public interface IBocCustomColumnDefinitionCell
{
  /// <param name="writer"></param>
  /// <param name="list"> The <see cref="BocList"/> containing the column. </param>
  /// <param name="businessObject"> The <see cref="IBusinessObject"/> to be rendered. </param>
  /// <param name="columnDefiniton"> The column definition of the rendered column. </param>
  /// <param name="columnIndex"> 
  ///   The index of the rendered column. Pass this value to the <paramref name="list"/>'s 
  ///   <see cref="BocList.GetCustomCellPostBackClientHyperlink"/> or 
  ///   <see cref="BocList.GetCustomCellPostBackClientEvent"/>.
  /// </param>
  /// <param name="listIndex"> 
  ///   The index of the <see cref="IBusinessObject"/> in the values collection of the <see cref="BocList"/>.
  ///    Pass this value to the <paramref name="list"/>'s <see cref="BocList.GetCustomCellPostBackClientHyperlink"/> 
  ///    or <see cref="BocList.GetCustomCellPostBackClientEvent"/>.
  /// </param>
  /// <param name="onClick"> 
  ///   A function to be appended to the client side <c>OnClick</c> event handler. The function tasked with
  ///   preventing the row from being selected/highlighted when clicking on the link itself instead of the row.
  /// </param>
  void Render (
      HtmlTextWriter writer, 
      BocList list,
      IBusinessObject businessObject, 
      BocCustomColumnDefinition columnDefiniton, 
      int columnIndex,
      int listIndex,
      string onClick);

  void OnClick (
      BocList list, 
      IBusinessObject businessObject, 
      BocCustomColumnDefinition columnDefiniton, 
      string argument);
}

/// <summary> Represents the method that handles the <see cref="BocList.CustomCellClick"/> event. </summary>
public delegate void BocCustomColumnClickEventHandler (object sender, BocCustomColumnClickEventArgs e);

/// <summary> Provides data for the <see cref="BocList.CustomCellClick"/> event. </summary>
public class BocCustomColumnClickEventArgs: EventArgs
{
  private IBusinessObject _businessObject;
  private BocCustomColumnDefinition _column;
  private string _argument;

  /// <summary> Initializes a new instance. </summary>
  public BocCustomColumnClickEventArgs (
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

}

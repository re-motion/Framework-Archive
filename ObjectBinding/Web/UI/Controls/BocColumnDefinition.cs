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
using Rubicon.Web.UI.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{
/// <summary>
///   A BocColumnDefinition defines how to display a column of a list. 
/// </summary>
[Editor (typeof(ExpandableObjectConverter), typeof(UITypeEditor))]
public abstract class BocColumnDefinition// : IComponent // for designer support
{
  private Unit _width; 
  private string _columnHeader;
  private IBusinessObjectBoundWebControl _ownerControl;

  public BocColumnDefinition (string columnHeader, Unit width)
  {
    _width = width;
    _columnHeader = StringUtility.NullToEmpty (columnHeader);
  }

  private BocColumnDefinition()
    : this (null, Unit.Empty)
  {}

  /// <summary>
  ///   The displayed value of the column header.
  /// </summary>
  /// <remarks>
  ///   Override this property to change the way the column header text is generated.
  /// </remarks>
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public virtual string ColumnHeaderDisplayValue
  {
    get { return ColumnHeader; }
  }

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

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue(typeof (Unit), "")]
  public Unit Width 
  { 
    get { return _width; } 
    set { _width = value; }
  }

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

  protected virtual void OnOwnerControlChanged()
  {}
}

/// <summary>
///   A column definition containing no data, but a <see cref="BocItemCommand"/>.
/// </summary>
public class BocCommandColumnDefinition: BocColumnDefinition
{
  private BocItemCommand _command;
  private object _label;
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
    : base(null, Unit.Empty)// this (null, new EmptyItemCommand(), null, null, Unit.Empty)
  {}

//  protected void RenderLabel (HtmlTextWriter writer)
//  {
//    if (_label != null)
//    {
//      HttpUtility.HtmlEncode (Label, writer);
//    }
//    else
//    {
//      writer.AddAttribute (HtmlTextWriterAttribute.Href, _iconPath);
//      writer.RenderBeginTag (HtmlTextWriterTag.Img);
//      writer.RenderEndTag ();
//    }
//  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  public string Label
  {
    get { return (_label != null) ? _label.ToString() : string.Empty; }
    set { _label = value; }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  public string IconPath 
  {
    get { return _iconPath; }
    set { _iconPath = value; }
  }

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  public BocItemCommand Command
  {
    get { return _command; }
    set { _command = value; }
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
  private BusinessObjectPropertyPathBinding _propertyPathBinding;

  public BocSimpleColumnDefinition (
      BusinessObjectPropertyPath propertyPath,
      string columnHeader, 
      Unit width)
    : base (columnHeader, width)
  {
    ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);
    _propertyPathBinding = new BusinessObjectPropertyPathBinding (propertyPath);
  }

  public BocSimpleColumnDefinition (
      string propertyPathIdentifier,
      string columnHeader, 
      Unit width)
    : base (columnHeader, width)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);
    _propertyPathBinding = new BusinessObjectPropertyPathBinding (propertyPathIdentifier);
  }

  public BocSimpleColumnDefinition ()
    : base (string.Empty, Unit.Empty)
  {
    _propertyPathBinding = new BusinessObjectPropertyPathBinding();
  }

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

  public override string GetStringValue (IBusinessObject obj)
  {
    if (PropertyPath != null)
      return PropertyPath.GetStringValue (obj);
    else
      return string.Empty;
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
//  private string _columnHeader;
  private string _formatString;

  private BusinessObjectPropertyPathBindingCollection _propertyPathBindings;

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
    _propertyPathBindings = new BusinessObjectPropertyPathBindingCollection (null, propertyPaths);
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
    _propertyPathBindings = new BusinessObjectPropertyPathBindingCollection (null, propertyPathIdentifiers);
  }

  public BocCompoundColumnDefinition()
    : base (string.Empty, Unit.Empty)
  {
    _propertyPathBindings = new BusinessObjectPropertyPathBindingCollection (null);
    _formatString = string.Empty;
  }

  protected override void OnOwnerControlChanged()
  {
    base.OnOwnerControlChanged();

    _propertyPathBindings.OwnerControl = OwnerControl;
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  public string FormatString
  {
    get { return _formatString; }
    set { _formatString = value; }
  }

  [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
  [ListBindable (false)]
  [DefaultValue ((string) null)]
  public BusinessObjectPropertyPathBindingCollection PropertyPathBindings
  {
    get 
    { 
      return _propertyPathBindings; 
    }
  }

//  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
//  [Browsable (false)]
//  public BusinessObjectPropertyPath PropertyPath 
//  { 
//    get
//    {
//      if (OwnerControl == null)
//        throw new InvalidOperationException ("PropertyPath could not be resolved because the object is not part of an IBusinessObjectBoundWebControl.");
//      
//      if (! ControlHelper.IsDesignMode (OwnerControl))
//      {
//        _propertyPathBinding.DataSource = OwnerControl.DataSource;
//        return _propertyPathBinding.PropertyPath;
//      }
//      else
//        return null;
//    }
//    set
//    {
//      _propertyPathBinding.PropertyPath = value;
//    }
//  }
//
//  [PersistenceMode (PersistenceMode.Attribute)]
//  [DefaultValue("")]
//  public string PropertyPathIdentifier
//  { 
//    get
//    { 
//      return _propertyPathBinding.PropertyPathIdentifier; 
//    }
//    set
//    { 
//      _propertyPathBinding.PropertyPathIdentifier = value; 
//    }
//  }

  public override string GetStringValue (IBusinessObject obj)
  {
    string[] strings = new string[_propertyPathBindings.Count];
    for (int i = 0; i < _propertyPathBindings.Count; ++i)
      strings[i] = _propertyPathBindings[i].PropertyPath.GetStringValue (obj);

    return string.Format (_formatString, strings);
  }

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

/// <summary>
///   A BocColumnDefinition defines how to display a column of a list. 
/// </summary>
internal class BocDesignerColumnDefinition : BocColumnDefinition
{
  public BocDesignerColumnDefinition (string columnHeader, Unit width)
    : base (columnHeader, width)
  {}
}

}

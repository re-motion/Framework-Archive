using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Globalization;
using Rubicon.Utilities;
using Rubicon.ObjectBinding.Web.Design;
using System.Collections;
using Rubicon.Web.UI.Controls;
using Rubicon.Web.UI.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{
/// <summary>
///   A BocColumnDefinition defines how to display a column of a list. 
/// </summary>
[DesignTimeVisible (false)]
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
    get { return _ownerControl; }
    set { _ownerControl = value; }
  }

}

/// <summary>
///   A column definition containing no data, but an <see cref="ItemCommand"/>.
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
  private BusinessObjectPropertyPath _propertyPath;
  private string _propertyPathIdentifier;

  public BocSimpleColumnDefinition (
      BusinessObjectPropertyPath propertyPath,
      string columnHeader, 
      Unit width)
    : base (columnHeader, width)
  {
    ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

    _propertyPath = propertyPath;
  }

  public BocSimpleColumnDefinition()
    : this (
        new BusinessObjectPropertyPath (
          new IBusinessObjectProperty[] {new EmptyBusinessObjectProperty()}),
        null, 
        Unit.Empty)
  {}

  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [Browsable (false)]
  public BusinessObjectPropertyPath PropertyPath 
  { 
    get 
    {
      if (_propertyPath != null)
        return _propertyPath; 

      if (OwnerControl == null)
        throw new InvalidOperationException ("PropertyPath could not be resolved because the object is not part of an IBusinessObjectBoundWebControl.");

      bool isDesignMode = ControlHelper.IsDesignMode (OwnerControl);
      
      if (! isDesignMode && OwnerControl.DataSource == null)
        throw new InvalidOperationException ("PropertyPath could not be resolved because the DataSource is not set in the containing IBusinessObjectBoundWebControl.");

      _propertyPath = BusinessObjectPropertyPath.Parse (
        OwnerControl.DataSource,
        _propertyPathIdentifier);
      
      return _propertyPath;
    }
  }

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  public string PropertyPathIdentifier
  { 
    get { return _propertyPathIdentifier; }
    set { _propertyPathIdentifier = value; }
  }

  public override string ColumnHeaderDisplayValue
  {
    get 
    {
      bool isHeaderEmpty = StringUtility.IsNullOrEmpty(ColumnHeader);
      return isHeaderEmpty ? _propertyPath.LastProperty.DisplayName : ColumnHeader;  
    }
  }

  public override string GetStringValue (IBusinessObject obj)
  {
    return PropertyPath.GetStringValue (obj);
  }
}

/// <summary>
///   A column definition for displaying a string made up from different property paths.
/// </summary>
/// <remarks>
///   Note that values in these columns can usually not be modified directly.
/// </remarks>
public class BocCompoundColumnDefinition: BocValueColumnDefinition
{
//  private string _columnHeader;
  private string _formatString;
  private BusinessObjectPropertyPath[] _propertyPaths;

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
    _propertyPaths = propertyPaths;
  }

  public BocCompoundColumnDefinition ()
    : this (
      string.Empty, 
      new BusinessObjectPropertyPath[] {
        new BusinessObjectPropertyPath (
          new IBusinessObjectProperty[] {new EmptyBusinessObjectProperty()})},
      string.Empty, 
      Unit.Empty)
  {}

  public override string GetStringValue(IBusinessObject obj)
  {
    string[] strings = new string[_propertyPaths.Length];
    for (int i = 0; i < _propertyPaths.Length; ++i)
      strings[i] = _propertyPaths[i].GetStringValue (obj);

    return String.Format (_formatString, strings);
  }

  [Description ("The assigned value of the column header, must not be empty or null.")]
  public override string ColumnHeader
  {
    get { return ColumnHeader; }
    set 
    {
      ArgumentUtility.CheckNotNullOrEmpty ("ColumnHeader", value);
      ColumnHeader = value;
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
 
internal class EmptyBusinessObjectProperty : Rubicon.ObjectBinding.IBusinessObjectProperty
{
  public bool IsList
  {
    get { return false; }
  }

  public IList CreateList (int count)
  {
    throw new InvalidOperationException ("Cannot create lists for non-list properties.");
  }

  public Type ItemType
  {
    get { return typeof (object); }
  }

  public virtual Type PropertyType
  {
    get { return typeof (object);  }
  }

  public string Identifier
  {
    get { return string.Empty; }
  }

  public string DisplayName
  {
    get { return "Empty Property"; }
  }

  public virtual bool IsRequired
  {
    get { return false;  }
  }

  public bool IsAccessible (IBusinessObject obj)
  {
    return false;
  }

  public bool IsReadOnly(IBusinessObject obj)
  {
    return true;
  }
}

[Editor (typeof (BocColumnDefinitionCollectionEditor), typeof (UITypeEditor))]
public  class BocColumnDefinitionCollection : IList, ICollection, IEnumerable
{
  private bool _isEditing;

  private IBusinessObjectBoundWebControl _ownerControl;

  // Events
  public event CollectionChangeEventHandler CollectionChanged;

  // Construction and disposing
  internal BocColumnDefinitionCollection (IBusinessObjectBoundWebControl ownerControl)
  {
    _ownerControl = ownerControl;
    _items = new ArrayList();
  }

  internal BocColumnDefinitionCollection (
      IBusinessObjectBoundWebControl ownerControl, 
      BocColumnDefinition[] columnDefinitions)
    : this (ownerControl)
  {
    _items.AddRange (columnDefinitions);
  }

  public void BeginEdit()
  {
    _isEditing = true;
  }

  public void EndEdit()
  {
    if (_isEditing)
    {
      _isEditing = false;
      OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Refresh, null));
    }
  }

  // Methods
  public virtual int Add (BocColumnDefinition columnDefinition)
  {
    if (isDefault)
    {
      throw new ArgumentException("DefaultColumnDefinitionCollectionChanged");
    }
    //      column.SetDataGridTableInColumn(owner, true);
    //      column.MappingNameChanged += new EventHandler(ColumnStyleMappingNameChanged);
    //      column.PropertyDescriptorChanged += new EventHandler(ColumnStylePropDescChanged);
    //      if ((DataGridTableStyle != null) && (column.Width == -1))
    //      {
    //            column.width = DataGridTableStyle.PreferredColumnWidth;
    //      }

    int count = _items.Add (columnDefinition);
    columnDefinition.OwnerControl = _ownerControl;
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Add, columnDefinition));
    return count;
  }

  public void AddRange (BocColumnDefinition[] columnDefinitions)
  {
    if (columnDefinitions == null)
      throw new ArgumentNullException("columnDefinitions");

    BeginEdit();
    for (int i = 0; i < columnDefinitions.Length; i++)
      Add (columnDefinitions[i]);
    EndEdit();
  }

  public void Clear()
  {
    _items.Clear();
   OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Refresh, null));
  }

  public bool Contains(BocColumnDefinition columnDefinition)
  { 
    int index = _items.IndexOf(columnDefinition); 
    return (index != -1);
  }

  public int IndexOf(BocColumnDefinition element)
  { 
    return _items.IndexOf(element);
  }

  protected void OnCollectionChanged(CollectionChangeEventArgs e)
  {
    if (CollectionChanged != null && !_isEditing)
      CollectionChanged(this, e);
  }

  public void Remove(BocColumnDefinition columnDefinition)
  {
    if (isDefault)
      throw new ArgumentException("DefaultColumnDefinitionCollectionChanged");

    int num1 = -1;
    int num2 = _items.Count;
    for (int num3 = 0; num3 < num2; num3++)
    {
      if (_items[num3] == columnDefinition)
      {
        num1 = num3;
        break;
      }
    }

    if (num1 == -1)
      throw new InvalidOperationException("ColumnDefinitionCollectionMissing");

    RemoveAt(num1);
  }

  public void RemoveAt(int index)
  {
    if (isDefault)
      throw new ArgumentException("DefaultColumnDefinitionCollectionChanged");

    BocColumnDefinition columnDefinition = (BocColumnDefinition) _items[index];
    //      style1.SetDataGridTableInColumn(null, true);
    //      style1.MappingNameChanged -= new EventHandler(ColumnStyleMappingNameChanged);
    //      style1.PropertyDescriptorChanged -= new EventHandler(ColumnStylePropDescChanged);
    _items.RemoveAt(index);
    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, columnDefinition));
  }

  void ICollection.CopyTo(Array array, int index)
  {
    _items.CopyTo (array, index);
  }

  int ICollection.Count
  {
    get  
    { 
      return _items.Count;
    }
  }
  bool ICollection.IsSynchronized
  {
    get
    { 
      return false;
    }
  }
  object ICollection.SyncRoot
  {
    get
    {
      return this;
    }
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return _items.GetEnumerator();
  }

  int IList.Add(object value)
  {
    return Add ((BocColumnDefinition) value);
  }

  void IList.Clear()
  {
    Clear();
  }

  bool IList.Contains(object value)
  {
    return _items.Contains (value);
  }

  bool IList.IsFixedSize
  {
    get  
    { 
      return false;
    }
  }

  bool IList.IsReadOnly
  {
    get
    { 
      return false;
    }
  }

  object IList.this [int index]
  {
    get
    {
      return _items[index];
    }
    set
    {
      throw new NotSupportedException();
    }
  }
 
  int IList.IndexOf (object value)
  {
    return _items.IndexOf (value);
  }

  void IList.Insert(int index, object value)
  {
    throw new NotSupportedException();
  }

  void IList.Remove(object value)
  { 
    Remove((BocColumnDefinition) value);
  }

  void IList.RemoveAt(int index)
  {
    RemoveAt(index);
  }

  public BocColumnDefinition[] ToArray()
  {
    return (BocColumnDefinition[]) _items.ToArray (typeof (BocColumnDefinition));
  }
  // Properties
//      public TestType this[PropertyDescriptor propDesc] { get
//      { int num1 = _items.Count;
//            for (int num2 = 0; num2 < num1; num2++)
//            {
//                  DataGridColumnStyle style1 = (DataGridColumnStyle) _items[num2];
//                  if (propDesc.Equals(style1.PropertyDescriptor))
//                  {
//                        return style1;
//                  }
//            }
//            return null;
//} }
//      public TestType this[string columnName] { get
//      {return null;} }
  public BocColumnDefinition this[int index]
  {
    get
    {
      return (BocColumnDefinition) _items[index];
    }
  }

  [Browsable(false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  protected virtual ArrayList List
  {
    get
    {
      return _items;
    }
  }

  [Browsable(false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public virtual int Count 
  {
    get
    {
      return  List.Count;
    } 
  }

  [Browsable(false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public bool IsReadOnly 
  { 
    get
    {
      return false;                       
    } 
  }

  [Browsable(false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public bool IsSynchronized 
  { 
    get
    {
      return false;
    }
  }

  [Browsable(false)]
  [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
  [EditorBrowsable(EditorBrowsableState.Advanced)]
  public object SyncRoot
  { 
    get
    {
      return this;
    } 
  }

  // Fields
  private bool isDefault;
  private ArrayList _items;
}

}

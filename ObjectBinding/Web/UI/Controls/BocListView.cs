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

namespace Rubicon.ObjectBinding.Web.Controls
{
/// <summary>
///   A BocColumnDefinitionSet is a named collection of column definitions.
/// </summary>
//[Designer (typeof(ColumnDefinitionSetDesigner), typeof (ControlDesigner))]
//[TypeConverter (typeof (ColumnDefinitionSetConverter))]
[DesignTimeVisible (false)]
[ParseChildren(true, "Columns")]
public class BocColumnDefinitionSet
{
  private object _title;
  private BocColumnDefinitionCollection _columns;

  public BocColumnDefinitionSet (object title, BocColumnDefinition[] columns)
  {
    //  TODO: ownerControl
    _title = title;
    _columns = new BocColumnDefinitionCollection (null);
    if (columns != null)
      _columns.AddRange (columns);
  }

  public BocColumnDefinitionSet (IBusinessObjectBoundWebControl ownerControl, object title, BocColumnDefinition[] columns)
  {
    _title = title;
    _columns = new BocColumnDefinitionCollection (ownerControl);
    if (columns != null)
      _columns.AddRange (columns);
  }

  public BocColumnDefinitionSet(IBusinessObjectBoundWebControl ownerControl, object title)
    : this (ownerControl, title, null)
  {}

  public BocColumnDefinitionSet (IBusinessObjectBoundWebControl ownerControl)
    : this (ownerControl, string.Empty, null)
  {}

  [PersistenceMode (PersistenceMode.Attribute)]
  [DefaultValue("")]
  public string Title
  {
    get { return (_title != null) ? _title.ToString() : string.Empty; }
    set { _title = value; }
  }

  [PersistenceMode (PersistenceMode.InnerDefaultProperty)]
  [ListBindable (false)]
  [MergableProperty (false)]
  [DefaultValue((string) null)]
  public BocColumnDefinitionCollection Columns
  {
    get { return _columns; }
  }
}

[Editor (typeof (BocColumnDefinitionSetCollectionEditor), typeof (UITypeEditor))]
public  class BocColumnDefinitionSetCollection : IList, ICollection, IEnumerable
{
  private bool _isEditing;
  private IBusinessObjectBoundWebControl _ownerControl;

  // Events
  public event CollectionChangeEventHandler CollectionChanged;

  // Construction and disposing
  internal BocColumnDefinitionSetCollection (IBusinessObjectBoundWebControl ownerControl)
  {
    _ownerControl = ownerControl;
    _items = new ArrayList();
  }

  internal BocColumnDefinitionSetCollection (
      IBusinessObjectBoundWebControl ownerControl, 
      BocColumnDefinitionSet[] columnDefinitionSets)
    : this (ownerControl)
  {
    _items.AddRange (columnDefinitionSets);
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
  public virtual int Add (BocColumnDefinitionSet columnDefinitionSet)
  {
    if (isDefault)
    {
      throw new ArgumentException("DefaultColumnDefinitionSetCollectionChanged");
    }
    //      column.SetDataGridTableInColumn(owner, true);
    //      column.MappingNameChanged += new EventHandler(ColumnStyleMappingNameChanged);
    //      column.PropertyDescriptorChanged += new EventHandler(ColumnStylePropDescChanged);
    //      if ((DataGridTableStyle != null) && (column.Width == -1))
    //      {
    //            column.width = DataGridTableStyle.PreferredColumnWidth;
    //      }

    int count = _items.Add (columnDefinitionSet);
    //  TODO: OwnerControl
    //columnDefinitionSet.OwnerControl = _ownerControl;
    OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Add, columnDefinitionSet));
    return count;
  }

  public void AddRange (BocColumnDefinitionSet[] columnDefinitionSets)
  {
    if (columnDefinitionSets == null)
      throw new ArgumentNullException("columnDefinitions");

    BeginEdit();
    for (int i = 0; i < columnDefinitionSets.Length; i++)
      Add (columnDefinitionSets[i]);
    EndEdit();
  }

  public void Clear()
  {
    _items.Clear();
   OnCollectionChanged (new CollectionChangeEventArgs (CollectionChangeAction.Refresh, null));
  }

  public bool Contains(BocColumnDefinitionSet columnDefinition)
  { 
    int index = _items.IndexOf(columnDefinition); 
    return (index != -1);
  }

  public int IndexOf(BocColumnDefinitionSet element)
  { 
    return _items.IndexOf(element);
  }

  protected void OnCollectionChanged(CollectionChangeEventArgs e)
  {
    if (CollectionChanged != null && !_isEditing)
      CollectionChanged(this, e);
  }

  public void Remove(BocColumnDefinitionSet columnDefinition)
  {
    if (isDefault)
      throw new ArgumentException("DefaultColumnDefinitionSetCollectionChanged");

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
      throw new InvalidOperationException("ColumnDefinitionSetCollectionMissing");

    RemoveAt(num1);
  }

  public void RemoveAt(int index)
  {
    if (isDefault)
      throw new ArgumentException("DefaultColumnDefinitionSetCollectionChanged");

    BocColumnDefinitionSet columnDefinitionSet = (BocColumnDefinitionSet) _items[index];
    //      style1.SetDataGridTableInColumn(null, true);
    //      style1.MappingNameChanged -= new EventHandler(ColumnStyleMappingNameChanged);
    //      style1.PropertyDescriptorChanged -= new EventHandler(ColumnStylePropDescChanged);
    _items.RemoveAt(index);
    OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, columnDefinitionSet));
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
    return Add ((BocColumnDefinitionSet) value);
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
    Remove((BocColumnDefinitionSet) value);
  }

  void IList.RemoveAt(int index)
  {
    RemoveAt(index);
  }

  public BocColumnDefinitionSet[] ToArray()
  {
    return (BocColumnDefinitionSet[]) _items.ToArray (typeof (BocColumnDefinitionSet));
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
  public BocColumnDefinitionSet this[int index]
  {
    get
    {
      return (BocColumnDefinitionSet) _items[index];
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
  
//internal class ColumnDefinitionSetConverter : ExpandableObjectConverter
//{
//  public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
//  {
//    return true;
//  }
//
//  public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
//  {
//    BocColumnDefinitionSet columnDefinitionCollection = new BocColumnDefinitionSet();
//    return columnDefinitionCollection;
//  }
//
//  public override bool CanConvertFrom (ITypeDescriptorContext context, Type t) 
//  {
//    if (t == typeof(string))
//    {
//      return true;
//    }
//    return base.CanConvertFrom(context, t);
//  }
//
//  public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo info, object value) 
//  {
//    if (value is string) 
//    {
//      try
//      {
//        return new BocColumnDefinitionSet();      
//      }
//      catch 
//      {}
//      // if we got this far, complain that we
//      // couldn't parse the string
//      //
//      throw new ArgumentException("Can not convert '" + (string)value + "' to type BocColumnDefinitionSet");
//    }      
//    return base.ConvertFrom(context, info, value);
//  }
//
//  public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType) 
//  {
//    if (destType == typeof(string) && value is BocColumnDefinitionSet) 
//    {   
//      return "Hello World"; 
//    }
//            
//    return base.ConvertTo(context, culture, value, destType); 
//  }   
//}

}

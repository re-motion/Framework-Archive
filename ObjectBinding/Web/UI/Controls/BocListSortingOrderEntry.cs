using System;
using System.ComponentModel;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.UI.Controls
{

/// <summary> Represents the sorting direction for an individual column. </summary>
/// <remarks> Used when evaluating the current or new sorting order as well as to persist it into the view state. </remarks>
[TypeConverter (typeof (BocListSortingOrderEntryConverter))]
public class BocListSortingOrderEntry
{
  private int _columnIndex;
  private BocColumnDefinition _column;
  private SortingDirection _direction;
  private bool _isEmpty = false;

  /// <summary> Represents a null <see cref="BocListSortingOrderEntry"/>. </summary>
  public static readonly BocListSortingOrderEntry Empty = new BocListSortingOrderEntry ();

  public BocListSortingOrderEntry (BocColumnDefinition column, SortingDirection direction)
  {
    ArgumentUtility.CheckNotNull ("column", column);
    _columnIndex = Int32.MinValue;
    SetColumn (column);
    _direction = direction;
  }

  protected internal BocListSortingOrderEntry (int columnIndex, SortingDirection direction)
  {
    _columnIndex = columnIndex;
    _column = null;
    _direction = direction;
  }

  private BocListSortingOrderEntry ()
  {
    _isEmpty = true;
  }

  /// <summary> <see langword="true"/> if this sorting order entry is empty. </summary>
  public bool IsEmpty
  {
    get { return _isEmpty; }
  }

  protected internal int ColumnIndex
  {
    get { return _columnIndex; }
  }

  protected internal void SetColumnIndex (int columnIndex)
  {
    _columnIndex = columnIndex;
  }

  /// <summary> Gets the column to sort by. </summary>
  public BocColumnDefinition Column
  {
    get { return _column; }
  }

  /// <summary> Sets the column to sort by. </summary>
  /// <param name="column">
  ///   Must not be <see langword="null"/>. 
  ///   Must be of type <see cref="BocValueColumnDefinition"/> 
  ///   or <see cref="BocCustomColumnDefinition"/> with <see cref="BocCustomColumnDefinition.IsSortable"/> set
  ///   <see langword="true"/>.
  /// </param>
  protected internal void SetColumn (BocColumnDefinition column)
  {
    ArgumentUtility.CheckNotNull ("column", column);
    if (   ! (column is BocValueColumnDefinition) 
        && ! (   column is BocCustomColumnDefinition
              && ((BocCustomColumnDefinition) column).IsSortable))
    {
      throw new ArgumentException ("BocListSortingOrderEntry can only use columns of type BocValueColumnDefinition or BocCustomColumnDefinition with BocCustomColumnDefinition.IsSortable set true.", "column");
    }
    _column = column; 
  }

  /// <summary> Gets the sorting direction. </summary>
  public SortingDirection Direction
  {
    get { return _direction; }
  }

  protected internal void SetDirection (SortingDirection direction)
  {
    _direction = direction;
  }

  /// <summary>
  ///   Tests whether the specified object is of type <see cref="BocListSortingOrderEntry"/> 
  ///   and is equivalent to this <see cref="BocListSortingOrderEntry"/>.
  /// </summary>
  /// <remarks> 
  ///   Required for identifying the <see cref="BocListSortingOrderEntry"/> in an 
  ///   <see cref="System.Collections.ArrayList"/>.
  /// </remarks>
  /// <param name="obj">
  ///   The object to test. 
  /// </param>
  /// <returns>
  ///   This method returns <see langword="true"/> if <paramref name="obj"/> 
  ///   is of type <see cref="BocListSortingOrderEntry"/> and equivalent to this 
  ///   <see cref="BocListSortingOrderEntry"/>; otherwise, <see langword="false"/>.
  /// </returns>
  public override bool Equals (object obj)
  {
    if (obj is BocListSortingOrderEntry)
    {
      BocListSortingOrderEntry entry = (BocListSortingOrderEntry) obj;
      if (_isEmpty && entry.IsEmpty)
        return true;
      else
        return ColumnIndex == entry.ColumnIndex && Direction == entry.Direction;;
    }
    return false;
  }

  /// <summary> Returns a hash code for this <see cref="BocListSortingOrderEntry"/>. </summary>
  /// <returns> An integer value that specifies the hash code for this  <see cref="BocListSortingOrderEntry"/>. </returns>
  public override int GetHashCode()
  {
    return ColumnIndex.GetHashCode() ^ Direction.GetHashCode();
  }
}

/// <summary> The possible sorting directions. </summary>
public enum SortingDirection
{
  /// <summary> Do not sort. </summary>
  None,
  /// <summary> Sort ascending. </summary>
  Ascending,
  /// <summary> Sort descending. </summary>
  Descending
}

/// <summary> Converts a <see cref="BocListSortingOrderEntry"/> from and to a string. </summary>
/// <remarks> Used for persisting a <see cref="BocListSortingOrderEntry"/> into the view state. </remarks>
public class BocListSortingOrderEntryConverter: TypeConverter
{
  private const string c_empty = "empty";

  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    return base.CanConvertFrom (context, sourceType);
  }

  public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
  {
    if (destinationType == typeof (string))
      return true;
    return base.CanConvertTo (context, destinationType);
  }

  public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
  {
    if (value is string)
    {
      string stringValue = (string) value;
      if (stringValue.CompareTo (c_empty) == 0)
      {
        return BocListSortingOrderEntry.Empty;
      }
      else
      {
        string[] values = stringValue.Split (new char[] {','}, 2);
        int columnIndex = Int32.Parse (values[0]);
        int directionValue = Int32.Parse (values[1]);
        SortingDirection direction = (SortingDirection) Enum.ToObject (typeof (SortingDirection), directionValue);
        return new BocListSortingOrderEntry (columnIndex, direction);
      }
    }
    return base.ConvertFrom (context, culture, value);
  }

  public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    ArgumentUtility.CheckNotNullAndType ("value", value, typeof (BocListSortingOrderEntry));
    if(destinationType == typeof (string))
    {
      BocListSortingOrderEntry entry = (BocListSortingOrderEntry) value;
      if (entry.IsEmpty)
        return c_empty;
      else
        return entry.ColumnIndex.ToString() + "," + ((int) entry.Direction).ToString();
    }
    return base.ConvertTo (context, culture, value, destinationType);
  }

}

}

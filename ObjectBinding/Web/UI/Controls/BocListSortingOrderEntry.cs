using System;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.Web.Controls
{

/// <summary> Represents the sorting direction for an individual column. </summary>
/// <remarks> Used when evaluating the current or new sorting order as well as to persist it into the view state. </remarks>
[Serializable]
public class BocListSortingOrderEntry
{
  /// <summary> Gets or sets the index of the column for which the <see cref="Direction"/> is entered. </summary>
  private int _columnIndex;
  [NonSerialized]
  private BocColumnDefinition _column;
  /// <summary> Gets or sets the <see cref="SortingDirection"/> for the column at <see cref="ColumnIndex"/>. </summary>
  private SortingDirection _direction;
  [NonSerialized]
  private bool _isEmpty = false;

  /// <summary> Represents a null <see cref="BocListSortingOrderEntry"/>. </summary>
  public static readonly BocListSortingOrderEntry Empty = new BocListSortingOrderEntry ();

  /// <summary> Initializes a new instance. </summary>
  public BocListSortingOrderEntry (BocColumnDefinition column, SortingDirection direction)
  {
    ArgumentUtility.CheckNotNull ("column", column);
    _columnIndex = Int32.MinValue;
    Column = column;
    _direction = direction;
  }

  /// <summary> Initializes a new instance. </summary>
  internal BocListSortingOrderEntry (int columnIndex, SortingDirection direction)
  {
    _columnIndex = columnIndex;
    _column = null;
    _direction = direction;
  }

  /// <summary> Initializes the empty instance. </summary>
  private BocListSortingOrderEntry ()
  {
    _isEmpty = true;
  }

  public bool IsEmpty
  {
    get { return _isEmpty; }
  }

  internal int ColumnIndex
  {
    get { return _columnIndex; }
    set { _columnIndex = value; }
  }

  /// <summary> The the column to sort by. </summary>
  /// <remarks>
  ///   Must not be <see langword="null"/>. 
  ///   Must be of type <see cref="BocValueColumnDefinition"/> 
  ///   or <see cref="BocCustomColumnDefinition"/> with <see cref="BocCustomColumnDefinition.IsSortable"/> set
  ///   <see langword="true"/>.
  /// </remarks>
  public BocColumnDefinition Column
  {
    get { return _column; }
    set 
    {
      ArgumentUtility.CheckNotNull ("value", value);
      if (   ! (value is BocValueColumnDefinition) 
          && ! (   value is BocCustomColumnDefinition
                && ((BocCustomColumnDefinition) value).IsSortable))
      {
        throw new ArgumentException ("BocListSortingOrderEntry can only use columns of type BocValueColumnDefinition or BocCustomColumnDefinition with BocCustomColumnDefinition.IsSortable set true.", "value");
      }
      _column = value; 
    }
  }

  public SortingDirection Direction
  {
    get { return _direction; }
    set { _direction = value; }
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
  /// <summary> Don't sort. </summary>
  None,
  /// <summary> Sort ascending. </summary>
  Ascending,
  /// <summary> Sort descending. </summary>
  Descending
}

}

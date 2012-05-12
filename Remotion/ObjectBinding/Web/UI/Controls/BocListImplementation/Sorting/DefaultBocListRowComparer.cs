// This file is part of the re-motion Core Framework (www.re-motion.org)
// CopyrowB (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting
{
  public class DefaultBocListRowComparer : IComparer<BocListRow>
  {
    private static readonly ILog s_log = LogManager.GetLogger (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private readonly ICache<Tuple<BocListRow, IBocSortableColumnDefinition, IBusinessObjectPropertyPath>, object> _rowValueCache =
        new Cache<Tuple<BocListRow, IBocSortableColumnDefinition, IBusinessObjectPropertyPath>, object>();

    private readonly Tuple<SortingDirection, IComparer<BocListRow>>[] _sorting;

    public DefaultBocListRowComparer (BocListSortingOrderEntry[] sortingOrder)
    {
      ArgumentUtility.CheckNotNull ("sortingOrder", sortingOrder);

      _sorting = sortingOrder.Select (entry => Tuple.Create (entry.Direction, CreateComparer (entry.Column))).ToArray();
    }

    private IComparer<BocListRow> CreateComparer (IBocSortableColumnDefinition column)
    {
      if (column is BocSimpleColumnDefinition)
        return new BocSimpleColumnDefinitionCellValueComparer ((BocSimpleColumnDefinition) column);
      else if (column is BocCustomColumnDefinition)
        return new BocCustomColumnDefinitionCellValueComparer ((BocCustomColumnDefinition) column);
      else if (column is BocCompoundColumnDefinition)
        return new BocCompoundColumnDefinitionCellValueComparer ((BocCompoundColumnDefinition) column);
      else
        throw new NotSupportedException (column.GetType().FullName);
    }

    /// <summary>
    ///   Compares the current instance with another object of type <see cref="BocListRow"/>.
    /// </summary>
    /// <returns>
    ///   <list type="table">
    ///     <listheader>
    ///       <term> Value </term>
    ///       <description> Condition </description>
    ///     </listheader>
    ///     <item>
    ///       <term> Less than zero </term>
    ///       <description> <paramref name="rowA"/> is less than <paramref name="rowB"/>. </description>
    ///     </item>
    ///     <item>
    ///       <term> Zero </term>
    ///       <description> <paramref name="rowA"/> is equal to <paramref name="rowB"/>. </description>
    ///     </item>
    ///     <item>
    ///       <term> Greater than zero </term>
    ///       <description> <paramref name="rowA"/> is greater than <paramref name="rowB"/>. </description>
    ///     </item>
    ///   </list>
    /// </returns>
    public int Compare (BocListRow rowA, BocListRow rowB)
    {
      ArgumentUtility.CheckNotNull ("rowA", rowA);
      ArgumentUtility.CheckNotNull ("rowB", rowB);

      if (rowA == rowB)
        return 0;

      foreach (var sortingEntry in _sorting)
      {
        int compareResult = sortingEntry.Item2.Compare (rowA, rowB);
        if (compareResult != 0)
        {
          if (sortingEntry.Item1 == SortingDirection.Descending)
            return compareResult * -1;
          else
            return compareResult;
        }
      }

      return rowA.Index - rowB.Index;
    }

    private int CompareToRowBySortingOrderEntry (BocListRow rowA, BocListRow rowB, BocListSortingOrderEntry entry)
    {
      if (entry.Direction == SortingDirection.None)
        return 0;

      if (entry.Column is BocSimpleColumnDefinition)
      {
        return CompareToRowBySimpleColumn (rowA, rowB, entry.Column);
      }
      else if (entry.Column is BocCustomColumnDefinition)
      {
        return CompareToRowByCustomColumn (rowA, rowB, entry.Column);
      }
      else if (entry.Column is BocCompoundColumnDefinition)
      {
        return CompareToRowByCompundColumn (rowA, rowB, entry.Column);
      }

      return 0;
    }

    private int CompareToRowBySimpleColumn (BocListRow rowA, BocListRow rowB, IBocSortableColumnDefinition column)
    {
      BocSimpleColumnDefinition simpleColumn = (BocSimpleColumnDefinition) column;

      IBusinessObjectPropertyPath propertyPathRowA;
      IBusinessObjectPropertyPath propertyPathRowB;

      if (simpleColumn.IsDynamic)
      {
        // TODO: UnitTests
        propertyPathRowA = simpleColumn.GetDynamicPropertyPath (rowA.BusinessObject.BusinessObjectClass);
        propertyPathRowB = simpleColumn.GetDynamicPropertyPath (rowB.BusinessObject.BusinessObjectClass);
      }
      else
      {
        propertyPathRowA = simpleColumn.GetPropertyPath();
        propertyPathRowB = simpleColumn.GetPropertyPath();
      }

      int compareResult = ComparePropertyPathValues (propertyPathRowA, rowA, propertyPathRowB, rowB);

      if (compareResult != 0)
        return compareResult;

      string stringValueA = GetStringValueForSimpleColumnFromCache (rowA, simpleColumn);
      string stringValueB = GetStringValueForSimpleColumnFromCache (rowB, simpleColumn);
      return CompareStrings (stringValueA, stringValueB);
    }

    private int CompareToRowByCustomColumn (BocListRow rowA, BocListRow rowB, IBocSortableColumnDefinition column)
    {
      BocCustomColumnDefinition customColumn = (BocCustomColumnDefinition) column;

      IBusinessObjectPropertyPath propertyPathRowA;
      IBusinessObjectPropertyPath propertyPathRowB;

      if (customColumn.IsDynamic)
      {
        // TODO: UnitTests
        propertyPathRowA = customColumn.GetDynamicPropertyPath (rowA.BusinessObject.BusinessObjectClass);
        propertyPathRowB = customColumn.GetDynamicPropertyPath (rowB.BusinessObject.BusinessObjectClass);
      }
      else
      {
        propertyPathRowA = customColumn.GetPropertyPath();
        propertyPathRowB = customColumn.GetPropertyPath();
      }

      int compareResult = ComparePropertyPathValues (propertyPathRowA, rowA, propertyPathRowB, rowB);
     
      if (compareResult != 0)
        return compareResult;

      string stringValueA = GetStringValueForCustomColumnFromCache (rowA, customColumn);
      string stringValueB = GetStringValueForCustomColumnFromCache (rowB, customColumn);
      return CompareStrings (stringValueA, stringValueB);
    }

    private int CompareToRowByCompundColumn (BocListRow rowA, BocListRow rowB, IBocSortableColumnDefinition column)
    {
      BocCompoundColumnDefinition compoundColumn = (BocCompoundColumnDefinition) column;

      for (int idxBindings = 0; idxBindings < compoundColumn.PropertyPathBindings.Count; idxBindings++)
      {
        PropertyPathBinding propertyPathBinding = compoundColumn.PropertyPathBindings[idxBindings];
        IBusinessObjectPropertyPath propertyPathRowA;
        IBusinessObjectPropertyPath propertyPathRowB;

        if (propertyPathBinding.IsDynamic)
        {
          // TODO: UnitTests
          propertyPathRowA = propertyPathBinding.GetDynamicPropertyPath (rowA.BusinessObject.BusinessObjectClass);
          propertyPathRowB = propertyPathBinding.GetDynamicPropertyPath (rowB.BusinessObject.BusinessObjectClass);
        }
        else
        {
          propertyPathRowA = propertyPathBinding.GetPropertyPath();
          propertyPathRowB = propertyPathBinding.GetPropertyPath();
        }

        int compareResult = ComparePropertyPathValues (propertyPathRowA, rowA, propertyPathRowB, rowB);

        if (compareResult != 0)
          return compareResult;
      }

      string stringValueA = GetStringValueForCompoundColumnFromCache (rowA, compoundColumn);
      string stringValueB = GetStringValueForCompoundColumnFromCache (rowB, compoundColumn);
      return CompareStrings (stringValueA, stringValueB);
    }

    private int ComparePropertyPathValues (
        IBusinessObjectPropertyPath propertyPathA,
        BocListRow rowA,
        IBusinessObjectPropertyPath propertyPathB,
        BocListRow rowB)
    {
      object valueA = GetPropertyPathValueFromCache (rowA, propertyPathA);
      object valueB = GetPropertyPathValueFromCache (rowB, propertyPathB);

      if (valueA == null && valueB == null)
        return 0;
      if (valueA == null)
        return -1;
      if (valueB == null)
        return 1;

      IList listA = valueA as IList;
      IList listB = valueB as IList;
      if (listA != null || listB != null)
      {
        if (listA == null || listB == null)
          return 0;
        if (listA.Count == 0 && listB.Count == 0)
          return 0;
        if (listA.Count == 0)
          return -1;
        if (listB.Count == 0)
          return 1;
        valueA = listA[0];
        valueB = listB[0];
      }

      if (valueA is IComparable && valueB is IComparable)
        return Comparer.Default.Compare (valueA, valueB);

      return 0;
      //Better leave the comparisson of non-ICompareables to the calling method. ToString is not always the rowB choice.
      //return Comparer.Default.Compare (valueA.ToString(), valueB.ToString());
    }

    private object GetPropertyPathValueFromCache (BocListRow row, IBusinessObjectPropertyPath propertyPath)
    {
      if (propertyPath == null)
        return null;

      return _rowValueCache.GetOrCreateValue ( CreateCacheKey(row, propertyPath), key => GetPropertyPathValue(key.Item1, key.Item3));
    }

    private object GetPropertyPathValue (BocListRow row, IBusinessObjectPropertyPath propertyPath)
    {
      try
      {
        return propertyPath.GetValue (row.BusinessObject, false, true);
      }
      catch (Exception e)
      {
        s_log.ErrorFormat (
            e, "Exception thrown while reading string value for property path '{0}' in row {1} of BocList.", propertyPath, row.Index);
        return null;
      }
    }

    private string GetStringValueForSimpleColumnFromCache (BocListRow row, BocSimpleColumnDefinition column)
    {
      return (string) _rowValueCache.GetOrCreateValue (CreateCacheKey (row, column), key => GetStringValueForSimpleColumn (key.Item1, key.Item2));
    }

    private string GetStringValueForSimpleColumn (BocListRow row, IBocSortableColumnDefinition column)
    {
      var simpleColumn = (BocSimpleColumnDefinition) column;
      try
      {
        return simpleColumn.GetStringValue (row.BusinessObject);
      }
      catch (Exception e)
      {
        s_log.ErrorFormat (e, "Exception thrown while reading string value for column '{0}' in row {1} of BocList.", column.ItemID, row.Index);
        return null;
      }
    }

    private string GetStringValueForCustomColumnFromCache (BocListRow row, BocCustomColumnDefinition column)
    {
      return (string) _rowValueCache.GetOrCreateValue (CreateCacheKey (row, column), key => GetStringValueForCustomColumn(key.Item1, key.Item2));
    }

    private string GetStringValueForCustomColumn (BocListRow row, IBocSortableColumnDefinition column)
    {
      var customColumn = (BocCustomColumnDefinition) column;
      try
      {
        //TODO: Support DynamicPropertyPaths
        IBusinessObjectPropertyPath propertyPath = customColumn.GetPropertyPath();
        return propertyPath.GetString (row.BusinessObject, string.Empty);
      }
      catch (Exception e)
      {
        s_log.ErrorFormat (e, "Exception thrown while reading string value for column '{0}' in row {1} of BocList.", column.ItemID, row.Index);
        return null;
      }
    }

    private string GetStringValueForCompoundColumnFromCache (BocListRow row, BocCompoundColumnDefinition column)
    {
      return (string) _rowValueCache.GetOrCreateValue (CreateCacheKey (row, column), key => GetStringValueForCompoundColumn(key.Item1, key.Item2));
    }

    private string GetStringValueForCompoundColumn (BocListRow row, IBocSortableColumnDefinition column)
    {
      var compoundColumn = (BocCompoundColumnDefinition) column;
      try
      {
        return compoundColumn.GetStringValue (row.BusinessObject);
      }
      catch (Exception e)
      {
        s_log.ErrorFormat (e, "Exception thrown while reading string value for column '{0}' in row {1} of BocList.", column.ItemID, row.Index);
        return null;
      }
    }

    private int CompareStrings (string valueA, string valueB)
    {
      return string.Compare (valueA, valueB);
    }

    private Tuple<BocListRow, IBocSortableColumnDefinition, IBusinessObjectPropertyPath> CreateCacheKey (BocListRow row, IBocSortableColumnDefinition column)
    {
      return Tuple.Create (row, column, (IBusinessObjectPropertyPath) null);
    }

    private Tuple<BocListRow, IBocSortableColumnDefinition, IBusinessObjectPropertyPath> CreateCacheKey (BocListRow row, IBusinessObjectPropertyPath propertyPath)
    {
      return Tuple.Create (row, (IBocSortableColumnDefinition) null, propertyPath);
    }
  }
}
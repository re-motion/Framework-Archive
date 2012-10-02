﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting
{
  public class BocSimpleColumnDefinitionCellValueComparer : BocColumnDefinitionCellValueComparerBase
  {
    private readonly ICache<BocListRow, string> _stringValueCache = new Cache<BocListRow, string>();
    private readonly BocSimpleColumnDefinition _column;

    public BocSimpleColumnDefinitionCellValueComparer (BocSimpleColumnDefinition column)
    {
      ArgumentUtility.CheckNotNull ("column", column);
      _column = column;
    }

    public override int Compare (BocListRow rowA, BocListRow rowB)
    {
      ArgumentUtility.CheckNotNull ("rowA", rowA);
      ArgumentUtility.CheckNotNull ("rowB", rowB);

      IBusinessObjectPropertyPath propertyPathRowA;
      IBusinessObjectPropertyPath propertyPathRowB;

      if (_column.IsDynamic)
      {
        // TODO: UnitTests
        // TODO: ComparePropertyPathValues also caches based on property path.
        //       This does not work with dynamic property path and causes a temporary memory leak until the current sort operation has completed.
        propertyPathRowA = _column.GetDynamicPropertyPath (rowA.BusinessObject.BusinessObjectClass);
        propertyPathRowB = _column.GetDynamicPropertyPath (rowB.BusinessObject.BusinessObjectClass);
      }
      else
      {
        propertyPathRowA = _column.GetPropertyPath();
        propertyPathRowB = _column.GetPropertyPath();
      }

      var compareResult = ComparePropertyPathValues (propertyPathRowA, rowA, propertyPathRowB, rowB);

      if (compareResult.HasValue)
        return compareResult.Value;

      string stringValueA = _stringValueCache.GetOrCreateValue (rowA, GetStringValueForSimpleColumn);
      string stringValueB = _stringValueCache.GetOrCreateValue (rowB, GetStringValueForSimpleColumn);
      return CompareStrings (stringValueA, stringValueB);
    }

    private string GetStringValueForSimpleColumn (BocListRow row)
    {
      try
      {
        return _column.GetStringValue (row.BusinessObject);
      }
      catch (Exception e)
      {
        s_log.ErrorFormat (e, "Exception thrown while reading string value for column '{0}' in row {1} of BocList.", _column.ItemID, row.Index);
        return null;
      }
    }
  }
}
// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Collections;
using System.Globalization;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation
{
  /// <summary>
  /// Row-index-based implementation of the <see cref="IRowIDProvider"/> interface. 
  /// Used when the <see cref="BocList"/> is bound to objects of type <see cref="IBusinessObject"/> (without identity).
  /// </summary>
  public class IndexBasedRowIDProvider : IRowIDProvider
  {
    public string GetControlRowID (BocListRow row)
    {
      ArgumentUtility.CheckNotNull ("row", row);

      return row.Index.ToString (CultureInfo.InvariantCulture);
    }

    public string GetItemRowID (BocListRow row)
    {
      ArgumentUtility.CheckNotNull ("row", row);

      return row.Index.ToString (CultureInfo.InvariantCulture);
    }

    public BocListRow GetRowFromItemRowID (IList values, string rowID)
    {
      ArgumentUtility.CheckNotNull ("values", values);
      ArgumentUtility.CheckNotNull ("rowID", rowID);

      int rowIndex;
      try
      {
        rowIndex = int.Parse (rowID, CultureInfo.InvariantCulture);
      }
      catch (Exception ex)
      {
        throw new FormatException (string.Format ("RowID '{0}' could not be parsed as an integer number.", rowID), ex);
      }

      if (rowIndex < values.Count)
        return new BocListRow (rowIndex, (IBusinessObject) values[rowIndex]);
      else
        return null;
    }
  }
}
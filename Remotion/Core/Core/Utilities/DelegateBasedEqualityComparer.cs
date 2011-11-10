// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;

namespace Remotion.Utilities
{
  /// <summary>
  /// <see cref="DelegateBasedEqualityComparer{T}"/> implements <see cref="IEqualityComparer{T}"/> to compare two objects and calculate hash codes 
  /// with the specified delegates.
  /// </summary>
  /// <typeparam name="T">The type of the objects to be compared.</typeparam>
  public class DelegateBasedEqualityComparer<T> : IEqualityComparer<T>
  {
    private readonly Func<T, T, bool> _comparison;
    private readonly Func<T, int> _hash;
    
    public DelegateBasedEqualityComparer (Func<T, T, bool> comparison, Func<T, int> hash)
    {
      ArgumentUtility.CheckNotNull ("comparison", comparison);
      ArgumentUtility.CheckNotNull ("hash", hash);

      _comparison = comparison;
      _hash = hash;
    }

    public bool Equals (T x, T y)
    {
      return _comparison (x, y);
    }

    public int GetHashCode (T obj)
    {
      return _hash (obj);
    }
  }
}
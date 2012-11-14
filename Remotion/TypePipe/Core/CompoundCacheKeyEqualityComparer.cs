﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Remotion.Utilities;

// TODO Review: Move caching classes to separate namespace

namespace Remotion.TypePipe
{
  /// <summary>
  /// Compares compound cache keys, i.e., compares object arrays.
  /// This class is an implementation detail of <see cref="TypeCache"/>.
  /// </summary>
  /// <remarks>
  /// Note that the length of the object arrays is assumed to be equal.
  /// </remarks>
  public class CompoundCacheKeyEqualityComparer : IEqualityComparer<object[]>
  {
    public bool Equals (object[] x, object[] y)
    {
      // Using Debug.Assert because it will be compiled away.
      Debug.Assert (x != null);
      Debug.Assert (y != null);
      Debug.Assert (x.Length == y.Length);

      for (int i = 0; i < x.Length; ++i)
      {
        if (!object.Equals (x[i], y[i]))
          return false;
      }

      return true;
    }

    public int GetHashCode (object[] compoundKey)
    {
      Debug.Assert (compoundKey != null);

      return EqualityUtility.GetRotatedHashCode (compoundKey);
    }
  }
}
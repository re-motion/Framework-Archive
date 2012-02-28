// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Diagnostics;

namespace Remotion.TypePipe.Utilities
{
  [DebuggerStepThrough]
  internal static class Adapter
  {
    public static Adapter<T> New<T> (T item)
      where T : class 
    {
      if (item == null)
        throw new ArgumentNullException ("item");

      return new Adapter<T> (item);
    }
  }

  [DebuggerStepThrough]
  internal class Adapter<T>
    where T : class
  {
    private readonly T _item;

    public Adapter (T item)
    {
      _item = item;
    }

    public T Item
    {
      get { return _item; }
    }
  }
}
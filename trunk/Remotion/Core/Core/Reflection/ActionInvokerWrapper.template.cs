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

namespace Remotion.Reflection
{
  // @begin-skip
  /// <summary>
  /// Used to wrap an <see cref="IActionInvoker"/> object rather than returning it directly.
  /// </summary>
  // @end-skip
  public partial struct ActionInvokerWrapper : IActionInvoker
  {
    // @begin-skip
    private readonly IActionInvoker _invoker;

    public ActionInvokerWrapper (IActionInvoker invoker)
    {
      _invoker = invoker;
    }

    public void Invoke (Type[] valueTypes, object[] values)
    {
      _invoker.Invoke (valueTypes, values);
    }

    public void Invoke (object[] values)
    {
      _invoker.Invoke (values);
    }
    // @end-skip

    // @begin-template first=1 generate=0..17 suppressTemplate=true

    // @replace "A<n>" ", " "<" ">"
    // @replace "A<n> a<n>" ", "
    public void With<A1> (A1 a1)
    {
      // @replace "a<n>" ", "
      _invoker.With (a1);
    }
    // @end-template
  }
}

// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Globalization;

namespace Remotion.Scripting.UnitTests.TestDomain
{
  public class ProxiedChildGeneric<T0, T1> : ProxiedChild
  {
    private readonly T0 _t0;
    private readonly T1 _t1;

    public ProxiedChildGeneric ()
    {
      
    }

    public ProxiedChildGeneric (string name, T0 t0, T1 t1)
        : base (name)
    {
      _t0 = t0;
      _t1 = t1;
    }

    public string ToStringKebap (int theNumber)
    {
      var s0 = String.Format (CultureInfo.InvariantCulture, "{0}", _t0);
      var s1 = String.Format (CultureInfo.InvariantCulture, "{0}", _t1);
      return Name + "_" + s0 + "_" + s1 + "_" + theNumber.ToString (CultureInfo.InvariantCulture);
    }

    public string ProxiedChildGenericToString (T0 t0, T1 t1)
    {
      return "ProxiedChildGenericToString: " + t0 + t1;
    }

    public string ProxiedChildGenericToString<T2> (T0 t0, T1 t1, T2 t2)
    {
      return "ProxiedChildGenericToString: " + t0 + t1 + t2;
    }

    public string ProxiedChildGenericToString<T2,T3> (T0 t0, T1 t1, T2 t2, T3 t3)
    {
      return "ProxiedChildGenericToString: " + t0 + t1 + t2 + t3;
    }
  }
}
// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class StateCombinationComparer : IEqualityComparer<StateCombination>
  {
    public bool Equals (StateCombination x, StateCombination y)
    {
      HashSet<StateDefinition> statesX = new HashSet<StateDefinition> (x.GetStates());
      StateDefinition[] statesY = y.GetStates ();

      return statesX.SetEquals (statesY);
    }

    public int GetHashCode (StateCombination obj)
    {
      Assertion.IsNotNull (obj.Class);
      int hashCode = obj.Class.GetHashCode ();

      foreach (StateDefinition state in obj.GetStates ())
        hashCode ^= state.GetHashCode ();

      return hashCode;
    }
  }
}

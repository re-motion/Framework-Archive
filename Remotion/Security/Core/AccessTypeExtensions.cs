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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Security
{
  public static class AccessTypeExtensions
  {
    //TODO RM-6183: Test, refactor to nested for loops, performance test, compare with using IEnumerable<> vs. IReadOnlyList<>
    public static bool HasAccess ([NotNull]this AccessType[] actualAccessTypes, [NotNull]AccessType[] requiredAccessTypes)
    {
      ArgumentUtility.CheckNotNull ("actualAccessTypes", actualAccessTypes);
      ArgumentUtility.CheckNotNullOrEmpty ("requiredAccessTypes", requiredAccessTypes);

      // This section is performance critical. No closure should be created, therefor converting this code to Linq is not possible.
      // requiredAccessTypes.All (requiredAccessType => actualAccessTypes.Contains (requiredAccessType));
      // ReSharper disable LoopCanBeConvertedToQuery
      foreach (var requiredAccessType in requiredAccessTypes)
      {
        if (Array.IndexOf (actualAccessTypes, requiredAccessType) < 0)
          return false;
      }

      return true;
      // ReSharper restore LoopCanBeConvertedToQuery
    }
  }
}
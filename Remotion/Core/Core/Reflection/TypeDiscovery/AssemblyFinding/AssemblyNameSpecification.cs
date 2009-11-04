// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery.AssemblyFinding
{
  /// <summary>
  /// Holds an <see cref="System.Reflection.AssemblyName"/> for the <see cref="NamedRootAssemblyFinder"/> as well as a flag indicating whether to 
  /// include referenced assemblies.
  /// </summary>
  public struct AssemblyNameSpecification
  {
    public AssemblyNameSpecification (AssemblyName assemblyName, bool followReferences)
        : this()
    {
      ArgumentUtility.CheckNotNull ("assemblyName", assemblyName);

      AssemblyName = assemblyName;
      FollowReferences = followReferences;
    }

    public AssemblyName AssemblyName { get; private set; }
    public bool FollowReferences { get; private set; }

    public override string ToString ()
    {
      return "Specification: " + AssemblyName;
    }
  }
}

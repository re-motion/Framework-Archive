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
using System.Diagnostics;
using System.Reflection;
using Remotion.Mixins.Definitions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: docs
  // TODO 5370: tests
  public class AttributeGenerator : IAttributeGenerator
  {
    private static readonly ConstructorInfo s_debuggerBrowsableAttributeConstructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new DebuggerBrowsableAttribute (DebuggerBrowsableState.Never));

    private static readonly ConstructorInfo s_debuggerDisplayAttributeConstructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new DebuggerDisplayAttribute ("display message"));
    private static readonly PropertyInfo s_debuggerDisplayAttributeNameProperty =
        MemberInfoFromExpressionUtility.GetProperty ((DebuggerDisplayAttribute o) => o.Name);

    public void AddDebuggerBrowsableAttribute (IMutableMember member, DebuggerBrowsableState debuggerBrowsableState)
    {
      var debuggerAttribute = new CustomAttributeDeclaration (s_debuggerBrowsableAttributeConstructor, new object[] { debuggerBrowsableState });
      member.AddCustomAttribute (debuggerAttribute);
    }

    public void AddDebuggerDisplayAttribute (IMutableMember member, string debuggerDisplayString, string debuggerDisplayNameString)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      ArgumentUtility.CheckNotNullOrEmpty ("debuggerDisplayString", debuggerDisplayString);
      ArgumentUtility.CheckNotNullOrEmpty ("debuggerDisplayNameString", debuggerDisplayNameString);

      var debuggerDisplayAttribute = new CustomAttributeDeclaration (
          s_debuggerDisplayAttributeConstructor,
          new object[] { debuggerDisplayString },
          new NamedArgumentDeclaration (s_debuggerDisplayAttributeNameProperty, debuggerDisplayNameString));
      member.AddCustomAttribute (debuggerDisplayAttribute);
    }

    public void ReplicateAttributes (IAttributableDefinition source, IMutableMember destination)
    {
      throw new System.NotImplementedException();
    }
  }
}
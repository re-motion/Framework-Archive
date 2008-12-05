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
using System.Diagnostics;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{InterfaceMethod}")]
  public class RequiredMethodDefinition : IVisitableDefinition
  {
    private RequirementDefinitionBase _declaringRequirement;
    private readonly MethodInfo _interfaceMethod;
    private readonly MethodDefinition _implementingMethod;

    public RequiredMethodDefinition (RequirementDefinitionBase declaringRequirement, MethodInfo interfaceMethod, MethodDefinition implementingMethod)
    {
      ArgumentUtility.CheckNotNull ("declaringRequirement", declaringRequirement);
      ArgumentUtility.CheckNotNull ("implementingMethod", implementingMethod);
      ArgumentUtility.CheckNotNull ("interfaceMethod", interfaceMethod);

      _declaringRequirement = declaringRequirement;
      _interfaceMethod = interfaceMethod;
      _implementingMethod = implementingMethod;
    }

    public RequirementDefinitionBase DeclaringRequirement
    {
      get { return _declaringRequirement; }
    }

    public MethodInfo InterfaceMethod
    {
      get { return _interfaceMethod; }
    }

    public MethodDefinition ImplementingMethod
    {
      get { return _implementingMethod; }
    }

    public string FullName
    {
      get { return _declaringRequirement.FullName + "." + _interfaceMethod.Name; }
    }

    public IVisitableDefinition Parent
    {
      get { return _declaringRequirement; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }
  }
}

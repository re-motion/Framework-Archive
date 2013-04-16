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
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: Docs.
  // TODO 5370: Tests.
  public class TargetTypeModifierContext
  {
    private readonly Type _target;
    private readonly MutableType _concreteTarget;

    public TargetTypeModifierContext (Type target, MutableType concreteTarget)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("concreteTarget", concreteTarget);

      _target = target;
      _concreteTarget = concreteTarget;
    }

    public Type Target
    {
      get { return _target; }
    }

    public MutableType ConcreteTarget
    {
      get { return _concreteTarget; }
    }

    public Expression ClassContextField { get; set; }
    public Expression MixinArrayInitializerField { get; set; }
    public Expression ExtensionsField { get; set; }
    public Expression FirstField { get; set; }

    public ConstructorInfo NextCallProxyConstructor { get; set; }
  }
}
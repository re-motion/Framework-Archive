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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Mixins.Definitions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  // TODO 5370: Docs.
  // TODO 5370: Tests.
  public class TargetTypeModifierContext
  {
    private readonly TargetClassDefinition _targetClassDefinition;
    private readonly ReadOnlyCollection<ConcreteMixinType> _concreteMixinTypes;
    private readonly ReadOnlyCollection<Type> _interfacesToImplement;
    private readonly INextCallProxyGenerator _nextCallProxyGenerator;
    private readonly MutableType _concreteTarget;

    public TargetTypeModifierContext (
        TargetClassDefinition targetClassDefinition,
        IEnumerable<ConcreteMixinType> concreteMixinTypes,
        IEnumerable<Type> interfacesToImplement,
        INextCallProxyGenerator nextCallProxyGenerator,
        MutableType concreteTarget)
    {
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull ("concreteMixinTypes", concreteMixinTypes);
      ArgumentUtility.CheckNotNull ("interfacesToImplement", interfacesToImplement);
      ArgumentUtility.CheckNotNull ("nextCallProxyGenerator", nextCallProxyGenerator);
      ArgumentUtility.CheckNotNull ("concreteTarget", concreteTarget);
      
      _targetClassDefinition = targetClassDefinition;
      _concreteMixinTypes = concreteMixinTypes.ToList().AsReadOnly();
      _interfacesToImplement = interfacesToImplement.ToList().AsReadOnly();
      _nextCallProxyGenerator = nextCallProxyGenerator;
      _concreteTarget = concreteTarget;
    }

    public TargetClassDefinition TargetClassDefinition
    {
      get { return _targetClassDefinition; }
    }

    public IEnumerable<Type> MixinTypes
    {
      get { return _targetClassDefinition.Mixins.Select (m => m.Type); }
    }

    public ReadOnlyCollection<ConcreteMixinType> ConcreteMixinTypes
    {
      get { return _concreteMixinTypes; }
    }

    public ReadOnlyCollection<Type> InterfacesToImplement
    {
      get { return _interfacesToImplement; }
    }

    public INextCallProxyGenerator NextCallProxyGenerator
    {
      get { return _nextCallProxyGenerator; }
    }

    public Type Target
    {
      get { return _targetClassDefinition.Type; }
    }

    public MutableType ConcreteTarget
    {
      get { return _concreteTarget; }
    }

    public Expression ClassContextField { get; set; }
    public Expression MixinArrayInitializerField { get; set; }
    public Expression ExtensionsField { get; set; }
    public Expression FirstField { get; set; }

    // TODO 5370: Make one interface.
    public ConstructorInfo NextCallProxyConstructor { get; set; } // tODO: to method that creates expression.
  }
}
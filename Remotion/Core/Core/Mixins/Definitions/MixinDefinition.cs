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
using System.Collections.Generic;
using System.Diagnostics;
using Remotion.Mixins.CodeGeneration;
using Remotion.Utilities;
using System.Reflection;
using System.Linq;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{Type}, TargetClass = {TargetClass.Type}")]
  public class MixinDefinition : ClassDefinitionBase, IAttributeIntroductionSource
  {
    private readonly UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> _interfaceIntroductions =
        new UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> (i => i.InterfaceType);
    private readonly UniqueDefinitionCollection<Type, NonInterfaceIntroductionDefinition> _nonInterfaceIntroductions =
        new UniqueDefinitionCollection<Type, NonInterfaceIntroductionDefinition> (i => i.InterfaceType);

    private readonly UniqueDefinitionCollection<Type, ThisDependencyDefinition> _thisDependencies =
        new UniqueDefinitionCollection<Type, ThisDependencyDefinition> (d => d.RequiredType.Type);
    private readonly UniqueDefinitionCollection<Type, BaseDependencyDefinition> _baseDependencies =
        new UniqueDefinitionCollection<Type, BaseDependencyDefinition> (d => d.RequiredType.Type);
    private readonly UniqueDefinitionCollection<Type, MixinDependencyDefinition> _mixinDependencies =
        new UniqueDefinitionCollection<Type, MixinDependencyDefinition> (d => d.RequiredType.Type);

    private readonly MultiDefinitionCollection<Type, AttributeIntroductionDefinition> _attributeIntroductions = 
        new MultiDefinitionCollection<Type, AttributeIntroductionDefinition> (a => a.AttributeType);
    private readonly MultiDefinitionCollection<Type, NonAttributeIntroductionDefinition> _nonAttributeIntroductions =
        new MultiDefinitionCollection<Type, NonAttributeIntroductionDefinition> (a => a.AttributeType);
    private readonly MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> _suppressedAttributeIntroductions =
        new MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> (a => a.AttributeType);

    private readonly TargetClassDefinition _targetClass;
    private readonly MixinKind _mixinKind;
    private readonly bool _acceptsAlphabeticOrdering;

    private ConcreteMixinTypeIdentifier _concreteTypeIdentifier;
    
    public MixinDefinition (MixinKind mixinKind, Type type, TargetClassDefinition targetClass, bool acceptsAlphabeticOrdering)
        : base (type)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);

      _mixinKind = mixinKind;
      _targetClass = targetClass;
      _acceptsAlphabeticOrdering = acceptsAlphabeticOrdering;
    }

    public MultiDefinitionCollection<Type, AttributeIntroductionDefinition> AttributeIntroductions
    {
      get { return _attributeIntroductions; }
    }

    public MultiDefinitionCollection<Type, NonAttributeIntroductionDefinition> NonAttributeIntroductions
    {
      get { return _nonAttributeIntroductions; }
    }

    public MultiDefinitionCollection<Type, SuppressedAttributeIntroductionDefinition> SuppressedAttributeIntroductions
    {
      get { return _suppressedAttributeIntroductions; }
    }

    public TargetClassDefinition TargetClass
    {
      get { return _targetClass; }
    }

    public MixinKind MixinKind
    {
      get { return _mixinKind; }
    }

    public bool AcceptsAlphabeticOrdering
    {
      get { return _acceptsAlphabeticOrdering; }
    }

    public override IVisitableDefinition Parent
    {
      get { return TargetClass; }
    }

    public int MixinIndex { get; internal set; }

    public UniqueDefinitionCollection<Type, ThisDependencyDefinition> ThisDependencies
    {
      get { return _thisDependencies; }
    }

    public UniqueDefinitionCollection<Type, BaseDependencyDefinition> BaseDependencies
    {
      get { return _baseDependencies; }
    }

    public UniqueDefinitionCollection<Type, MixinDependencyDefinition> MixinDependencies
    {
      get { return _mixinDependencies; }
    }

    public UniqueDefinitionCollection<Type, NonInterfaceIntroductionDefinition> NonInterfaceIntroductions
    {
      get { return _nonInterfaceIntroductions; }
    }

    public UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> InterfaceIntroductions
    {
      get { return _interfaceIntroductions; }
    }

    public IEnumerable<MemberDefinitionBase> GetAllOverrides ()
    {
      foreach (MemberDefinitionBase member in GetAllMembers ())
      {
        if (member.BaseAsMember != null)
          yield return member;
      }
    }

    protected override void ChildSpecificAccept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.Visit (this);

      _interfaceIntroductions.Accept (visitor);
      _nonInterfaceIntroductions.Accept (visitor);
      
      AttributeIntroductions.Accept (visitor);
      NonAttributeIntroductions.Accept (visitor);
      SuppressedAttributeIntroductions.Accept (visitor);

      _thisDependencies.Accept (visitor);
      _baseDependencies.Accept (visitor);
      _mixinDependencies.Accept (visitor);
    }

    public IEnumerable<DependencyDefinitionBase> GetOrderRelevantDependencies ()
    {
      foreach (var dependency in _baseDependencies)
        yield return dependency;
      foreach (var dependency in _mixinDependencies)
        yield return dependency;
    }

    public bool NeedsDerivedMixinType ()
    {
      return Type.IsAbstract || HasOverriddenMembers () || HasProtectedOverriders ();
    }

    public ConcreteMixinTypeIdentifier GetConcreteMixinTypeIdentifier ()
    {
      if (_concreteTypeIdentifier == null)
        _concreteTypeIdentifier = CalculateConcreteTypeIdentifier ();
      return _concreteTypeIdentifier;
    }

    private ConcreteMixinTypeIdentifier CalculateConcreteTypeIdentifier ()
    {
      // for each overridden member, find its topmost definition - that way we can share concrete mixin types between target classes that
      // overrider their base class' [OverrideMixin] methods

      var topMostOverriders = new HashSet<MethodInfo> (
          GetAllMethods ()
              .SelectMany (md => md.Overrides)
              .Select (ovr => ovr.MethodInfo.GetBaseDefinition()));
      var protectedOverriders = new HashSet<MethodInfo> (GetProtectedOverriders ().Select (md => md.MethodInfo));

      return new ConcreteMixinTypeIdentifier (Type, topMostOverriders, protectedOverriders);
    }
  }
}

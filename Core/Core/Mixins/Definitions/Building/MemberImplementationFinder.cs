/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  internal class MemberImplementationFinder
  {
    private readonly Type _declaringType;
    private readonly MixinDefinition _mixin;
    private readonly IDictionary<MethodInfo, MethodInfo> _mappingDictionary;
    private readonly UniqueDefinitionCollection<MethodInfo, MethodDefinition> _allMethods;

    public MemberImplementationFinder (Type declaringType, MixinDefinition implementingMixin)
    {
      ArgumentUtility.CheckNotNull ("implementingMixin", implementingMixin);
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);

      _declaringType = declaringType;
      _mixin = implementingMixin;

      _mappingDictionary = new Dictionary<MethodInfo, MethodInfo> ();
      InterfaceMapping mapping = _mixin.GetAdjustedInterfaceMap (_declaringType);
      for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
        _mappingDictionary.Add (mapping.InterfaceMethods[i], mapping.TargetMethods[i]);

      _allMethods = new UniqueDefinitionCollection<MethodInfo, MethodDefinition> (delegate (MethodDefinition m) {return m.MethodInfo; });
      foreach (MethodDefinition method in _mixin.GetAllMethods ())
        _allMethods.Add (method);
    }

    public MethodDefinition FindMethodImplementation (MethodInfo methodToFind)
    {
      MethodInfo targetMethod;
      if (_mappingDictionary.TryGetValue (methodToFind, out targetMethod))
        return _allMethods[targetMethod];
      else
        return null;
    }

    public PropertyDefinition FindPropertyImplementation (PropertyInfo propertyToFind)
    {
      MethodDefinition accessorImplementer = null;

      MethodInfo getter = propertyToFind.GetGetMethod ();
      MethodInfo setter = propertyToFind.GetSetMethod ();

      if (getter != null)
        accessorImplementer = FindMethodImplementation (getter);

      if (accessorImplementer == null && setter != null)
        accessorImplementer = FindMethodImplementation (setter);

      if (accessorImplementer != null)
      {
        PropertyDefinition property = accessorImplementer.Parent as PropertyDefinition;
        return property;
      }
      else
        return null;
    }

    public EventDefinition FindEventImplementation (EventInfo eventToFind)
    {
      MethodInfo accessor = eventToFind.GetAddMethod ();
      MethodDefinition accessorImplementer = FindMethodImplementation (accessor);

      if (accessorImplementer != null)
      {
        EventDefinition eventDefinition = accessorImplementer.Parent as EventDefinition;
        return eventDefinition;
      }
      else
        return null;
    }

    /*public TDefinition FindImplementerBySignature<TDefinition, TMemberInfo> (
        TMemberInfo interfaceMember,
        IEnumerable<TDefinition> candidates,
        IEqualityComparer<TMemberInfo> comparer)
        where TDefinition: MemberDefinitionBase
        where TMemberInfo: MemberInfo
    {
      List<TDefinition> strongCandidates = new List<TDefinition>();
      List<TDefinition> weakCandidates = new List<TDefinition>();

      foreach (TDefinition candidate in candidates)
        if (interfaceMember.Name == candidate.Name && comparer.Equals (interfaceMember, (TMemberInfo) candidate.MemberInfo))
          strongCandidates.Add (candidate);
        else if (candidate.Name.EndsWith (interfaceMember.Name) && comparer.Equals (interfaceMember, (TMemberInfo) candidate.MemberInfo))
          weakCandidates.Add (candidate);

      Assertion.IsTrue (
          strongCandidates.Count == 0 || strongCandidates.Count == 1, "If this throws, we have an oversight in the candidate algorithm.");

      if (strongCandidates.Count == 1)
        return strongCandidates[0];
      else if (weakCandidates.Count == 0)
        return null;
      else if (weakCandidates.Count == 1)
        return weakCandidates[0]; // probably an explicit interface implementation
      else // weakCandidates.Count > 1
      {
        string message = string.Format (
            "There are more than one implementer candidates for member {0}.{1}: {2}. The mixin engine cannot detect the right one.",
            interfaceMember.DeclaringType.FullName,
            interfaceMember.Name,
            SeparatedStringBuilder.Build (", ", weakCandidates, delegate (TDefinition d) { return d.FullName; }));
        throw new NotSupportedException (message);
      }
    }*/
  }
}

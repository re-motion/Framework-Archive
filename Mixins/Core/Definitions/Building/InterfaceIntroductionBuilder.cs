using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Mixins.Utilities;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions.Building
{
  public class InterfaceIntroductionBuilder
  {
    private static PropertySignatureEqualityComparer s_propertyComparer = new PropertySignatureEqualityComparer ();
    private static EventSignatureEqualityComparer s_eventComparer = new EventSignatureEqualityComparer ();

    private readonly MixinDefinition _mixin;

    public InterfaceIntroductionBuilder (MixinDefinition mixin)
    {
      _mixin = mixin;
    }

    public void Apply ()
    {
      foreach (Type implementedInterface in _mixin.ImplementedInterfaces)
      {
        if (!implementedInterface.Equals (typeof (System.Runtime.Serialization.ISerializable)))
          Apply (implementedInterface);
      }
    }

    public void Apply (Type implementedInterface)
    {
      if (_mixin.BaseClass.IntroducedInterfaces.ContainsKey (implementedInterface))
      {
        MixinDefinition otherIntroducer = _mixin.BaseClass.IntroducedInterfaces[implementedInterface].Implementer;
        string message = string.Format (
            "Two mixins introduce the same interface {0} to base class {1}: {2} and {3}.",
            implementedInterface.FullName,
            _mixin.BaseClass.FullName,
            otherIntroducer.FullName,
            _mixin.FullName);
        throw new ConfigurationException (message);
      }

      InterfaceIntroductionDefinition introducedInterface = new InterfaceIntroductionDefinition (implementedInterface, _mixin);
      _mixin.InterfaceIntroductions.Add (introducedInterface);
      _mixin.BaseClass.IntroducedInterfaces.Add (introducedInterface);

      AnalyzeIntroducedMembers (introducedInterface);
    }

    private void AnalyzeIntroducedMembers (InterfaceIntroductionDefinition introducedInterface)
    {
      Set<MethodInfo> specialMethods = new Set<MethodInfo> ();

      AnalyzeProperties(introducedInterface, specialMethods);
      AnalyzeEvents(introducedInterface, specialMethods);
      AnalyzeMethods(introducedInterface, specialMethods);
    }

    private TDefinition FindImplementer<TDefinition, TMemberInfo> (TMemberInfo interfaceMember, IEnumerable<TDefinition> candidates,
    IEqualityComparer<TMemberInfo> comparer)
      where TDefinition : MemberDefinition
      where TMemberInfo : MemberInfo
    {
      List<TDefinition> strongCandidates = new List<TDefinition> ();
      List<TDefinition> weakCandidates = new List<TDefinition> ();

      foreach (TDefinition candidate in candidates)
        if (interfaceMember.Name == candidate.Name && comparer.Equals (interfaceMember, (TMemberInfo) candidate.MemberInfo))
          strongCandidates.Add (candidate);
        else if (candidate.Name.EndsWith (interfaceMember.Name) && comparer.Equals (interfaceMember, (TMemberInfo) candidate.MemberInfo))
          weakCandidates.Add (candidate);

      Assertion.IsTrue (strongCandidates.Count == 0 || strongCandidates.Count == 1, "If this throws, we have an oversight in the candidate algorithm.");

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
            CollectionStringBuilder.BuildCollectionString (weakCandidates, ", ", delegate (TDefinition d) { return d.FullName; }));
        throw new NotSupportedException (message);
      }
    }

    private void AnalyzeProperties(InterfaceIntroductionDefinition introducedInterface, Set<MethodInfo> specialMethods)
    {
      foreach (PropertyInfo interfaceProperty in introducedInterface.Type.GetProperties ())
      {
        PropertyDefinition implementer = FindImplementer (interfaceProperty, _mixin.Properties, s_propertyComparer);
        CheckMemberImplementationFound (implementer, interfaceProperty);
        introducedInterface.IntroducedProperties.Add (new PropertyIntroductionDefinition (introducedInterface, interfaceProperty, implementer));

        MethodInfo getMethod = interfaceProperty.GetGetMethod();
        if (getMethod != null)
          specialMethods.Add (getMethod);

        MethodInfo setMethod = interfaceProperty.GetSetMethod();
        if (setMethod != null)
          specialMethods.Add (setMethod);
      }
    }

    private void AnalyzeEvents (InterfaceIntroductionDefinition introducedInterface, Set<MethodInfo> specialMethods)
    {
      foreach (EventInfo interfaceEvent in introducedInterface.Type.GetEvents ())
      {
        EventDefinition implementer = FindImplementer (interfaceEvent, _mixin.Events, s_eventComparer);
        CheckMemberImplementationFound (implementer, interfaceEvent);
        introducedInterface.IntroducedEvents.Add (new EventIntroductionDefinition (introducedInterface, interfaceEvent, implementer));

        specialMethods.Add (interfaceEvent.GetAddMethod());
        specialMethods.Add (interfaceEvent.GetRemoveMethod());
      }
    }

    private void AnalyzeMethods (InterfaceIntroductionDefinition introducedInterface, Set<MethodInfo> specialMethods)
    {
      InterfaceMapping mapping = _mixin.GetAdjustedInterfaceMap(introducedInterface.Type);
      for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
      {
        MethodInfo interfaceMethod = mapping.InterfaceMethods[i];
        if (!specialMethods.Contains (interfaceMethod))
        {
          MethodInfo targetMethod = mapping.TargetMethods[i];
          MethodDefinition implementer = _mixin.Methods[targetMethod];
          CheckMemberImplementationFound (implementer, mapping.InterfaceMethods[i]);
          introducedInterface.IntroducedMethods.Add (new MethodIntroductionDefinition (introducedInterface, mapping.InterfaceMethods[i], implementer));
        }
      }
    }

    private void CheckMemberImplementationFound (object implementation, MemberInfo interfaceMember)
    {
      if (implementation == null)
      {
        string message = string.Format ("An implementation for interface member {0}.{1} could not be found in mixin {2}.",
            interfaceMember.DeclaringType.FullName, interfaceMember.Name, _mixin.FullName);
        throw new ConfigurationException (message);
      }
    }
  }
}
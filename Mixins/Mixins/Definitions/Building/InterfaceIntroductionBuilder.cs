using System;
using System.Reflection;
using Mixins.Utilities;
using Rubicon.Collections;

namespace Mixins.Definitions.Building
{
  public class InterfaceIntroductionBuilder
  {
    private readonly MixinDefinition _mixin;

    public InterfaceIntroductionBuilder (MixinDefinition mixin)
    {
      _mixin = mixin;
    }

    public void Apply (Type implementedInterface)
    {
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

    private void AnalyzeProperties(InterfaceIntroductionDefinition introducedInterface, Set<MethodInfo> specialMethods)
    {
      foreach (PropertyInfo interfaceProperty in introducedInterface.Type.GetProperties ())
      {
        PropertyInfo correspondingMixinProperty = _mixin.Type.GetProperty (interfaceProperty.Name);
        CheckMemberImplementationFound (correspondingMixinProperty, interfaceProperty);
        PropertyDefinition implementer = _mixin.Properties[correspondingMixinProperty];
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
        EventInfo correspondingMixinEvent = _mixin.Type.GetEvent (interfaceEvent.Name);
        CheckMemberImplementationFound (correspondingMixinEvent, interfaceEvent);
        EventDefinition implementer = _mixin.Events[correspondingMixinEvent];
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
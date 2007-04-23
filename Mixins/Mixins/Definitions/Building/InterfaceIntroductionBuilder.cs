using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

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
      InterfaceMapping mapping = _mixin.Type.GetInterfaceMap (introducedInterface.Type);
      SpecialMethodSet specialMethods = new SpecialMethodSet();

      foreach (PropertyInfo interfaceProperty in introducedInterface.Type.GetProperties ())
      {
        PropertyInfo correspondingMixinProperty = _mixin.Type.GetProperty (interfaceProperty.Name);
        CheckMemberImplementationFound (correspondingMixinProperty, interfaceProperty);
        PropertyDefinition implementer = _mixin.Properties[correspondingMixinProperty];
        CheckMemberImplementationFound (implementer, interfaceProperty);
        introducedInterface.IntroducedMembers.Add (new MemberIntroductionDefinition (introducedInterface, interfaceProperty, implementer));

        specialMethods.Add (interfaceProperty.GetGetMethod());
        specialMethods.Add (interfaceProperty.GetSetMethod());
      }

      foreach (EventInfo interfaceEvent in introducedInterface.Type.GetEvents ())
      {
        EventInfo correspondingMixinEvent = _mixin.Type.GetEvent (interfaceEvent.Name);
        CheckMemberImplementationFound (correspondingMixinEvent, interfaceEvent);
        EventDefinition implementer = _mixin.Events[correspondingMixinEvent];
        CheckMemberImplementationFound (implementer, interfaceEvent);
        introducedInterface.IntroducedMembers.Add (new MemberIntroductionDefinition (introducedInterface, interfaceEvent, implementer));

        specialMethods.Add (interfaceEvent.GetAddMethod());
        specialMethods.Add (interfaceEvent.GetRemoveMethod());
      }

      for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
      {
        MethodInfo interfaceMethod = mapping.InterfaceMethods[i];
        if (!specialMethods.Contains (interfaceMethod))
        {
          MethodDefinition implementer = _mixin.Methods[mapping.TargetMethods[i]];
          CheckMemberImplementationFound (implementer, mapping.InterfaceMethods[i]);
          introducedInterface.IntroducedMembers.Add (new MemberIntroductionDefinition (introducedInterface, mapping.InterfaceMethods[i], implementer));
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
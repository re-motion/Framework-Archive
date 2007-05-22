using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mixins.Definitions.Building
{
  public class RequiredBaseCallMethodDefinitionBuilder
  {
    private BaseClassDefinition _classDefinition;

    public RequiredBaseCallMethodDefinitionBuilder(BaseClassDefinition classDefinition)
    {
      _classDefinition = classDefinition;
    }

    public void Apply (RequiredBaseCallTypeDefinition declaringRequirement)
    {
      Type requiredType = declaringRequirement.Type;
      Apply(declaringRequirement, requiredType);
    }

    private void Apply(RequiredBaseCallTypeDefinition declaringRequirement, Type requiredType)
    {
      if (!declaringRequirement.IsEmptyInterface)
      {
        if (_classDefinition.ImplementedInterfaces.Contains (requiredType)/* || requiredType.Equals (typeof (object))*/)
        {
          ApplyForImplementedInterface (declaringRequirement, requiredType);
        }
        else if (_classDefinition.IntroducedInterfaces.HasItem (requiredType))
        {
          ApplyForIntroducedInterface (declaringRequirement, requiredType);
        }
        else
        {
          string message = string.Format ("The base call dependency {0} in base class {1} is not fulfilled - the type is neither introduced "
              + "nor implemented.", declaringRequirement.FullName, _classDefinition.FullName);
          throw new ConfigurationException (message);
        }
      }
    }

    private void ApplyForImplementedInterface (RequiredBaseCallTypeDefinition declaringRequirement, Type requiredType)
    {
      InterfaceMapping interfaceMapping = _classDefinition.GetAdjustedInterfaceMap (requiredType);
      for (int i = 0; i < interfaceMapping.InterfaceMethods.Length; ++i)
      {
        MethodInfo interfaceMethod = interfaceMapping.InterfaceMethods[i];
        MethodDefinition implementingMethod = _classDefinition.Methods[interfaceMapping.TargetMethods[i]];
        declaringRequirement.BaseCallMethods.Add (new RequiredBaseCallMethodDefinition (declaringRequirement, interfaceMethod, implementingMethod));
      }
    }

    private void ApplyForIntroducedInterface (RequiredBaseCallTypeDefinition declaringRequirement, Type requiredType)
    {
      InterfaceIntroductionDefinition introduction = _classDefinition.IntroducedInterfaces[declaringRequirement.Type];
      foreach (MethodIntroductionDefinition methodIntroduction in introduction.IntroducedMethods)
      {
        MethodInfo interfaceMethod = methodIntroduction.InterfaceMember;
        MethodDefinition implementingMethod = methodIntroduction.ImplementingMember;
        declaringRequirement.BaseCallMethods.Add (new RequiredBaseCallMethodDefinition(declaringRequirement, interfaceMethod, implementingMethod));
      }
    }
  }
}

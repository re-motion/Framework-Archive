using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mixins.Utilities;
using Rubicon.Text;

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
      if (!declaringRequirement.IsAggregatorInterface)
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
          string dependenciesString = CollectionStringBuilder.BuildCollectionString (declaringRequirement.FindRequiringMixins(),
              ", ", delegate (MixinDefinition m) { return m.FullName; });
          string message = string.Format ("The base call dependency {0} (mixins {1}) applied to class {2} is not fulfilled - the type is neither "
              + "introduced nor implemented as an interface.", declaringRequirement.FullName, dependenciesString, _classDefinition.FullName);
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

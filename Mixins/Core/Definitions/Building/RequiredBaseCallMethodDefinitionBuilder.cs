using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Collections;
using Rubicon.Mixins.Utilities;
using Rubicon.Text;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions.Building
{
  public class RequiredBaseCallMethodDefinitionBuilder
  {
    private static MethodNameAndSignatureEqualityComparer s_methodComparer = new MethodNameAndSignatureEqualityComparer ();

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
        if (_classDefinition.ImplementedInterfaces.Contains (requiredType) /* || requiredType.Equals (typeof (object))*/)
          ApplyForImplementedInterface (declaringRequirement, requiredType);
        else if (_classDefinition.IntroducedInterfaces.ContainsKey (requiredType))
          ApplyForIntroducedInterface (declaringRequirement, requiredType);
        else
          ApplyForDuckInterface (declaringRequirement, requiredType);
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

    private void ApplyForDuckInterface (RequiredBaseCallTypeDefinition declaringRequirement, Type requiredType)
    {
      Assertion.Assert (requiredType.IsInterface);
      foreach (MethodInfo interfaceMethod in requiredType.GetMethods ())
      {
        MethodDefinition implementingMethod = FindDuckMethod (declaringRequirement, interfaceMethod);
        declaringRequirement.BaseCallMethods.Add (new RequiredBaseCallMethodDefinition (declaringRequirement, interfaceMethod, implementingMethod));
      }
    }

    private MethodDefinition FindDuckMethod (RequiredBaseCallTypeDefinition declaringRequirement, MethodInfo interfaceMethod)
    {
      foreach (MethodDefinition baseMethod in _classDefinition.Methods)
      {
        if (s_methodComparer.Equals (baseMethod.MethodInfo, interfaceMethod))
          return baseMethod;
      }

      string dependenciesString = CollectionStringBuilder.BuildCollectionString (declaringRequirement.FindRequiringMixins (),
          ", ", delegate (MixinDefinition m) { return m.FullName; });
      string message = string.Format ("The base call dependency {0} (mixins {1}) applied to class {2} is not fulfilled - method {3} does not have "
          + "an equivalent on the base class.", declaringRequirement.FullName, dependenciesString, _classDefinition.FullName, interfaceMethod.Name);
      throw new ConfigurationException (message);

    }
  }
}

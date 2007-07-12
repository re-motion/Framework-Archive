using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class RequiredMethodDefinitionBuilderTests
  {
    [Test]
    public void RequiredFaceMethodsImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (MixinRequiringAllMembers)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);
      
      Assert.AreEqual (5, requirement.Methods.Count);

      RequiredMethodDefinition method = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method")];
      Assert.IsNotNull (method);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method"), method.InterfaceMethod);
      Assert.AreSame (baseClassDefinition.Methods[typeof (ClassFulfillingAllMemberRequirements).GetMethod ("Method")],
          method.ImplementingMethod);

      RequiredMethodDefinition propertyGetter = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("get_Property")];
      Assert.IsNotNull (propertyGetter);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("get_Property"), propertyGetter.InterfaceMethod);
      Assert.AreSame (baseClassDefinition.Properties[typeof (ClassFulfillingAllMemberRequirements).GetProperty ("Property")].GetMethod,
          propertyGetter.ImplementingMethod);

      RequiredMethodDefinition propertySetter = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("set_Property")];
      Assert.IsNotNull (propertySetter);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("set_Property"), propertySetter.InterfaceMethod);
      Assert.AreSame (baseClassDefinition.Properties[typeof (ClassFulfillingAllMemberRequirements).GetProperty ("Property")].SetMethod,
          propertySetter.ImplementingMethod);

      RequiredMethodDefinition eventAdder = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("add_Event")];
      Assert.IsNotNull (eventAdder);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("add_Event"), eventAdder.InterfaceMethod);
      Assert.AreSame (baseClassDefinition.Events[typeof (ClassFulfillingAllMemberRequirements).GetEvent ("Event")].AddMethod,
          eventAdder.ImplementingMethod);

      RequiredMethodDefinition eventRemover = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("remove_Event")];
      Assert.IsNotNull (eventRemover);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("remove_Event"), eventRemover.InterfaceMethod);
      Assert.AreSame (baseClassDefinition.Events[typeof (ClassFulfillingAllMemberRequirements).GetEvent ("Event")].RemoveMethod,
          eventRemover.ImplementingMethod);
    }

    [Test]
    [Ignore ("TODO")]
    public void RequiredBaseMethodsAndMethodsIntroducedAndDuckMethods ()
    {
    }
  }
}
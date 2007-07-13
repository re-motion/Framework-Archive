using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class RequiredMethodDefinitionBuilderTests
  {
    private static void CheckRequiredMethods (RequiredFaceTypeDefinition requirement, ClassDefinitionBase implementer)
    {
      Assert.AreEqual (5, requirement.Methods.Count);

      RequiredMethodDefinition method = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method")];
      Assert.IsNotNull (method);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method"), method.InterfaceMethod);
      Assert.AreSame (implementer.Methods[implementer.Type.GetMethod ("Method")],
          method.ImplementingMethod);

      RequiredMethodDefinition propertyGetter = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("get_Property")];
      Assert.IsNotNull (propertyGetter);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("get_Property"), propertyGetter.InterfaceMethod);
      Assert.AreSame (implementer.Properties[implementer.Type.GetProperty ("Property")].GetMethod,
          propertyGetter.ImplementingMethod);

      RequiredMethodDefinition propertySetter = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("set_Property")];
      Assert.IsNotNull (propertySetter);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("set_Property"), propertySetter.InterfaceMethod);
      Assert.AreSame (implementer.Properties[implementer.Type.GetProperty ("Property")].SetMethod,
          propertySetter.ImplementingMethod);

      RequiredMethodDefinition eventAdder = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("add_Event")];
      Assert.IsNotNull (eventAdder);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("add_Event"), eventAdder.InterfaceMethod);
      Assert.AreSame (implementer.Events[implementer.Type.GetEvent ("Event")].AddMethod,
          eventAdder.ImplementingMethod);

      RequiredMethodDefinition eventRemover = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("remove_Event")];
      Assert.IsNotNull (eventRemover);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("remove_Event"), eventRemover.InterfaceMethod);
      Assert.AreSame (implementer.Events[implementer.Type.GetEvent ("Event")].RemoveMethod,
          eventRemover.ImplementingMethod);
    }

    [Test]
    public void RequiredFaceMethodsInterfaceImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (MixinRequiringAllMembers)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods(requirement, baseClassDefinition);
    }

    [Test]
    public void RequiredFaceMethodsInterfaceImplementedOnMixin ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassFulfillingNoMemberRequirements), typeof (MixinRequiringAllMembers),
          typeof (MixinFulfillingAllMemberRequirements)))
      {
        BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingNoMemberRequirements));
        MixinDefinition mixin = baseClassDefinition.Mixins[typeof (MixinRequiringAllMembers)];
        Assert.IsNotNull (mixin);

        MixinDefinition implementingMixin = baseClassDefinition.Mixins[typeof (MixinFulfillingAllMemberRequirements)];
        Assert.IsNotNull (implementingMixin);

        RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
        Assert.IsNotNull (requirement);

        CheckRequiredMethods (requirement, implementingMixin);
      }
    }

    [Test]
    public void RequiredFaceMethodsDuckImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirementsDuck));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (MixinRequiringAllMembers)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, baseClassDefinition);
    }

    public class MixinRequiringSingleMethod : Mixin<MixinRequiringSingleMethod.IRequirement>
    {
      public interface IRequirement
      {
        void Method ();
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency IRequirement (mixins Rubicon.Mixins.UnitTests.Configuration.RequiredMethodDefinitionBuilderTests+"
        + "MixinRequiringSingleMethod applied to class System.Object) is not fulfilled - method Method could not be found on the base class.")]
    public void ThrowsIfMethodRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (object), typeof (MixinRequiringSingleMethod)))
      {
        TypeFactory.GetActiveConfiguration (typeof (object));
        Assert.Fail ();
      }
    }

    public class MixinRequiringSingleProperty : Mixin<MixinRequiringSingleProperty.IRequirement>
    {
      public interface IRequirement
      {
        int Property { get; }
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
         ExpectedMessage = "The dependency IRequirement (mixins Rubicon.Mixins.UnitTests.Configuration.RequiredMethodDefinitionBuilderTests+"
        + "MixinRequiringSingleProperty applied to class System.Object) is not fulfilled - property Property could not be found on the base class.")]
    public void ThrowsIfPropertyRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (object), typeof (MixinRequiringSingleProperty)))
      {
        TypeFactory.GetActiveConfiguration (typeof (object));
        Assert.Fail ();
      }
    }

    public class MixinRequiringSingleEvent : Mixin<MixinRequiringSingleEvent.IRequirement>
    {
      public interface IRequirement
      {
        event EventHandler Event;
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
         ExpectedMessage = "The dependency IRequirement (mixins Rubicon.Mixins.UnitTests.Configuration.RequiredMethodDefinitionBuilderTests+"
        + "MixinRequiringSingleEvent applied to class System.Object) is not fulfilled - event Event could not be found on the base class.")]
    public void ThrowsIfEventRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (object), typeof (MixinRequiringSingleEvent)))
      {
        TypeFactory.GetActiveConfiguration (typeof (object));
        Assert.Fail ();
      }
    }

    [Test]
    [Ignore ("TODO")]
    public void RequiredBaseMethodsAndMethodsIntroducedAndDuckMethods ()
    {
    }
  }
}
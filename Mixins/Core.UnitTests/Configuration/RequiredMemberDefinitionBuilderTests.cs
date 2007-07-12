using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class RequiredMemberDefinitionBuilderTests
  {
    /*
    [Test]
    public void RequiredFaceMethodsImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (ClassFulfillingAllMemberRequirements)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);
      
      Assert.AreEqual (1, requirement.Methods.Count);

      RequiredMethodDefinition method = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method")];
      Assert.IsNotNull (method);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method"), method.InterfaceMethod);
      Assert.AreSame (baseClassDefinition.Methods[typeof (ClassFulfillingAllMemberRequirements).GetMethod ("Method")],
          method.ImplementingMethod);
    }

    [Test]
    public void RequiredFacePropertiesImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (ClassFulfillingAllMemberRequirements)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      Assert.AreEqual (1, requirement.Properties.Count);

      RequiredPropertyDefinition Property = requirement.Properties[typeof (IMixinRequiringAllMembersRequirements).GetProperty ("Property")];
      Assert.IsNotNull (Property);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetProperty ("Property"), Property.InterfaceProperty);
      Assert.AreSame (baseClassDefinition.Properties[typeof (ClassFulfillingAllMemberRequirements).GetProperty ("Property")],
          Property.ImplementingProperty);
    }

    [Test]
    public void RequiredFaceEventsImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (ClassFulfillingAllMemberRequirements)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      Assert.AreEqual (1, requirement.Events.Count);

      RequiredEventDefinition Event = requirement.Events[typeof (IMixinRequiringAllMembersRequirements).GetEvent ("Event")];
      Assert.IsNotNull (Event);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetEvent ("Event"), Event.InterfaceEvent);
      Assert.AreSame (baseClassDefinition.Events[typeof (ClassFulfillingAllMemberRequirements).GetEvent ("Event")],
          Event.ImplementingEvent);
    }

    [Test]
    public void RequiredBaseCallMethodsImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (ClassFulfillingAllMemberRequirements)];
      Assert.IsNotNull (mixin);

      RequiredBaseCallTypeDefinition requirement = mixin.BaseDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      Assert.AreEqual (1, requirement.Methods.Count);

      RequiredMethodDefinition method = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method")];
      Assert.IsNotNull (method);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method"), method.InterfaceMethod);
      Assert.AreSame (baseClassDefinition.Methods[typeof (ClassFulfillingAllMemberRequirements).GetMethod ("Method")],
          method.ImplementingMethod);
    }

    [Test]
    public void RequiredBaseCallPropertiesImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (ClassFulfillingAllMemberRequirements)];
      Assert.IsNotNull (mixin);

      RequiredBaseCallTypeDefinition requirement = mixin.BaseDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      Assert.AreEqual (1, requirement.Properties.Count);

      RequiredPropertyDefinition Property = requirement.Properties[typeof (IMixinRequiringAllMembersRequirements).GetProperty ("Property")];
      Assert.IsNotNull (Property);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetProperty ("Property"), Property.InterBaseCallProperty);
      Assert.AreSame (baseClassDefinition.Properties[typeof (ClassFulfillingAllMemberRequirements).GetProperty ("Property")],
          Property.ImplementingProperty);
    }

    [Test]
    public void RequiredBaseCallEventsImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (ClassFulfillingAllMemberRequirements)];
      Assert.IsNotNull (mixin);

      RequiredBaseCallTypeDefinition requirement = mixin.BaseDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      Assert.AreEqual (1, requirement.Events.Count);

      RequiredEventDefinition Event = requirement.Events[typeof (IMixinRequiringAllMembersRequirements).GetEvent ("Event")];
      Assert.IsNotNull (Event);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetEvent ("Event"), Event.InterBaseCallEvent);
      Assert.AreSame (baseClassDefinition.Events[typeof (ClassFulfillingAllMemberRequirements).GetEvent ("Event")],
          Event.ImplementingEvent);
    }
    */
  }
}
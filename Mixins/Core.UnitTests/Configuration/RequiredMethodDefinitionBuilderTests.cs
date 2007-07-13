using System;
using NUnit.Framework;
using Rubicon;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using System.Reflection;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class RequiredMethodDefinitionBuilderTests
  {
    private static void CheckRequiredMethods (RequirementDefinitionBase requirement, ClassDefinitionBase implementer, string memberPrefix)
    {
      BindingFlags bf = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
      Assert.AreEqual (5, requirement.Methods.Count);

      RequiredMethodDefinition method = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method", bf)];
      Assert.IsNotNull (method);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("Method", bf), method.InterfaceMethod);
      Assert.AreSame (implementer.Methods[implementer.Type.GetMethod (memberPrefix + "Method", bf)],
          method.ImplementingMethod);

      RequiredMethodDefinition propertyGetter = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("get_Property", bf)];
      Assert.IsNotNull (propertyGetter);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("get_Property", bf), propertyGetter.InterfaceMethod);
      Assert.AreSame (implementer.Properties[implementer.Type.GetProperty (memberPrefix + "Property", bf)].GetMethod,
          propertyGetter.ImplementingMethod);

      RequiredMethodDefinition propertySetter = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("set_Property", bf)];
      Assert.IsNotNull (propertySetter);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("set_Property", bf), propertySetter.InterfaceMethod);
      Assert.AreSame (implementer.Properties[implementer.Type.GetProperty (memberPrefix + "Property", bf)].SetMethod,
          propertySetter.ImplementingMethod);

      RequiredMethodDefinition eventAdder = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("add_Event", bf)];
      Assert.IsNotNull (eventAdder);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("add_Event", bf), eventAdder.InterfaceMethod);
      Assert.AreSame (implementer.Events[implementer.Type.GetEvent (memberPrefix + "Event", bf)].AddMethod,
          eventAdder.ImplementingMethod);

      RequiredMethodDefinition eventRemover = requirement.Methods[typeof (IMixinRequiringAllMembersRequirements).GetMethod ("remove_Event", bf)];
      Assert.IsNotNull (eventRemover);
      Assert.AreEqual (typeof (IMixinRequiringAllMembersRequirements).GetMethod ("remove_Event", bf), eventRemover.InterfaceMethod);
      Assert.AreSame (implementer.Events[implementer.Type.GetEvent (memberPrefix + "Event", bf)].RemoveMethod,
          eventRemover.ImplementingMethod);
    }

    [Test]
    public void RequiredFaceMethodsInterfaceImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (MixinRequiringAllMembersFace)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods(requirement, baseClassDefinition, "");
    }

    [Test]
    public void RequiredBaseCallMethodsInterfaceImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirements));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (MixinRequiringAllMembersBase)];
      Assert.IsNotNull (mixin);

      RequiredBaseCallTypeDefinition requirement = mixin.BaseDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, baseClassDefinition, "");
    }

    [Test]
    public void RequiredFaceMethodsInterfaceImplementedOnMixin ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassFulfillingNoMemberRequirements), typeof (MixinRequiringAllMembersFace),
          typeof (MixinFulfillingAllMemberRequirements)))
      {
        BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingNoMemberRequirements));
        MixinDefinition mixin = baseClassDefinition.Mixins[typeof (MixinRequiringAllMembersFace)];
        Assert.IsNotNull (mixin);

        MixinDefinition implementingMixin = baseClassDefinition.Mixins[typeof (MixinFulfillingAllMemberRequirements)];
        Assert.IsNotNull (implementingMixin);

        RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
        Assert.IsNotNull (requirement);

        CheckRequiredMethods (requirement, implementingMixin, "");
      }
    }

    [Test]
    public void RequiredBaseCallMethodsInterfaceImplementedOnMixin ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassFulfillingNoMemberRequirements), typeof (MixinRequiringAllMembersBase),
          typeof (MixinFulfillingAllMemberRequirements)))
      {
        BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingNoMemberRequirements));
        MixinDefinition mixin = baseClassDefinition.Mixins[typeof (MixinRequiringAllMembersBase)];
        Assert.IsNotNull (mixin);

        MixinDefinition implementingMixin = baseClassDefinition.Mixins[typeof (MixinFulfillingAllMemberRequirements)];
        Assert.IsNotNull (implementingMixin);

        RequiredBaseCallTypeDefinition requirement = mixin.BaseDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
        Assert.IsNotNull (requirement);

        CheckRequiredMethods (requirement, implementingMixin, "");
      }
    }

    [Test]
    public void RequiredFaceMethodsDuckImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirementsDuck));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (MixinRequiringAllMembersFace)];
      Assert.IsNotNull (mixin);

      RequiredFaceTypeDefinition requirement = mixin.ThisDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, baseClassDefinition, "");
    }

    [Test]
    public void RequiredBaseCallMethodsDuckImplementedOnBase ()
    {
      BaseClassDefinition baseClassDefinition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirementsDuck));
      MixinDefinition mixin = baseClassDefinition.Mixins[typeof (MixinRequiringAllMembersBase)];
      Assert.IsNotNull (mixin);

      RequiredBaseCallTypeDefinition requirement = mixin.BaseDependencies[typeof (IMixinRequiringAllMembersRequirements)].RequiredType;
      Assert.IsNotNull (requirement);

      CheckRequiredMethods (requirement, baseClassDefinition, "");
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
       + "MixinRequiringSingleMethod applied to class System.Object) is not fulfilled - public or protected method Method could not be found on the base class.")]
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
       + "MixinRequiringSingleProperty applied to class System.Object) is not fulfilled - public or protected property Property could not be found on the base class.")]
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
       + "MixinRequiringSingleEvent applied to class System.Object) is not fulfilled - public or protected event Event could not be found on the base class.")]
    public void ThrowsIfEventRequirementIsNotFulfilled ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (object), typeof (MixinRequiringSingleEvent)))
      {
        TypeFactory.GetActiveConfiguration (typeof (object));
        Assert.Fail ();
      }
    }

    public class ClassFulfillingPrivately
    {
      private void Method ()
      {
        throw new NotImplementedException ();
      }

      public int Property
      {
        get { throw new NotImplementedException (); }
        set { throw new NotImplementedException (); }
      }

      public event Func<string> Event;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The dependency IMixinRequiringAllMembersRequirements (mixins "
      + "Rubicon.Mixins.UnitTests.SampleTypes.MixinRequiringAllMembersFace applied to class "
      + "Rubicon.Mixins.UnitTests.Configuration.RequiredMethodDefinitionBuilderTests+ClassFulfillingPrivately) is not fulfilled - "
      + "public or protected method Method could not be found on the base class.")]
    public void ThrowsIfRequiredMethodIsPrivate ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassFulfillingPrivately), typeof (MixinRequiringAllMembersFace)))
      {
        TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingPrivately));
      }
    }

    public class ClassFulfillingInternally
    {
      internal void Method ()
      {
        throw new NotImplementedException ();
      }

      public int Property
      {
        get { throw new NotImplementedException (); }
        set { throw new NotImplementedException (); }
      }

      public event Func<string> Event;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The dependency IMixinRequiringAllMembersRequirements (mixins "
      + "Rubicon.Mixins.UnitTests.SampleTypes.MixinRequiringAllMembersFace applied to class "
      + "Rubicon.Mixins.UnitTests.Configuration.RequiredMethodDefinitionBuilderTests+ClassFulfillingInternally) is not fulfilled - "
      + "public or protected method Method could not be found on the base class.")]
    public void ThrowsIfRequiredMethodIsInternal ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassFulfillingInternally), typeof (MixinRequiringAllMembersFace)))
      {
        TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingInternally));
      }
    }

    public class ClassFulfillingProtectedly
    {
      protected void Method ()
      {
        throw new NotImplementedException ();
      }

      protected int Property
      {
        get { throw new NotImplementedException (); }
        set { throw new NotImplementedException (); }
      }

      protected event Func<string> Event;
    }

    [Test]
    public void WorksIfRequiredMethodIsProtected ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassFulfillingProtectedly), typeof (MixinRequiringAllMembersFace)))
      {
        BaseClassDefinition definition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingProtectedly));
        RequiredFaceTypeDefinition requirement = definition.RequiredFaceTypes[typeof (IMixinRequiringAllMembersRequirements)];

        CheckRequiredMethods (requirement, definition, "");
      }
    }

    [Test]
    public void WorksIfExplicitlyImplemented ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ClassFulfillingAllMemberRequirementsExplicitly), typeof (MixinRequiringAllMembersFace)))
      {
        BaseClassDefinition definition = TypeFactory.GetActiveConfiguration (typeof (ClassFulfillingAllMemberRequirementsExplicitly));
        RequiredFaceTypeDefinition requirement = definition.RequiredFaceTypes[typeof (IMixinRequiringAllMembersRequirements)];

        CheckRequiredMethods (requirement, definition, typeof (IMixinRequiringAllMembersRequirements).FullName + ".");
      }
    }
  }
}
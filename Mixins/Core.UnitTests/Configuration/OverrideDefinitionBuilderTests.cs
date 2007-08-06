using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Definitions.Building;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class OverrideDefinitionBuilderTests
  {
    [Test]
    public void MethodOverrides ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
        MixinDefinition mixin2 = baseClass.Mixins[typeof (BT1Mixin2)];

        Assert.IsFalse (mixin1.HasOverriddenMembers());
        Assert.IsFalse (mixin2.HasOverriddenMembers ());
        Assert.IsTrue (baseClass.HasOverriddenMembers ());

        MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
        MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] {typeof (string)});
        MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

        MethodDefinition overridden = baseClass.Methods[baseMethod1];

        Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin1)));
        MethodDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

        Assert.AreSame (overrider, mixin1.Methods[mixinMethod1]);
        Assert.IsNotNull (overrider.Base);
        Assert.AreSame (overridden, overrider.Base);

        MethodDefinition notOverridden = baseClass.Methods[baseMethod2];
        Assert.AreEqual (0, notOverridden.Overrides.Count);

        Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin2)));
        overrider = overridden.Overrides[typeof (BT1Mixin2)];

        Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides()).Contains (overrider));
        Assert.AreSame (overridden, overrider.Base);
      }
    }

    [Test]
    public void PropertyOverrides ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
        MixinDefinition mixin2 = baseClass.Mixins[typeof (BT1Mixin2)];

        PropertyInfo baseProperty1 = typeof (BaseType1).GetProperty ("VirtualProperty");
        PropertyInfo baseProperty2 = typeof (BaseType1).GetProperty ("Item", new Type[] {typeof (string)});
        PropertyInfo mixinProperty1 = typeof (BT1Mixin1).GetProperty ("VirtualProperty");

        PropertyDefinition overridden = baseClass.Properties[baseProperty1];

        Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin1)));

        PropertyDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

        Assert.AreSame (overrider, mixin1.Properties[mixinProperty1]);
        Assert.IsNotNull (overrider.Base);
        Assert.AreSame (overridden, overrider.Base);
        Assert.AreSame (overridden.SetMethod, overrider.SetMethod.Base);

        PropertyDefinition notOverridden = baseClass.Properties[baseProperty2];
        Assert.AreEqual (0, notOverridden.Overrides.Count);

        Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin2)));
        overrider = overridden.Overrides[typeof (BT1Mixin2)];

        Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides()).Contains (overrider));
        Assert.AreSame (overridden, overrider.Base);
        Assert.AreSame (overridden.GetMethod, overrider.GetMethod.Base);
      }
    }

    [Test]
    public void EventOverrides ()
    {
      using (MixinConfiguration.ScopedExtend (Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
        MixinDefinition mixin2 = baseClass.Mixins[typeof (BT1Mixin2)];

        EventInfo baseEvent1 = typeof (BaseType1).GetEvent ("VirtualEvent");
        EventInfo baseEvent2 = typeof (BaseType1).GetEvent ("ExplicitEvent");
        EventInfo mixinEvent1 = typeof (BT1Mixin1).GetEvent ("VirtualEvent");

        EventDefinition overridden = baseClass.Events[baseEvent1];

        Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin1)));

        EventDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

        Assert.AreSame (overrider, mixin1.Events[mixinEvent1]);
        Assert.IsNotNull (overrider.Base);
        Assert.AreSame (overridden, overrider.Base);
        Assert.AreSame (overridden.RemoveMethod, overrider.RemoveMethod.Base);
        Assert.AreSame (overridden.AddMethod, overrider.AddMethod.Base);

        EventDefinition notOverridden = baseClass.Events[baseEvent2];
        Assert.AreEqual (0, notOverridden.Overrides.Count);

        Assert.IsTrue (overridden.Overrides.ContainsKey (typeof (BT1Mixin2)));
        overrider = overridden.Overrides[typeof (BT1Mixin2)];

        Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides()).Contains (overrider));
        Assert.AreSame (overridden, overrider.Base);
        Assert.AreSame (overridden.AddMethod, overrider.AddMethod.Base);
        Assert.AreSame (overridden.RemoveMethod, overrider.RemoveMethod.Base);
      }
    }

    [Test]
    public void OverrideNonVirtualMethod ()
    {
      BaseClassDefinition baseClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      MixinDefinition mixin = baseClass.Mixins[typeof (BT4Mixin1)];
      Assert.IsNotNull (mixin);

      MethodDefinition overrider = mixin.Methods[typeof (BT4Mixin1).GetMethod ("NonVirtualMethod")];
      Assert.IsNotNull (overrider);
      Assert.IsNotNull (overrider.Base);

      Assert.AreSame (baseClass, overrider.Base.DeclaringClass);

      List<MethodDefinition> overrides = new List<MethodDefinition> (baseClass.Methods[typeof (BaseType4).GetMethod ("NonVirtualMethod")].Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    public void OverrideNonVirtualProperty ()
    {
      BaseClassDefinition baseClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      MixinDefinition mixin = baseClass.Mixins[typeof (BT4Mixin1)];
      Assert.IsNotNull (mixin);

      PropertyDefinition overrider = mixin.Properties[typeof (BT4Mixin1).GetProperty ("NonVirtualProperty")];
      Assert.IsNotNull (overrider);
      Assert.IsNotNull (overrider.Base);

      Assert.AreSame (baseClass, overrider.Base.DeclaringClass);

      List<PropertyDefinition> overrides = new List<PropertyDefinition> (baseClass.Properties[typeof (BaseType4).GetProperty ("NonVirtualProperty")].Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    public void OverrideNonVirtualEvent ()
    {
      BaseClassDefinition baseClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      MixinDefinition mixin = baseClass.Mixins[typeof (BT4Mixin1)];
      Assert.IsNotNull (mixin);

      EventDefinition overrider = mixin.Events[typeof (BT4Mixin1).GetEvent ("NonVirtualEvent")];
      Assert.IsNotNull (overrider);
      Assert.IsNotNull (overrider.Base);

      Assert.AreSame (baseClass, overrider.Base.DeclaringClass);

      List<EventDefinition> overrides = new List<EventDefinition> (baseClass.Events[typeof (BaseType4).GetEvent ("NonVirtualEvent")].Overrides);
      Assert.AreEqual (1, overrides.Count);
      Assert.AreSame (overrider, overrides[0]);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Could not find base member for overrider .*BT5Mixin1.Method.",
       MatchType = MessageMatch.Regex)]
    public void ThrowsWhenInexistingOverrideBaseMethod ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin1));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Could not find base member for overrider .*BT5Mixin4.Property.",
       MatchType = MessageMatch.Regex)]
    public void ThrowsWhenInexistingOverrideBaseProperty ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin4));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Could not find base member for overrider .*BT5Mixin5.Event.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsWhenInexistingOverrideBaseEvent ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin5));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Ambiguous override", MatchType = MessageMatch.Contains)]
    public void ThrowsOnTargetClassOverridingMultipleMixinMethods()
    {
      using (MixinConfiguration.ScopedExtend(typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers), typeof(MixinWithSingleAbstractMethod2)))
      {
        TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMembers));
      }
    }

    [Test]
    public void OverridingProtectedInheritedClassMethod ()
    {
      BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (ClassWithInheritedMethod));
      MethodDefinition inheritedMethod = baseClass.Methods[typeof (BaseClassWithInheritedMethod).GetMethod ("ProtectedInheritedMethod",
          BindingFlags.NonPublic | BindingFlags.Instance)];
      Assert.IsNotNull (inheritedMethod);
      Assert.AreEqual (1, inheritedMethod.Overrides.Count);
      Assert.AreSame (
          baseClass.Mixins[typeof (MixinOverridingInheritedMethod)].Methods[typeof (MixinOverridingInheritedMethod).GetMethod ("ProtectedInheritedMethod")],
          inheritedMethod.Overrides[0]);
    }

    [Test]
    public void OverridingProtectedInternalInheritedClassMethod ()
    {
      BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (ClassWithInheritedMethod));
      MethodDefinition inheritedMethod = baseClass.Methods[typeof (BaseClassWithInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod",
          BindingFlags.NonPublic | BindingFlags.Instance)];
      Assert.IsNotNull (inheritedMethod);
      Assert.AreEqual (1, inheritedMethod.Overrides.Count);
      Assert.AreSame (
          baseClass.Mixins[typeof (MixinOverridingInheritedMethod)].Methods[typeof (MixinOverridingInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod")],
          inheritedMethod.Overrides[0]);
    }

    [Test]
    public void OverridingPublicInheritedClassMethod ()
    {
      BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (ClassWithInheritedMethod));
      MethodDefinition inheritedMethod = baseClass.Methods[typeof (BaseClassWithInheritedMethod).GetMethod ("PublicInheritedMethod")];
      Assert.IsNotNull (inheritedMethod);
      Assert.AreEqual (1, inheritedMethod.Overrides.Count);
      Assert.AreSame (
          baseClass.Mixins[typeof (MixinOverridingInheritedMethod)].Methods[typeof (MixinOverridingInheritedMethod).GetMethod ("PublicInheritedMethod")],
          inheritedMethod.Overrides[0]);
    }

    [Test]
    public void OverridingProtectedInheritedMixinMethod ()
    {
      BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (ClassOverridingInheritedMixinMethod));
      MethodDefinition inheritedMethod = baseClass.Methods[typeof (ClassOverridingInheritedMixinMethod).GetMethod ("ProtectedInheritedMethod")];
      Assert.IsNotNull (inheritedMethod);
      Assert.IsNotNull (inheritedMethod.Base);
      Assert.AreSame (
          baseClass.Mixins[typeof (MixinWithInheritedMethod)].Methods[
              typeof (BaseMixinWithInheritedMethod).GetMethod ("ProtectedInheritedMethod", BindingFlags.NonPublic | BindingFlags.Instance)],
          inheritedMethod.Base);
    }

    [Test]
    public void OverridingProtectedInternelInheritedMixinMethod ()
    {
      BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (ClassOverridingInheritedMixinMethod));
      MethodDefinition inheritedMethod = baseClass.Methods[typeof (ClassOverridingInheritedMixinMethod).GetMethod ("ProtectedInternalInheritedMethod")];
      Assert.IsNotNull (inheritedMethod);
      Assert.IsNotNull (inheritedMethod.Base);
      Assert.AreSame (
          baseClass.Mixins[typeof (MixinWithInheritedMethod)].Methods[
              typeof (BaseMixinWithInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod", BindingFlags.NonPublic | BindingFlags.Instance)],
          inheritedMethod.Base);
    }

    [Test]
    public void OverridingPublicInheritedMixinMethod ()
    {
      BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (ClassOverridingInheritedMixinMethod));
      MethodDefinition inheritedMethod = baseClass.Methods[typeof (ClassOverridingInheritedMixinMethod).GetMethod ("PublicInheritedMethod")];
      Assert.IsNotNull (inheritedMethod);
      Assert.IsNotNull (inheritedMethod.Base);
      Assert.AreSame (
          baseClass.Mixins[typeof (MixinWithInheritedMethod)].Methods[typeof (BaseMixinWithInheritedMethod).GetMethod ("PublicInheritedMethod")],
          inheritedMethod.Base);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class OverrideDefinitionBuilderTests
  {
    private static ApplicationDefinition GetApplicationDefinition ()
    {
      return DefBuilder.Build();
    }

    [Test]
    public void MethodOverrides ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = baseClass.Mixins[typeof (BT1Mixin2)];

      Assert.IsFalse (mixin1.HasOverriddenMembers ());
      Assert.IsFalse (mixin2.HasOverriddenMembers ());

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[] { typeof (string) });
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      MethodDefinition overridden = baseClass.Methods[baseMethod1];

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin1)));
      MethodDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.AreSame (overrider, mixin1.Methods[mixinMethod1]);
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame (overridden, overrider.Base);

      MethodDefinition notOverridden = baseClass.Methods[baseMethod2];
      Assert.AreEqual (0, notOverridden.Overrides.Count);

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin2)));
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides ()).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
    }

    [Test]
    public void PropertyOverrides ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = baseClass.Mixins[typeof (BT1Mixin2)];

      PropertyInfo baseProperty1 = typeof (BaseType1).GetProperty ("VirtualProperty");
      PropertyInfo baseProperty2 = typeof (BaseType1).GetProperty ("Item", new Type[] { typeof (string) });
      PropertyInfo mixinProperty1 = typeof (BT1Mixin1).GetProperty ("VirtualProperty");

      PropertyDefinition overridden = baseClass.Properties[baseProperty1];

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin1)));

      PropertyDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.AreSame (overrider, mixin1.Properties[mixinProperty1]);
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.SetMethod, overrider.SetMethod.Base);

      PropertyDefinition notOverridden = baseClass.Properties[baseProperty2];
      Assert.AreEqual (0, notOverridden.Overrides.Count);

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin2)));
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides ()).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.GetMethod, overrider.GetMethod.Base);
    }

    [Test]
    public void EventOverrides ()
    {
      ApplicationDefinition application = GetApplicationDefinition ();
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin1 = baseClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = baseClass.Mixins[typeof (BT1Mixin2)];

      EventInfo baseEvent1 = typeof (BaseType1).GetEvent ("VirtualEvent");
      EventInfo baseEvent2 = typeof (BaseType1).GetEvent ("ExplicitEvent");
      EventInfo mixinEvent1 = typeof (BT1Mixin1).GetEvent ("VirtualEvent");

      EventDefinition overridden = baseClass.Events[baseEvent1];

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin1)));

      EventDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.AreSame (overrider, mixin1.Events[mixinEvent1]);
      Assert.IsNotNull (overrider.Base);
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.RemoveMethod, overrider.RemoveMethod.Base);
      Assert.AreSame (overridden.AddMethod, overrider.AddMethod.Base);

      EventDefinition notOverridden = baseClass.Events[baseEvent2];
      Assert.AreEqual (0, notOverridden.Overrides.Count);

      Assert.IsTrue (overridden.Overrides.HasItem (typeof (BT1Mixin2)));
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.IsTrue (new List<MemberDefinition> (mixin2.GetAllOverrides ()).Contains (overrider));
      Assert.AreSame (overridden, overrider.Base);
      Assert.AreSame (overridden.AddMethod, overrider.AddMethod.Base);
      Assert.AreSame (overridden.RemoveMethod, overrider.RemoveMethod.Base);
    }

    [Test]
    public void OverrideNonVirtualMethod ()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType4), typeof (BT4Mixin1));

      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType4)];
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
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType4), typeof (BT4Mixin1));

      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType4)];
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
      ApplicationDefinition application = DefBuilder.Build (typeof (BaseType4), typeof (BT4Mixin1));

      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType4)];
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
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Could not find base member for overrider Mixins.UnitTests.SampleTypes.BT5Mixin1.Method.")]
    public void ThrowsWhenInexistingOverrideBaseMethod ()
    {
      DefBuilder.Build (typeof (BaseType5), typeof (BT5Mixin1));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Could not find base member for overrider Mixins.UnitTests.SampleTypes.BT5Mixin4.Property.")]
    public void ThrowsWhenInexistingOverrideBaseProperty ()
    {
      DefBuilder.Build (typeof (BaseType5), typeof (BT5Mixin4));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Could not find base member for overrider Mixins.UnitTests.SampleTypes.BT5Mixin5.Event.")]
    public void ThrowsWhenInexistingOverrideBaseEvent ()
    {
      DefBuilder.Build (typeof (BaseType5), typeof (BT5Mixin5));
    }
  }
}

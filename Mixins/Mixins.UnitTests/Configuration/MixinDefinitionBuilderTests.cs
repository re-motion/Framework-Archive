using System;
using System.Collections.Generic;
using Mixins.Definitions.Building;
using NUnit.Framework;
using Mixins.Context;
using System.Reflection;
using Mixins.Definitions;
using Mixins.UnitTests.SampleTypes;
using System.Runtime.Serialization;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class MixinDefinitionBuilderTests
  {
    private static ApplicationDefinition GetApplicationDefinition()
    {
      return DefBuilder.Build();
    }

    [Test]
    public void CorrectlyCopiesContext()
    {
      ApplicationDefinition application = GetApplicationDefinition();
      Assert.IsNull (application.Parent);

      Assert.IsTrue (application.BaseClasses.HasItem (typeof (BaseType1)));
      BaseClassDefinition baseClass = application.BaseClasses[typeof (BaseType1)];
      Assert.AreSame (application, baseClass.Parent);
      Assert.AreSame (application, baseClass.Application);
      Assert.AreEqual ("BaseType1", baseClass.Name);

      Assert.IsTrue (baseClass.Mixins.HasItem (typeof (BT1Mixin1)));
      Assert.IsTrue (baseClass.Mixins.HasItem (typeof (BT1Mixin2)));
      Assert.AreSame (baseClass, baseClass.Mixins[typeof (BT1Mixin1)].Parent);
    }

    [Test]
    public void MixinAppliedToInterface()
    {
      ApplicationDefinition application = DefBuilder.Build (typeof (IBaseType2), typeof (BT2Mixin1));
      Assert.IsFalse (application.BaseClasses.HasItem (typeof (BaseType2)));
      Assert.IsTrue (application.BaseClasses.HasItem (typeof (IBaseType2)));

      BaseClassDefinition baseClass = application.BaseClasses[typeof (IBaseType2)];
      Assert.IsTrue (baseClass.IsInterface);

      MethodInfo method = typeof (IBaseType2).GetMethod ("IfcMethod");
      Assert.IsNotNull (method);

      MemberDefinition member = baseClass.Methods[method];
      Assert.IsNotNull (member);

      MixinDefinition mixin = baseClass.Mixins[typeof (BT2Mixin1)];
      Assert.IsNotNull (mixin);
      Assert.AreSame (baseClass, mixin.BaseClass);
    }

    [Test]
    public void MixinIndicesCorrespondToPositionInArray()
    {
      BaseClassDefinition bt3 = GetApplicationDefinition().BaseClasses[typeof (BaseType3)];
      for (int i = 0; i < bt3.Mixins.Count; ++i)
        Assert.AreEqual (i, bt3.Mixins[i].MixinIndex);
    }

    [Test]
    public void OverriddenMixinMethod()
    {
      BaseClassDefinition overrider = GetApplicationDefinition().BaseClasses[typeof (ClassOverridingMixinMethod)];
      MixinDefinition mixin = overrider.Mixins[typeof (AbstractMixin)];
      Assert.IsNotNull (mixin);
      Assert.IsTrue (mixin.HasOverriddenMembers());

      MethodDefinition method = mixin.Methods[typeof (AbstractMixin).GetMethod ("AbstractMethod", BindingFlags.Instance | BindingFlags.NonPublic)];
      Assert.IsNotNull (method);
      MethodDefinition overridingMethod = overrider.Methods[typeof (ClassOverridingMixinMethod).GetMethod ("AbstractMethod")];
      Assert.IsNotNull (overridingMethod);
      Assert.AreSame (method, overridingMethod.Base);
      Assert.IsTrue (method.Overrides.HasItem (typeof (ClassOverridingMixinMethod)));
      Assert.AreSame (overridingMethod, method.Overrides[typeof (ClassOverridingMixinMethod)]);
    }

    [Test]
    public void NotOverriddenAbstractMixinMethodSucceeds()
    {
      BaseClassDefinition bt1 = DefBuilder.Build (typeof (BaseType1), typeof (AbstractMixin)).BaseClasses[typeof (BaseType1)];
      MixinDefinition mixin = bt1.Mixins[typeof (AbstractMixin)];
      MethodDefinition method = mixin.Methods[typeof (AbstractMixin).GetMethod ("AbstractMethod", BindingFlags.Instance | BindingFlags.NonPublic)];
      Assert.AreEqual (0, method.Overrides.Count);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void ThrowsOnMixinMethodOverridedWrongSig()
    {
      DefBuilder.Build (typeof (ClassOverridingMixinMethodWrongSig), typeof (AbstractMixin));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void ThrowsOnMixinOverrideWithoutMixin()
    {
      DefBuilder.Build (typeof (ClassOverridingMixinMethod));
    }

    [Test]
    public void GenericMixinAreAllowed()
    {
      MixinDefinition def = DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin3<,>)).BaseClasses[typeof(BaseType3)].Mixins[typeof(BT3Mixin3<,>)];
      Assert.IsNotNull (def);
    }

    [Test]
    [Ignore("TODO: Implement generic mixins")]
    public void GenericMixinsAreClosed ()
    {
      MixinDefinition def = DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin3<,>)).BaseClasses[typeof (BaseType3)].Mixins[typeof (BT3Mixin3<,>)];
      Assert.IsFalse (def.Type.IsGenericTypeDefinition);
    }
  }
}
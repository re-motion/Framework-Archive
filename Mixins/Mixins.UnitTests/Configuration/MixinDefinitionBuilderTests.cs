using System;
using System.Collections.Generic;
using Mixins.Definitions.Building;
using NUnit.Framework;
using Mixins.Context;
using System.Reflection;
using Mixins.Definitions;
using Mixins.UnitTests.SampleTypes;
using System.Runtime.Serialization;
using Mixins.UnitTests.Utilities;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class MixinDefinitionBuilderTests
  {
    [Test]
    public void CorrectlyCopiesContext()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        Assert.IsNull (baseClass.Parent);
        Assert.AreEqual ("BaseType1", baseClass.Name);

        Assert.IsTrue (baseClass.Mixins.HasItem (typeof (BT1Mixin1)));
        Assert.IsTrue (baseClass.Mixins.HasItem (typeof (BT1Mixin2)));
        Assert.AreSame (baseClass, baseClass.Mixins[typeof (BT1Mixin1)].Parent);
      }
    }

    [Test]
    public void MixinAppliedToInterface()
    {
      BaseClassDefinition baseClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (IBaseType2), typeof (BT2Mixin1));
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
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition bt3 = TypeFactory.GetActiveConfiguration (typeof (BaseType3));
        for (int i = 0; i < bt3.Mixins.Count; ++i)
          Assert.AreEqual (i, bt3.Mixins[i].MixinIndex);
      }
    }

    [Test]
    public void OverriddenMixinMethod()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition overrider = TypeFactory.GetActiveConfiguration (typeof (ClassOverridingMixinMethod));
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
    }

    [Test]
    public void NotOverriddenAbstractMixinMethodSucceeds()
    {
      BaseClassDefinition bt1 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (AbstractMixin));
      MixinDefinition mixin = bt1.Mixins[typeof (AbstractMixin)];
      MethodDefinition method = mixin.Methods[typeof (AbstractMixin).GetMethod ("AbstractMethod", BindingFlags.Instance | BindingFlags.NonPublic)];
      Assert.AreEqual (0, method.Overrides.Count);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void ThrowsOnMixinMethodOverridedWrongSig()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMethodWrongSig), typeof (AbstractMixin));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void ThrowsOnMixinOverrideWithoutMixin()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMethod));
    }

    [Test]
    public void GenericMixinsAreAllowed()
    {
      Assert.IsTrue (UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin3<,>))
          .HasMixinWithConfiguredType(typeof(BT3Mixin3<,>)));
    }

    [Test]
    public void GenericMixinsAreClosed ()
    {
      MixinDefinition def = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin3<,>))
          .GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
      Assert.IsFalse (def.Type.IsGenericTypeDefinition);
    }

    [Test]
    public void GenericMixinsAreSetToConstraintOrBaseType ()
    {
      MixinDefinition def = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin3<,>))
          .GetMixinByConfiguredType (typeof (BT3Mixin3<,>));
      Assert.AreEqual (typeof (BaseType3), def.Type.GetGenericArguments()[0]);
      Assert.AreEqual (typeof (IBaseType33), def.Type.GetGenericArguments()[1]);
    }

    private class MixinIntroducingGenericInterfaceWithTargetAsThisType<T>: Mixin<T>, IEquatable<T>
        where T: class
    {
      public bool Equals (T other)
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void GenericInterfaceArgumentIsBaseTypeWhenPossible()
    {
      BaseClassDefinition def = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1),
          typeof (MixinIntroducingGenericInterfaceWithTargetAsThisType<>));
      Assert.IsTrue (def.IntroducedInterfaces.HasItem (typeof (IEquatable<BaseType1>)));
    }

    [Test]
    public void ExplicitBaseCallDependenciesCorrectlyCopied ()
    {
      BaseClassDefinition bt3 = TypeFactory.GetActiveConfiguration (typeof (BaseType3));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.HasItem (typeof (IBaseType32)));
      Assert.IsTrue (bt3.Mixins[typeof (BT3Mixin5)].BaseDependencies.HasItem (typeof (IBaseType32)));
    }

    public class MixinWithDependency : Mixin<object, IMixinTargetWithExplicitDependencies>
    {
    }

    public interface IMixinTargetWithExplicitDependencies {}

    [Uses (typeof (MixinWithDependency), AdditionalDependencies = new Type[] {typeof (IMixinTargetWithExplicitDependencies)})]
    public class MixinTargetWithExplicitDependencies : IMixinTargetWithExplicitDependencies
    { }

    [Test]
    public void DuplicateExplicitDependenciesDontMatter ()
    {
      BaseClassDefinition mt = TypeFactory.GetActiveConfiguration (typeof (MixinTargetWithExplicitDependencies));
      Assert.IsTrue (mt.RequiredBaseCallTypes.HasItem (typeof (IMixinTargetWithExplicitDependencies)));
      Assert.IsTrue (mt.Mixins[typeof (MixinWithDependency)].BaseDependencies.HasItem (typeof (IMixinTargetWithExplicitDependencies)));
    }
  }
}
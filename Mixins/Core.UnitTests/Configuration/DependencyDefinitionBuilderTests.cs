using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Definitions.Building;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using System.Reflection;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class DependencyDefinitionBuilderTests
  {
    [Test]
    public void FaceInterfaces ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType3));

        Assert.IsTrue (baseClass.RequiredFaceTypes.ContainsKey (typeof (IBaseType31)));
        Assert.IsTrue (baseClass.RequiredFaceTypes.ContainsKey (typeof (IBaseType32)));
        Assert.IsTrue (baseClass.RequiredFaceTypes.ContainsKey (typeof (IBaseType33)));
        Assert.IsTrue (baseClass.RequiredFaceTypes.ContainsKey (typeof (IBaseType34)), "indirect dependency via BT3Mixin4");
        Assert.IsTrue (baseClass.RequiredFaceTypes.ContainsKey (typeof (IBaseType35)), "indirect dependency via BT3Mixin4");
        Assert.IsFalse (baseClass.RequiredFaceTypes.ContainsKey (typeof (IBaseType2)));

        List<MixinDefinition> requirers = new List<MixinDefinition> (baseClass.RequiredFaceTypes[typeof (IBaseType31)].FindRequiringMixins());
        Assert.Contains (baseClass.Mixins[typeof (BT3Mixin1)], requirers);
        Assert.Contains (baseClass.Mixins[typeof (BT3Mixin4)], requirers, "indirect dependency");
        Assert.Contains (baseClass.GetMixinByConfiguredType (typeof (BT3Mixin6<,>)), requirers);
        Assert.Contains (baseClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)), requirers);
        Assert.AreEqual (4, requirers.Count);

        Assert.IsFalse (baseClass.RequiredFaceTypes[typeof (IBaseType31)].IsEmptyInterface);
        Assert.IsFalse (baseClass.RequiredFaceTypes[typeof (IBaseType31)].IsAggregatorInterface);

        baseClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Face));
        Assert.IsTrue (baseClass.RequiredFaceTypes.ContainsKey (typeof (ICBaseType3BT3Mixin4)));
        requirers = new List<MixinDefinition> (baseClass.RequiredFaceTypes[typeof (ICBaseType3BT3Mixin4)].FindRequiringMixins());
        Assert.Contains (baseClass.Mixins[typeof (BT3Mixin7Face)], requirers);
      }
    }

    [Test]
    public void FaceInterfacesAddedViaContext ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType6));

        Assert.IsTrue (baseClass.RequiredFaceTypes.ContainsKey (typeof (ICBT6Mixin1)), "This is added via a dependency of BT6Mixin3.");
        Assert.IsTrue (baseClass.RequiredFaceTypes.ContainsKey (typeof (ICBT6Mixin2)), "This is added via a dependency of BT6Mixin3.");
        Assert.IsTrue (baseClass.RequiredFaceTypes.ContainsKey (typeof (ICBT6Mixin3)), "This is added because of the CompleteInterfaceAttribute.");
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency IBT3Mixin4 (mixins Rubicon.Mixins.UnitTests.SampleTypes.BT3Mixin7Face applied to class "
        + "Rubicon.Mixins.UnitTests.SampleTypes.BaseType3) is not fulfilled - method Foo could not be found on the base class.")]
    public void ThrowsIfAggregateThisDependencyIsNotFullyImplemented ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin7Face));
    }

    [Test]
    [Ignore ("TODO - after removing RequiredBaseCallDependencies")]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "The dependency IBT3Mixin4 (mixins Rubicon.Mixins.UnitTests.SampleTypes.BT3Mixin7Face applied to class "
        + "Rubicon.Mixins.UnitTests.SampleTypes.BaseType3) is not fulfilled - method Foo could not be found on the base class.")]
    public void ThrowsIfAggregateBaseDependencyIsNotFullyImplemented ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin7Base));
    }

    [Test]
    public void BaseInterfaces ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType3));

        List<Type> requiredBaseCallTypes = new List<RequiredBaseCallTypeDefinition> (baseClass.RequiredBaseCallTypes).ConvertAll<Type>
            (delegate (RequiredBaseCallTypeDefinition def) { return def.Type; });
        Assert.Contains (typeof (IBaseType31), requiredBaseCallTypes);
        Assert.Contains (typeof (IBaseType33), requiredBaseCallTypes);
        Assert.Contains (typeof (IBaseType34), requiredBaseCallTypes);
        Assert.IsFalse (requiredBaseCallTypes.Contains (typeof (IBaseType35)));

        List<MixinDefinition> requirers = new List<MixinDefinition> (baseClass.RequiredBaseCallTypes[typeof (IBaseType33)].FindRequiringMixins());
        Assert.Contains (baseClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)), requirers);
      }
    }

    [Test]
    public void BaseMethods ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType3));

        RequiredBaseCallTypeDefinition req1 = baseClass.RequiredBaseCallTypes[typeof (IBaseType31)];
        Assert.AreEqual (typeof (IBaseType31).GetMembers().Length, req1.BaseCallMethods.Count);

        RequiredBaseCallMethodDefinition member1 = req1.BaseCallMethods[typeof (IBaseType31).GetMethod ("IfcMethod")];
        Assert.AreEqual ("Rubicon.Mixins.UnitTests.SampleTypes.IBaseType31.IfcMethod", member1.FullName);
        Assert.AreSame (req1, member1.DeclaringType);
        Assert.AreSame (req1, member1.Parent);

        Assert.AreEqual (typeof (IBaseType31).GetMethod ("IfcMethod"), member1.InterfaceMethod);
        Assert.AreEqual (baseClass.Methods[typeof (BaseType3).GetMethod ("IfcMethod")], member1.ImplementingMethod);

        RequiredBaseCallTypeDefinition req2 = baseClass.RequiredBaseCallTypes[typeof (IBT3Mixin4)];
        Assert.AreEqual (typeof (IBT3Mixin4).GetMembers().Length, req2.BaseCallMethods.Count);

        RequiredBaseCallMethodDefinition member2 = req2.BaseCallMethods[typeof (IBT3Mixin4).GetMethod ("Foo")];
        Assert.AreEqual ("Rubicon.Mixins.UnitTests.SampleTypes.IBT3Mixin4.Foo", member2.FullName);
        Assert.AreSame (req2, member2.DeclaringType);
        Assert.AreSame (req2, member2.Parent);

        Assert.AreEqual (typeof (IBT3Mixin4).GetMethod ("Foo"), member2.InterfaceMethod);
        Assert.AreEqual (baseClass.Mixins[typeof (BT3Mixin4)].Methods[typeof (BT3Mixin4).GetMethod ("Foo")], member2.ImplementingMethod);
      }

      using (MixinConfiguration.ScopedExtend(typeof (BaseType3), typeof (BT3Mixin7Base), typeof (BT3Mixin4)))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType3));

        RequiredBaseCallTypeDefinition req3 = baseClass.RequiredBaseCallTypes[typeof (ICBaseType3BT3Mixin4)];
        Assert.AreEqual (0, req3.BaseCallMethods.Count);

        req3 = baseClass.RequiredBaseCallTypes[typeof (ICBaseType3)];
        Assert.AreEqual (0, req3.BaseCallMethods.Count);

        req3 = baseClass.RequiredBaseCallTypes[typeof (IBaseType31)];
        Assert.AreEqual (1, req3.BaseCallMethods.Count);

        req3 = baseClass.RequiredBaseCallTypes[typeof (IBT3Mixin4)];
        Assert.AreEqual (1, req3.BaseCallMethods.Count);

        RequiredBaseCallMethodDefinition member3 = req3.BaseCallMethods[typeof (IBT3Mixin4).GetMethod ("Foo")];
        Assert.IsNotNull (member3);
        Assert.AreEqual (baseClass.Mixins[typeof (BT3Mixin4)].Methods[typeof (BT3Mixin4).GetMethod ("Foo")], member3.ImplementingMethod);
      }
    }

    [Test]
    public void DuckTypingBaseInterface ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseTypeWithDuckBaseMixin), typeof (DuckBaseMixin)))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseTypeWithDuckBaseMixin));
        Assert.IsTrue (baseClass.Mixins.ContainsKey (typeof (DuckBaseMixin)));
        MixinDefinition mixin = baseClass.Mixins[typeof (DuckBaseMixin)];
        Assert.IsTrue (baseClass.RequiredBaseCallTypes.ContainsKey (typeof (IDuckBaseRequirements)));
        Assert.IsTrue (new List<MixinDefinition> (baseClass.RequiredBaseCallTypes[typeof (IDuckBaseRequirements)].FindRequiringMixins()).Contains (mixin));

        Assert.IsTrue (mixin.BaseDependencies.ContainsKey (typeof (IDuckBaseRequirements)));
        Assert.AreSame (baseClass, mixin.BaseDependencies[typeof (IDuckBaseRequirements)].GetImplementer());

        Assert.AreSame (mixin, mixin.BaseDependencies[typeof (IDuckBaseRequirements)].Depender);
        Assert.IsNull (mixin.BaseDependencies[typeof (IDuckBaseRequirements)].Aggregator);
        Assert.AreEqual (0, mixin.BaseDependencies[typeof (IDuckBaseRequirements)].AggregatedDependencies.Count);

        Assert.AreSame (baseClass.RequiredBaseCallTypes[typeof (IDuckBaseRequirements)],
            mixin.BaseDependencies[typeof (IDuckBaseRequirements)].RequiredType);

        Assert.AreEqual (2, baseClass.RequiredBaseCallTypes[typeof (IDuckBaseRequirements)].BaseCallMethods.Count);
        Assert.AreSame (typeof (IDuckBaseRequirements).GetMethod ("MethodImplementedOnBase"),
            baseClass.RequiredBaseCallTypes[typeof (IDuckBaseRequirements)].BaseCallMethods[0].InterfaceMethod);
        Assert.AreSame (baseClass.Methods[typeof (BaseTypeWithDuckBaseMixin).GetMethod ("MethodImplementedOnBase")],
            baseClass.RequiredBaseCallTypes[typeof (IDuckBaseRequirements)].BaseCallMethods[0].ImplementingMethod);
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException),
        ExpectedMessage = "is not fulfilled - method MethodImplementedOnBase does not have an equivalent", MatchType = MessageMatch.Regex)]
    public void ThrowsWhenUnfulfilledDuckBase ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (object), typeof (DuckBaseMixinWithoutOverrides)))
      {
        TypeFactory.GetActiveConfiguration (typeof (object));
      }
    }

    [Test]
    public void Dependencies ()
    {
      using (MixinConfiguration.ScopedExtend(Assembly.GetExecutingAssembly()))
      {
        MixinDefinition bt3Mixin1 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];

        Assert.IsTrue (bt3Mixin1.ThisDependencies.ContainsKey (typeof (IBaseType31)));
        Assert.AreEqual (1, bt3Mixin1.ThisDependencies.Count);

        Assert.IsTrue (bt3Mixin1.BaseDependencies.ContainsKey (typeof (IBaseType31)));
        Assert.AreEqual (1, bt3Mixin1.BaseDependencies.Count);

        MixinDefinition bt3Mixin2 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
        Assert.IsTrue (bt3Mixin2.ThisDependencies.ContainsKey (typeof (IBaseType32)));
        Assert.AreEqual (1, bt3Mixin2.ThisDependencies.Count);

        Assert.AreEqual (0, bt3Mixin2.BaseDependencies.Count);

        MixinDefinition bt3Mixin6 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType (typeof (BT3Mixin6<,>));

        Assert.IsTrue (bt3Mixin6.ThisDependencies.ContainsKey (typeof (IBaseType31)));
        Assert.IsTrue (bt3Mixin6.ThisDependencies.ContainsKey (typeof (IBaseType32)));
        Assert.IsTrue (bt3Mixin6.ThisDependencies.ContainsKey (typeof (IBaseType33)));
        Assert.IsTrue (bt3Mixin6.ThisDependencies.ContainsKey (typeof (IBT3Mixin4)));
        Assert.IsFalse (bt3Mixin6.ThisDependencies.ContainsKey (typeof (IBaseType34)));

        Assert.IsFalse (bt3Mixin6.ThisDependencies[typeof (IBaseType31)].IsAggregate);
        Assert.IsFalse (bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].IsAggregate);

        Assert.AreEqual (0, bt3Mixin6.ThisDependencies[typeof (IBaseType31)].AggregatedDependencies.Count);

        Assert.IsTrue (
            bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].RequiredType.RequiringDependencies.ContainsKey (
                bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)]));
        Assert.IsNull (bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].Aggregator);

        Assert.AreEqual (
            TypeFactory.GetActiveConfiguration (typeof (BaseType3)).RequiredFaceTypes[typeof (IBaseType31)],
            bt3Mixin6.ThisDependencies[typeof (IBaseType31)].RequiredType);

        Assert.AreSame (TypeFactory.GetActiveConfiguration (typeof (BaseType3)), bt3Mixin6.ThisDependencies[typeof (IBaseType32)].GetImplementer());
        Assert.AreSame (
            TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin4)],
            bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].GetImplementer());

        Assert.IsTrue (bt3Mixin6.BaseDependencies.ContainsKey (typeof (IBaseType34)));
        Assert.IsTrue (bt3Mixin6.BaseDependencies.ContainsKey (typeof (IBT3Mixin4)));
        Assert.IsFalse (bt3Mixin6.BaseDependencies.ContainsKey (typeof (IBaseType31)));
        Assert.IsFalse (bt3Mixin6.BaseDependencies.ContainsKey (typeof (IBaseType32)));
        Assert.IsTrue (bt3Mixin6.BaseDependencies.ContainsKey (typeof (IBaseType33)), "indirect dependency");

        Assert.AreEqual (
            TypeFactory.GetActiveConfiguration (typeof (BaseType3)).RequiredBaseCallTypes[typeof (IBaseType34)],
            bt3Mixin6.BaseDependencies[typeof (IBaseType34)].RequiredType);

        Assert.AreSame (TypeFactory.GetActiveConfiguration (typeof (BaseType3)), bt3Mixin6.BaseDependencies[typeof (IBaseType34)].GetImplementer());
        Assert.AreSame (
            TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin4)],
            bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].GetImplementer());

        Assert.IsFalse (bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].IsAggregate);
        Assert.IsFalse (bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].IsAggregate);

        Assert.AreEqual (0, bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].AggregatedDependencies.Count);

        Assert.IsTrue (
            bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].RequiredType.RequiringDependencies.ContainsKey (
                bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)]));
        Assert.IsNull (bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].Aggregator);
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The base call dependency .* is not fulfilled",
        MatchType = MessageMatch.Regex)]
    public void ThrowsIfBaseDependencyNotFulfilled ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin7Base));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Base call dependencies must be interfaces.*MixinWithClassBase",
        MatchType = MessageMatch.Regex)]
    public void ThrowsIfRequiredBaseIsNotInterface ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithClassBase));
    }

    [Test]
    public void WorksIfRequiredBaseIsSystemObject ()
    {
      UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithObjectBase));
    }

    [Test]
    public void CompleteInterfacesAndDependenciesForFace ()
    {
      BaseClassDefinition bt3 = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Face));

      MixinDefinition m4 = bt3.Mixins[typeof (BT3Mixin4)];
      MixinDefinition m7 = bt3.Mixins[typeof (BT3Mixin7Face)];

      ThisDependencyDefinition d1 = m7.ThisDependencies[typeof (ICBaseType3BT3Mixin4)];
      Assert.IsNull (d1.GetImplementer ());
      Assert.AreEqual ("Rubicon.Mixins.UnitTests.SampleTypes.ICBaseType3BT3Mixin4", d1.FullName);
      Assert.AreSame (m7, d1.Parent);

      Assert.IsTrue (d1.IsAggregate);
      Assert.IsTrue (d1.AggregatedDependencies[typeof (ICBaseType3)].IsAggregate);
      Assert.IsFalse (d1.AggregatedDependencies[typeof (ICBaseType3)]
                          .AggregatedDependencies[typeof (IBaseType31)].IsAggregate);
      Assert.AreSame (bt3, d1.AggregatedDependencies[typeof (ICBaseType3)]
                               .AggregatedDependencies[typeof (IBaseType31)].GetImplementer ());

      Assert.IsFalse (d1.AggregatedDependencies[typeof (IBT3Mixin4)].IsAggregate);
      Assert.AreSame (m4, d1.AggregatedDependencies[typeof (IBT3Mixin4)].GetImplementer ());

      Assert.AreSame (d1, d1.AggregatedDependencies[typeof (IBT3Mixin4)].Aggregator);

      Assert.IsTrue (bt3.RequiredFaceTypes[typeof (ICBaseType3)].IsEmptyInterface);
      Assert.IsTrue (bt3.RequiredFaceTypes[typeof (ICBaseType3)].IsAggregatorInterface);

      Assert.IsTrue (bt3.RequiredFaceTypes.ContainsKey (typeof (ICBaseType3BT3Mixin4)));
      Assert.IsTrue (bt3.RequiredFaceTypes.ContainsKey (typeof (ICBaseType3)));
      Assert.IsTrue (bt3.RequiredFaceTypes.ContainsKey (typeof (IBaseType31)));
      Assert.IsTrue (bt3.RequiredFaceTypes.ContainsKey (typeof (IBT3Mixin4)));
    }

    [Test]
    public void CompleteInterfacesAndDependenciesForBase ()
    {
      BaseClassDefinition bt3 =
          UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin4), typeof (BT3Mixin7Base));

      MixinDefinition m4 = bt3.Mixins[typeof (BT3Mixin4)];
      MixinDefinition m7 = bt3.Mixins[typeof (BT3Mixin7Base)];

      BaseDependencyDefinition d2 = m7.BaseDependencies[typeof (ICBaseType3BT3Mixin4)];
      Assert.IsNull (d2.GetImplementer ());

      Assert.IsTrue (d2.IsAggregate);

      Assert.IsTrue (d2.AggregatedDependencies[typeof (ICBaseType3)].IsAggregate);
      Assert.AreSame (d2, d2.AggregatedDependencies[typeof (ICBaseType3)].Parent);

      Assert.IsFalse (d2.AggregatedDependencies[typeof (ICBaseType3)]
                          .AggregatedDependencies[typeof (IBaseType31)].IsAggregate);
      Assert.AreSame (bt3, d2.AggregatedDependencies[typeof (ICBaseType3)]
                               .AggregatedDependencies[typeof (IBaseType31)].GetImplementer ());

      Assert.IsFalse (d2.AggregatedDependencies[typeof (IBT3Mixin4)].IsAggregate);
      Assert.AreSame (m4, d2.AggregatedDependencies[typeof (IBT3Mixin4)].GetImplementer ());

      Assert.AreSame (d2, d2.AggregatedDependencies[typeof (IBT3Mixin4)].Aggregator);

      Assert.IsTrue (bt3.RequiredBaseCallTypes[typeof (ICBaseType3)].IsEmptyInterface);
      Assert.IsTrue (bt3.RequiredBaseCallTypes[typeof (ICBaseType3)].IsAggregatorInterface);

      Assert.IsTrue (bt3.RequiredBaseCallTypes.ContainsKey (typeof (ICBaseType3BT3Mixin4)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.ContainsKey (typeof (ICBaseType3)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.ContainsKey (typeof (IBaseType31)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.ContainsKey (typeof (IBT3Mixin4)));
    }

    public interface IMixinWithEmptyInterface {}
    public class MixinWithEmptyInterface : IMixinWithEmptyInterface { }
    public class MixinRequiringEmptyInterface : Mixin<object, IMixinWithEmptyInterface> { }

    [Test]
    public void EmptyInterface()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType1), typeof (MixinWithEmptyInterface), typeof (MixinRequiringEmptyInterface)))
      {
        BaseClassDefinition bt1 = TypeFactory.GetActiveConfiguration (typeof (BaseType1));
        MixinDefinition m1 = bt1.Mixins[typeof (MixinWithEmptyInterface)];
        MixinDefinition m2 = bt1.Mixins[typeof (MixinRequiringEmptyInterface)];
        BaseDependencyDefinition dependency = m2.BaseDependencies[0];
        RequiredBaseCallTypeDefinition requirement = dependency.RequiredType;
        Assert.IsTrue (requirement.IsEmptyInterface);
        Assert.IsFalse (requirement.IsAggregatorInterface);
        Assert.AreSame (m1, dependency.GetImplementer());
      }
    }
  }
}

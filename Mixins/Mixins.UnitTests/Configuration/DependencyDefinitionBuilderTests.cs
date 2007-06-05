using System;
using System.Collections.Generic;
using System.Text;
using Mixins.Definitions;
using Mixins.Definitions.Building;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using System.Reflection;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class DependencyDefinitionBuilderTests
  {
    [Test]
    public void FaceInterfaces ()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType3));

        Assert.IsTrue (baseClass.RequiredFaceTypes.HasItem (typeof (IBaseType31)));
        Assert.IsTrue (baseClass.RequiredFaceTypes.HasItem (typeof (IBaseType32)));
        Assert.IsTrue (baseClass.RequiredFaceTypes.HasItem (typeof (IBaseType33)));
        Assert.IsTrue (baseClass.RequiredFaceTypes.HasItem (typeof (IBaseType34)), "indirect dependency via BT3Mixin4");
        Assert.IsTrue (baseClass.RequiredFaceTypes.HasItem (typeof (IBaseType35)), "indirect dependency via BT3Mixin4");
        Assert.IsFalse (baseClass.RequiredFaceTypes.HasItem (typeof (IBaseType2)));

        List<MixinDefinition> requirers = new List<MixinDefinition> (baseClass.RequiredFaceTypes[typeof (IBaseType31)].FindRequiringMixins());
        Assert.Contains (baseClass.Mixins[typeof (BT3Mixin1)], requirers);
        Assert.Contains (baseClass.Mixins[typeof (BT3Mixin4)], requirers, "indirect dependency");
        Assert.Contains (baseClass.GetMixinByConfiguredType (typeof (BT3Mixin6<,>)), requirers);
        Assert.Contains (baseClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)), requirers);
        Assert.AreEqual (4, requirers.Count);

        Assert.IsFalse (baseClass.RequiredFaceTypes[typeof (IBaseType31)].IsEmptyInterface);

        baseClass = UnvalidatedDefinitionBuilder.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin7Face));
        Assert.IsTrue (baseClass.RequiredFaceTypes.HasItem (typeof (ICBaseType3BT3Mixin4)));
        requirers = new List<MixinDefinition> (baseClass.RequiredFaceTypes[typeof (ICBaseType3BT3Mixin4)].FindRequiringMixins());
        Assert.Contains (baseClass.Mixins[typeof (BT3Mixin7Face)], requirers);
      }
    }

    [Test]
    public void BaseInterfaces ()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType3));

        List<Type> requiredBaseCallTypes = new List<RequiredBaseCallTypeDefinition> (baseClass.RequiredBaseCallTypes).ConvertAll<Type>
            (delegate (RequiredBaseCallTypeDefinition def) { return def.Type; });
        Assert.Contains (typeof (IBaseType31), requiredBaseCallTypes);
        Assert.Contains (typeof (IBaseType33), requiredBaseCallTypes);
        Assert.Contains (typeof (IBaseType34), requiredBaseCallTypes);
        Assert.IsFalse (requiredBaseCallTypes.Contains (typeof (IBaseType32)));
        Assert.IsFalse (requiredBaseCallTypes.Contains (typeof (IBaseType35)));

        List<MixinDefinition> requirers = new List<MixinDefinition> (baseClass.RequiredBaseCallTypes[typeof (IBaseType33)].FindRequiringMixins());
        Assert.Contains (baseClass.GetMixinByConfiguredType (typeof (BT3Mixin3<,>)), requirers);
      }
    }

    [Test]
    public void BaseMethods ()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly ()))
      {
        BaseClassDefinition baseClass = TypeFactory.GetActiveConfiguration (typeof (BaseType3));

        RequiredBaseCallTypeDefinition req1 = baseClass.RequiredBaseCallTypes[typeof (IBaseType31)];
        Assert.AreEqual (typeof (IBaseType31).GetMembers().Length, req1.BaseCallMethods.Count);

        RequiredBaseCallMethodDefinition member1 = req1.BaseCallMethods[typeof (IBaseType31).GetMethod ("IfcMethod")];
        Assert.AreEqual ("Mixins.UnitTests.SampleTypes.IBaseType31.IfcMethod", member1.FullName);
        Assert.AreSame (req1, member1.DeclaringType);
        Assert.AreSame (req1, member1.Parent);

        Assert.AreEqual (typeof (IBaseType31).GetMethod ("IfcMethod"), member1.InterfaceMethod);
        Assert.AreEqual (baseClass.Methods[typeof (BaseType3).GetMethod ("IfcMethod")], member1.ImplementingMethod);

        RequiredBaseCallTypeDefinition req2 = baseClass.RequiredBaseCallTypes[typeof (IBT3Mixin4)];
        Assert.AreEqual (typeof (IBT3Mixin4).GetMembers().Length, req2.BaseCallMethods.Count);

        RequiredBaseCallMethodDefinition member2 = req2.BaseCallMethods[typeof (IBT3Mixin4).GetMethod ("Foo")];
        Assert.AreEqual ("Mixins.UnitTests.SampleTypes.IBT3Mixin4.Foo", member2.FullName);
        Assert.AreSame (req2, member2.DeclaringType);
        Assert.AreSame (req2, member2.Parent);

        Assert.AreEqual (typeof (IBT3Mixin4).GetMethod ("Foo"), member2.InterfaceMethod);
        Assert.AreEqual (baseClass.Mixins[typeof (BT3Mixin4)].Methods[typeof (BT3Mixin4).GetMethod ("Foo")], member2.ImplementingMethod);
      }

      using (new MixinConfiguration (typeof (BaseType3), typeof (BT3Mixin7Base), typeof (BT3Mixin4)))
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
    public void Dependencies ()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly()))
      {
        MixinDefinition bt3Mixin1 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];

        Assert.IsTrue (bt3Mixin1.ThisDependencies.HasItem (typeof (IBaseType31)));
        Assert.AreEqual (1, bt3Mixin1.ThisDependencies.Count);

        Assert.IsTrue (bt3Mixin1.BaseDependencies.HasItem (typeof (IBaseType31)));
        Assert.AreEqual (1, bt3Mixin1.BaseDependencies.Count);

        MixinDefinition bt3Mixin2 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin2)];
        Assert.IsTrue (bt3Mixin2.ThisDependencies.HasItem (typeof (IBaseType32)));
        Assert.AreEqual (1, bt3Mixin2.ThisDependencies.Count);

        Assert.AreEqual (0, bt3Mixin2.BaseDependencies.Count);

        MixinDefinition bt3Mixin6 = TypeFactory.GetActiveConfiguration (typeof (BaseType3)).GetMixinByConfiguredType (typeof (BT3Mixin6<,>));

        Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType31)));
        Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType32)));
        Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType33)));
        Assert.IsTrue (bt3Mixin6.ThisDependencies.HasItem (typeof (IBT3Mixin4)));
        Assert.IsFalse (bt3Mixin6.ThisDependencies.HasItem (typeof (IBaseType34)));

        Assert.IsFalse (bt3Mixin6.ThisDependencies[typeof (IBaseType31)].IsAggregate);
        Assert.IsFalse (bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].IsAggregate);

        Assert.AreEqual (0, bt3Mixin6.ThisDependencies[typeof (IBaseType31)].AggregatedDependencies.Count);

        Assert.IsTrue (
            bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].RequiredType.RequiringDependencies.HasItem (
                bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)]));
        Assert.IsNull (bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].Aggregator);

        Assert.AreEqual (
            TypeFactory.GetActiveConfiguration (typeof (BaseType3)).RequiredFaceTypes[typeof (IBaseType31)],
            bt3Mixin6.ThisDependencies[typeof (IBaseType31)].RequiredType);

        Assert.AreSame (TypeFactory.GetActiveConfiguration (typeof (BaseType3)), bt3Mixin6.ThisDependencies[typeof (IBaseType32)].GetImplementer());
        Assert.AreSame (
            TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin4)],
            bt3Mixin6.ThisDependencies[typeof (IBT3Mixin4)].GetImplementer());

        Assert.IsTrue (bt3Mixin6.BaseDependencies.HasItem (typeof (IBaseType34)));
        Assert.IsTrue (bt3Mixin6.BaseDependencies.HasItem (typeof (IBT3Mixin4)));
        Assert.IsFalse (bt3Mixin6.BaseDependencies.HasItem (typeof (IBaseType31)));
        Assert.IsFalse (bt3Mixin6.BaseDependencies.HasItem (typeof (IBaseType32)));
        Assert.IsTrue (bt3Mixin6.BaseDependencies.HasItem (typeof (IBaseType33)), "indirect dependency");

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
            bt3Mixin6.BaseDependencies[typeof (IBT3Mixin4)].RequiredType.RequiringDependencies.HasItem (
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
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Base call dependencies must be interfaces",
        MatchType = MessageMatch.Contains)]
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
      Assert.AreEqual ("Mixins.UnitTests.SampleTypes.ICBaseType3BT3Mixin4", d1.FullName);
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

      Assert.IsTrue (bt3.RequiredFaceTypes.HasItem (typeof (ICBaseType3BT3Mixin4)));
      Assert.IsTrue (bt3.RequiredFaceTypes.HasItem (typeof (ICBaseType3)));
      Assert.IsTrue (bt3.RequiredFaceTypes.HasItem (typeof (IBaseType31)));
      Assert.IsTrue (bt3.RequiredFaceTypes.HasItem (typeof (IBT3Mixin4)));
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

      Assert.IsTrue (bt3.RequiredBaseCallTypes.HasItem (typeof (ICBaseType3BT3Mixin4)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.HasItem (typeof (ICBaseType3)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.HasItem (typeof (IBaseType31)));
      Assert.IsTrue (bt3.RequiredBaseCallTypes.HasItem (typeof (IBT3Mixin4)));
    }
  }
}

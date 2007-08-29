using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Mixins.UnitTests.Mixins.CodeGenSampleTypes;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Mixins.MixedTypeCodeGeneration
{
  [TestFixture]
  public class IntroductionTests : MixinBaseTest
  {
    [Test]
    public void IntroducedInterfacesAreImplementedViaDelegation ()
    {
      BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
      IBT1Mixin1 bt1AsMixedIface = bt1 as IBT1Mixin1;
      Assert.IsNotNull (bt1AsMixedIface);
      Assert.AreEqual ("BT1Mixin1.IntroducedMethod", bt1AsMixedIface.IntroducedMethod ());
    }

    [Test]
    public void MixinCanImplementMethodsExplicitly ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinWithExplicitImplementation)).With ();
      IExplicit explicito = bt1 as IExplicit;
      Assert.IsNotNull (explicito);
      Assert.AreEqual ("XXX", explicito.Explicit ());
    }

    [Test]
    public void MixinCanIntroduceGenericInterface ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinIntroducingGenericInterface<>)).With ();
      IGeneric<BaseType1> generic = bt1 as IGeneric<BaseType1>;
      Assert.IsNotNull (generic);
      Assert.AreEqual ("Generic", generic.Generic (bt1));
    }

    [Test]
    public void InheritedIntroducedInterfaces ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1> (typeof (MixinIntroducingInheritedInterface)).With ();
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method1", ((IMixinIII1) bt1).Method1 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method1", ((IMixinIII2) bt1).Method1 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method2", ((IMixinIII2) bt1).Method2 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method3", ((IMixinIII3) bt1).Method3 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method4", ((IMixinIII4) bt1).Method4 ());
      Assert.AreEqual ("MixinIntroducingInheritedInterface.Method2", ((IMixinIII4) bt1).Method2 ());
    }

    [Test]
    public void MixinImplementingFullPropertiesWithPartialIntroduction ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (MixinImplementingFullPropertiesWithPartialIntroduction)))
      {
        BaseType1 bt1 = ObjectFactory.Create<BaseType1> ().With ();
        MethodInfo[] allMethods = bt1.GetType ().GetMethods (BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        string[] allMethodNames = Array.ConvertAll<MethodInfo, string> (allMethods, delegate (MethodInfo mi) { return mi.Name; });
        Assert.That (allMethodNames, List.Contains ("Rubicon.Mixins.UnitTests.SampleTypes.InterfaceWithPartialProperties.get_Prop1"));
        Assert.That (allMethodNames, List.Contains ("Rubicon.Mixins.UnitTests.SampleTypes.InterfaceWithPartialProperties.set_Prop2"));

        Assert.That (allMethodNames, List.Not.Contains ("Rubicon.Mixins.UnitTests.SampleTypes.InterfaceWithPartialProperties.set_Prop1"));
        Assert.That (allMethodNames, List.Not.Contains ("Rubicon.Mixins.UnitTests.SampleTypes.InterfaceWithPartialProperties.get_Prop2"));
      }
    }

    [Test]
    public void ExplicitlySuppressedInterfaceIntroduction ()
    {
      object o = CreateMixedObject<object> (typeof (MixinSuppressingSimpleInterface)).With();
      Assert.IsFalse (o is ISimpleInterface);
      Assert.IsTrue (Mixin.Get<MixinSuppressingSimpleInterface> (o) is ISimpleInterface);
    }

    [Test]
    public void ImplicitlySuppressedInterfaceIntroduction ()
    {
      ClassImplementingSimpleInterface o = CreateMixedObject<ClassImplementingSimpleInterface> (typeof (MixinImplementingSimpleInterface)).With();
      Assert.IsTrue (o is ISimpleInterface);
      Assert.AreEqual ("ClassImplementingSimpleInterface.Method", o.Method ());
    }
  }
}
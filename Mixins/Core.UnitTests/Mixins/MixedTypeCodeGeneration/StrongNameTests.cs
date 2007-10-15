using System;
using NUnit.Framework;
using Rubicon.Mixins.UnitTests.SampleTypes;
using System.Collections.Generic;

namespace Rubicon.Mixins.UnitTests.Mixins.MixedTypeCodeGeneration
{
  [TestFixture]
  public class StrongNameTests : MixinBaseTest
  {
    [Test]
    public void SignedBaseClassGeneratedIntoSignedAssembly ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (object)))
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
        Assert.IsNotEmpty (concreteType.Assembly.GetName ().GetPublicKeyToken ());
      }
    }

    [Test]
    public void UnsignedBaseClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType1)))
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
        Assert.IsEmpty (concreteType.Assembly.GetName ().GetPublicKeyToken ());
      }
    }

    [Test]
    public void SignedBaseClassSignedMixinGeneratedIntoSignedAssembly ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (List<int>), typeof (object)))
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (List<int>), GenerationPolicy.ForceGeneration);
        Assert.IsNotEmpty (concreteType.Assembly.GetName ().GetPublicKeyToken ());
      }
    }

    [Test]
    public void UnsignedBaseClassUnsignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (BT1Mixin1)))
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
        Assert.IsEmpty (concreteType.Assembly.GetName ().GetPublicKeyToken ());
      }
    }

    [Test]
    public void UnsignedBaseClassSignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (object)))
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
        Assert.IsEmpty (concreteType.Assembly.GetName ().GetPublicKeyToken ());
      }
    }

    [Test]
    public void SignedBaseClassUnsignedMixinGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (NullMixin)))
      {
        Type concreteType = TypeFactory.GetConcreteType (typeof (NullTarget), GenerationPolicy.ForceGeneration);
        Assert.IsEmpty (concreteType.Assembly.GetName ().GetPublicKeyToken ());
      }
    }

    [Test]
    public void SignedBaseClassUnsignedMixinWithOverride ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (MixinOverridingToString)))
      {
        object instance = ObjectFactory.Create<NullTarget> ().With ();
        Assert.AreEqual ("Overridden", instance.ToString ());
      }
    }
  }
  
}
using System;
using NUnit.Framework;
using Rubicon.CodeGeneration;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Samples;
using Rubicon.Mixins.UnitTests.SampleTypes;
using System.Collections.Generic;
using Rubicon.Development.UnitTesting;
using System.Collections;
using Rubicon.Mixins.Utilities;

namespace Rubicon.Mixins.UnitTests.Mixins.MixinTypeCodeGeneration
{
  [TestFixture]
  public class StrongNameTests : MixinBaseTest
  {
    public class UnsignedClass
    {
      [OverrideMixin]
      public new string ToString ()
      {
        return "Overridden";
      }
    }

    public class UnsignedMixin : Mixin<object>
    {
      [OverrideTarget]
      protected new string ToString ()
      {
        return "Overridden";
      }
    }

    [Test]
    public void SignedMixinWithSignedTargetClassGeneratedIntoSignedAssembly ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (ArrayList), typeof (EquatableMixin<>)))
      {
        MixinDefinition mixinDefinition = TypeFactory.GetActiveConfiguration (typeof (ArrayList)).GetMixinByConfiguredType (typeof (EquatableMixin<>));
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
        Assert.IsTrue (ReflectionUtility.IsAssemblySigned (generatedType.Assembly));
      }
    }

    [Test]
    public void UnsignedMixinWithUnsignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (UnsignedMixin)))
      {
        MixinDefinition mixinDefinition = TypeFactory.GetActiveConfiguration (typeof (BaseType1)).GetMixinByConfiguredType (typeof (UnsignedMixin));
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (generatedType.Assembly));
      }
    }

    [Test]
    public void SignedMixinWithUnsignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (UnsignedClass), typeof (EquatableMixin<>)))
      {
        MixinDefinition mixinDefinition =
            TypeFactory.GetActiveConfiguration (typeof (UnsignedClass)).GetMixinByConfiguredType (typeof (EquatableMixin<>));
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (generatedType.Assembly));

        Assert.AreEqual ("Overridden", Mixin.Get<EquatableMixin<UnsignedClass>> (ObjectFactory.Create<UnsignedClass>().With()).ToString());
      }
    }

    [Test]
    public void UnsignedMixinWithSignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (UnsignedMixin)))
      {
        MixinDefinition mixinDefinition = TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (UnsignedMixin)];
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (generatedType.Assembly));

        Assert.AreEqual ("Overridden", ObjectFactory.Create<NullTarget>().With().ToString());
      }
    }
  }
}
using System;
using NUnit.Framework;
using Remotion.CodeGeneration;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Samples;
using Remotion.Mixins.UnitTests.SampleTypes;
using System.Collections.Generic;
using Remotion.Development.UnitTesting;
using System.Collections;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.UnitTests.CodeGeneration.MixinTypeCodeGeneration
{
  [TestFixture]
  public class StrongNameTests : CodeGenerationBaseTest
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
      using (MixinConfiguration.BuildFromActive().ForClass<ArrayList> ().Clear().AddMixins (typeof (EquatableMixin<>)).EnterScope())
      {
        MixinDefinition mixinDefinition = TypeFactory.GetActiveConfiguration (typeof (ArrayList)).GetMixinByConfiguredType (typeof (EquatableMixin<>));
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
        Assert.IsTrue (ReflectionUtility.IsAssemblySigned (generatedType.Assembly));
      }
    }

    [Test]
    public void UnsignedMixinWithUnsignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (UnsignedMixin)).EnterScope())
      {
        MixinDefinition mixinDefinition = TypeFactory.GetActiveConfiguration (typeof (BaseType1)).GetMixinByConfiguredType (typeof (UnsignedMixin));
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (generatedType.Assembly));
      }
    }

    [Test]
    public void SignedMixinWithUnsignedTargetClassGeneratedIntoUnsignedAssembly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<UnsignedClass> ().Clear().AddMixins (typeof (EquatableMixin<>)).EnterScope())
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
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (UnsignedMixin)).EnterScope())
      {
        MixinDefinition mixinDefinition = TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (UnsignedMixin)];
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition);
        Assert.IsFalse (ReflectionUtility.IsAssemblySigned (generatedType.Assembly));

        Assert.AreEqual ("Overridden", ObjectFactory.Create<NullTarget>().With().ToString());
      }
    }
  }
}
using System;
using NUnit.Framework;
using Rubicon.CodeGeneration;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.CodeGeneration.DynamicProxy;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Mixins.MixedTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTests : MixinBaseTest
  {
    [Test]
    public void GeneratedMixinTypeWorks ()
    {
      CustomClassEmitter typeEmitter = new CustomClassEmitter (((ModuleManager)ConcreteTypeBuilder.Current.Scope).Scope,
          "GeneratedType", typeof (object));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), generatedType))
      {
        object instance = ObjectFactory.Create (typeof (NullTarget)).With ();
        Assert.IsNotNull (Mixin.Get (generatedType, instance));
      }
    }

    [Test]
    public void GeneratedTargetTypeWorks ()
    {
      CustomClassEmitter typeEmitter = new CustomClassEmitter (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope,
          "GeneratedType", typeof (object));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.ScopedExtend (generatedType, typeof (NullMixin)))
      {
        object instance = ObjectFactory.Create (generatedType).With ();
        Assert.IsNotNull (Mixin.Get (typeof (NullMixin), instance));
      }
    }
  }
}
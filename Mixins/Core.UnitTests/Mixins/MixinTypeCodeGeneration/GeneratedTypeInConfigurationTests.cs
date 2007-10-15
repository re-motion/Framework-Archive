using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Rubicon.CodeGeneration;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.CodeGeneration.DynamicProxy;

namespace Rubicon.Mixins.UnitTests.Mixins.MixinTypeCodeGeneration
{
  [TestFixture]
  public class GeneratedTypeInConfigurationTests : MixinBaseTest
  {
    public class ClassOverridingMixinMethod
    {
      [OverrideMixinMember]
      public new string ToString ()
      {
        return "Overridden!";
      }
    }

    public class SimpleMixin : Mixin<object>
    {
    }

    [Test]
    public void GeneratedMixinTypeWithOverriddenMethodWorks ()
    {
      CustomClassEmitter typeEmitter = new CustomClassEmitter (((ModuleManager)ConcreteTypeBuilder.Current.Scope).Scope,
          "GeneratedType", typeof (Mixin<object>));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.ScopedExtend (typeof (ClassOverridingMixinMethod), generatedType))
      {
        object instance = ObjectFactory.Create (typeof (ClassOverridingMixinMethod)).With ();
        Assert.AreEqual ("Overridden!", Mixin.Get (generatedType, instance).ToString ());
      }
    }

    [Test]
    public void GeneratedTargetTypeOverridingMixinMethodWorks ()
    {
      CustomClassEmitter typeEmitter = new CustomClassEmitter (((ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope,
          "GeneratedType", typeof (object));
      typeEmitter.CreateMethod ("ToString", MethodAttributes.Public)
          .SetReturnType (typeof (string))
          .ImplementByReturning (new ConstReference ("Generated _and_ overridden").ToExpression ())
          .AddCustomAttribute (new CustomAttributeBuilder (typeof (OverrideMixinMemberAttribute).GetConstructor (Type.EmptyTypes), new object[0]));
      Type generatedType = typeEmitter.BuildType ();

      using (MixinConfiguration.ScopedExtend (generatedType, typeof (SimpleMixin)))
      {
        object instance = ObjectFactory.Create (generatedType).With ();
        Assert.AreEqual ("Generated _and_ overridden", Mixin.Get<SimpleMixin> (instance).ToString ());
      }
    }
  }
}
using System;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Definitions;
using NUnit.Framework;
using Rubicon.Mixins.UnitTests.SampleTypes;
using System.Reflection;
using Rubicon.Mixins.UnitTests.Mixins.CodeGenSampleTypes;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class BaseCallProxyCodeGenerationTests : MixinTestBase
  {
    [Test]
    public void GeneratedTypeExists ()
    {
      Type t = CreateMixedType (typeof (BaseType3), typeof (BT3Mixin3<,>));
      Assert.IsNotNull (t.GetNestedType ("BaseCallProxy"));
    }

    [Test]
    public void SubclassProxyHasBaseCallProxyField ()
    {
      Type t = CreateMixedType (typeof (BaseType3), typeof (BT3Mixin3<,>));
      FieldInfo firstField = t.GetField ("__first");
      Assert.IsNotNull (firstField);
      Assert.AreEqual (t.GetNestedType ("BaseCallProxy"), firstField.FieldType);
    }

    [Test]
    public void GeneratedTypeInstantiableWithDepthAndBase ()
    {
      Type t = CreateMixedType (typeof (BaseType3), typeof (BT3Mixin3<,>));
      Type proxyType = t.GetNestedType ("BaseCallProxy");
      Activator.CreateInstance (proxyType, new object[] { null, -1 });
    }

    [Test]
    public void GeneratedTypeHoldsDepthAndBase ()
    {
      Type t = CreateMixedType(typeof (BaseType3), typeof (BT3Mixin3<,>));
      Type proxyType = t.GetNestedType ("BaseCallProxy");
      object proxy = Activator.CreateInstance (proxyType, new object[] { null, -1 });

      Assert.IsNotNull (proxyType.GetField ("__depth"));
      Assert.IsNotNull (proxyType.GetField ("__this"));

      Assert.AreEqual (-1, proxyType.GetField ("__depth").GetValue (proxy));
      Assert.AreEqual (t, proxyType.GetField ("__this").FieldType);
      Assert.IsNull (proxyType.GetField ("__this").GetValue (proxy));
    }

    [Test]
    public void InstantiatedSubclassProxyHasBaseCallProxy ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin3<,>)).With();
      FieldInfo firstField = bt3.GetType().GetField ("__first");
      Assert.IsNotNull (firstField.GetValue (bt3));
    }

    [Test]
    public void GeneratedTypeImplementsRequiredBaseCallInterfaces1 ()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType3), typeof (MixinWithThisAsBase)))
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType3));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        foreach (RequiredBaseCallTypeDefinition req in TypeFactory.GetActiveConfiguration(typeof (BaseType3)).RequiredBaseCallTypes)
          Assert.IsTrue (req.Type.IsAssignableFrom (proxyType));
      }
    }

    [Test]
    public void GeneratedTypeImplementsRequiredBaseCallInterfaces2 ()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType3), typeof (BT3Mixin7Base), typeof (BT3Mixin4)))
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType3));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        RequiredBaseCallTypeDefinition bt3Mixin4Req =
            TypeFactory.GetActiveConfiguration (typeof (BaseType3)).RequiredBaseCallTypes[typeof (IBT3Mixin4)];
        Assert.IsNotNull (bt3Mixin4Req);
        Assert.IsTrue (bt3Mixin4Req.Type.IsAssignableFrom (proxyType));

        foreach (RequiredBaseCallTypeDefinition req in TypeFactory.GetActiveConfiguration (typeof (BaseType3)).RequiredBaseCallTypes)
          Assert.IsTrue (req.Type.IsAssignableFrom (proxyType));

        MethodInfo methodImplementdByMixin =
            proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.IBT3Mixin4.Foo", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByMixin);

        MethodInfo methodImplementdByBCOverridden =
            proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.IBaseType31.IfcMethod", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByBCOverridden);

        MethodInfo methodImplementdByBCNotOverridden =
            proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.IBaseType35.IfcMethod2", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByBCNotOverridden);
      }
    }

    [Test]
    public void GeneratedTypeImplementsOverriddenMemthods ()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType3), typeof (BT3Mixin7Base), typeof(BT3Mixin4)))
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType3));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType3.IfcMethod", BindingFlags.Public | BindingFlags.Instance));
      }
    }

    [Test]
    public void GeneratedTypeImplementsOverriddenMethods ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (BT1Mixin1)))
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType1));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.VirtualMethod", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.get_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.set_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.add_VirtualEvent", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.remove_VirtualEvent", BindingFlags.Public | BindingFlags.Instance));
      }

      using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (BT1Mixin2)))
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType1));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.get_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.set_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
      }

    }

    [Test]
    public void OverriddenMemberCalls ()
    {
      using (MixinConfiguration.ScopedExtend(typeof (BaseType3), typeof (BT3Mixin7Base), typeof (BT3Mixin4)))
      {
        BaseType3 bt3 = ObjectFactory.Create<BaseType3> ().With ();
        Assert.AreEqual ("BT3Mixin7Base.IfcMethod-BT3Mixin4.Foo-BaseType3.IfcMethod-BaseType3.IfcMethod2", bt3.IfcMethod ());
      }
    }

    [Test]
    public void BaseCallMethodToThis()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (MixinWithThisAsBase)).With ();
      Assert.AreEqual ("MixinWithThisAsBase.IfcMethod-BaseType3.IfcMethod", bt3.IfcMethod());
    }

    [Test]
    public void BaseCallMethodToDuckInterface ()
    {
      BaseTypeWithDuckBaseMixin duckBase = ObjectFactory.Create<BaseTypeWithDuckBaseMixin> ().With ();
      Assert.AreEqual ("DuckBaseMixin.MethodImplementedOnBase-BaseTypeWithDuckBaseMixin.MethodImplementedOnBase-"
          + "DuckBaseMixin.ProtectedMethodImplementedOnBase-BaseTypeWithDuckBaseMixin.ProtectedMethodImplementedOnBase",
          duckBase.MethodImplementedOnBase ());
    }

    [Test]
    public void BaseCallsToIndirectlyRequiredInterfaces ()
    {
      ClassImplementingIndirectRequirements ciir = ObjectFactory.Create<ClassImplementingIndirectRequirements> ().With ();
      MixinWithIndirectRequirements mixin = Mixin.Get<MixinWithIndirectRequirements> (ciir);
      Assert.AreEqual ("ClassImplementingIndirectRequirements.Method1-ClassImplementingIndirectRequirements.BaseMethod1-"
          + "ClassImplementingIndirectRequirements.Method3", mixin.GetStuffViaBase ());
    }

  }
}

using System;
using Mixins.CodeGeneration;
using Mixins.Definitions;
using NUnit.Framework;
using Mixins.UnitTests.SampleTypes;
using System.Reflection;
using Mixins.UnitTests.Mixins.CodeGenSampleTypes;

namespace Mixins.UnitTests.Mixins
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
      using (new CurrentTypeFactoryScope (DefBuilder.Build (typeof (BaseType3), typeof (MixinWithThisAsBase))))
      {
        Type t = TypeFactory.Current.GetConcreteType (typeof (BaseType3));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        foreach (RequiredBaseCallTypeDefinition req in TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].RequiredBaseCallTypes)
          Assert.IsTrue (req.Type.IsAssignableFrom (proxyType));
      }
    }

    [Test]
    public void GeneratedTypeImplementsRequiredBaseCallInterfaces2 ()
    {
      using (new CurrentTypeFactoryScope (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin7Base), typeof (BT3Mixin4))))
      {
        Type t = TypeFactory.Current.GetConcreteType (typeof (BaseType3));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        RequiredBaseCallTypeDefinition bt3Mixin4Req =
            TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].RequiredBaseCallTypes[typeof (IBT3Mixin4)];
        Assert.IsNotNull (bt3Mixin4Req);
        Assert.IsTrue (bt3Mixin4Req.Type.IsAssignableFrom (proxyType));

        foreach (RequiredBaseCallTypeDefinition req in TypeFactory.Current.Configuration.BaseClasses[typeof (BaseType3)].RequiredBaseCallTypes)
          Assert.IsTrue (req.Type.IsAssignableFrom (proxyType));

        MethodInfo methodImplementdByMixin =
            proxyType.GetMethod ("Mixins.UnitTests.SampleTypes.IBT3Mixin4.Foo", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByMixin);

        MethodInfo methodImplementdByBCOverridden =
            proxyType.GetMethod ("Mixins.UnitTests.SampleTypes.IBaseType31.IfcMethod", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByBCOverridden);

        MethodInfo methodImplementdByBCNotOverridden =
            proxyType.GetMethod ("Mixins.UnitTests.SampleTypes.IBaseType35.IfcMethod2", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull (methodImplementdByBCNotOverridden);
      }
    }

    [Test]
    public void GeneratedTypeImplementsOverriddenMembers ()
    {
      using (new CurrentTypeFactoryScope (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin7Base), typeof(BT3Mixin4))))
      {
        Type t = TypeFactory.Current.GetConcreteType (typeof(BaseType3));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        Assert.IsNotNull (proxyType.GetMethod ("Mixins.UnitTests.SampleTypes.BaseType3.IfcMethod", BindingFlags.Public | BindingFlags.Instance));
      }
    }

    [Test]
    public void OverriddenMemberCalls ()
    {
      using (new CurrentTypeFactoryScope (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin7Base), typeof (BT3Mixin4))))
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
  }
}

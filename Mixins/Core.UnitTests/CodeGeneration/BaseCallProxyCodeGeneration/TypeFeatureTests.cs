using System;
using Rubicon.Mixins.Definitions;
using NUnit.Framework;
using Rubicon.Mixins.UnitTests.SampleTypes;
using System.Reflection;
using Rubicon.Mixins.UnitTests.CodeGeneration.SampleTypes;

namespace Rubicon.Mixins.UnitTests.CodeGeneration.BaseCallProxyCodeGeneration
{
  [TestFixture]
  public class TypeFeatureTests : CodeGenerationBaseTest
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
    public void GeneratedTypeImplementsOverriddenMethods1 ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3> ().Clear().AddMixins (typeof (BT3Mixin7Base), typeof (BT3Mixin4)).EnterScope())
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType3));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType3.IfcMethod", BindingFlags.Public | BindingFlags.Instance));
      }
    }

    [Test]
    public void GeneratedTypeImplementsOverriddenMethods2 ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType1));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.VirtualMethod", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.get_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.set_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.add_VirtualEvent", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.remove_VirtualEvent", BindingFlags.Public | BindingFlags.Instance));
      }

      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (BT1Mixin2)).EnterScope())
      {
        Type t = TypeFactory.GetConcreteType (typeof (BaseType1));
        Type proxyType = t.GetNestedType ("BaseCallProxy");

        Assert.IsNotNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.get_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
        Assert.IsNull (proxyType.GetMethod ("Rubicon.Mixins.UnitTests.SampleTypes.BaseType1.set_VirtualProperty", BindingFlags.Public | BindingFlags.Instance));
      }

    }
  }
}
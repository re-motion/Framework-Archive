using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Mixins.CodeGeneration;
using Mixins.Definitions;
using NUnit.Framework;
using Mixins.UnitTests.SampleTypes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using Mixins.Validation;
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

      Assert.IsNotNull (proxyType.GetField ("_depth"));
      Assert.IsNotNull (proxyType.GetField ("_this"));

      Assert.AreEqual (-1, proxyType.GetField ("_depth").GetValue (proxy));
      Assert.AreEqual (t, proxyType.GetField ("_this").FieldType);
      Assert.IsNull (proxyType.GetField ("_this").GetValue (proxy));
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
      ApplicationDefinition configuration = DefBuilder.Build (typeof (BaseType3), typeof (MixinWithThisAsBase));
      TypeFactory tf = NewTypeFactory (configuration);
      Type t = tf.GetConcreteType(typeof (BaseType3));
      Type proxyType = t.GetNestedType ("BaseCallProxy");

      foreach (RequiredBaseCallTypeDefinition req in configuration.BaseClasses[typeof (BaseType3)].RequiredBaseCallTypes)
        Assert.IsTrue (req.Type.IsAssignableFrom (proxyType));
    }

    [Test]
    [Ignore("TODO: Implement non-this interfaces on proxy")]
    public void GeneratedTypeImplementsRequiredBaseCallInterfaces2 ()
    {
      Type t = TypeFactory.GetConcreteType (typeof (BaseType3));
      Type proxyType = t.GetNestedType ("BaseCallProxy");

      foreach (RequiredBaseCallTypeDefinition req in Configuration.BaseClasses[typeof (BaseType3)].RequiredBaseCallTypes)
        Assert.IsTrue (req.Type.IsAssignableFrom (proxyType));
    }

    [Test]
    [Ignore("TODO: Implement overrides")]
    public void BaseCallMethodToThis()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (MixinWithThisAsBase)).With ();
      Assert.AreEqual ("MixinWithThisAsBase.IfcMethod-BaseType3.IfcMethod", bt3.IfcMethod());
    }
  }
}

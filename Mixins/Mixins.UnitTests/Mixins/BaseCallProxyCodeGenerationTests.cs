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

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class BaseCallProxyCodeGenerationTests : MixinTestBase
  {
    [Test]
    public void GeneratedTypeExists ()
    {
      Type t = new TypeFactory (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin3<,>))).GetConcreteType (typeof (BaseType3));
      Assert.IsNotNull (t.GetNestedType ("BaseCallProxy"));
    }

    [Test]
    public void GeneratedTypeInstantiableWithDepthAndBase ()
    {
      Type t = new TypeFactory (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin3<,>))).GetConcreteType (typeof (BaseType3));
      Type proxyType = t.GetNestedType ("BaseCallProxy");
      Activator.CreateInstance (proxyType, new object[] { null, -1 });
    }

    [Test]
    public void GeneratedTypeHoldsDepthAndBase ()
    {
      Type t = new TypeFactory (DefBuilder.Build (typeof (BaseType3), typeof (BT3Mixin3<,>))).GetConcreteType (typeof (BaseType3));
      Type proxyType = t.GetNestedType ("BaseCallProxy");
      object proxy = Activator.CreateInstance (proxyType, new object[] { null, -1 });

      Assert.IsNotNull (proxyType.GetField ("_depth"));
      Assert.IsNotNull (proxyType.GetField ("_this"));

      Assert.AreEqual (-1, proxyType.GetField ("_depth").GetValue (proxy));
      Assert.AreEqual (t, proxyType.GetField ("_this").FieldType);
      Assert.IsNull (proxyType.GetField ("_this").GetValue (proxy));
    }

    [Test]
    [Ignore ("TODO")]
    public void GeneratedTypeImplementsRequiredBaseCallInterfaces ()
    {
      Type t = TypeFactory.GetConcreteType (typeof (BaseType3));
      Type proxyType = t.GetNestedType ("BaseCallProxy");

      foreach (RequiredBaseCallTypeDefinition req in Configuration.BaseClasses[typeof (BaseType3)].RequiredBaseCallTypes)
        Assert.IsTrue (req.Type.IsAssignableFrom (proxyType));
    }
  }
}

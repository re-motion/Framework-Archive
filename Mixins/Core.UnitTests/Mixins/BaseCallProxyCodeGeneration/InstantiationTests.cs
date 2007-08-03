using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Mixins.BaseCallProxyCodeGeneration
{
  [TestFixture]
  public class InstantiationTests : MixinTestBase
  {
    [Test]
    public void GeneratedTypeInstantiableWithDepthAndBase ()
    {
      Type t = CreateMixedType (typeof (BaseType3), typeof (BT3Mixin3<,>));
      Type proxyType = t.GetNestedType ("BaseCallProxy");
      Activator.CreateInstance (proxyType, new object[] { null, -1 });
    }

    [Test]
    public void InstantiatedSubclassProxyHasBaseCallProxy ()
    {
      BaseType3 bt3 = CreateMixedObject<BaseType3> (typeof (BT3Mixin3<,>)).With ();
      FieldInfo firstField = bt3.GetType ().GetField ("__first");
      Assert.IsNotNull (firstField.GetValue (bt3));
    }
  }
}
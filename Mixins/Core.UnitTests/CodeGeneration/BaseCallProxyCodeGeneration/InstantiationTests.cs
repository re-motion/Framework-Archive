using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.SampleTypes;

namespace Remotion.Mixins.UnitTests.CodeGeneration.BaseCallProxyCodeGeneration
{
  [TestFixture]
  public class InstantiationTests : CodeGenerationBaseTest
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
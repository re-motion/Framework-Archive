using System;
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters;
using NUnit.Framework;
using Rubicon.CodeGeneration;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Core.UnitTests.CodeGeneration.SampleTypes;
using System.Reflection.Emit;

namespace Rubicon.Core.UnitTests.CodeGeneration
{
  [TestFixture]
  public class LdArrayElementTest : CodeGenerationBaseTest
  {
    [Test]
    public void LoadArrayElementFromExpression ()
    {
      CustomClassEmitter classEmitter = new CustomClassEmitter (Scope, "Foo", typeof (object));
      CustomMethodEmitter method = classEmitter.CreateMethod ("LoadArrayElementFromMethodCall", MethodAttributes.Public);
      method.SetParameters (new Type[] { typeof (IArrayProvider), typeof (int) });
      method.SetReturnType (typeof (object));
      method.AddStatement (new ILStatement (delegate (IMemberEmitter member, ILGenerator ilgen)
      {
        ilgen.Emit (OpCodes.Ldarg_1); // array provider
        ilgen.Emit (OpCodes.Callvirt, typeof (IArrayProvider).GetMethod ("GetArray")); // array
        ilgen.Emit (OpCodes.Castclass, typeof (object[])); // essentially a nop
        ilgen.Emit (OpCodes.Ldarg_2); // index
        ilgen.Emit (OpCodes.Ldelem, typeof (object));
        ilgen.Emit (OpCodes.Castclass, typeof (object));
        ilgen.Emit (OpCodes.Ret);
      }));

      object instance = Activator.CreateInstance (classEmitter.BuildType ());
      SimpleArrayProvider provider = new SimpleArrayProvider();
      object result = instance.GetType ().GetMethod ("LoadArrayElementFromMethodCall").Invoke (instance, new object[] { provider, 1 });
      Assert.AreEqual (2, result);
    }
  }
}
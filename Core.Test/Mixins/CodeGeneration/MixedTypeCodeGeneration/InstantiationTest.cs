using System;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using Remotion.Core.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.CodeGeneration.MixedTypeCodeGeneration
{
  [TestFixture]
  public class InstantiationTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeCanBeInstantiatedViaCtorCall ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      BaseType3 bt3 = (BaseType3) Activator.CreateInstance (generatedType);
      Assert.IsNotNull (bt3);
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3));
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).This);
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).Base);
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).Configuration);
      Assert.AreSame (bt3, Mixin.Get<BT3Mixin1> (bt3).This);
      Assert.AreSame (TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)], Mixin.Get<BT3Mixin1> (bt3).Configuration);
    }

    [Test]
    public void GeneratedTypeCanBeInstantiatedViaCtorCallEvenWhenDerived ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));

      AssemblyBuilder builder =
          AppDomain.CurrentDomain.DefineDynamicAssembly (new AssemblyName ("Foo"), AssemblyBuilderAccess.Run);
      TypeBuilder typeBuilder = builder.DefineDynamicModule ("Foo.dll").DefineType ("Derived", TypeAttributes.Public, generatedType);
      ConstructorBuilder ctor = typeBuilder.DefineConstructor (MethodAttributes.Public, CallingConventions.HasThis, Type.EmptyTypes);
      ILGenerator ilgen = ctor.GetILGenerator ();
      ilgen.Emit (OpCodes.Ldarg_0);
      ilgen.Emit (OpCodes.Callvirt, generatedType.GetConstructor (Type.EmptyTypes));
      ilgen.Emit (OpCodes.Ret);

      Type evenDerivedType = typeBuilder.CreateType ();

      BaseType3 bt3 = (BaseType3) Activator.CreateInstance (evenDerivedType);
      Assert.AreSame (generatedType, bt3.GetType ().BaseType);
      Assert.IsNotNull (bt3);
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3));
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).This);
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).Base);
      Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).Configuration);
      Assert.AreSame (bt3, Mixin.Get<BT3Mixin1> (bt3).This);
      Assert.AreSame (TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)], Mixin.Get<BT3Mixin1> (bt3).Configuration);
    }

    [Test]
    public void CtorsRespectMixedTypeInstantiationScope ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType3));
      BT3Mixin1 suppliedMixinInstance = new BT3Mixin1 ();

      using (new MixedTypeInstantiationScope (suppliedMixinInstance))
      {
        BaseType3 bt3 = (BaseType3) Activator.CreateInstance (generatedType);
        Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3));
        Assert.AreSame (suppliedMixinInstance, Mixin.Get<BT3Mixin1> (bt3));
        Assert.AreSame (bt3, suppliedMixinInstance.This);
        Assert.IsNotNull (Mixin.Get<BT3Mixin1> (bt3).Base);
        Assert.AreSame (TypeFactory.GetActiveConfiguration (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)], Mixin.Get<BT3Mixin1> (bt3).Configuration);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The supplied mixin of type Remotion.Core.UnitTests.Mixins.SampleTypes.BT3Mixin1 is not valid in the current configuration.",
        MatchType = MessageMatch.Contains)]
    public void ThrowsIfWrongMixinInstancesInScope ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      BT3Mixin1 suppliedMixinInstance = new BT3Mixin1 ();

      using (new MixedTypeInstantiationScope (suppliedMixinInstance))
      {
        try
        {
          Activator.CreateInstance (generatedType);
        }
        catch (TargetInvocationException ex)
        {
          throw ex.InnerException;
        }
      }
    }
  }
}

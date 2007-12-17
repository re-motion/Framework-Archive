using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Reflection;
using Rubicon.Mixins.CodeGeneration.DynamicProxy;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  public abstract class MixinBaseTest
  {
    [SetUp]
    public virtual void SetUp()
    {
      ResetGeneratedAssemblies ();
      ConcreteTypeBuilder.SetCurrent (null);
    }

    private void ResetGeneratedAssemblies ()
    {
      if (File.Exists (ModuleManager.DefaultWeakModulePath))
        File.Delete (ModuleManager.DefaultWeakModulePath);
      if (File.Exists (ModuleManager.DefaultStrongModulePath))
        File.Delete (ModuleManager.DefaultStrongModulePath);
    }

    [TearDown]
    public virtual void TearDown()
    {

#if !NO_PEVERIFY
      string[] paths;
      try
      {
         paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope ();
      }
      catch (Exception ex)
      {
        Assert.Fail ("Error when saving assemblies: {0}", ex);
        return;
      }

      foreach (string path in paths)
        PEVerifier.VerifyPEFile (path);

      ResetGeneratedAssemblies (); // delete assemblies if everything went fine
#endif
      ConcreteTypeBuilder.SetCurrent (null);
    }

    public Type CreateMixedType (Type targetType, params Type[] mixinTypes)
    {
      using (MixinConfiguration.BuildFromActive().ForClass (targetType).Clear().AddMixins (mixinTypes).EnterScope())
        return TypeFactory.GetConcreteType (targetType, GenerationPolicy.ForceGeneration);
    }

    public FuncInvokerWrapper<T> CreateMixedObject<T> (params Type[] mixinTypes)
    {
      using (MixinConfiguration.BuildFromActive().ForClass<T> ().Clear().AddMixins (mixinTypes).EnterScope())
        return ObjectFactory.Create<T>(GenerationPolicy.ForceGeneration);
    }
  }
}

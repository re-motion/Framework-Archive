using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Reflection;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  public abstract class MixinTestBase
  {
    private IDisposable currentConfiguration = null;

    [SetUp]
    public virtual void SetUp()
    {
      ConcreteTypeBuilder.SetCurrent (null);
      currentConfiguration = MixinConfiguration.ScopedExtend (Assembly.GetExecutingAssembly ());
    }

    [TearDown]
    public virtual void TearDown()
    {
      if (currentConfiguration != null)
        currentConfiguration.Dispose();

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

      ConcreteTypeBuilder.SetCurrent (null);
    }

    public Type CreateMixedType (Type targetType, params Type[] mixinTypes)
    {
      using (MixinConfiguration.ScopedExtend (targetType, mixinTypes))
        return TypeFactory.GetConcreteType (targetType);
    }

    public FuncInvokerWrapper<T> CreateMixedObject<T> (params Type[] mixinTypes)
    {
      using (MixinConfiguration.ScopedExtend (typeof (T), mixinTypes))
        return ObjectFactory.Create<T>();
    }
  }
}

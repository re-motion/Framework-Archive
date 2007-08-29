using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.MixerTool;

namespace Rubicon.Mixins.UnitTests.MixerTool
{
  [Serializable]
  public class MixerToolBaseTest
  {
    private MixerParameters _parameters;

    [SetUp]
    public void SetUp ()
    {
      _parameters = new MixerParameters();
      ResetGeneratedFiles();
    }

    [TearDown]
    public void TearDown ()
    {
      ResetGeneratedFiles ();
    }

    public MixerParameters Parameters
    {
      get { return _parameters; }
    }

    public string UnsignedAssemblyPath
    {
      get { return Path.Combine (_parameters.AssemblyOutputDirectory, _parameters.UnsignedAssemblyName + ".dll"); }
    }

    public string SignedAssemblyPath
    {
      get { return Path.Combine (_parameters.AssemblyOutputDirectory, _parameters.SignedAssemblyName + ".dll"); }
    }

    public void ResetGeneratedFiles ()
    {
      if (File.Exists (UnsignedAssemblyPath))
        File.Delete (UnsignedAssemblyPath);
      if (File.Exists (SignedAssemblyPath))
        File.Delete (SignedAssemblyPath);
    }

    public Type GetFirstMixedType (Assembly assembly)
    {
      foreach (Type t in assembly.GetTypes ())
      {
        if (t.IsDefined (typeof (ConcreteMixedTypeAttribute), false))
          return t;
      }
      return null;
    }

    public Set<ClassContext> GetContextsFromGeneratedTypes (Assembly assembly)
    {
      Set<ClassContext> contextsFromTypes = new Set<ClassContext> ();
      foreach (Type concreteType in assembly.GetTypes ())
      {
        ClassContext context = Mixin.GetMixinConfigurationFromConcreteType (concreteType);
        if (context != null)
          contextsFromTypes.Add (context);
      }
      return contextsFromTypes;
    }
  }
}
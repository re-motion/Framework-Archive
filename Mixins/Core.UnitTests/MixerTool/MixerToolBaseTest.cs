using System;
using System.IO;
using NUnit.Framework;
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
  }
}
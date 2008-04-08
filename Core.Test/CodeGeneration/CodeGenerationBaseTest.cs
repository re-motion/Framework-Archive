using System;
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.CodeGeneration.DPExtensions;
using Remotion.Development.UnitTesting;

namespace Remotion.Core.UnitTests.CodeGeneration
{
  public abstract class CodeGenerationBaseTest
  {
    private ModuleScope _scope;
    private bool _assemblySaveSuppressed;

    [SetUp]
    public virtual void SetUp ()
    {
      _scope = new ModuleScope (true);
      _assemblySaveSuppressed = false;
    }

    [TearDown]
    public virtual void TearDown ()
    {
      if (!_assemblySaveSuppressed)
      {
        string[] paths = AssemblySaver.SaveAssemblies (_scope);
        foreach (string path in paths)
          PEVerifier.VerifyPEFile (path);
      }
    }

    public ModuleScope Scope
    {
      get { return _scope; }
    }

    public void SuppressAssemblySave ()
    {
      _assemblySaveSuppressed = true;
    }
  }
}
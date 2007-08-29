using System;
using System.IO;
using Castle.DynamicProxy;
using NUnit.Framework;
using Rubicon.CodeGeneration.DPExtensions;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Core.UnitTests.CodeGeneration
{
  public abstract class CodeGenerationBaseTest
  {
    private ModuleScope _scope;

    [SetUp]
    public virtual void SetUp ()
    {
      _scope = new ModuleScope (true);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      string[] paths = AssemblySaver.SaveAssemblies (_scope);
      foreach (string path in paths)
        PEVerifier.VerifyPEFile (path);
    }

    public ModuleScope Scope
    {
      get { return _scope; }
    }
  }
}
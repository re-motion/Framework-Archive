using System;
using System.IO;
using Castle.DynamicProxy;
using NUnit.Framework;
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
      if (_scope.StrongNamedModule != null)
      {
        _scope.SaveAssembly (true);
        PEVerifier.VerifyPEFile (_scope.StrongNamedModule.FullyQualifiedName);
      }
      if (_scope.WeakNamedModule != null)
      {
        _scope.SaveAssembly (true);
        PEVerifier.VerifyPEFile (_scope.WeakNamedModule.FullyQualifiedName);
      }
    }

    public ModuleScope Scope
    {
      get { return _scope; }
    }
  }
}
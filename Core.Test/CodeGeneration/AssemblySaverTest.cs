using System;
using System.IO;
using NUnit.Framework;
using Castle.DynamicProxy;
using Rubicon.CodeGeneration;
using Rubicon.CodeGeneration.DPExtensions;

namespace Rubicon.Core.UnitTests.CodeGeneration
{
  [TestFixture]
  public class AssemblySaverTest
  {
    [Test]
    public void SaveAssemblyNoGeneratedTypes ()
    {
      ModuleScope scope = new ModuleScope (true);
      string[] paths = AssemblySaver.SaveAssemblies (scope);
      Assert.AreEqual (0, paths.Length);
    }

    [Test]
    public void SaveAssemblySigned ()
    {
      ModuleScope scope = new ModuleScope (true);
      CustomClassEmitter emitter = new CustomClassEmitter (scope, "SignedType", typeof (object));
      emitter.BuildType ();
      string[] paths = AssemblySaver.SaveAssemblies (scope);
      Assert.AreEqual (1, paths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, scope.StrongNamedModuleName), paths[0]);
    }

    //[Test]
    //public void SaveAssemblyUnsigned ()
    //{
    //  This test cannot be implemented in a signed test assembly, it's semantics are tested indirectly in
    //  Rubicon.Mixins.UnitTests.Mixins.ModuleManagerTests.
    //}
  }
}
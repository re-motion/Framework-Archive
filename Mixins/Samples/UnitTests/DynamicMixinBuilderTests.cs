using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Development.UnitTesting;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Mixins.Samples.UnitTests
{
  [TestFixture]
  public class DynamicMixinBuilderTests
  {
    public class SampleTarget
    {
      public virtual string StringMethod (int intArg)
      {
        return "SampleTarget.StringMethod (" + intArg + ")";
      }
    }

    private readonly List<Tuple<object, MethodInfo, object[]>> _calls = new List<Tuple<object, MethodInfo, object[]>> ();
    private MethodInvocationHandler _invocationHandler;
    private DynamicMixinBuilder _builder;

    [SetUp]
    public void SetUp ()
    {
      string directory = PrepareDirectory();

      DynamicMixinBuilder.Scope = new ModuleScope (true, "DynamicMixinBuilder.Signed", Path.Combine (directory, "DynamicMixinBuilder.Signed.dll"),
        "DynamicMixinBuilder.Unsigned", Path.Combine (directory, "DynamicMixinBuilder.Unsigned.dll"));

      _invocationHandler = delegate (object instance, MethodInfo method, object[] args, Func<object[], object> baseMethod)
      {
        _calls.Add (Tuple.NewTuple (instance, method, args));
        return "Intercepted: " + baseMethod (args);
      };
      _calls.Clear ();

      _builder = new DynamicMixinBuilder (typeof (SampleTarget));
    }

    private string PrepareDirectory ()
    {
      string directory = Path.Combine (Environment.CurrentDirectory, "DynamicMixinBuilder.Generated");
      if (Directory.Exists (directory))
        Directory.Delete (directory, true);
      Directory.CreateDirectory (directory);

      File.Copy (typeof (Mixin<,>).Assembly.ManifestModule.FullyQualifiedName,
          Path.Combine (directory, typeof (Mixin<,>).Assembly.ManifestModule.Name));
      return directory;
    }

    [TearDown]
    public void TearDown ()
    {
      if (DynamicMixinBuilder.Scope.StrongNamedModule != null)
      {
        DynamicMixinBuilder.Scope.SaveAssembly (true);
        PEVerifier.VerifyPEFile (DynamicMixinBuilder.Scope.StrongNamedModule.FullyQualifiedName);
      }
      if (DynamicMixinBuilder.Scope.WeakNamedModule != null)
      {
        DynamicMixinBuilder.Scope.SaveAssembly (false);
        PEVerifier.VerifyPEFile (DynamicMixinBuilder.Scope.WeakNamedModule.FullyQualifiedName);
      }
    }

    [Test]
    public void BuildMixinType_CreatesType ()
    {
      Type t = new DynamicMixinBuilder (typeof (object)).BuildMixinType (_invocationHandler);
      Assert.IsNotNull (t);
    }

    [Test]
    public void BuildMixinType_CreatesTypeDerivedFromMixin ()
    {
      Type t = new DynamicMixinBuilder (typeof (object)).BuildMixinType (_invocationHandler);
      Assert.IsTrue (Rubicon.Utilities.ReflectionUtility.CanAscribe (t, typeof (Mixin<,>)));
    }

    [Test]
    public void BuildMixinType_AddsMethodsWithOverrideAttribute ()
    {
      _builder.OverrideMethod (typeof (SampleTarget).GetMethod ("StringMethod"));
      Type t = _builder.BuildMixinType (_invocationHandler);

      MethodInfo overriderMethod = t.GetMethod ("StringMethod");
      Assert.IsNotNull (overriderMethod);
      Assert.IsTrue (overriderMethod.IsDefined (typeof (OverrideAttribute), false));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The declaring type of the method must be the "
        + "target type.\r\nParameter name: method")]
    public void BuildMixinType_OverrideMethod_FromWrongType ()
    {
      _builder.OverrideMethod (typeof (object).GetMethod ("ToString"));
    }

    [Test]
    [Ignore ("TODO: Fix issue with generated mixin types")]
    public void GeneratedMethodIsIntercepted ()
    {
      _builder.OverrideMethod (typeof (SampleTarget).GetMethod ("StringMethod"));
      Type t = _builder.BuildMixinType (_invocationHandler);

      using (MixinConfiguration.ScopedExtend (typeof (SampleTarget), t))
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget> ().With();
        target.StringMethod (4);
        Assert.IsTrue (_calls.Count == 1);
      }
    }

    [Test]
    [Ignore ("TODO: Fix issue with generated mixin types")]
    public void GeneratedMethodIsIntercepted_WithRightParameters ()
    {
      _builder.OverrideMethod (typeof (SampleTarget).GetMethod ("StringMethod"));
      Type t = _builder.BuildMixinType (_invocationHandler);

      using (MixinConfiguration.ScopedExtend (typeof (SampleTarget), t))
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget> ().With ();
        target.StringMethod (4);

        Tuple<object, MethodInfo, object[]> callInfo = _calls[0];
        Assert.AreSame (target, callInfo.A);
        Assert.AreEqual (typeof (SampleTarget).GetMethod ("StringMethod"), callInfo.B);
        Assert.That (callInfo.C, Is.EquivalentTo (new object[] { 4 } ));
      }
    }

    [Test]
    [Ignore ("TODO: Fix issue with generated mixin types")]
    public void GeneratedMethodIsIntercepted_WithCorrectBase ()
    {
      _builder.OverrideMethod (typeof (SampleTarget).GetMethod ("StringMethod"));
      Type t = _builder.BuildMixinType (_invocationHandler);

      using (MixinConfiguration.ScopedExtend (typeof (SampleTarget), t))
      {
        SampleTarget target = ObjectFactory.Create<SampleTarget> ().With ();
        string result = target.StringMethod (4);
        Assert.AreEqual ("Intercepted: SampleTarget.StringMethod (4)", result);
      }
    }
  }
}
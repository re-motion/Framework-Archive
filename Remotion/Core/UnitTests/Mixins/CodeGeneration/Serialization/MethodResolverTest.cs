// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration.Serialization;

namespace Remotion.UnitTests.Mixins.CodeGeneration.Serialization
{
  [TestFixture]
  public class MethodResolverTest
  {
    [Test]
    public void ResolveMethod_InstanceMethod ()
    {
      var method = MethodResolver.ResolveMethod (typeof (MethodResolverTest), "ResolveMethod_InstanceMethod", "Void ResolveMethod_InstanceMethod()");

      Assert.That (method, Is.EqualTo (MethodInfo.GetCurrentMethod ()));
    }

    [Test]
    public void ResolveMethod_StaticMethod ()
    {
      var method = MethodResolver.ResolveMethod (typeof (object), "ReferenceEquals", "Boolean ReferenceEquals(System.Object, System.Object)");

      Assert.That (method, Is.EqualTo (typeof (object).GetMethod ("ReferenceEquals")));
      Console.WriteLine (typeof (object).GetMethod ("ReferenceEquals"));
    }

    [Test]
    public void ResolveMethod_MethodWithOverloads ()
    {
      var method = MethodResolver.ResolveMethod (typeof (Console), "WriteLine", "Void WriteLine(System.String)");

      Assert.That (method, Is.EqualTo (typeof (Console).GetMethod ("WriteLine", new[] { typeof (string) })));
    }

    [Test]
    public void ResolveMethod_ProtectedMethod ()
    {
      var method = MethodResolver.ResolveMethod (typeof (object), "Finalize", "Void Finalize()");

      Assert.That (method, Is.EqualTo (typeof (object).GetMethod ("Finalize", BindingFlags.NonPublic | BindingFlags.Instance)));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "The method 'Void Foo()' could not be found on type 'System.Object'.")]
    public void ResolveMethod_NonMatchingName ()
    {
      MethodResolver.ResolveMethod (typeof (object), "Foo", "Void Foo()");
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "The method 'Void Foo()' could not be found on type 'System.Console'.")]
    public void ResolveMethod_NonMatchingSignature ()
    {
      MethodResolver.ResolveMethod (typeof (Console), "WriteLine", "Void Foo()");
    }
  }
}
using System;
using System.Text;
using NUnit.Framework;

using Rubicon.Development.UnitTesting;

namespace Rubicon.Development.UnitTests.UnitTesting
{
  public class TypeWithMethods
  {
    public string f ()
    {
      return "f";
    }

    public string f (int i)
    {
      return "f int";
    }

    public string f (int i, string s)
    {
      return "f int string";
    }

    public string f (string s)
    {
      return "f string";
    }

    public string f (StringBuilder sb)
    {
      return "f StringBuilder";
    }

    private string f (int i, StringBuilder s)
    {
      return "f int StringBuilder";
    }

    public static string s (int i)
    {
      return "s int";
    }

    private static string s (string s)
    {
      return "s string";
    }
  }

  [TestFixture]
	public class TestMethodInvoke
	{
    TypeWithMethods _twm = new TypeWithMethods();

    [Test]
    public void TestInvoke()
    {
      Assertion.AssertEquals ("f",                    PrivateInvoke.InvokePublicMethod (_twm, "f"));
      Assertion.AssertEquals ("f int",                PrivateInvoke.InvokePublicMethod (_twm, "f", 1));
      Assertion.AssertEquals ("f int string",         PrivateInvoke.InvokePublicMethod (_twm, "f", 1, null));
      Assertion.AssertEquals ("f string",             PrivateInvoke.InvokePublicMethod (_twm, "f", "test"));
      Assertion.AssertEquals ("f StringBuilder",      PrivateInvoke.InvokePublicMethod (_twm, "f", new StringBuilder()));
      Assertion.AssertEquals ("f int StringBuilder",  PrivateInvoke.InvokeNonPublicMethod (_twm, "f", 1, new StringBuilder()));
    }

    [Test]
    public void TestStaticInvoke()
    {
      Assertion.AssertEquals ("s int",                PrivateInvoke.InvokePublicStaticMethod (typeof(TypeWithMethods), "s", 1));
      Assertion.AssertEquals ("s string",             PrivateInvoke.InvokeNonPublicStaticMethod (typeof(TypeWithMethods), "s", "test"));
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMethodNameException))]
    public void TestPublicInvokeAmbiguous()
    {
      PrivateInvoke.InvokePublicMethod (_twm, "f", null);
    }

    [Test]
    [ExpectedException (typeof (MethodNotFoundException))]
    public void TestPublicInvokeMethodNotFound()
    {
      PrivateInvoke.InvokePublicMethod (_twm, "f", 1.0);
    }
	}
}

using System;
using System.Text;
using NUnit.Framework;

using Rubicon.Development.UnitTesting;

namespace Rubicon.Development.UnitTests.UnitTesting
{

public class PublicClass
{
  private class InternalClass: PublicClass
  {
    private string _s;

    public InternalClass (string s)
    {
      _s = s;
    }

    private InternalClass ()
    {
      _s = "private ctor";
    }

    protected override string f()
    {
      return _s;
    }

    public InternalClass (StringBuilder s)
    {
      _s = s.ToString();
    }

  }

  protected virtual string f ()
  {
    return "PublicClass";
  }
}

[TestFixture]
public class TestCreateInstance
{
  const string c_assemblyName = "UnitTestsTest";
  const string c_publicClassName = "Rubicon.UnitTests.Test.PublicClass";
  const string c_internalClassName = "Rubicon.UnitTests.Test.PublicClass+InternalClass";

  [Test]
  public void TestCreateInstances()
  {
    PublicClass internalInstance;

    internalInstance = (PublicClass) PrivateInvoke.CreateInstancePublicCtor (
        c_assemblyName, c_internalClassName, 
        "test 1");
    Assertion.AssertEquals ("test 1", PrivateInvoke.InvokeNonPublicMethod (internalInstance, "f"));

    internalInstance = (PublicClass) PrivateInvoke.CreateInstanceNonPublicCtor (
        c_assemblyName, c_internalClassName);
    Assertion.AssertEquals ("private ctor", PrivateInvoke.InvokeNonPublicMethod (internalInstance, "f"));

    PublicClass publicInstance = (PublicClass) PrivateInvoke.CreateInstancePublicCtor (
        c_assemblyName, c_publicClassName);
    Assertion.AssertEquals ("PublicClass", PrivateInvoke.InvokeNonPublicMethod (publicInstance, "f"));
  }

  [Test]
  [ExpectedException (typeof (AmbiguousMethodNameException))]
  public void TestCreateInstanceAmbiguous()
  {
    PublicClass internalInstance = (PublicClass) PrivateInvoke.CreateInstancePublicCtor (
        c_assemblyName, c_internalClassName, 
        null);
  }
}

}

using System;
using NUnit.Framework;

using Rubicon.UnitTests;

namespace Rubicon.UnitTests.UnitTests
{

public class TypeWithProperties
{
  private int _intField;
  public int IntProperty
  {
    get { return _intField; }
    set { _intField = value; }
  }

  private string _stringField;
  protected string StringProperty
  {
    get { return _stringField; }
    set { _stringField = value; }
  }

  private static int s_intField;
  public static int StaticIntProperty
  {
    get { return s_intField; }
    set { s_intField = value; }
  }

  private static string s_stringField;
  protected static string StaticStringProperty
  {
    get { return s_stringField; }
    set { s_stringField = value; }
  }
}

[TestFixture]
public class TestPropertyAccess
{
  [Test]
	public void TestInstanceProperties()
	{
    TypeWithProperties twp = new TypeWithProperties();

    PrivateInvoke.SetPublicProperty (twp, "IntProperty", 12);
    Assertion.AssertEquals (12, PrivateInvoke.GetPublicProperty (twp, "IntProperty"));

    PrivateInvoke.SetNonPublicProperty (twp, "StringProperty", "test 1");
    Assertion.AssertEquals ("test 1", PrivateInvoke.GetNonPublicProperty (twp, "StringProperty"));
	}

  [Test]
	public void TestStaticProperties()
	{
    PrivateInvoke.SetPublicStaticProperty (typeof (TypeWithProperties), "StaticIntProperty", 13);
    Assertion.AssertEquals (13, PrivateInvoke.GetPublicStaticProperty (typeof (TypeWithProperties), "StaticIntProperty"));

    PrivateInvoke.SetNonPublicStaticProperty (typeof (TypeWithProperties), "StaticStringProperty", "test 2");
    Assertion.AssertEquals ("test 2", PrivateInvoke.GetNonPublicStaticProperty (typeof (TypeWithProperties), "StaticStringProperty"));
	}
}

}

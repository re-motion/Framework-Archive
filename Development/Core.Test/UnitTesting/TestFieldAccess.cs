using System;
using NUnit.Framework;

using Rubicon.UnitTests;

namespace Rubicon.UnitTests.UnitTests
{

public class TypeWithFields
{
  public int IntField;
  private string _stringField = null;

  public static int StaticIntField;
  private static string s_stringField = null;

  private void DummyReferenceFieldsToSupressWarnings()
  {
    _stringField = s_stringField;
  }
}

[TestFixture]
public class TestFieldAccess
{
  [Test]
	public void TestInstanceFields()
	{
    TypeWithFields twp = new TypeWithFields();

    PrivateInvoke.SetPublicField (twp, "IntField", 21);
    Assertion.AssertEquals (21, PrivateInvoke.GetPublicField (twp, "IntField"));

    PrivateInvoke.SetNonPublicField (twp, "_stringField", "test 3");
    Assertion.AssertEquals ("test 3", PrivateInvoke.GetNonPublicField (twp, "_stringField"));
	}

  [Test]
	public void TestStaticFields()
	{
    PrivateInvoke.SetPublicStaticField (typeof (TypeWithFields), "StaticIntField", 22);
    Assertion.AssertEquals (22, PrivateInvoke.GetPublicStaticField (typeof (TypeWithFields), "StaticIntField"));

    PrivateInvoke.SetNonPublicStaticField (typeof (TypeWithFields), "s_stringField", "test 4");
    Assertion.AssertEquals ("test 4", PrivateInvoke.GetNonPublicStaticField (typeof (TypeWithFields), "s_stringField"));
	}
}

}

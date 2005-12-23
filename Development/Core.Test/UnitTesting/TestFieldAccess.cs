using System;
using NUnit.Framework;

using Rubicon.Development.UnitTesting;

namespace Rubicon.Development.UnitTests.UnitTesting
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
    Assert.AreEqual (21, PrivateInvoke.GetPublicField (twp, "IntField"));

    PrivateInvoke.SetNonPublicField (twp, "_stringField", "test 3");
    Assert.AreEqual ("test 3", PrivateInvoke.GetNonPublicField (twp, "_stringField"));
	}

  [Test]
	public void TestStaticFields()
	{
    PrivateInvoke.SetPublicStaticField (typeof (TypeWithFields), "StaticIntField", 22);
    Assert.AreEqual (22, PrivateInvoke.GetPublicStaticField (typeof (TypeWithFields), "StaticIntField"));

    PrivateInvoke.SetNonPublicStaticField (typeof (TypeWithFields), "s_stringField", "test 4");
    Assert.AreEqual ("test 4", PrivateInvoke.GetNonPublicStaticField (typeof (TypeWithFields), "s_stringField"));
	}
}

}

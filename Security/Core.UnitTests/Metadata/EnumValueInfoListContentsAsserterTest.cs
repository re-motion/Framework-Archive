using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{
  [TestFixture]
  public class EnumValueInfoListContentsAsserterTest
  {
    // types

    // static members

    // member fields

    private List<EnumValueInfo> _list;

    // construction and disposing

    public EnumValueInfoListContentsAsserterTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _list = new List<EnumValueInfo> ();
      _list.Add (new EnumValueInfo (1, "First"));
      _list.Add (new EnumValueInfo (2, "Second"));
    }

    [Test]
    public void AssertWithValidValue ()
    {
      EnumValueInfoListContentsAsserter asserter = new EnumValueInfoListContentsAsserter ("First", _list);
      Assert.IsTrue (asserter.Test ());
    }

    [Test]
    public void AssertWithListNull ()
    {
      EnumValueInfoListContentsAsserter asserter = new EnumValueInfoListContentsAsserter ("First", null);
      Assert.IsFalse (asserter.Test ());
    }

    [Test]
    public void AssertWithInvalidValue ()
    {
      EnumValueInfoListContentsAsserter asserter = new EnumValueInfoListContentsAsserter ("Other", _list);
      Assert.IsFalse (asserter.Test ());
    }

    [Test]
    public void GetMessage ()
    {
      EnumValueInfoListContentsAsserter asserter = new EnumValueInfoListContentsAsserter ("Expected", _list);
      Assert.AreEqual ("\r\n\texpected: <\"Expected\">\r\n\t but was: <<\"First\">,<\"Second\">>", asserter.Message);
    }

    [Test]
    public void GetMessageWithUserMessage ()
    {
      EnumValueInfoListContentsAsserter asserter = new EnumValueInfoListContentsAsserter ("Expected", _list, "Custom: {0}", "value");
      Assert.AreEqual ("Custom: value\r\n\texpected: <\"Expected\">\r\n\t but was: <<\"First\">,<\"Second\">>", asserter.Message);
    }
  }
}
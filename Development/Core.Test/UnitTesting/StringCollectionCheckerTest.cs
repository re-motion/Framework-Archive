using System;
using System.Collections.Specialized;

using NUnit.Framework;

using Rubicon.Development.UnitTesting;

namespace Rubicon.Development.UnitTests.UnitTesting
{

  [TestFixture]
  public class StringCollectionCheckerTest
  {
    // types

    // static members and constants

    // member fields

    private StringCollectionChecker _checker;

    // construction and disposing

    public StringCollectionCheckerTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _checker = new StringCollectionChecker ();
    }
    
    [Test]
    public void AreEqualWithNull ()
    {
      _checker.AreEqual (null, null);
    }
    
    [Test]
    public void AreEqualWithEmpty ()
    {
      StringCollection expected = new StringCollection ();
      StringCollection actual = new StringCollection ();

      _checker.AreEqual (expected, actual);
    }
    
    [Test]
    public void AreEqualWithValues ()
    {
      StringCollection expected = new StringCollection ();
      expected.Add ("First");

      StringCollection actual = new StringCollection ();
      actual.Add ("First");

      _checker.AreEqual (expected, actual);
    }
    
    [Test]
    public void AreEqualWithValuesNotEqual ()
    {
      StringCollection expected = new StringCollection ();
      expected.Add ("A");

      StringCollection actual = new StringCollection ();
      actual.Add ("B");

      try
      {
        _checker.AreEqual (expected, actual);
        Assert.Fail ();
      }
      catch (AssertionException e)
      {
        if (e.Message != "\r\n\tArray lengths are both 1.\r\n\tArrays differ at index 0.\r\n\t\r\n\tString lengths are both 1.\r\n\tStrings differ at index 0.\r\n\t\r\n\texpected:<\"A\">\r\n\t but was:<\"B\">\r\n\t-----------^\r\n\t")
          throw;
      }
    }
  }

}

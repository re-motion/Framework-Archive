using System;
using System.Globalization;
using NUnit.Framework;
using Rubicon.Data.NullableValueTypes;

namespace NullableValueTypesTest
{


[TestFixture]
public class FormattingTests
{
  [Test]
  public void TestNaInt32Formatting()
  {
    NaInt32 i10 = 10;
    Assertion.AssertEquals ("10", i10.ToString());
    Assertion.AssertEquals ("A", i10.ToString("X"));
    Assertion.AssertEquals ("A", i10.ToString("~X"));
    Assertion.AssertEquals ("null", NaInt32.Null.ToString());
    Assertion.AssertEquals ("", NaInt32.Null.ToString("~"));
    Assertion.AssertEquals ("null", string.Format ("{0}", NaInt32.Null));
    Assertion.AssertEquals ("", string.Format ("{0:~}", NaInt32.Null));
  }
	
  [Test]
  public void TestNaInt32Parsing()
  {
    NaInt32 i10 = 10;
    Assertion.AssertEquals (i10, NaInt32.Parse ("10"));
    Assertion.AssertEquals (i10, NaInt32.Parse ("A", NumberStyles.HexNumber));
    Assertion.AssertEquals (NaInt32.Null, NaInt32.Parse(null));
    Assertion.AssertEquals (NaInt32.Null, NaInt32.Parse(""));
    Assertion.AssertEquals (NaInt32.Null, NaInt32.Parse("null"));
  }
}


}

using System;
using System.Globalization;
using NUnit.Framework;
using Remotion.NullableValueTypes;

namespace Remotion.UnitTests.NullableValueTypes
{

[TestFixture]
public class FormattingTests
{
  [Test]
  public void TestNaInt32Formatting()
  {
    NaInt32 i10 = 10;
    Assert.AreEqual ("10", i10.ToString());
    Assert.AreEqual ("A", i10.ToString("X"));
    Assert.AreEqual ("A", i10.ToString("~X"));
    Assert.AreEqual ("null", NaInt32.Null.ToString());
    Assert.AreEqual ("", NaInt32.Null.ToString("~"));
    Assert.AreEqual ("null", string.Format ("{0}", NaInt32.Null));
    Assert.AreEqual ("", string.Format ("{0:~}", NaInt32.Null));
  }
	
  [Test]
  public void TestNaInt32Parsing()
  {
    NaInt32 i10 = 10;
    Assert.AreEqual (i10, NaInt32.Parse ("10"));
    Assert.AreEqual (i10, NaInt32.Parse ("A", NumberStyles.HexNumber));
    Assert.AreEqual (NaInt32.Null, NaInt32.Parse(null));
    Assert.AreEqual (NaInt32.Null, NaInt32.Parse(""));
    Assert.AreEqual (NaInt32.Null, NaInt32.Parse("null"));
  }
}


}

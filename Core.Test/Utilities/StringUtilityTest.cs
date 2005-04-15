using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{

[TestFixture]
public class StringUtilityTest
{
  [Test]
	public void NullToEmpty()
	{
    Assertion.AssertEquals (string.Empty, StringUtility.NullToEmpty (null));
    Assertion.AssertEquals ("1", StringUtility.NullToEmpty ("1"));
	}

  [Test]
  public void IsNullOrEmpty()
  {
    Assertion.AssertEquals (true, StringUtility.IsNullOrEmpty (null));
    Assertion.AssertEquals (true, StringUtility.IsNullOrEmpty (""));
    Assertion.AssertEquals (false, StringUtility.IsNullOrEmpty (" "));
  }

  [Test]
  public void AreEqual()
  {
    Assertion.AssertEquals (true, StringUtility.AreEqual ("test1", "test1", false));
    Assertion.AssertEquals (true, StringUtility.AreEqual ("test1", "test1", true));
    Assertion.AssertEquals (false, StringUtility.AreEqual ("test1", "TEST1", false));
    Assertion.AssertEquals (true, StringUtility.AreEqual ("test1", "TEST1", true));
    Assertion.AssertEquals (false, StringUtility.AreEqual ("täst1", "TÄST1", false));
    Assertion.AssertEquals (true, StringUtility.AreEqual ("täst1", "TÄST1", true));
  }
}

[TestFixture]
public class StringUtility_ParseSeparatedListTest
{
  [Test]
  public void TestParseSeparatedList()
  {
    // char doubleq = '\"';
    char singleq = '\'';
    char backsl = '\\';
    char comma = ',';
    string whitespace = " ";

    Check ("1", comma, singleq, singleq, backsl, whitespace, true,
           unquoted ("1"));
    Check ("1,2", comma, singleq, singleq, backsl, whitespace, true,
           unquoted ("1"), unquoted ("2"));
    Check ("'1', '2'", comma, singleq, singleq, backsl, whitespace, true,
           quoted ("1"), quoted ("2"));
    Check ("<1>, <2>", comma, '<', '>', backsl, whitespace, true,
           quoted ("1"), quoted ("2"));
    Check ("a='A', b='B'", comma, singleq, singleq, backsl, whitespace, true,
           unquoted ("a='A'"), unquoted ("b='B'"));
    Check ("a='A', b='B,B\\'B\\''", comma, singleq, singleq, backsl, whitespace, true,
           unquoted ("a='A'"), unquoted ("b='B,B'B''"));
    Check ("a b c = 'd,e' f 'g,h'", comma, singleq, singleq, backsl, whitespace, true,
           unquoted ("a b c = 'd,e' f 'g,h'"));
    Check ("a <a ,<a,> a, <b", comma, '<', '>', backsl, whitespace, true,
           unquoted ("a <a ,<a,> a"), quoted ("b"));
  }

  private StringUtility.ParsedItem quoted (string value)
  {
    return new StringUtility.ParsedItem (value, true);
  }

  private StringUtility.ParsedItem unquoted (string value)
  {
    return new StringUtility.ParsedItem (value, false);
  }

  private void Check (
      string value,
      char delimiter, char openingQuote, char closingQuote, char escapingChar, string whitespaceCharacters, 
      bool interpretSpecialCharacters,
      params StringUtility.ParsedItem[] expectedItems)
  {
    StringUtility.ParsedItem[] actualItems = StringUtility.ParseSeparatedList (
      value, delimiter, openingQuote, closingQuote, escapingChar, whitespaceCharacters, interpretSpecialCharacters);

    Assert.AreEqual (expectedItems.Length, actualItems.Length);
    for (int i = 0; i < expectedItems.Length; ++i)
    {
      Assert.AreEqual (expectedItems[i].Value, actualItems[i].Value, string.Format ("[{0}].Value", i));
      Assert.AreEqual (expectedItems[i].IsQuoted, actualItems[i].IsQuoted, string.Format ("[{0}].IsQuoted", i));
    }
  }
}

}

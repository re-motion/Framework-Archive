using System;
using System.Reflection;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{

[TestFixture]
public class StringUtilityTest
{
  private CultureInfo _cultureBackup;
  private CultureInfo _cultureEnUs;
  private CultureInfo _cultureDeAt;

  private Type _int32 = typeof (int);
  private Type _double = typeof (double);
  private Type _string = typeof (string);
  private Type _object = typeof (object);
  private Type _guid = typeof (Guid);

  [SetUp]
  public void SetUp()
  {
    _cultureEnUs = new CultureInfo ("en-US");
    _cultureDeAt = new CultureInfo ("de-AT");
    
    _cultureBackup = Thread.CurrentThread.CurrentCulture;
    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

    StringUtilityMock.ClearCache();
  }

  [TearDown]
  public void TearDown()
  {
    Thread.CurrentThread.CurrentCulture = _cultureBackup;
  }

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
  [Test]
  public void GetParseMethodFromCache()
  {
    MethodInfo parseMethod = StringUtilityMock.GetParseMethodFromType (_int32);
    StringUtilityMock.AddParseMethodToCache (_int32, parseMethod);
    Assert.AreSame (parseMethod, StringUtilityMock.GetParseMethodFromCache (_int32));
  }

  [Test]
  public void HasTypeInCache()
  {
    MethodInfo parseMethod = StringUtilityMock.GetParseMethodFromType (_int32);
    StringUtilityMock.AddParseMethodToCache (_int32, parseMethod);
    Assert.IsTrue (StringUtilityMock.HasTypeInCache (_int32));
  }

  [Test]
  public void HasParseMethodForInt32()
  {
    Assert.IsTrue (StringUtility.HasParseMethod (_int32));
  }

  [Test]
  public void GetParseMethodForInt32()
  {
    MethodInfo parseMethod = StringUtilityMock.GetParseMethod (_int32, true);
    Assert.IsNotNull (parseMethod);
    Assert.AreEqual ("Parse", parseMethod.Name);
    Assert.AreEqual (2, parseMethod.GetParameters().Length);
    Assert.AreEqual (typeof (string), parseMethod.GetParameters()[0].ParameterType);
    Assert.AreEqual (typeof (IFormatProvider), parseMethod.GetParameters()[1].ParameterType);
    Assert.AreEqual (typeof (int), parseMethod.ReturnType);
    Assert.IsTrue (parseMethod.IsPublic);
    Assert.IsTrue (parseMethod.IsStatic);
  }

  [Test]
  public void GetParseMethodFromTypeForInt32()
  {
    MethodInfo parseMethod = StringUtilityMock.GetParseMethodFromType (_int32);
    Assert.IsNotNull (parseMethod);
    Assert.AreEqual ("Parse", parseMethod.Name);
    Assert.AreEqual (1, parseMethod.GetParameters().Length);
    Assert.AreEqual (typeof (string), parseMethod.GetParameters()[0].ParameterType);
    Assert.AreEqual (typeof (int), parseMethod.ReturnType);
    Assert.IsTrue (parseMethod.IsPublic);
    Assert.IsTrue (parseMethod.IsStatic);
  }

  [Test]
  public void GetParseMethodWithFormatProviderFromTypeForInt32()
  {
    MethodInfo parseMethod = StringUtilityMock.GetParseMethodWithFormatProviderFromType (_int32);
    Assert.IsNotNull (parseMethod);
    Assert.AreEqual ("Parse", parseMethod.Name);
    Assert.AreEqual (2, parseMethod.GetParameters().Length);
    Assert.AreEqual (typeof (string), parseMethod.GetParameters()[0].ParameterType);
    Assert.AreEqual (typeof (IFormatProvider), parseMethod.GetParameters()[1].ParameterType);
    Assert.AreEqual (typeof (int), parseMethod.ReturnType);
    Assert.IsTrue (parseMethod.IsPublic);
    Assert.IsTrue (parseMethod.IsStatic);
  }

  [Test]
  public void HasParseMethodForObject()
  {
    Assert.IsFalse (StringUtility.HasParseMethod (_object));
  }
  
  [Test]
  [ExpectedException (typeof (ParseException))]
  public void GetParseMethodForObjectWithException()
  {
    StringUtilityMock.GetParseMethod (_object, true);
    Assert.Fail();
  }

  [Test]
  public void GetParseMethodForObjectWithoutException()
  {
    Assert.IsNull (StringUtilityMock.GetParseMethod (_object, false));
  }

  [Test]
  public void GetParseMethodFromTypeForObject()
  {
    Assert.IsNull (StringUtilityMock.GetParseMethodFromType (_object));
  }

  [Test]
  public void GetParseMethodWithFormatProviderFromTypeForObject()
  {
    Assert.IsNull (StringUtilityMock.GetParseMethodWithFormatProviderFromType (_object));
  }

  [Test]
  public void ParseScalarValueDoubleWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;
    object value = StringUtilityMock.ParseScalarValue (_double, "4,321.123", _cultureEnUs);
    Assert.IsNotNull (value);
    Assert.AreEqual (_double, value.GetType());
    Assert.AreEqual (4321.123, value);
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ParseScalarValueDoubleEnUsWithCultureDeAt()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    StringUtilityMock.ParseScalarValue (_double, "4,321.123", _cultureDeAt);
    Assert.Fail();
  }

  [Test]
  public void ParseScalarValueDoubleWithCultureDeAt()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    object value = StringUtilityMock.ParseScalarValue (_double, "4.321,123", _cultureDeAt);
    Assert.IsNotNull (value);
    Assert.AreEqual (_double, value.GetType());
    Assert.AreEqual (4321.123, value);
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ParseScalarValueDoubleDeAtWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;
    StringUtilityMock.ParseScalarValue (_double, "4.321,123", _cultureEnUs);
    Assert.Fail();
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

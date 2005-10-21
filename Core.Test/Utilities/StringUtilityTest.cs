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
  private Type _dbNull = typeof (DBNull);
  private Type _doubleArray = typeof (double[]);
  private Type _stringArray = typeof (string[]);

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
  public void CanParseInt32()
  {
    Assert.IsTrue (StringUtility.CanParse (_int32));
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
  public void CanParseObject()
  {
    Assert.IsFalse (StringUtility.CanParse (_object));
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
  public void ParseDoubleWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;
    object value = StringUtility.Parse (_double, "4,321.123", _cultureEnUs);
    Assert.IsNotNull (value);
    Assert.AreEqual (_double, value.GetType());
    Assert.AreEqual (4321.123, value);
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ParseDoubleEnUsWithCultureDeAt()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    StringUtility.Parse (_double, "4,321.123", _cultureDeAt);
    Assert.Fail();
  }

  [Test]
  public void ParseDoubleWithCultureDeAt()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    object value = StringUtility.Parse (_double, "4.321,123", _cultureDeAt);
    Assert.IsNotNull (value);
    Assert.AreEqual (_double, value.GetType());
    Assert.AreEqual (4321.123, value);
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ParseDoubleDeAtWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;
    StringUtility.Parse (_double, "4.321,123", _cultureEnUs);
    Assert.Fail();
  }

  [Test]
  [Ignore (@"Bug in ParseArrayItem: Escape Sequence '\,' does not work.")]
  public void ParseDoubleArrayWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;
    object value = StringUtility.Parse (_doubleArray, @"6\,543.123,5\,432.123,4\,321.123", _cultureEnUs);
    Assert.IsNotNull (value);
    Assert.AreEqual (_doubleArray, value.GetType());
    double[] values = (double[]) value;
    Assert.AreEqual (3, values.Length);
    Assert.AreEqual (6543.123, values[0]);
    Assert.AreEqual (5432.123, values[1]);
    Assert.AreEqual (4321.123, values[2]);
  }

  [Test]
  public void ParseDoubleArrayWithCultureEnUsNoThousands()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;
    object value = StringUtility.Parse (_doubleArray, @"6543.123,5432.123,4321.123", _cultureEnUs);
    Assert.IsNotNull (value);
    Assert.AreEqual (_doubleArray, value.GetType());
    double[] values = (double[]) value;
    Assert.AreEqual (3, values.Length);
    Assert.AreEqual (6543.123, values[0]);
    Assert.AreEqual (5432.123, values[1]);
    Assert.AreEqual (4321.123, values[2]);
  }

  [Test]
  [Ignore (@"Bug in ParseArrayItem: Escape Sequence '\,' does not work.")]
  public void ParseDoubleArrayWithCultureDeAt()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    object value = StringUtility.Parse (_doubleArray, @"6.543\,123,5.432\,123,4.321\,123", _cultureDeAt);
    Assert.IsNotNull (value);
    Assert.AreEqual (_doubleArray, value.GetType());
    double[] values = (double[]) value;
    Assert.AreEqual (3, values.Length);
    Assert.AreEqual (6543.123, values[0]);
    Assert.AreEqual (5432.123, values[1]);
    Assert.AreEqual (4321.123, values[2]);
  }

  [Test]
  public void ParseStringArray()
  {
    object value = StringUtility.Parse (_stringArray, "\"a\",\"b\",\"c\",\"d\"", null);
    Assert.IsNotNull (value);
    Assert.AreEqual (_stringArray, value.GetType());
    string[] values = (string[]) value;
    Assert.AreEqual (4, values.Length);
    Assert.AreEqual ("a", values[0]);
    Assert.AreEqual ("b", values[1]);
    Assert.AreEqual ("c", values[2]);
    Assert.AreEqual ("d", values[3]);
  }

  [Test]
  public void CanParseDoubleArray()
  {
    Assert.IsTrue (StringUtility.CanParse (_doubleArray));
  }

  [Test]
  public void CanParseArayDoubleArray()
  {
    Assert.IsFalse (StringUtility.CanParse (typeof (double[][])));
  }

  [Test]
  public void CanParseString()
  {
    Assert.IsTrue (StringUtility.CanParse (_string));
  }

  [Test]
  public void CanParseDBNull()
  {
    Assert.IsTrue (StringUtility.CanParse (_dbNull));
  }

  [Test]
  public void ParseDBNull()
  {
    object value = StringUtility.Parse (_dbNull, DBNull.Value.ToString(), null);
    Assert.IsNotNull (value);
    Assert.AreEqual (_dbNull, value.GetType());
    Assert.AreEqual (DBNull.Value, value);
  }

  [Test]
  public void CanParseGuid()
  {
    Assert.IsTrue (StringUtility.CanParse (_guid));
  }

  [Test]
  public void ParseGuid()
  {
    Guid guid = Guid.NewGuid();
    object value = StringUtility.Parse (_guid, guid.ToString(), null);
    Assert.IsNotNull (value);
    Assert.AreEqual (_guid, value.GetType());
    Assert.AreEqual (guid, value);
  }

  [Test]
  public void ParseEmptyGuid()
  {
    Guid guid = Guid.Empty;
    object value = StringUtility.Parse (_guid, guid.ToString(), null);
    Assert.IsNotNull (value);
    Assert.AreEqual (_guid, value.GetType());
    Assert.AreEqual (guid, value);
  }

  [Test]
  public void FormatNull()
  {
    Assert.AreEqual ("", StringUtility.Format (null, null));
  }

  [Test]
  public void FormatString()
  {
    string value = "Hello World!";
    Assert.AreEqual (value, StringUtility.Format (value, null));
  }

  [Test]
  public void FormatDoubleWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;
    Assert.AreEqual ("4321.123", StringUtility.Format (4321.123, _cultureEnUs));
  }

  [Test]
  public void FormatDoubleWithCultureDeAt()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;
    Assert.AreEqual ("4321,123", StringUtility.Format (4321.123, _cultureDeAt));
  }

  [Test]
  public void FormatGuid()
  {
    Guid guid = Guid.Empty;
    Assert.AreEqual (guid.ToString(), StringUtility.Format (guid, null));
  }

  [Test]
  public void FormatDoubleArrayWithCultureEnUsNoThousands()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;
    double[] values = new double[] {6543.123, 5432.123, 4321.123};
    Assert.AreEqual (@"6543.123,5432.123,4321.123", StringUtility.Format (values, _cultureEnUs));
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

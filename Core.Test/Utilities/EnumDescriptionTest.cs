using System;
using System.Threading;
using System.Globalization;
using Rubicon;
using NUnit.Framework;


namespace Rubicon.Core.UnitTests
{

public enum EnumWithDescriptions
{
  [EnumDescription("Value One")]
  Value1 = 1,
  [EnumDescription("Value 2")]
  Value2 = 2,
  [EnumDescription("Value III")]
  Value3 = 3
}

[EnumDescriptionResource("Rubicon.Core.UnitTests.Resources.strings")]
public enum EnumFromResource
{
  Value1 = 1,
  Value2 = 2,
  Value3 = 3
}

[TestFixture]
public class EnumDescriptionTest
{
  [SetUp]
  public void SetUp ()
  {
    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
  }

  [Test]
  public void TestGetDescriptionForEnumWithDescriptions()
  {
    // try twice to test caching
    for (int i = 0; i < 2; ++i)
    {
      Assertion.AssertEquals ("Value One", EnumDescription.GetDescription (EnumWithDescriptions.Value1));
      Assertion.AssertEquals ("Value 2", EnumDescription.GetDescription (EnumWithDescriptions.Value2));
      Assertion.AssertEquals ("Value III", EnumDescription.GetDescription (EnumWithDescriptions.Value3));
    }
  }

  [Test]
  public void TestGetAvailableValuesForEnumWithDescriptions()
  {
    // try twice to test caching
    for (int i = 0; i < 2; ++i)
    {
      EnumValue[] enumValues = EnumDescription.GetAllValues (typeof (EnumWithDescriptions));
      Assertion.AssertEquals (3, enumValues.Length);
      Assertion.AssertEquals (EnumWithDescriptions.Value1, enumValues[0].Value);
      Assertion.AssertEquals ("Value One",                 enumValues[0].Description);
      Assertion.AssertEquals (EnumWithDescriptions.Value2, enumValues[1].Value);
      Assertion.AssertEquals ("Value 2",                   enumValues[1].Description);
      Assertion.AssertEquals (EnumWithDescriptions.Value3, enumValues[2].Value);
      Assertion.AssertEquals ("Value III",                 enumValues[2].Description);
    }
  }

  [Test]
  [NUnit.Framework.Ignore ("resource linking not implemented in build scripts.")]
  public void TestGetDescriptionForEnumFromResource()
  {
    Assertion.AssertEquals ("Wert Eins", EnumDescription.GetDescription (EnumFromResource.Value1));
    Assertion.AssertEquals ("Wert 2", EnumDescription.GetDescription (EnumFromResource.Value2));
    Assertion.AssertEquals ("Wert III", EnumDescription.GetDescription (EnumFromResource.Value3));

    CultureInfo culture = new CultureInfo ("en-US");
    Assertion.AssertEquals ("Val1", EnumDescription.GetDescription (EnumFromResource.Value1, culture));
    Assertion.AssertEquals ("Val2", EnumDescription.GetDescription (EnumFromResource.Value2, culture));
    Assertion.AssertEquals ("Val3", EnumDescription.GetDescription (EnumFromResource.Value3, culture));
  }

  [Test]
  [NUnit.Framework.Ignore ("resource linking not implemented in build scripts.")]
  public void TestGetAvailableValuesForEnumFromResource ()
  {
    // try twice to test caching
    for (int i = 0; i < 2; ++i)
    {
      EnumValue[] enumValues = EnumDescription.GetAllValues (typeof (EnumFromResource));
      Assertion.AssertEquals (3, enumValues.Length);
      Assertion.AssertEquals (EnumFromResource.Value1, enumValues[0].Value);
      Assertion.AssertEquals ("Wert Eins",             enumValues[0].Description);
      Assertion.AssertEquals (EnumFromResource.Value2, enumValues[1].Value);
      Assertion.AssertEquals ("Wert 2",                enumValues[1].Description);
      Assertion.AssertEquals (EnumFromResource.Value3, enumValues[2].Value);
      Assertion.AssertEquals ("Wert III",              enumValues[2].Description);
    }
  }
}

}

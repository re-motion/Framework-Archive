using System;
using System.ComponentModel;
using NUnit.Framework;
using Rubicon.Utilities;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.Utilities
{

[TestFixture]
public class TypeConverterFactoryTest
{
  private TypeConverterFactory _factory;

  [SetUp]
  public void SetUp()
  {
    _factory = new TypeConverterFactory();
  }

  [Test]
  public void GetTypeConverter ()
  {
    Type from = typeof (NaInt32);
    Type to = typeof (NaInt32);
    TypeConverter converter = _factory.GetTypeConverter (from, to);

    Assert.IsNotNull (converter, "TypeConverter is null.");
  }

  [Test]
  public void GetTypeConverterByAttributeFromNaInt32()
  {
    Type type = typeof (NaInt32);
    TypeConverter converter = _factory.GetTypeConverterByAttribute (type);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterByAttributeFromInt32()
  {
    Type type = typeof (int);
    TypeConverter converter = _factory.GetTypeConverterByAttribute (type);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }
}

}

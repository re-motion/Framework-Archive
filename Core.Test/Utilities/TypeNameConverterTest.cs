using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{
  [TestFixture]
  public class TypeNameConverterTest
  {
    // types

    // static members

    // member fields

    TypeNameConverter _converter;

    // construction and disposing

    public TypeNameConverterTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _converter = new TypeNameConverter ();
    }

    [Test]
    public void CanConvertToString ()
    {
      Assert.IsTrue (_converter.CanConvertTo (typeof (string)));
    }

    [Test]
    public void CanConvertFromString ()
    {
      Assert.IsTrue (_converter.CanConvertFrom (typeof (string)));
    }

    [Test]
    public void ConvertToString ()
    {
      Type destinationType = typeof (string);

      Assert.AreEqual ("", _converter.ConvertTo (null, null, null, destinationType));
      Assert.AreEqual (
          "Rubicon.Core.UnitTests.Utilities.TypeNameConverterTest, Rubicon.Core.UnitTests", 
          (string) _converter.ConvertTo (null, null, typeof (TypeNameConverterTest), destinationType));
    }

    [Test]
    public void ConvertFromString ()
    {
      Assert.AreEqual (null, _converter.ConvertFrom (null, null, ""));
      Assert.AreEqual (
          typeof (TypeNameConverterTest),
          _converter.ConvertFrom (null, null, "Rubicon.Core.UnitTests.Utilities.TypeNameConverterTest, Rubicon.Core.UnitTests"));
      Assert.AreEqual (
          typeof (TypeNameConverterTest),
          _converter.ConvertFrom (null, null, "Rubicon.Core.UnitTests::Utilities.TypeNameConverterTest"));
    }
  }
}

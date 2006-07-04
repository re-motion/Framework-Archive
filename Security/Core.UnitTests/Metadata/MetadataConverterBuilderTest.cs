using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Security.Metadata;
using System.Globalization;

namespace Rubicon.Security.UnitTests.Metadata
{
  [TestFixture]
  public class MetadataConverterBuilderTest
  {
    private MetadataConverterBuilder _converterBuilder;

    [SetUp]
    public void SetUp ()
    {
      _converterBuilder = new MetadataConverterBuilder ();
    }

    [Test]
    public void Create_SimpleMetadataToXmlConverter ()
    {
      _converterBuilder.ConvertMetadataToXml = true;

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (MetadataToXmlConverter), converter);
    }

    [Test]
    public void Create_LocalizingConverterForOneLanguage ()
    {
      _converterBuilder.AddLocalization ("de");

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsNull (localizingConverter.MetadataConverter);
      Assert.AreEqual (1, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
    }

    [Test]
    public void Create_LocalizingConverterForLanguageWithWhitespaces ()
    {
      _converterBuilder.AddLocalization (" de ");

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsNull (localizingConverter.MetadataConverter);
      Assert.AreEqual (1, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
    }

    [Test]
    public void Create_LocalizingConverterForOneCultureInfo ()
    {
      _converterBuilder.AddLocalization (new CultureInfo("de"));

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsNull (localizingConverter.MetadataConverter);
      Assert.AreEqual (1, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
    }

    [Test]
    public void Create_LocalizingConverterForTwoLanguages ()
    {
      _converterBuilder.AddLocalization ("de");
      _converterBuilder.AddLocalization ("fr");

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsNull (localizingConverter.MetadataConverter);
      Assert.AreEqual (2, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
      Assert.AreEqual ("fr", localizingConverter.Cultures[1].TwoLetterISOLanguageName);
    }

    [Test]
    public void Create_LocalizingConverterWithMetadataToXmlConverter ()
    {
      _converterBuilder.AddLocalization ("de");
      _converterBuilder.AddLocalization ("fr");
      _converterBuilder.ConvertMetadataToXml = true;

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsInstanceOfType (typeof (MetadataToXmlConverter), localizingConverter.MetadataConverter);
      Assert.AreEqual (2, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
      Assert.AreEqual ("fr", localizingConverter.Cultures[1].TwoLetterISOLanguageName);
    }

    [Test]
    public void Create_LocalizingConverterWithInvariantCulture ()
    {
      _converterBuilder.AddLocalization ("de");
      _converterBuilder.AddLocalization (CultureInfo.InvariantCulture);

      IMetadataConverter converter = _converterBuilder.Create ();

      Assert.IsInstanceOfType (typeof (LocalizingMetadataConverter), converter);
      LocalizingMetadataConverter localizingConverter = (LocalizingMetadataConverter) converter;
      Assert.IsNull (localizingConverter.MetadataConverter);
      Assert.AreEqual (2, localizingConverter.Cultures.Length);
      Assert.AreEqual ("de", localizingConverter.Cultures[0].TwoLetterISOLanguageName);
      Assert.AreEqual (CultureInfo.InvariantCulture.TwoLetterISOLanguageName, localizingConverter.Cultures[1].TwoLetterISOLanguageName);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "You must specify at least a localization or a metadata converter.")]
    public void Create_ExceptionWithoutLocalizationAndMetadataConverter ()
    {
      _converterBuilder.Create ();
    }
  }
}

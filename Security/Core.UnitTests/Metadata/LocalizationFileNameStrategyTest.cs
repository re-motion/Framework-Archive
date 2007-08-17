using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Core.Metadata
{
  [TestFixture]
  public class LocalizationFileNameStrategyTest
  {
    [Test]
    public void GetLocalizationFileNames_NoLocalizationFiles ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Metadata\LocalizationFiles\notexisting.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.IsNotNull (localizationFileNames);
      Assert.AreEqual (0, localizationFileNames.Length);
    }

    [Test]
    public void GetLocalizationFileNames_OneLocalizationFile ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Metadata\LocalizationFiles\OneLocalizationFile.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.IsNotNull (localizationFileNames);
      Assert.AreEqual (1, localizationFileNames.Length);
      Assert.Contains (@"Metadata\LocalizationFiles\OneLocalizationFile.Localization.de.xml", localizationFileNames);
    }

    [Test]
    public void GetLocalizationFileNames_TwoLocalizationFiles ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Metadata\LocalizationFiles\TwoLocalizationFiles.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.IsNotNull (localizationFileNames);
      Assert.AreEqual (2, localizationFileNames.Length);
      Assert.Contains (@"Metadata\LocalizationFiles\TwoLocalizationFiles.Localization.de.xml", localizationFileNames);
      Assert.Contains (@"Metadata\LocalizationFiles\TwoLocalizationFiles.Localization.en.xml", localizationFileNames);
    }

    [Test]
    public void GetLocalizationFileNames_TwoLocalizationFilesIncludingInvariantCulture ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string metadataFileName = @"Metadata\LocalizationFiles\TwoLocalizationFilesIncludingInvariantCulture.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Assert.IsNotNull (localizationFileNames);
      Assert.AreEqual (2, localizationFileNames.Length);
      Assert.Contains (@"Metadata\LocalizationFiles\TwoLocalizationFilesIncludingInvariantCulture.Localization.de.xml", localizationFileNames);
      Assert.Contains (@"Metadata\LocalizationFiles\TwoLocalizationFilesIncludingInvariantCulture.Localization.xml", localizationFileNames);
    }

    [Test]
    public void GetLocalizationFileNames_WithoutBaseDirectory ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string wd = Directory.GetCurrentDirectory ();
      Directory.SetCurrentDirectory (@"Metadata\LocalizationFiles");
      string metadataFileName = "TwoLocalizationFilesIncludingInvariantCulture.xml";

      string[] localizationFileNames = nameStrategy.GetLocalizationFileNames (metadataFileName);

      Directory.SetCurrentDirectory (wd);

      Assert.IsNotNull (localizationFileNames);
      Assert.AreEqual (2, localizationFileNames.Length);
      Assert.Contains (@".\TwoLocalizationFilesIncludingInvariantCulture.Localization.de.xml", localizationFileNames);
      Assert.Contains (@".\TwoLocalizationFilesIncludingInvariantCulture.Localization.xml", localizationFileNames);
    }

    [Test]
    public void GetLocalizationFileName_GermanLanguage ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string filename = "metadata.xml";

      string localizationFilename = nameStrategy.GetLocalizationFileName (filename, new CultureInfo ("de"));

      Assert.AreEqual ("metadata.Localization.de.xml", localizationFilename);
    }

    [Test]
    public void GetLocalizationFileName_InvariantCulture ()
    {
      LocalizationFileNameStrategy nameStrategy = new LocalizationFileNameStrategy ();
      string filename = "metadata.xml";

      string localizationFilename = nameStrategy.GetLocalizationFileName (filename, CultureInfo.InvariantCulture);

      Assert.AreEqual ("metadata.Localization.xml", localizationFilename);
    }
  }
}

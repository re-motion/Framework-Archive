using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;

namespace Rubicon.Security.Metadata
{
  public class LocalizationFileNameStrategy
  {
    public string GetLocalizationFileName (string metadataFilename, CultureInfo culture)
    {
      string baseFilename = Path.GetFileNameWithoutExtension (metadataFilename);
      string basePath = Path.GetDirectoryName (metadataFilename);
      string baseFilePath = Path.Combine (basePath, baseFilename);

      if (string.IsNullOrEmpty (culture.Name))
        return baseFilePath + ".Localization.xml";

      return baseFilePath + ".Localization." + culture.Name + ".xml";
    }

    public string[] GetLocalizationFileNames (string metadataFilename)
    {
      string baseFileName = Path.GetFileNameWithoutExtension (metadataFilename);
      string basePath = Path.GetDirectoryName (metadataFilename);
      string searchPattern = baseFileName + ".Localization.*xml";

      return Directory.GetFiles (basePath, searchPattern, SearchOption.TopDirectoryOnly);
    }
  }
}

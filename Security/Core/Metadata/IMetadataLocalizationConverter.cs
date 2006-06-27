using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Rubicon.Security.Metadata
{
  public interface IMetadataLocalizationConverter
  {
    void ConvertAndSave (LocalizedName[] localizedNames, CultureInfo culture, string filename);
  }
}

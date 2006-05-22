using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.Metadata
{
  public interface IMetadataConverter
  {
    void ConvertAndSave (MetadataCache cache, string filename);
  }
}

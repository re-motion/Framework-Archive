using System;

namespace Rubicon.Security.Metadata
{
  public interface IMetadataConverter
  {
    void ConvertAndSave (MetadataCache cache, string filename);
  }
}

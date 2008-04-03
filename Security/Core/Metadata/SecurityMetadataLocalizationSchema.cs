using System;
using Remotion.Xml;

namespace Remotion.Security.Metadata
{
  public class SecurityMetadataLocalizationSchema : SchemaLoaderBase
  {
    protected override string SchemaFile
    {
      get { return "SecurityMetadataLocalization.xsd"; }
    }

    public override string SchemaUri
    {
      get { return "http://www.rubicon-it.com/Security/Metadata/Localization/1.0"; }
    }
  }
}

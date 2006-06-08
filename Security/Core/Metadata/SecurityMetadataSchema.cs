using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Xml;

namespace Rubicon.Security.Metadata
{
  public class SecurityMetadataSchema : SchemaBase
  {
    protected override string SchemaFile
    {
      get { return "SecurityMetadata.xsd"; }
    }

    public override string SchemaUri
    {
      get { return "http://www.rubicon-it.com/Security/Metadata/1.0"; }
    }
  }
}

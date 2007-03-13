using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Configuration
{
  public class DomainObjectsConfiguration: ConfigurationSectionGroup
  {
    protected const string StoragePropertyName = "storage";

    public DomainObjectsConfiguration()
    {
    }

    [ConfigurationProperty (StoragePropertyName)]
    public PersistenceConfiguration Storage
    {
      get { return (PersistenceConfiguration) Sections[StoragePropertyName]; }
    }
  }
}
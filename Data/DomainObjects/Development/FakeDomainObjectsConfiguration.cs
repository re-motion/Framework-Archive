using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using System.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Development
{
  public class FakeDomainObjectsConfiguration: IDomainObjectsConfiguration
  {
    private PersistenceConfiguration _storage;
    
    public FakeDomainObjectsConfiguration (PersistenceConfiguration storage)
    {
      ArgumentUtility.CheckNotNull ("storage", storage);

      _storage = storage;
    }

    public PersistenceConfiguration Storage
    {
      get { return _storage; }
    }
  }
}
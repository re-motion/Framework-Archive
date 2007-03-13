using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.ConfigurationSection
{
  public class StubDomainObjectsConfiguration: DomainObjectsConfiguration
  {
    public StubDomainObjectsConfiguration (PersistenceConfiguration storage)
    {
      ArgumentUtility.CheckNotNull ("storage", storage);

    }
  }
}
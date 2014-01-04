// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Data.SqlClient;
using Remotion.Security;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private ServiceLocatorScope _serviceLocatorScope;
    
    public static string TestDomainConnectionString
    {
      get { return string.Format ("Integrated Security=SSPI;Initial Catalog=RemotionDataDomainObjectsObjectBindingTestDomain;Data Source={0}", DatabaseConfiguration.DataSource); }
    }

    public static string MasterConnectionString
    {
      get { return string.Format ("Integrated Security=SSPI;Initial Catalog=master;Data Source={0}", DatabaseConfiguration.DataSource); }
    }

    [SetUp]
    public virtual void SetUp ()
    {
      var serviceLocator = new DefaultServiceLocator();
      serviceLocator.Register (typeof (IObjectSecurityAdapter));
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);

      try
      {
        var providers = new ProviderCollection<StorageProviderDefinition>();
        providers.Add (new RdbmsProviderDefinition ("TheStorageProvider", new SqlStorageObjectFactory(), TestDomainConnectionString));
        var storageConfiguration = new StorageConfiguration (providers, providers["TheStorageProvider"]);

        DomainObjectsConfiguration.SetCurrent (
            new FakeDomainObjectsConfiguration (new MappingLoaderConfiguration(), storageConfiguration, new QueryConfiguration()));

        SqlConnection.ClearAllPools();

        var scriptGenerator = new ScriptGenerator (
            pd => pd.Factory.CreateSchemaScriptBuilder (pd),
            new RdbmsStorageEntityDefinitionProvider(),
            new ScriptToStringConverter());
        var scripts = scriptGenerator.GetScripts (MappingConfiguration.Current.GetTypeDefinitions()).Single();

        var masterAgent = new DatabaseAgent (MasterConnectionString);
        masterAgent.ExecuteBatchFile ("Database\\CreateDB.sql", false, DatabaseConfiguration.GetReplacementDictionary());

        var databaseAgent = new DatabaseAgent (TestDomainConnectionString);
        databaseAgent.ExecuteBatchString (scripts.SetUpScript, true);
      }
      catch (Exception e)
      {
        Console.WriteLine (e);
      }
    }

    [TearDown]
    public virtual void TearDown ()
    {
      _serviceLocatorScope.Dispose();
      SqlConnection.ClearAllPools();
    }
  }
}
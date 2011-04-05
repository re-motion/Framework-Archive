// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class MappingReflectionTestBase
  {
    private ReflectionBasedMappingObjectFactory _mappingObjectFactory;
    public const string DefaultStorageProviderID = "DefaultStorageProvider";
    public const string TestDomainProviderID = "TestDomain";
    public const string c_testDomainProviderID = "TestDomain";
    public const string c_unitTestStorageProviderStubID = "UnitTestStorageProviderStub";

    [SetUp]
    public virtual void SetUp ()
    {
      DomainObjectsConfiguration.SetCurrent (TestMappingConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent (TestMappingConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent (null);

      _mappingObjectFactory = new ReflectionBasedMappingObjectFactory (Configuration.NameResolver);
    }

    [TearDown]
    public virtual void TearDown ()
    {
    }

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp ()
    {
      DomainObjectsConfiguration.SetCurrent (TestMappingConfiguration.Instance.GetDomainObjectsConfiguration());
      MappingConfiguration.SetCurrent (TestMappingConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent (null);
      FakeMappingConfiguration.Reset ();
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return TestMappingConfiguration.Instance.GetDomainObjectIDs(); }
    }

    protected MappingConfiguration Configuration
    {
      get { return MappingConfiguration.Current; }
    }

    protected StorageProviderDefinition TestDomainStorageProviderDefinition
    {
      get { return DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_testDomainProviderID]; }
    }

    protected StorageProviderDefinition UnitTestDomainStorageProviderDefinition
    {
      get { return DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[DatabaseTest.c_unitTestStorageProviderStubID]; }
    }

    protected ReflectionBasedMappingObjectFactory MappingObjectFactory
    {
      get { return _mappingObjectFactory; }
    }

    protected IEnumerable<PropertyInfo> GetRelationPropertyInfos (ClassDefinition classDefinition)
    {
      var relationPropertyFinder = new RelationPropertyFinder (
          classDefinition.ClassType,
          classDefinition.BaseClass == null,
          true,
          Configuration.NameResolver,
          classDefinition.PersistentMixinFinder);
      return relationPropertyFinder.FindPropertyInfos ();
    }
  }
}
using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Mapping;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests
{
  public class MappingBaseTest 
  {
    // types

    // static members and constants

    // member fields

    private StorageProviderConfiguration _storageProviderConfiguration;
    private MappingConfiguration _mappingConfiguration;

    // construction and disposing

    public MappingBaseTest ()
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public virtual void TextFixtureSetUp ()
    {
    }

    [SetUp]
    public virtual void SetUp ()
    {
      _storageProviderConfiguration = new StorageProviderConfiguration ("storageProviders.xml", "storageProviders.xsd");
      _mappingConfiguration = new MappingConfiguration ("mapping.xml", "mapping.xsd", false);
    }

    [TearDown]
    public virtual void TearDown ()
    {
    }

    protected StorageProviderConfiguration StorageProviderConfiguration
    {
      get { return _storageProviderConfiguration; }
    }

    protected MappingConfiguration MappingConfiguration
    {
      get { return _mappingConfiguration; }
    }
  }
}

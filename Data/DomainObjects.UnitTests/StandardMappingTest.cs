using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  public class StandardMappingTest
  {
    // types

    // static members and constants

    private static readonly MappingConfiguration s_mappingConfiguration = new MappingConfiguration (@"mapping.xml", @"mapping.xsd");

    // member fields

    private DomainObjectIDs _domainObjectIDs;

    // construction and disposing

    protected StandardMappingTest ()
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      MappingConfiguration.SetCurrent (s_mappingConfiguration);
      _domainObjectIDs = new DomainObjectIDs ();
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return _domainObjectIDs; }
    }
  }
}

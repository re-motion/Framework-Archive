using System;
using System.IO;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class MappingConfigurationTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public MappingConfigurationTest ()
  {
  }

  // methods and properties

  [Test]
  public void InitializeWithFileNames ()
  {
    MappingConfiguration.SetCurrent (new MappingConfiguration (@"..\..\mapping.xml", @"..\..\..\mapping.xsd"));

    string configurationFile = Path.GetFullPath (@"..\..\mapping.xml");
    string schemaFile = Path.GetFullPath (@"..\..\..\mapping.xsd");

    Assert.AreEqual (configurationFile, MappingConfiguration.Current.ConfigurationFile);
    Assert.AreEqual (schemaFile, MappingConfiguration.Current.SchemaFile);
  }
}
}

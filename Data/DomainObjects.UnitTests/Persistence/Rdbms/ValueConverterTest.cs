using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class DBValueConverterTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public DBValueConverterTest ()
  {
  }

  // methods and properties

  [Test]
  [ExpectedException (typeof (ArgumentException), "DBValueConverter does not support ObjectID values of type 'System.Int32'.\r\nParameter name: value")]
  public void GetObjectIDWithValueOfInvalidType ()
  {
    DBValueConverter converter = new DBValueConverter ();
    converter.GetObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], 1);
  }
}
}

using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
[TestFixture]
public class ValueConverterTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ValueConverterTest ()
  {
  }

  // methods and properties

  [Test]
  [ExpectedException (typeof (ArgumentException), "ValueConverter does not support ObjectID values of type 'System.Int32'.\r\nParameter name: value")]
  public void GetObjectIDWithValueOfInvalidType ()
  {
    ValueConverter converter = new ValueConverter ();
    converter.GetObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], 1);
  }
}
}

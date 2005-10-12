using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
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
  public void GetObjectIDWithGuidValue ()
  {
    ValueConverter converter = new ValueConverter ();

    Guid value = Guid.NewGuid ();
    ObjectID expectedID = new ObjectID ("Order", value);
    ObjectID actualID = converter.GetObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], value);
    
    Assert.AreEqual (expectedID, actualID);
  }

  [Test]
  [ExpectedException (typeof (ConverterException), "Invalid null value for not-nullable property 'CustomerType' encountered, class 'Customer'.")]
  public void GetNullValueForEnum ()
  {
    ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
    PropertyDefinition enumProperty = customerDefinition["CustomerType"];

    ValueConverter valueConverter = new ValueConverter ();
    valueConverter.GetValue (customerDefinition, enumProperty, DBNull.Value);
  }
}
}

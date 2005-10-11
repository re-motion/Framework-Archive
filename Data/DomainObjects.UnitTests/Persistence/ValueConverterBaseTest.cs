using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class ValueConverterBaseTest
{
  // types

  // static members and constants

  // member fields

  private ValueConverterBaseMock _converterMock;

  // construction and disposing

  public ValueConverterBaseTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _converterMock = new ValueConverterBaseMock ();
  }

  [Test]
  [ExpectedException (typeof (ConverterException), "Property 'OrderNumber' of class 'Order' has no relations assigned.")]
  public void GetOppositeClassDefinitionOverNonRelationalProperty ()
  {
    ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Order");
    PropertyDefinition nonRelationalProperty = orderDefinition["OrderNumber"];

    _converterMock.GetOppositeClassDefinition (orderDefinition, nonRelationalProperty);
  }

  [Test]
  [ExpectedException (typeof (ConverterException), 
      "Enumeration 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer+CustomerType'"
      + " does not define the value 'InvalidEnumValue', property 'CustomerType'.")]
  public void GetInvalidEnumValue ()
  {
    ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
    PropertyDefinition enumProperty = customerDefinition["CustomerType"];

    _converterMock.GetEnumValue (enumProperty, "InvalidEnumValue");    
  }

  [Test]
  [ExpectedException (typeof (ConverterException), "Invalid null value for not-nullable property 'CustomerType' encountered, class 'Customer'.")]
  public void GetNullValueForEnum ()
  {
    ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
    PropertyDefinition enumProperty = customerDefinition["CustomerType"];

    _converterMock.GetValue (customerDefinition, enumProperty, null);
  }

  [Test]
  public void GetNullValueForNaDateTime ()
  {
    ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
    PropertyDefinition dateTimeProperty = customerDefinition["CustomerSince"];

    Assert.AreEqual (NaDateTime.Null, _converterMock.GetValue (customerDefinition, dateTimeProperty, null));
  }

  [Test]
  public void GetObjectIDWithInt32Value ()
  {
    ObjectID expectedID = new ObjectID ("Official", 1);
    ObjectID actualID = _converterMock.GetObjectID (MappingConfiguration.Current.ClassDefinitions["Official"], 1);
    
    Assert.AreEqual (expectedID, actualID);
  }

  [Test]
  public void GetObjectIDWithStringValue ()
  {
    ObjectID expectedID = new ObjectID ("Official", "StringValue");
    ObjectID actualID = _converterMock.GetObjectID (MappingConfiguration.Current.ClassDefinitions["Official"], "StringValue");
    
    Assert.AreEqual (expectedID, actualID);
  }

  [Test]
  public void GetObjectIDWithGuidValue ()
  {
    Guid value = Guid.NewGuid ();
    ObjectID expectedID = new ObjectID ("Order", value);
    ObjectID actualID = _converterMock.GetObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], value);
    
    Assert.AreEqual (expectedID, actualID);
  }
}
}

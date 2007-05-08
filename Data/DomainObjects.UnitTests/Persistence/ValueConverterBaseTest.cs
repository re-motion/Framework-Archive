using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
  [TestFixture]
  public class ValueConverterBaseTest: ReflectionBasedMappingTest
  {
    private StubValueConverterBase _stubValueConverterBase;
    private TypeConversionProvider _typeConversionProvider;

    public override void SetUp()
    {
      base.SetUp();

      _typeConversionProvider = TypeConversionProvider.Create ();
      _stubValueConverterBase = new StubValueConverterBase(_typeConversionProvider);
    }

    [Test]
    public void Initialize()
    {
      Assert.That (_typeConversionProvider, Is.SameAs (_typeConversionProvider));
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage = 
        "Enumeration 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer+CustomerType'"
        + " does not define the value 'InvalidEnumValue', property 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Type'.")]
    public void GetInvalidEnumValue()
    {
      ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
      PropertyDefinition enumProperty = customerDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Type"];

      _stubValueConverterBase.GetEnumValue (enumProperty, "InvalidEnumValue");
    }

    [Test]
    [ExpectedException (typeof (ConverterException), ExpectedMessage = 
        "Invalid null value for not-nullable property 'Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Type' encountered. "
        + "Class: 'Customer'.")]
    public void GetNullValueForEnum()
    {
      ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
      PropertyDefinition enumProperty = customerDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Type"];

      _stubValueConverterBase.GetValue (customerDefinition, enumProperty, null);
    }

    [Test]
    public void GetNullValueForNaDateTime()
    {
      ClassDefinition customerDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Customer");
      PropertyDefinition dateTimeProperty = customerDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.CustomerSince"];

      Assert.IsNull (_stubValueConverterBase.GetValue (customerDefinition, dateTimeProperty, null));
    }

    [Test]
    public void GetObjectIDWithInt32Value()
    {
      ObjectID expectedID = new ObjectID ("Official", 1);
      ObjectID actualID = _stubValueConverterBase.GetObjectID (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Official"), 1);

      Assert.AreEqual (expectedID, actualID);
    }

    [Test]
    public void GetObjectIDWithStringValue()
    {
      ObjectID expectedID = new ObjectID ("Official", "StringValue");
      ObjectID actualID = _stubValueConverterBase.GetObjectID (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Official"), "StringValue");

      Assert.AreEqual (expectedID, actualID);
    }

    [Test]
    public void GetObjectIDWithGuidValue()
    {
      Guid value = Guid.NewGuid();
      ObjectID expectedID = new ObjectID ("Order", value);
      ObjectID actualID = _stubValueConverterBase.GetObjectID (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Order"), value);

      Assert.AreEqual (expectedID, actualID);
    }

    [Test]
    public void GetValue_ForString ()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"],
              "abcdeföäü"),
          Is.EqualTo ("abcdeföäü"));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"],
              string.Empty),
          Is.Empty);

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
              "abcdeföäü"),
          Is.EqualTo ("abcdeföäü"));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
              null),
          Is.Null);

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"],
              string.Empty),
          Is.Empty);
    }

    [Test]
    public void GetValue_ForEnums ()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"],
              ClassWithAllDataTypes.EnumType.Value1),
          Is.EqualTo (ClassWithAllDataTypes.EnumType.Value1));
    }

    [Test]
    public void GetValue_ForInt32_WithObject ()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"],
              2147483647),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              2147483647),
          Is.EqualTo (2147483647));

      Assert.That (
          _stubValueConverterBase.GetValue (
              classWithAllDataTypesDefinition,
              classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
              null),
          Is.Null);
    }

    [Test]
    public void GetValue_ForInt32_WithString ()
    {
      ClassDefinition classWithAllDataTypesDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (ClassWithAllDataTypes));
      
      Assert.That (
        _stubValueConverterBase.GetValue (
            classWithAllDataTypesDefinition,
            classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"],
            "2147483647"),
        Is.EqualTo (2147483647));

      Assert.That (
        _stubValueConverterBase.GetValue (
            classWithAllDataTypesDefinition,
            classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
            "2147483647"),
        Is.EqualTo (2147483647));

      Assert.That (
        _stubValueConverterBase.GetValue (
            classWithAllDataTypesDefinition,
            classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
            string.Empty),
        Is.Null);

      Assert.That (
        _stubValueConverterBase.GetValue (
            classWithAllDataTypesDefinition,
            classWithAllDataTypesDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"],
            null),
        Is.Null);
    }
  }
}
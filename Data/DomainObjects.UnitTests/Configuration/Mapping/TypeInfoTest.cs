using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class TypeInfoTest
{
  // types

  private enum TypeMappingTestEnum
  {
    Value0 = 0,
    Value1 = 1
  }

  // static members and constants

  // member fields

  // construction and disposing

  public TypeInfoTest ()
  {
  }

  // methods and properties

  [Test]
  public void MappingTypes ()
  {
    // TODO: Test new nullable types too.

    Check (new TypeInfo (typeof (bool), "boolean", false, false), TypeInfo.GetInstance ("boolean", false));
    Check (new TypeInfo (typeof (byte), "byte", false, (byte) 0), TypeInfo.GetInstance ("byte", false));
    Check (new TypeInfo (typeof (DateTime), "dateTime", false, DateTime.MinValue), TypeInfo.GetInstance ("dateTime", false));
    Check (new TypeInfo (typeof (DateTime), "date", false, DateTime.MinValue), TypeInfo.GetInstance ("date", false));
    Check (new TypeInfo (typeof (decimal), "decimal", false, (decimal) 0), TypeInfo.GetInstance ("decimal", false));
    Check (new TypeInfo (typeof (double), "double", false, (double) 0), TypeInfo.GetInstance ("double", false));
    Check (new TypeInfo (typeof (Guid), "guid", false, Guid.Empty), TypeInfo.GetInstance ("guid", false));
    Check (new TypeInfo (typeof (short), "int16", false, (short) 0), TypeInfo.GetInstance ("int16", false));
    Check (new TypeInfo (typeof (int), "int32", false, (int) 0), TypeInfo.GetInstance ("int32", false));
    Check (new TypeInfo (typeof (long), "int64", false, (long) 0), TypeInfo.GetInstance ("int64", false));
    Check (new TypeInfo (typeof (float), "single", false, (float) 0), TypeInfo.GetInstance ("single", false));
    Check (new TypeInfo (typeof (string), "string", false, string.Empty), TypeInfo.GetInstance ("string", false));
    Check (new TypeInfo (typeof (ObjectID), "objectID", false, null), TypeInfo.GetInstance ("objectID", false));

    Check (new TypeInfo (typeof (NaBoolean), "boolean", true, NaBoolean.Null), TypeInfo.GetInstance ("boolean", true));
    Check (new TypeInfo (typeof (NaDateTime), "dateTime", true, NaDateTime.Null), TypeInfo.GetInstance ("dateTime", true));
    Check (new TypeInfo (typeof (NaDateTime), "date", true, NaDateTime.Null), TypeInfo.GetInstance ("date", true));
    Check (new TypeInfo (typeof (NaInt32), "int32", true, NaInt32.Null), TypeInfo.GetInstance ("int32", true));
    Check (new TypeInfo (typeof (NaDouble), "double", true, NaDouble.Null), TypeInfo.GetInstance ("double", true));
    Check (new TypeInfo (typeof (string), "string", true, null), TypeInfo.GetInstance ("string", true));
    Check (new TypeInfo (typeof (ObjectID), "objectID", true, null), TypeInfo.GetInstance ("objectID", true));
  }


  [Test]
  public void Types ()
  {
    // TODO: Test new nullable types too.

    Check (new TypeInfo (typeof (bool), "boolean", false, false), TypeInfo.GetInstance (typeof (bool)));
    Check (new TypeInfo (typeof (byte), "byte", false, (byte) 0), TypeInfo.GetInstance (typeof (byte)));
    Check (new TypeInfo (typeof (DateTime), "dateTime", false, DateTime.MinValue), TypeInfo.GetInstance (typeof (DateTime)));
    Check (new TypeInfo (typeof (decimal), "decimal", false, (decimal) 0), TypeInfo.GetInstance (typeof (decimal)));
    Check (new TypeInfo (typeof (double), "double", false, (double) 0), TypeInfo.GetInstance (typeof (double)));
    Check (new TypeInfo (typeof (Guid), "guid", false, Guid.Empty), TypeInfo.GetInstance (typeof (Guid)));
    Check (new TypeInfo (typeof (short), "int16", false, (short) 0), TypeInfo.GetInstance (typeof (short)));
    Check (new TypeInfo (typeof (int), "int32", false, (int) 0), TypeInfo.GetInstance (typeof (int)));
    Check (new TypeInfo (typeof (long), "int64", false, (long) 0), TypeInfo.GetInstance (typeof (long)));
    Check (new TypeInfo (typeof (float), "single", false, (float) 0), TypeInfo.GetInstance (typeof (float)));

    Check (new TypeInfo (typeof (NaBoolean), "boolean", true, NaBoolean.Null), TypeInfo.GetInstance (typeof (NaBoolean)));
    Check (new TypeInfo (typeof (NaDateTime), "dateTime", true, NaDateTime.Null), TypeInfo.GetInstance (typeof (NaDateTime)));
    Check (new TypeInfo (typeof (NaInt32), "int32", true, NaInt32.Null), TypeInfo.GetInstance (typeof (NaInt32)));
    Check (new TypeInfo (typeof (NaDouble), "double", true, NaDouble.Null), TypeInfo.GetInstance (typeof (NaDouble)));
    Check (new TypeInfo (typeof (string), "string", true, null), TypeInfo.GetInstance (typeof (string)));
    Check (new TypeInfo (typeof (ObjectID), "objectID", true, null), TypeInfo.GetInstance (typeof (ObjectID)));
  }

  [Test]
  public void UnknownMappingType ()
  {
    Assert.IsNull (TypeInfo.GetInstance (
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TypeMappingTest+TypeMappingTestEnum, Rubicon.Data.DomainObjects.UnitTests", false));
  }

  [Test]
  public void UnknownType ()
  {
    Assert.IsNull (TypeInfo.GetInstance (this.GetType ()));
  }

  [Test]
  [ExpectedException (typeof (MandatoryTypeNotFoundException))]
  public void UnknownMandatoryType ()
  {
    TypeInfo.GetMandatory (this.GetType ());
  }

  [Test]
  [ExpectedException (typeof (MandatoryMappingTypeNotFoundException))]
  public void UnknownMandatoryMappingType ()
  {
    TypeInfo.GetMandatory ("UnknownType", false);
  }

  [Test]
  public void GetDefaultEnumValue ()
  {
    Customer.CustomerType defaultValue = (Customer.CustomerType) TypeInfo.GetDefaultEnumValue (typeof (Customer.CustomerType));
    Assert.AreEqual (Customer.CustomerType.Standard, defaultValue);

  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void GetInvalidDefaultEnumValue ()
  {
    TypeInfo.GetDefaultEnumValue (this.GetType ());
  }

  private void Check (TypeInfo expected, TypeInfo actual)
  {
    if (actual == null)
      Assert.Fail ("Actual TypeInfo was null.");

    string typeMessage = string.Format (", failed at expected type '{0}'.", expected.Type);

    Assert.AreEqual (expected.Type, actual.Type, "Type" + typeMessage);
    Assert.AreEqual (expected.MappingType, actual.MappingType, "MappingType" + typeMessage);
    Assert.AreEqual (expected.IsNullable, actual.IsNullable, "IsNullable" + typeMessage);
    Assert.AreEqual (expected.DefaultValue, actual.DefaultValue, "DefaultValue" + typeMessage);
  }
}
}

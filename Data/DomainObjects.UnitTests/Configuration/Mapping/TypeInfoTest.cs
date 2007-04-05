using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class TypeInfoTest: LegacyMappingTest
  {
    private enum TypeMappingTestEnum
    {
      Value0 = 0,
      Value1 = 1
    }

    [Test]
    public void MappingTypes()
    {
      Check (new TypeInfo (typeof (NaBoolean), "boolean", true, NaBoolean.Null), TypeInfo.GetMandatory ("boolean", true));
      Check (new TypeInfo (typeof (NaByte), "byte", true, NaByte.Null), TypeInfo.GetMandatory ("byte", true));
      Check (new TypeInfo (typeof (NaDateTime), "dateTime", true, NaDateTime.Null), TypeInfo.GetMandatory ("dateTime", true));
      Check (new TypeInfo (typeof (NaDateTime), "date", true, NaDateTime.Null), TypeInfo.GetMandatory ("date", true));
      Check (new TypeInfo (typeof (NaDecimal), "decimal", true, NaDecimal.Null), TypeInfo.GetMandatory ("decimal", true));
      Check (new TypeInfo (typeof (NaGuid), "guid", true, NaGuid.Null), TypeInfo.GetMandatory ("guid", true));
      Check (new TypeInfo (typeof (NaInt16), "int16", true, NaInt16.Null), TypeInfo.GetMandatory ("int16", true));
      Check (new TypeInfo (typeof (NaInt32), "int32", true, NaInt32.Null), TypeInfo.GetMandatory ("int32", true));
      Check (new TypeInfo (typeof (NaInt64), "int64", true, NaInt64.Null), TypeInfo.GetMandatory ("int64", true));
      Check (new TypeInfo (typeof (NaDouble), "double", true, NaDouble.Null), TypeInfo.GetMandatory ("double", true));
      Check (new TypeInfo (typeof (NaSingle), "single", true, NaSingle.Null), TypeInfo.GetMandatory ("single", true));
      Check (new TypeInfo (typeof (string), "string", true, null), TypeInfo.GetMandatory ("string", true));
      Check (
          new TypeInfo (typeof (ObjectID), TypeInfo.ObjectIDMappingTypeName, true, null),
          TypeInfo.GetMandatory (TypeInfo.ObjectIDMappingTypeName, true));
      Check (new TypeInfo (typeof (byte[]), "binary", true, null), TypeInfo.GetMandatory ("binary", true));

      Check (new TypeInfo (typeof (bool), "boolean", false, false), TypeInfo.GetMandatory ("boolean", false));
      Check (new TypeInfo (typeof (byte), "byte", false, byte.MinValue), TypeInfo.GetMandatory ("byte", false));
      Check (new TypeInfo (typeof (DateTime), "dateTime", false, DateTime.MinValue), TypeInfo.GetMandatory ("dateTime", false));
      Check (new TypeInfo (typeof (DateTime), "date", false, DateTime.MinValue), TypeInfo.GetMandatory ("date", false));
      Check (new TypeInfo (typeof (decimal), "decimal", false, decimal.MinValue), TypeInfo.GetMandatory ("decimal", false));
      Check (new TypeInfo (typeof (double), "double", false, double.MinValue), TypeInfo.GetMandatory ("double", false));
      Check (new TypeInfo (typeof (Guid), "guid", false, Guid.Empty), TypeInfo.GetMandatory ("guid", false));
      Check (new TypeInfo (typeof (short), "int16", false, short.MinValue), TypeInfo.GetMandatory ("int16", false));
      Check (new TypeInfo (typeof (int), "int32", false, int.MinValue), TypeInfo.GetMandatory ("int32", false));
      Check (new TypeInfo (typeof (long), "int64", false, long.MinValue), TypeInfo.GetMandatory ("int64", false));
      Check (new TypeInfo (typeof (float), "single", false, float.MinValue), TypeInfo.GetMandatory ("single", false));
      Check (new TypeInfo (typeof (string), "string", false, string.Empty), TypeInfo.GetMandatory ("string", false));
      Check (
          new TypeInfo (typeof (ObjectID), TypeInfo.ObjectIDMappingTypeName, false, null),
          TypeInfo.GetMandatory (TypeInfo.ObjectIDMappingTypeName, false));
      Check (new TypeInfo (typeof (byte[]), "binary", false, new byte[0]), TypeInfo.GetMandatory ("binary", false));
    }

    [Test]
    public void Types ()
    {
      Check (new TypeInfo (typeof (NaBoolean), "boolean", true, NaBoolean.Null), TypeInfo.GetMandatory (typeof (NaBoolean), true));
      Check (new TypeInfo (typeof (NaByte), "byte", true, NaByte.Null), TypeInfo.GetMandatory (typeof (NaByte), true));
      Check (new TypeInfo (typeof (NaDateTime), "dateTime", true, NaDateTime.Null), TypeInfo.GetMandatory (typeof (NaDateTime), true));
      //Check (new TypeInfo (typeof (NaDateTime), "date", true, NaDateTime.Null), TypeInfo.GetMandatory (typeof (NaDateTime), true));
      Check (new TypeInfo (typeof (NaDecimal), "decimal", true, NaDecimal.Null), TypeInfo.GetMandatory (typeof (NaDecimal), true));
      Check (new TypeInfo (typeof (NaGuid), "guid", true, NaGuid.Null), TypeInfo.GetMandatory (typeof (NaGuid), true));
      Check (new TypeInfo (typeof (NaInt16), "int16", true, NaInt16.Null), TypeInfo.GetMandatory (typeof (NaInt16), true));
      Check (new TypeInfo (typeof (NaInt32), "int32", true, NaInt32.Null), TypeInfo.GetMandatory (typeof (NaInt32), true));
      Check (new TypeInfo (typeof (NaInt64), "int64", true, NaInt64.Null), TypeInfo.GetMandatory (typeof (NaInt64), true));
      Check (new TypeInfo (typeof (NaDouble), "double", true, NaDouble.Null), TypeInfo.GetMandatory (typeof (NaDouble), true));
      Check (new TypeInfo (typeof (NaSingle), "single", true, NaSingle.Null), TypeInfo.GetMandatory (typeof (NaSingle), true));
      Check (new TypeInfo (typeof (string), "string", true, null), TypeInfo.GetMandatory (typeof (string), true));
      Check (
          new TypeInfo (typeof (ObjectID), TypeInfo.ObjectIDMappingTypeName, true, null),
          TypeInfo.GetMandatory (typeof (ObjectID), true));
      Check (new TypeInfo (typeof (byte[]), "binary", true, null), TypeInfo.GetMandatory (typeof (byte[]), true));

      Check (new TypeInfo (typeof (bool), "boolean", false, false), TypeInfo.GetMandatory (typeof (bool), false));
      Check (new TypeInfo (typeof (byte), "byte", false, byte.MinValue), TypeInfo.GetMandatory (typeof (byte), false));
      Check (new TypeInfo (typeof (DateTime), "dateTime", false, DateTime.MinValue), TypeInfo.GetMandatory (typeof (DateTime), false));
      //Check (new TypeInfo (typeof (DateTime), "date", false, DateTime.MinValue), TypeInfo.GetMandatory (typeof (DateTime), false));
      Check (new TypeInfo (typeof (decimal), "decimal", false, decimal.MinValue), TypeInfo.GetMandatory (typeof (decimal), false));
      Check (new TypeInfo (typeof (double), "double", false, double.MinValue), TypeInfo.GetMandatory (typeof (double), false));
      Check (new TypeInfo (typeof (Guid), "guid", false, Guid.Empty), TypeInfo.GetMandatory (typeof (Guid), false));
      Check (new TypeInfo (typeof (short), "int16", false, short.MinValue), TypeInfo.GetMandatory (typeof (short), false));
      Check (new TypeInfo (typeof (int), "int32", false, int.MinValue), TypeInfo.GetMandatory (typeof (int), false));
      Check (new TypeInfo (typeof (long), "int64", false, long.MinValue), TypeInfo.GetMandatory (typeof (long), false));
      Check (new TypeInfo (typeof (float), "single", false, float.MinValue), TypeInfo.GetMandatory (typeof (float), false));
      Check (new TypeInfo (typeof (string), "string", false, string.Empty), TypeInfo.GetMandatory (typeof (string), false));
      Check (
          new TypeInfo (typeof (ObjectID), TypeInfo.ObjectIDMappingTypeName, false, null),
          TypeInfo.GetMandatory (typeof (ObjectID), false));
      Check (new TypeInfo (typeof (byte[]), "binary", false, new byte[0]), TypeInfo.GetMandatory (typeof (byte[]), false));
    }

    [Test]
    public void UnknownMappingType()
    {
      Assert.IsNull (
          TypeInfo.GetInstance (
              "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TypeMappingTest+TypeMappingTestEnum, Rubicon.Data.DomainObjects.UnitTests",
              false));
    }

    [Test]
    [ExpectedException (typeof (MandatoryMappingTypeNotFoundException))]
    public void UnknownMandatoryMappingType()
    {
      TypeInfo.GetMandatory ("UnknownType", false);
    }

    [Test]
    public void GetDefaultEnumValue()
    {
      Customer.CustomerType defaultValue = (Customer.CustomerType) TypeInfo.GetDefaultEnumValue (typeof (Customer.CustomerType));
      Assert.AreEqual (Customer.CustomerType.Standard, defaultValue);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void GetInvalidDefaultEnumValue()
    {
      TypeInfo.GetDefaultEnumValue (GetType());
    }

    private void Check (TypeInfo expected, TypeInfo actual)
    {
      if (actual == null)
        Assert.Fail ("Actual TypeInfo was null.");

      string typeMessage = string.Format (", failed at expected type '{0}'.", expected.Type);

      Assert.AreEqual (expected.Type, actual.Type, "Type" + typeMessage);
      Assert.AreEqual (expected.MappingType, actual.MappingType, "MappingType" + typeMessage);
      Assert.AreEqual (expected.IsNullable, actual.IsNullable, "IsNullable" + typeMessage);

      if (expected.Type == typeof (byte[]))
        ResourceManager.AreEqual ((byte[]) expected.DefaultValue, (byte[]) actual.DefaultValue);
      else
        Assert.AreEqual (expected.DefaultValue, actual.DefaultValue, "DefaultValue" + typeMessage);
    }
  }
}
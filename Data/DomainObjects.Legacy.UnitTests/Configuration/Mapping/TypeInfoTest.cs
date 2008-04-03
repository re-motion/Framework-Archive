using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.Resources;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain;
using Remotion.NullableValueTypes;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class TypeInfoTest : StandardMappingTest
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
      Check (new TypeInfo (typeof (NaBoolean), "boolean", true, NaBoolean.Null), TypeInfo.GetInstance ("boolean", true));
      Check (new TypeInfo (typeof (NaByte), "byte", true, NaByte.Null), TypeInfo.GetInstance ("byte", true));
      Check (new TypeInfo (typeof (NaDateTime), "dateTime", true, NaDateTime.Null), TypeInfo.GetInstance ("dateTime", true));
      Check (new TypeInfo (typeof (NaDateTime), "date", true, NaDateTime.Null), TypeInfo.GetInstance ("date", true));
      Check (new TypeInfo (typeof (NaDecimal), "decimal", true, NaDecimal.Null), TypeInfo.GetInstance ("decimal", true));
      Check (new TypeInfo (typeof (NaGuid), "guid", true, NaGuid.Null), TypeInfo.GetInstance ("guid", true));
      Check (new TypeInfo (typeof (NaInt16), "int16", true, NaInt16.Null), TypeInfo.GetInstance ("int16", true));
      Check (new TypeInfo (typeof (NaInt32), "int32", true, NaInt32.Null), TypeInfo.GetInstance ("int32", true));
      Check (new TypeInfo (typeof (NaInt64), "int64", true, NaInt64.Null), TypeInfo.GetInstance ("int64", true));
      Check (new TypeInfo (typeof (NaDouble), "double", true, NaDouble.Null), TypeInfo.GetInstance ("double", true));
      Check (new TypeInfo (typeof (NaSingle), "single", true, NaSingle.Null), TypeInfo.GetInstance ("single", true));
      Check (new TypeInfo (typeof (string), "string", true, null), TypeInfo.GetInstance ("string", true));
      Check (new TypeInfo (typeof (ObjectID), TypeInfo.ObjectIDMappingTypeName, true, null), TypeInfo.GetInstance (TypeInfo.ObjectIDMappingTypeName, true));
      Check (new TypeInfo (typeof (byte[]), "binary", true, null), TypeInfo.GetInstance ("binary", true));

      Check (new TypeInfo (typeof (bool), "boolean", false, false), TypeInfo.GetInstance ("boolean", false));
      Check (new TypeInfo (typeof (byte), "byte", false, byte.MinValue), TypeInfo.GetInstance ("byte", false));
      Check (new TypeInfo (typeof (DateTime), "dateTime", false, DateTime.MinValue), TypeInfo.GetInstance ("dateTime", false));
      Check (new TypeInfo (typeof (DateTime), "date", false, DateTime.MinValue), TypeInfo.GetInstance ("date", false));
      Check (new TypeInfo (typeof (decimal), "decimal", false, decimal.MinValue), TypeInfo.GetInstance ("decimal", false));
      Check (new TypeInfo (typeof (double), "double", false, double.MinValue), TypeInfo.GetInstance ("double", false));
      Check (new TypeInfo (typeof (Guid), "guid", false, Guid.Empty), TypeInfo.GetInstance ("guid", false));
      Check (new TypeInfo (typeof (short), "int16", false, short.MinValue), TypeInfo.GetInstance ("int16", false));
      Check (new TypeInfo (typeof (int), "int32", false, int.MinValue), TypeInfo.GetInstance ("int32", false));
      Check (new TypeInfo (typeof (long), "int64", false, long.MinValue), TypeInfo.GetInstance ("int64", false));
      Check (new TypeInfo (typeof (float), "single", false, float.MinValue), TypeInfo.GetInstance ("single", false));
      Check (new TypeInfo (typeof (string), "string", false, string.Empty), TypeInfo.GetInstance ("string", false));
      Check (new TypeInfo (typeof (ObjectID), TypeInfo.ObjectIDMappingTypeName, false, null), TypeInfo.GetInstance (TypeInfo.ObjectIDMappingTypeName, false));
      Check (new TypeInfo (typeof (byte[]), "binary", false, new byte[0]), TypeInfo.GetInstance ("binary", false));
    }

    [Test]
    public void UnknownMappingType ()
    {
      Assert.IsNull (TypeInfo.GetInstance (
          "Remotion.Data.DomainObjects.Legacy.UnitTests.Configuration.Mapping.TypeMappingTest+TypeMappingTestEnum, Remotion.Data.DomainObjects.Legacy.UnitTests", false));
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
    [ExpectedException (typeof (ArgumentTypeException))]
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

      if (expected.Type == typeof (byte[]))
        ResourceManager.AreEqual ((byte[]) expected.DefaultValue, (byte[]) actual.DefaultValue);
      else
        Assert.AreEqual (expected.DefaultValue, actual.DefaultValue, "DefaultValue" + typeMessage);
    }
  }
}

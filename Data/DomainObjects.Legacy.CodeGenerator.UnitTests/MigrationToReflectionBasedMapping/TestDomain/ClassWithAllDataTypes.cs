using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.MigrationToReflectionBasedMapping.TestDomain
{
  [Instantiable]
  [DBStorageGroup]
  [DBTable ("TableWithAllDataTypes")]
  public abstract class ClassWithAllDataTypes : BindableDomainObject
  {
    // types

    public enum EnumType
    {
      DummyEntry = 0
    }

    // static members and constants

    public static ClassWithAllDataTypes NewObject ()
    {
      return DomainObject.NewObject<ClassWithAllDataTypes> ().With ();
    }

    public static ClassWithAllDataTypes NewObject (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        return ClassWithAllDataTypes.NewObject ();
      }
    }

    public static new ClassWithAllDataTypes GetObject (ObjectID id)
    {
      return DomainObject.GetObject<ClassWithAllDataTypes> (id);
    }

    public static new ClassWithAllDataTypes GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        return ClassWithAllDataTypes.GetObject (id);
      }
    }

    // member fields

    // construction and disposing

    protected ClassWithAllDataTypes ()
    {
    }

    // methods and properties

    [DBColumn ("Boolean")]
    public abstract bool BooleanProperty { get; set; }

    [DBColumn ("Byte")]
    public abstract byte ByteProperty { get; set; }

    [DBColumn ("Date")]
    public abstract DateTime DateProperty { get; set; }

    [DBColumn ("DateTime")]
    public abstract DateTime DateTimeProperty { get; set; }

    [DBColumn ("Decimal")]
    public abstract decimal DecimalProperty { get; set; }

    [DBColumn ("Double")]
    public abstract double DoubleProperty { get; set; }

    [DBColumn ("Enum")]
    public abstract ClassWithAllDataTypes.EnumType EnumProperty { get; set; }

    [DBColumn ("Guid")]
    public abstract Guid GuidProperty { get; set; }

    [DBColumn ("Int16")]
    public abstract short Int16Property { get; set; }

    [DBColumn ("Int32")]
    public abstract int Int32Property { get; set; }

    [DBColumn ("Int64")]
    public abstract long Int64Property { get; set; }

    [DBColumn ("Single")]
    public abstract float SingleProperty { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    [DBColumn ("String")]
    public abstract string StringProperty { get; set; }

    [StringProperty (IsNullable = false)]
    [DBColumn ("StringWithoutMaxLength")]
    public abstract string StringPropertyWithoutMaxLength { get; set; }

    [BinaryProperty (IsNullable = false)]
    [DBColumn ("Binary")]
    public abstract byte[] BinaryProperty { get; set; }

    [DBColumn ("NaBoolean")]
    public abstract bool? NaBooleanProperty { get; set; }

    [DBColumn ("NaByte")]
    public abstract byte? NaByteProperty { get; set; }

    [DBColumn ("NaDate")]
    public abstract DateTime? NaDateProperty { get; set; }

    [DBColumn ("NaDateTime")]
    public abstract DateTime? NaDateTimeProperty { get; set; }

    [DBColumn ("NaDecimal")]
    public abstract decimal? NaDecimalProperty { get; set; }

    [DBColumn ("NaDouble")]
    public abstract double? NaDoubleProperty { get; set; }

    [DBColumn ("NaGuid")]
    public abstract Guid? NaGuidProperty { get; set; }

    [DBColumn ("NaInt16")]
    public abstract short? NaInt16Property { get; set; }

    [DBColumn ("NaInt32")]
    public abstract int? NaInt32Property { get; set; }

    [DBColumn ("NaInt64")]
    public abstract long? NaInt64Property { get; set; }

    [DBColumn ("NaSingle")]
    public abstract float? NaSingleProperty { get; set; }

    [StringProperty (MaximumLength = 100)]
    [DBColumn ("StringWithNullValue")]
    public abstract string StringWithNullValueProperty { get; set; }

    [DBColumn ("NaBooleanWithNullValue")]
    public abstract bool? NaBooleanWithNullValueProperty { get; set; }

    [DBColumn ("NaByteWithNullValue")]
    public abstract byte? NaByteWithNullValueProperty { get; set; }

    [DBColumn ("NaDateWithNullValue")]
    public abstract DateTime? NaDateWithNullValueProperty { get; set; }

    [DBColumn ("NaDateTimeWithNullValue")]
    public abstract DateTime? NaDateTimeWithNullValueProperty { get; set; }

    [DBColumn ("NaDecimalWithNullValue")]
    public abstract decimal? NaDecimalWithNullValueProperty { get; set; }

    [DBColumn ("NaDoubleWithNullValue")]
    public abstract double? NaDoubleWithNullValueProperty { get; set; }

    [DBColumn ("NaGuidWithNullValue")]
    public abstract Guid? NaGuidWithNullValueProperty { get; set; }

    [DBColumn ("NaInt16WithNullValue")]
    public abstract short? NaInt16WithNullValueProperty { get; set; }

    [DBColumn ("NaInt32WithNullValue")]
    public abstract int? NaInt32WithNullValueProperty { get; set; }

    [DBColumn ("NaInt64WithNullValue")]
    public abstract long? NaInt64WithNullValueProperty { get; set; }

    [DBColumn ("NaSingleWithNullValue")]
    public abstract float? NaSingleWithNullValueProperty { get; set; }

    [BinaryProperty (MaximumLength = 1000000)]
    [DBColumn ("NullableBinary")]
    public abstract byte[] NullableBinaryProperty { get; set; }

  }
}

using System;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithAllDataTypes")]
  [TestDomain]
  [NotAbstract]
  public abstract class ClassWithAllDataTypes : TestDomainBase
  {
    public enum EnumType
    {
      Value0 = 0,
      Value1 = 1,
      Value2 = 2
    }

    public new static ClassWithAllDataTypes GetObject (ObjectID id)
    {
      return (ClassWithAllDataTypes) DomainObject.GetObject (id);
    }

    public static ClassWithAllDataTypes Create ()
    {
      return DomainObject.Create<ClassWithAllDataTypes> ();
    }

    private bool _onLoadedHasBeenCalled;

    protected ClassWithAllDataTypes (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [StorageClassNone]
    internal bool OnLoadedHasBeenCalled
    {
      get { return _onLoadedHasBeenCalled; }
    }

    protected override void OnLoaded()
    {
      base.OnLoaded();
      _onLoadedHasBeenCalled = true;
    }

    [StorageClassNone]
    public object ObjectProperty
    {
      get { return null; }
      set { }
    }

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
    public abstract EnumType EnumProperty { get; set; }

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
    public abstract NaBoolean NaBooleanProperty { get; set; }

    [DBColumn ("NaByte")]
    public abstract NaByte NaByteProperty { get; set; }

    [DBColumn ("NaDate")]
    public abstract NaDateTime NaDateProperty { get; set; }

    [DBColumn ("NaDateTime")]
    public abstract NaDateTime NaDateTimeProperty { get; set; }

    [DBColumn ("NaDecimal")]
    public abstract NaDecimal NaDecimalProperty { get; set; }

    [DBColumn ("NaDouble")]
    public abstract NaDouble NaDoubleProperty { get; set; }

    [DBColumn ("NaGuid")]
    public abstract NaGuid NaGuidProperty { get; set; }

    [DBColumn ("NaInt16")]
    public abstract NaInt16 NaInt16Property { get; set; }

    [DBColumn ("NaInt32")]
    public abstract NaInt32 NaInt32Property { get; set; }

    [DBColumn ("NaInt64")]
    public abstract NaInt64 NaInt64Property { get; set; }

    [DBColumn ("NaSingle")]
    public abstract NaSingle NaSingleProperty { get; set; }

    [StringProperty (MaximumLength = 100)]
    [DBColumn ("StringWithNullValue")]
    public abstract string StringWithNullValueProperty { get; set; }

    [DBColumn ("NaBooleanWithNullValue")]
    public abstract NaBoolean NaBooleanWithNullValueProperty { get; set; }

    [DBColumn ("NaByteWithNullValue")]
    public abstract NaByte NaByteWithNullValueProperty { get; set; }

    [DBColumn ("NaDateWithNullValue")]
    public abstract NaDateTime NaDateWithNullValueProperty { get; set; }

    [DBColumn ("NaDateTimeWithNullValue")]
    public abstract NaDateTime NaDateTimeWithNullValueProperty { get; set; }

    [DBColumn ("NaDecimalWithNullValue")]
    public abstract NaDecimal NaDecimalWithNullValueProperty { get; set; }

    [DBColumn ("NaDoubleWithNullValue")]
    public abstract NaDouble NaDoubleWithNullValueProperty { get; set; }

    [DBColumn ("NaGuidWithNullValue")]
    public abstract NaGuid NaGuidWithNullValueProperty { get; set; }

    [DBColumn ("NaInt16WithNullValue")]
    public abstract NaInt16 NaInt16WithNullValueProperty { get; set; }

    [DBColumn ("NaInt32WithNullValue")]
    public abstract NaInt32 NaInt32WithNullValueProperty { get; set; }

    [DBColumn ("NaInt64WithNullValue")]
    public abstract NaInt64 NaInt64WithNullValueProperty { get; set; }

    [DBColumn ("NaSingleWithNullValue")]
    public abstract NaSingle NaSingleWithNullValueProperty { get; set; }

    [BinaryProperty (MaximumLength = 1000000)]
    [DBColumn ("NullableBinary")]
    public abstract byte[] NullableBinaryProperty { get; set; }
  }
}

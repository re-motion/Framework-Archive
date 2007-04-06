using System;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithAllDataTypes")]
  public class ClassWithAllDataTypes: TestDomainBase
  {
    // types

    public enum EnumType
    {
      Value0 = 0,
      Value1 = 1,
      Value2 = 2
    }

    // static members and constants

    public new static ClassWithAllDataTypes GetObject (ObjectID id)
    {
      return (ClassWithAllDataTypes) DomainObject.GetObject (id);
    }

    // member fields

    private bool _onLoadedHasBeenCalled;

    // construction and disposing

    public ClassWithAllDataTypes()
    {
    }

    public ClassWithAllDataTypes (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    public ClassWithAllDataTypes (ClientTransaction clientTransaction)
        : base (clientTransaction)
    {
    }

    protected ClassWithAllDataTypes (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    // methods and properties

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
    public bool BooleanProperty
    {
      get { return DataContainer.GetBoolean ("BooleanProperty"); }
      set { DataContainer.SetValue ("BooleanProperty", value); }
    }

    [DBColumn ("Byte")]
    public byte ByteProperty
    {
      get { return DataContainer.GetByte ("ByteProperty"); }
      set { DataContainer.SetValue ("ByteProperty", value); }
    }

    [DBColumn ("Date")]
    public DateTime DateProperty
    {
      get { return DataContainer.GetDateTime ("DateProperty"); }
      set { DataContainer.SetValue ("DateProperty", value); }
    }

    [DBColumn ("DateTime")]
    public DateTime DateTimeProperty
    {
      get { return DataContainer.GetDateTime ("DateTimeProperty"); }
      set { DataContainer.SetValue ("DateTimeProperty", value); }
    }

    [DBColumn ("Decimal")]
    public decimal DecimalProperty
    {
      get { return DataContainer.GetDecimal ("DecimalProperty"); }
      set { DataContainer.SetValue ("DecimalProperty", value); }
    }

    [DBColumn ("Double")]
    public double DoubleProperty
    {
      get { return DataContainer.GetDouble ("DoubleProperty"); }
      set { DataContainer.SetValue ("DoubleProperty", value); }
    }

    [DBColumn ("Enum")]
    public EnumType EnumProperty
    {
      get { return (EnumType) DataContainer["EnumProperty"]; }
      set { DataContainer.SetValue ("EnumProperty", value); }
    }

    [DBColumn ("Guid")]
    public Guid GuidProperty
    {
      get { return DataContainer.GetGuid ("GuidProperty"); }
      set { DataContainer.SetValue ("GuidProperty", value); }
    }

    [DBColumn ("Int16")]
    public short Int16Property
    {
      get { return DataContainer.GetInt16 ("Int16Property"); }
      set { DataContainer.SetValue ("Int16Property", value); }
    }

    [DBColumn ("Int32")]
    public int Int32Property
    {
      get { return DataContainer.GetInt32 ("Int32Property"); }
      set { DataContainer.SetValue ("Int32Property", value); }
    }

    [DBColumn ("Int64")]
    public long Int64Property
    {
      get { return DataContainer.GetInt64 ("Int64Property"); }
      set { DataContainer.SetValue ("Int64Property", value); }
    }

    [DBColumn ("Single")]
    public float SingleProperty
    {
      get { return DataContainer.GetSingle ("SingleProperty"); }
      set { DataContainer.SetValue ("SingleProperty", value); }
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    [DBColumn ("String")]
    public string StringProperty
    {
      get { return DataContainer.GetString ("StringProperty"); }
      set { DataContainer.SetValue ("StringProperty", value); }
    }

    [StringProperty (IsNullable = false)]
    [DBColumn ("StringWithoutMaxLength")]
    public string StringPropertyWithoutMaxLength
    {
      get { return DataContainer.GetString ("StringPropertyWithoutMaxLength"); }
      set { DataContainer.SetValue ("StringPropertyWithoutMaxLength", value); }
    }

    [BinaryProperty (IsNullable = false)]
    [DBColumn ("Binary")]
    public byte[] BinaryProperty
    {
      get { return DataContainer.GetBytes ("BinaryProperty"); }
      set { DataContainer.SetValue ("BinaryProperty", value); }
    }

    [DBColumn ("NaBoolean")]
    public NaBoolean NaBooleanProperty
    {
      get { return DataContainer.GetNaBoolean ("NaBooleanProperty"); }
      set { DataContainer.SetValue ("NaBooleanProperty", value); }
    }

    [DBColumn ("NaByte")]
    public NaByte NaByteProperty
    {
      get { return DataContainer.GetNaByte ("NaByteProperty"); }
      set { DataContainer.SetValue ("NaByteProperty", value); }
    }

    [DBColumn ("NaDate")]
    public NaDateTime NaDateProperty
    {
      get { return DataContainer.GetNaDateTime ("NaDateProperty"); }
      set { DataContainer.SetValue ("NaDateProperty", value); }
    }

    [DBColumn ("NaDateTime")]
    public NaDateTime NaDateTimeProperty
    {
      get { return DataContainer.GetNaDateTime ("NaDateTimeProperty"); }
      set { DataContainer.SetValue ("NaDateTimeProperty", value); }
    }

    [DBColumn ("NaDecimal")]
    public NaDecimal NaDecimalProperty
    {
      get { return DataContainer.GetNaDecimal ("NaDecimalProperty"); }
      set { DataContainer.SetValue ("NaDecimalProperty", value); }
    }

    [DBColumn ("NaDouble")]
    public NaDouble NaDoubleProperty
    {
      get { return DataContainer.GetNaDouble ("NaDoubleProperty"); }
      set { DataContainer.SetValue ("NaDoubleProperty", value); }
    }

    [DBColumn ("NaGuid")]
    public NaGuid NaGuidProperty
    {
      get { return DataContainer.GetNaGuid ("NaGuidProperty"); }
      set { DataContainer.SetValue ("NaGuidProperty", value); }
    }

    [DBColumn ("NaInt16")]
    public NaInt16 NaInt16Property
    {
      get { return DataContainer.GetNaInt16 ("NaInt16Property"); }
      set { DataContainer.SetValue ("NaInt16Property", value); }
    }

    [DBColumn ("NaInt32")]
    public NaInt32 NaInt32Property
    {
      get { return DataContainer.GetNaInt32 ("NaInt32Property"); }
      set { DataContainer.SetValue ("NaInt32Property", value); }
    }

    [DBColumn ("NaInt64")]
    public NaInt64 NaInt64Property
    {
      get { return DataContainer.GetNaInt64 ("NaInt64Property"); }
      set { DataContainer.SetValue ("NaInt64Property", value); }
    }

    [DBColumn ("NaSingle")]
    public NaSingle NaSingleProperty
    {
      get { return DataContainer.GetNaSingle ("NaSingleProperty"); }
      set { DataContainer.SetValue ("NaSingleProperty", value); }
    }

    [StringProperty (MaximumLength = 100)]
    [DBColumn ("StringWithNullValue")]
    public string StringWithNullValueProperty
    {
      get { return DataContainer.GetString ("StringWithNullValueProperty"); }
      set { DataContainer.SetValue ("StringWithNullValueProperty", value); }
    }

    [DBColumn ("NaBooleanWithNullValue")]
    public NaBoolean NaBooleanWithNullValueProperty
    {
      get { return DataContainer.GetNaBoolean ("NaBooleanWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaBooleanWithNullValueProperty", value); }
    }

    [DBColumn ("NaByteWithNullValue")]
    public NaByte NaByteWithNullValueProperty
    {
      get { return DataContainer.GetNaByte ("NaByteWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaByteWithNullValueProperty", value); }
    }

    [DBColumn ("NaDateWithNullValue")]
    public NaDateTime NaDateWithNullValueProperty
    {
      get { return DataContainer.GetNaDateTime ("NaDateWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDateWithNullValueProperty", value); }
    }

    [DBColumn ("NaDateTimeWithNullValue")]
    public NaDateTime NaDateTimeWithNullValueProperty
    {
      get { return DataContainer.GetNaDateTime ("NaDateTimeWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDateTimeWithNullValueProperty", value); }
    }

    [DBColumn ("NaDecimalWithNullValue")]
    public NaDecimal NaDecimalWithNullValueProperty
    {
      get { return DataContainer.GetNaDecimal ("NaDecimalWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDecimalWithNullValueProperty", value); }
    }

    [DBColumn ("NaDoubleWithNullValue")]
    public NaDouble NaDoubleWithNullValueProperty
    {
      get { return DataContainer.GetNaDouble ("NaDoubleWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDoubleWithNullValueProperty", value); }
    }

    [DBColumn ("NaGuidWithNullValue")]
    public NaGuid NaGuidWithNullValueProperty
    {
      get { return DataContainer.GetNaGuid ("NaGuidWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaGuidWithNullValueProperty", value); }
    }

    [DBColumn ("NaInt16WithNullValue")]
    public NaInt16 NaInt16WithNullValueProperty
    {
      get { return DataContainer.GetNaInt16 ("NaInt16WithNullValueProperty"); }
      set { DataContainer.SetValue ("NaInt16WithNullValueProperty", value); }
    }

    [DBColumn ("NaInt32WithNullValue")]
    public NaInt32 NaInt32WithNullValueProperty
    {
      get { return DataContainer.GetNaInt32 ("NaInt32WithNullValueProperty"); }
      set { DataContainer.SetValue ("NaInt32WithNullValueProperty", value); }
    }

    [DBColumn ("NaInt64WithNullValue")]
    public NaInt64 NaInt64WithNullValueProperty
    {
      get { return DataContainer.GetNaInt64 ("NaInt64WithNullValueProperty"); }
      set { DataContainer.SetValue ("NaInt64WithNullValueProperty", value); }
    }

    [DBColumn ("NaSingleWithNullValue")]
    public NaSingle NaSingleWithNullValueProperty
    {
      get { return DataContainer.GetNaSingle ("NaSingleWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaSingleWithNullValueProperty", value); }
    }

    [BinaryProperty (MaximumLength = 1000000)]
    [DBColumn ("NullableBinary")]
    public byte[] NullableBinaryProperty
    {
      get { return DataContainer.GetBytes ("NullableBinaryProperty"); }
      set { DataContainer.SetValue ("NullableBinaryProperty", value); }
    }
  }
}
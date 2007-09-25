using System;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithAllDataTypes : TestDomainBase
  {
    // types

    public enum EnumType
    {
      Value0 = 0,
      Value1 = 1,
      Value2 = 2
    }

    // static members and constants

    public static new ClassWithAllDataTypes GetObject (ObjectID id)
    {
      return (ClassWithAllDataTypes) DomainObject.GetObject (id);
    }

    // member fields

    private bool _onLoadedHasBeenCalled;

    // construction and disposing

    public ClassWithAllDataTypes ()
    {
    }

    protected ClassWithAllDataTypes (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    internal bool OnLoadedHasBeenCalled
    {
      get { return _onLoadedHasBeenCalled; }
    }

    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);
      _onLoadedHasBeenCalled = true;
    }

    public bool BooleanProperty
    {
      get { return (Boolean) DataContainer.GetValue ("BooleanProperty"); }
      set { DataContainer.SetValue ("BooleanProperty", value); }
    }

    public byte ByteProperty
    {
      get { return (Byte) DataContainer.GetValue ("ByteProperty"); }
      set { DataContainer.SetValue ("ByteProperty", value); }
    }

    public DateTime DateProperty
    {
      get { return (DateTime) DataContainer.GetValue ("DateProperty"); }
      set { DataContainer.SetValue ("DateProperty", value); }
    }

    public DateTime DateTimeProperty
    {
      get { return (DateTime) DataContainer.GetValue ("DateTimeProperty"); }
      set { DataContainer.SetValue ("DateTimeProperty", value); }
    }

    public decimal DecimalProperty
    {
      get { return (Decimal) DataContainer.GetValue ("DecimalProperty"); }
      set { DataContainer.SetValue ("DecimalProperty", value); }
    }

    public double DoubleProperty
    {
      get { return (Double) DataContainer.GetValue ("DoubleProperty"); }
      set { DataContainer.SetValue ("DoubleProperty", value); }
    }

    public EnumType EnumProperty
    {
      get { return (EnumType) DataContainer.GetValue ("EnumProperty"); }
      set { DataContainer.SetValue ("EnumProperty", value); }
    }

    public Guid GuidProperty
    {
      get { return (Guid) DataContainer.GetValue ("GuidProperty"); }
      set { DataContainer.SetValue ("GuidProperty", value); }
    }

    public short Int16Property
    {
      get { return (Int16) DataContainer.GetValue ("Int16Property"); }
      set { DataContainer.SetValue ("Int16Property", value); }
    }

    public int Int32Property
    {
      get { return (Int32) DataContainer.GetValue ("Int32Property"); }
      set { DataContainer.SetValue ("Int32Property", value); }
    }

    public long Int64Property
    {
      get { return (Int64) DataContainer.GetValue ("Int64Property"); }
      set { DataContainer.SetValue ("Int64Property", value); }
    }

    public float SingleProperty
    {
      get { return (Single) DataContainer.GetValue ("SingleProperty"); }
      set { DataContainer.SetValue ("SingleProperty", value); }
    }

    public string StringProperty
    {
      get { return (String) DataContainer.GetValue ("StringProperty"); }
      set { DataContainer.SetValue ("StringProperty", value); }
    }

    public string StringPropertyWithoutMaxLength
    {
      get { return (String) DataContainer.GetValue ("StringPropertyWithoutMaxLength"); }
      set { DataContainer.SetValue ("StringPropertyWithoutMaxLength", value); }
    }

    public byte[] BinaryProperty
    {
      get { return (byte[]) DataContainer.GetValue ("BinaryProperty"); }
      set { DataContainer.SetValue ("BinaryProperty", value); }
    }

    public NaBoolean NaBooleanProperty
    {
      get { return (NaBoolean) DataContainer.GetValue ("NaBooleanProperty"); }
      set { DataContainer.SetValue ("NaBooleanProperty", value); }
    }

    public NaByte NaByteProperty
    {
      get { return (NaByte) DataContainer.GetValue ("NaByteProperty"); }
      set { DataContainer.SetValue ("NaByteProperty", value); }
    }

    public NaDateTime NaDateProperty
    {
      get { return (NaDateTime) DataContainer.GetValue ("NaDateProperty"); }
      set { DataContainer.SetValue ("NaDateProperty", value); }
    }

    public NaDateTime NaDateTimeProperty
    {
      get { return (NaDateTime) DataContainer.GetValue ("NaDateTimeProperty"); }
      set { DataContainer.SetValue ("NaDateTimeProperty", value); }
    }

    public NaDecimal NaDecimalProperty
    {
      get { return (NaDecimal) DataContainer.GetValue ("NaDecimalProperty"); }
      set { DataContainer.SetValue ("NaDecimalProperty", value); }
    }

    public NaDouble NaDoubleProperty
    {
      get { return (NaDouble) DataContainer.GetValue ("NaDoubleProperty"); }
      set { DataContainer.SetValue ("NaDoubleProperty", value); }
    }

    public NaGuid NaGuidProperty
    {
      get { return (NaGuid) DataContainer.GetValue ("NaGuidProperty"); }
      set { DataContainer.SetValue ("NaGuidProperty", value); }
    }

    public NaInt16 NaInt16Property
    {
      get { return (NaInt16) DataContainer.GetValue ("NaInt16Property"); }
      set { DataContainer.SetValue ("NaInt16Property", value); }
    }

    public NaInt32 NaInt32Property
    {
      get { return (NaInt32) DataContainer.GetValue ("NaInt32Property"); }
      set { DataContainer.SetValue ("NaInt32Property", value); }
    }

    public NaInt64 NaInt64Property
    {
      get { return (NaInt64) DataContainer.GetValue ("NaInt64Property"); }
      set { DataContainer.SetValue ("NaInt64Property", value); }
    }

    public NaSingle NaSingleProperty
    {
      get { return (NaSingle) DataContainer.GetValue ("NaSingleProperty"); }
      set { DataContainer.SetValue ("NaSingleProperty", value); }
    }

    public string StringWithNullValueProperty
    {
      get { return (String) DataContainer.GetValue ("StringWithNullValueProperty"); }
      set { DataContainer.SetValue ("StringWithNullValueProperty", value); }
    }

    public NaBoolean NaBooleanWithNullValueProperty
    {
      get { return (NaBoolean) DataContainer.GetValue ("NaBooleanWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaBooleanWithNullValueProperty", value); }
    }

    public NaByte NaByteWithNullValueProperty
    {
      get { return (NaByte) DataContainer.GetValue ("NaByteWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaByteWithNullValueProperty", value); }
    }

    public NaDateTime NaDateWithNullValueProperty
    {
      get { return (NaDateTime) DataContainer.GetValue ("NaDateWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDateWithNullValueProperty", value); }
    }

    public NaDateTime NaDateTimeWithNullValueProperty
    {
      get { return (NaDateTime) DataContainer.GetValue ("NaDateTimeWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDateTimeWithNullValueProperty", value); }
    }

    public NaDecimal NaDecimalWithNullValueProperty
    {
      get { return (NaDecimal) DataContainer.GetValue ("NaDecimalWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDecimalWithNullValueProperty", value); }
    }

    public NaDouble NaDoubleWithNullValueProperty
    {
      get { return (NaDouble) DataContainer.GetValue ("NaDoubleWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDoubleWithNullValueProperty", value); }
    }

    public NaGuid NaGuidWithNullValueProperty
    {
      get { return (NaGuid) DataContainer.GetValue ("NaGuidWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaGuidWithNullValueProperty", value); }
    }

    public NaInt16 NaInt16WithNullValueProperty
    {
      get { return (NaInt16) DataContainer.GetValue ("NaInt16WithNullValueProperty"); }
      set { DataContainer.SetValue ("NaInt16WithNullValueProperty", value); }
    }

    public NaInt32 NaInt32WithNullValueProperty
    {
      get { return (NaInt32) DataContainer.GetValue ("NaInt32WithNullValueProperty"); }
      set { DataContainer.SetValue ("NaInt32WithNullValueProperty", value); }
    }

    public NaInt64 NaInt64WithNullValueProperty
    {
      get { return (NaInt64) DataContainer.GetValue ("NaInt64WithNullValueProperty"); }
      set { DataContainer.SetValue ("NaInt64WithNullValueProperty", value); }
    }

    public NaSingle NaSingleWithNullValueProperty
    {
      get { return (NaSingle) DataContainer.GetValue ("NaSingleWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaSingleWithNullValueProperty", value); }
    }

    public byte[] NullableBinaryProperty
    {
      get { return (byte[]) DataContainer.GetValue ("NullableBinaryProperty"); }
      set { DataContainer.SetValue ("NullableBinaryProperty", value); }
    }
  }
}
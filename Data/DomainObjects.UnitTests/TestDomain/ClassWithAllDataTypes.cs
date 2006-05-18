using System;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
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

    public ClassWithAllDataTypes (ClientTransaction clientTransaction)
      : base (clientTransaction)
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

    protected override void OnLoaded ()
    {
      base.OnLoaded ();
      _onLoadedHasBeenCalled = true;
    }

    public bool BooleanProperty
    {
      get { return DataContainer.GetBoolean ("BooleanProperty"); }
      set { DataContainer.SetValue ("BooleanProperty", value); }
    }

    public byte ByteProperty
    {
      get { return DataContainer.GetByte ("ByteProperty"); }
      set { DataContainer.SetValue ("ByteProperty", value); }
    }

    public DateTime DateProperty
    {
      get { return DataContainer.GetDateTime ("DateProperty"); }
      set { DataContainer.SetValue ("DateProperty", value); }
    }

    public DateTime DateTimeProperty
    {
      get { return DataContainer.GetDateTime ("DateTimeProperty"); }
      set { DataContainer.SetValue ("DateTimeProperty", value); }
    }

    public decimal DecimalProperty
    {
      get { return DataContainer.GetDecimal ("DecimalProperty"); }
      set { DataContainer.SetValue ("DecimalProperty", value); }
    }

    public double DoubleProperty
    {
      get { return DataContainer.GetDouble ("DoubleProperty"); }
      set { DataContainer.SetValue ("DoubleProperty", value); }
    }

    public EnumType EnumProperty
    {
      get { return (EnumType) DataContainer["EnumProperty"]; }
      set { DataContainer.SetValue ("EnumProperty", value); }
    }

    public Guid GuidProperty
    {
      get { return DataContainer.GetGuid ("GuidProperty"); }
      set { DataContainer.SetValue ("GuidProperty", value); }
    }

    public short Int16Property
    {
      get { return DataContainer.GetInt16 ("Int16Property"); }
      set { DataContainer.SetValue ("Int16Property", value); }
    }

    public int Int32Property
    {
      get { return DataContainer.GetInt32 ("Int32Property"); }
      set { DataContainer.SetValue ("Int32Property", value); }
    }

    public long Int64Property
    {
      get { return DataContainer.GetInt64 ("Int64Property"); }
      set { DataContainer.SetValue ("Int64Property", value); }
    }

    public float SingleProperty
    {
      get { return DataContainer.GetSingle ("SingleProperty"); }
      set { DataContainer.SetValue ("SingleProperty", value); }
    }

    public string StringProperty
    {
      get { return DataContainer.GetString ("StringProperty"); }
      set { DataContainer.SetValue ("StringProperty", value); }
    }

    public byte[] BinaryProperty
    {
      get { return DataContainer.GetBytes ("BinaryProperty"); }
      set { DataContainer.SetValue ("BinaryProperty", value); }
    }

    public NaBoolean NaBooleanProperty
    {
      get { return DataContainer.GetNaBoolean ("NaBooleanProperty"); }
      set { DataContainer.SetValue ("NaBooleanProperty", value); }
    }

    public NaByte NaByteProperty
    {
      get { return DataContainer.GetNaByte ("NaByteProperty"); }
      set { DataContainer.SetValue ("NaByteProperty", value); }
    }

    public NaDateTime NaDateProperty
    {
      get { return DataContainer.GetNaDateTime ("NaDateProperty"); }
      set { DataContainer.SetValue ("NaDateProperty", value); }
    }

    public NaDateTime NaDateTimeProperty
    {
      get { return DataContainer.GetNaDateTime ("NaDateTimeProperty"); }
      set { DataContainer.SetValue ("NaDateTimeProperty", value); }
    }

    public NaDecimal NaDecimalProperty
    {
      get { return DataContainer.GetNaDecimal ("NaDecimalProperty"); }
      set { DataContainer.SetValue ("NaDecimalProperty", value); }
    }

    public NaDouble NaDoubleProperty
    {
      get { return DataContainer.GetNaDouble ("NaDoubleProperty"); }
      set { DataContainer.SetValue ("NaDoubleProperty", value); }
    }

    public NaGuid NaGuidProperty
    {
      get { return DataContainer.GetNaGuid ("NaGuidProperty"); }
      set { DataContainer.SetValue ("NaGuidProperty", value); }
    }

    public NaInt16 NaInt16Property
    {
      get { return DataContainer.GetNaInt16 ("NaInt16Property"); }
      set { DataContainer.SetValue ("NaInt16Property", value); }
    }

    public NaInt32 NaInt32Property
    {
      get { return DataContainer.GetNaInt32 ("NaInt32Property"); }
      set { DataContainer.SetValue ("NaInt32Property", value); }
    }

    public NaInt64 NaInt64Property
    {
      get { return DataContainer.GetNaInt64 ("NaInt64Property"); }
      set { DataContainer.SetValue ("NaInt64Property", value); }
    }

    public NaSingle NaSingleProperty
    {
      get { return DataContainer.GetNaSingle ("NaSingleProperty"); }
      set { DataContainer.SetValue ("NaSingleProperty", value); }
    }

    public string StringWithNullValueProperty
    {
      get { return DataContainer.GetString ("StringWithNullValueProperty"); }
      set { DataContainer.SetValue ("StringWithNullValueProperty", value); }
    }

    public NaBoolean NaBooleanWithNullValueProperty
    {
      get { return DataContainer.GetNaBoolean ("NaBooleanWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaBooleanWithNullValueProperty", value); }
    }

    public NaByte NaByteWithNullValueProperty
    {
      get { return DataContainer.GetNaByte ("NaByteWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaByteWithNullValueProperty", value); }
    }

    public NaDateTime NaDateWithNullValueProperty
    {
      get { return DataContainer.GetNaDateTime ("NaDateWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDateWithNullValueProperty", value); }
    }

    public NaDateTime NaDateTimeWithNullValueProperty
    {
      get { return DataContainer.GetNaDateTime ("NaDateTimeWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDateTimeWithNullValueProperty", value); }
    }

    public NaDecimal NaDecimalWithNullValueProperty
    {
      get { return DataContainer.GetNaDecimal ("NaDecimalWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDecimalWithNullValueProperty", value); }
    }

    public NaDouble NaDoubleWithNullValueProperty
    {
      get { return DataContainer.GetNaDouble ("NaDoubleWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaDoubleWithNullValueProperty", value); }
    }

    public NaGuid NaGuidWithNullValueProperty
    {
      get { return DataContainer.GetNaGuid ("NaGuidWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaGuidWithNullValueProperty", value); }
    }

    public NaInt16 NaInt16WithNullValueProperty
    {
      get { return DataContainer.GetNaInt16 ("NaInt16WithNullValueProperty"); }
      set { DataContainer.SetValue ("NaInt16WithNullValueProperty", value); }
    }

    public NaInt32 NaInt32WithNullValueProperty
    {
      get { return DataContainer.GetNaInt32 ("NaInt32WithNullValueProperty"); }
      set { DataContainer.SetValue ("NaInt32WithNullValueProperty", value); }
    }

    public NaInt64 NaInt64WithNullValueProperty
    {
      get { return DataContainer.GetNaInt64 ("NaInt64WithNullValueProperty"); }
      set { DataContainer.SetValue ("NaInt64WithNullValueProperty", value); }
    }

    public NaSingle NaSingleWithNullValueProperty
    {
      get { return DataContainer.GetNaSingle ("NaSingleWithNullValueProperty"); }
      set { DataContainer.SetValue ("NaSingleWithNullValueProperty", value); }
    }

    public byte[] NullableBinaryProperty
    {
      get { return DataContainer.GetBytes ("NullableBinaryProperty"); }
      set { DataContainer.SetValue ("NullableBinaryProperty", value); }
    }
  }
}
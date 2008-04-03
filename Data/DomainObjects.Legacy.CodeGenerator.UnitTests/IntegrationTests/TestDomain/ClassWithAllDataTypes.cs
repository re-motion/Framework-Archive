using System;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.IntegrationTests.TestDomain
{
  public class ClassWithAllDataTypes : BindableDomainObject
  {
    // types

    public enum EnumType
    {
      DummyEntry = 0
    }

    // static members and constants

    public static new ClassWithAllDataTypes GetObject (ObjectID id)
    {
      return (ClassWithAllDataTypes) DomainObject.GetObject (id);
    }

    public static new ClassWithAllDataTypes GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (ClassWithAllDataTypes) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

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
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
      // For any code that should run when a DomainObject is loaded, OnLoaded () should be overridden.
    }

    // methods and properties

    public bool BooleanProperty
    {
      get { return (bool) DataContainer["BooleanProperty"]; }
      set { DataContainer["BooleanProperty"] = value; }
    }

    public byte ByteProperty
    {
      get { return (byte) DataContainer["ByteProperty"]; }
      set { DataContainer["ByteProperty"] = value; }
    }

    public DateTime DateProperty
    {
      get { return (DateTime) DataContainer["DateProperty"]; }
      set { DataContainer["DateProperty"] = value; }
    }

    public DateTime DateTimeProperty
    {
      get { return (DateTime) DataContainer["DateTimeProperty"]; }
      set { DataContainer["DateTimeProperty"] = value; }
    }

    public decimal DecimalProperty
    {
      get { return (decimal) DataContainer["DecimalProperty"]; }
      set { DataContainer["DecimalProperty"] = value; }
    }

    public double DoubleProperty
    {
      get { return (double) DataContainer["DoubleProperty"]; }
      set { DataContainer["DoubleProperty"] = value; }
    }

    public ClassWithAllDataTypes.EnumType EnumProperty
    {
      get { return (ClassWithAllDataTypes.EnumType) DataContainer["EnumProperty"]; }
      set { DataContainer["EnumProperty"] = value; }
    }

    public Guid GuidProperty
    {
      get { return (Guid) DataContainer["GuidProperty"]; }
      set { DataContainer["GuidProperty"] = value; }
    }

    public short Int16Property
    {
      get { return (short) DataContainer["Int16Property"]; }
      set { DataContainer["Int16Property"] = value; }
    }

    public int Int32Property
    {
      get { return (int) DataContainer["Int32Property"]; }
      set { DataContainer["Int32Property"] = value; }
    }

    public long Int64Property
    {
      get { return (long) DataContainer["Int64Property"]; }
      set { DataContainer["Int64Property"] = value; }
    }

    public float SingleProperty
    {
      get { return (float) DataContainer["SingleProperty"]; }
      set { DataContainer["SingleProperty"] = value; }
    }

    public string StringProperty
    {
      get { return (string) DataContainer["StringProperty"]; }
      set { DataContainer["StringProperty"] = value; }
    }

    public byte[] BinaryProperty
    {
      get { return (byte[]) DataContainer["BinaryProperty"]; }
      set { DataContainer["BinaryProperty"] = value; }
    }

    public NaBoolean NaBooleanProperty
    {
      get { return (NaBoolean) DataContainer["NaBooleanProperty"]; }
      set { DataContainer["NaBooleanProperty"] = value; }
    }

    public NaByte NaByteProperty
    {
      get { return (NaByte) DataContainer["NaByteProperty"]; }
      set { DataContainer["NaByteProperty"] = value; }
    }

    public NaDateTime NaDateProperty
    {
      get { return (NaDateTime) DataContainer["NaDateProperty"]; }
      set { DataContainer["NaDateProperty"] = value; }
    }

    public NaDateTime NaDateTimeProperty
    {
      get { return (NaDateTime) DataContainer["NaDateTimeProperty"]; }
      set { DataContainer["NaDateTimeProperty"] = value; }
    }

    public NaDecimal NaDecimalProperty
    {
      get { return (NaDecimal) DataContainer["NaDecimalProperty"]; }
      set { DataContainer["NaDecimalProperty"] = value; }
    }

    public NaDouble NaDoubleProperty
    {
      get { return (NaDouble) DataContainer["NaDoubleProperty"]; }
      set { DataContainer["NaDoubleProperty"] = value; }
    }

    public NaGuid NaGuidProperty
    {
      get { return (NaGuid) DataContainer["NaGuidProperty"]; }
      set { DataContainer["NaGuidProperty"] = value; }
    }

    public NaInt16 NaInt16Property
    {
      get { return (NaInt16) DataContainer["NaInt16Property"]; }
      set { DataContainer["NaInt16Property"] = value; }
    }

    public NaInt32 NaInt32Property
    {
      get { return (NaInt32) DataContainer["NaInt32Property"]; }
      set { DataContainer["NaInt32Property"] = value; }
    }

    public NaInt64 NaInt64Property
    {
      get { return (NaInt64) DataContainer["NaInt64Property"]; }
      set { DataContainer["NaInt64Property"] = value; }
    }

    public NaSingle NaSingleProperty
    {
      get { return (NaSingle) DataContainer["NaSingleProperty"]; }
      set { DataContainer["NaSingleProperty"] = value; }
    }

    public string StringWithNullValueProperty
    {
      get { return (string) DataContainer["StringWithNullValueProperty"]; }
      set { DataContainer["StringWithNullValueProperty"] = value; }
    }

    public NaBoolean NaBooleanWithNullValueProperty
    {
      get { return (NaBoolean) DataContainer["NaBooleanWithNullValueProperty"]; }
      set { DataContainer["NaBooleanWithNullValueProperty"] = value; }
    }

    public NaByte NaByteWithNullValueProperty
    {
      get { return (NaByte) DataContainer["NaByteWithNullValueProperty"]; }
      set { DataContainer["NaByteWithNullValueProperty"] = value; }
    }

    public NaDateTime NaDateWithNullValueProperty
    {
      get { return (NaDateTime) DataContainer["NaDateWithNullValueProperty"]; }
      set { DataContainer["NaDateWithNullValueProperty"] = value; }
    }

    public NaDateTime NaDateTimeWithNullValueProperty
    {
      get { return (NaDateTime) DataContainer["NaDateTimeWithNullValueProperty"]; }
      set { DataContainer["NaDateTimeWithNullValueProperty"] = value; }
    }

    public NaDecimal NaDecimalWithNullValueProperty
    {
      get { return (NaDecimal) DataContainer["NaDecimalWithNullValueProperty"]; }
      set { DataContainer["NaDecimalWithNullValueProperty"] = value; }
    }

    public NaDouble NaDoubleWithNullValueProperty
    {
      get { return (NaDouble) DataContainer["NaDoubleWithNullValueProperty"]; }
      set { DataContainer["NaDoubleWithNullValueProperty"] = value; }
    }

    public NaGuid NaGuidWithNullValueProperty
    {
      get { return (NaGuid) DataContainer["NaGuidWithNullValueProperty"]; }
      set { DataContainer["NaGuidWithNullValueProperty"] = value; }
    }

    public NaInt16 NaInt16WithNullValueProperty
    {
      get { return (NaInt16) DataContainer["NaInt16WithNullValueProperty"]; }
      set { DataContainer["NaInt16WithNullValueProperty"] = value; }
    }

    public NaInt32 NaInt32WithNullValueProperty
    {
      get { return (NaInt32) DataContainer["NaInt32WithNullValueProperty"]; }
      set { DataContainer["NaInt32WithNullValueProperty"] = value; }
    }

    public NaInt64 NaInt64WithNullValueProperty
    {
      get { return (NaInt64) DataContainer["NaInt64WithNullValueProperty"]; }
      set { DataContainer["NaInt64WithNullValueProperty"] = value; }
    }

    public NaSingle NaSingleWithNullValueProperty
    {
      get { return (NaSingle) DataContainer["NaSingleWithNullValueProperty"]; }
      set { DataContainer["NaSingleWithNullValueProperty"] = value; }
    }

    public byte[] NullableBinaryProperty
    {
      get { return (byte[]) DataContainer["NullableBinaryProperty"]; }
      set { DataContainer["NullableBinaryProperty"] = value; }
    }

  }
}

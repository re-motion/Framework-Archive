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

  protected ClassWithAllDataTypes (DataContainer dataContainer) : base (dataContainer)
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
    set { DataContainer["BooleanProperty"] = value; }
  }

  public byte ByteProperty
  {
    get { return DataContainer.GetByte ("ByteProperty"); }
    set { DataContainer["ByteProperty"] = value; }
  }

  public char CharProperty
  {
    get { return DataContainer.GetChar ("CharProperty"); }
    set { DataContainer["CharProperty"] = value; }
  }

  public DateTime DateTimeProperty
  {
    get { return DataContainer.GetDateTime ("DateTimeProperty"); }
    set { DataContainer["DateTimeProperty"] = value; }
  }

  public decimal DecimalProperty
  {
    get { return DataContainer.GetDecimal ("DecimalProperty"); }
    set { DataContainer["DecimalProperty"] = value; }
  }

  public double DoubleProperty
  {
    get { return DataContainer.GetDouble ("DoubleProperty"); }
    set { DataContainer["DoubleProperty"] = value; }
  }

  public EnumType EnumProperty
  {
    get { return (EnumType) DataContainer["EnumProperty"]; }
    set { DataContainer["EnumProperty"] = value; }
  }

  public Guid GuidProperty
  {
    get { return DataContainer.GetGuid ("GuidProperty"); }
    set { DataContainer["GuidProperty"] = value; }
  }

  public short Int16Property
  {
    get { return DataContainer.GetInt16 ("Int16Property"); }
    set { DataContainer["Int16Property"] = value; }
  }

  public int Int32Property
  {
    get { return DataContainer.GetInt32 ("Int32Property"); }
    set { DataContainer["Int32Property"] = value; }
  }

  public long Int64Property
  {
    get { return DataContainer.GetInt64 ("Int64Property"); }
    set { DataContainer["Int64Property"] = value; }
  }

  public float SingleProperty
  {
    get { return DataContainer.GetSingle ("SingleProperty"); }
    set { DataContainer["SingleProperty"] = value; }
  }

  public string StringProperty
  {
    get { return DataContainer.GetString ("StringProperty"); }
    set { DataContainer["StringProperty"] = value; }
  }

  public NaBoolean NaBooleanProperty
  {
    get { return DataContainer.GetNaBoolean ("NaBooleanProperty"); }
    set { DataContainer["NaBooleanProperty"] = value; }
  }

  public NaDateTime NaDateTimeProperty
  {
    get { return DataContainer.GetNaDateTime ("NaDateTimeProperty"); }
    set { DataContainer["NaDateTimeProperty"] = value; }
  }

  public NaDouble NaDoubleProperty
  {
    get { return DataContainer.GetNaDouble ("NaDoubleProperty"); }
    set { DataContainer["NaDoubleProperty"] = value; }
  }

  public NaInt32 NaInt32Property
  {
    get { return DataContainer.GetNaInt32 ("NaInt32Property"); }
    set { DataContainer["NaInt32Property"] = value; }
  }

  public string StringWithNullValueProperty
  {
    get { return DataContainer.GetString ("StringWithNullValueProperty"); }
    set { DataContainer["StringWithNullValueProperty"] = value; }
  }

  public NaBoolean NaBooleanWithNullValueProperty
  {
    get { return DataContainer.GetNaBoolean ("NaBooleanWithNullValueProperty"); }
    set { DataContainer["NaBooleanWithNullValueProperty"] = value; }
  }

  public NaDateTime NaDateTimeWithNullValueProperty
  {
    get { return DataContainer.GetNaDateTime ("NaDateTimeWithNullValueProperty"); }
    set { DataContainer["NaDateTimeWithNullValueProperty"] = value; }
  }

  public NaDouble NaDoubleWithNullValueProperty
  {
    get { return DataContainer.GetNaDouble ("NaDoubleWithNullValueProperty"); }
    set { DataContainer["NaDoubleWithNullValueProperty"] = value; }
  }

  public NaInt32 NaInt32WithNullValueProperty
  {
    get { return DataContainer.GetNaInt32 ("NaInt32WithNullValueProperty"); }
    set { DataContainer["NaInt32WithNullValueProperty"] = value; }
  }
}
}
using System;
using Rubicon.NullableValueTypes;

using Rubicon.Globalization;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Domain
{
[MultiLingualResources ("Rubicon.Data.DomainObjects.ObjectBinding.Web.Test.Globalization.ClassWithAllDataTypes")]
public class ClassWithAllDataTypes : BindableDomainObject
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
    return (ClassWithAllDataTypes) BindableDomainObject.GetObject (id);
  }

  // member fields
  
  // construction and disposing
  
  public ClassWithAllDataTypes ()
  {
  }

  public ClassWithAllDataTypes (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected ClassWithAllDataTypes (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

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

  public char CharProperty
  {
    get { return DataContainer.GetChar ("CharProperty"); }
    set { DataContainer.SetValue ("CharProperty", value); }
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

  public DateTime ReadOnlyDateTimeProperty
  {
    get { return DataContainer.GetDateTime ("DateTimeProperty"); }
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

  public NaBoolean NaBooleanProperty
  {
    get { return DataContainer.GetNaBoolean ("NaBooleanProperty"); }
    set { DataContainer.SetValue ("NaBooleanProperty", value); }
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

  public NaDouble NaDoubleProperty
  {
    get { return DataContainer.GetNaDouble ("NaDoubleProperty"); }
    set { DataContainer.SetValue ("NaDoubleProperty", value); }
  }

  public NaInt32 NaInt32Property
  {
    get { return DataContainer.GetNaInt32 ("NaInt32Property"); }
    set { DataContainer.SetValue ("NaInt32Property", value); }
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

  public NaDouble NaDoubleWithNullValueProperty
  {
    get { return DataContainer.GetNaDouble ("NaDoubleWithNullValueProperty"); }
    set { DataContainer.SetValue ("NaDoubleWithNullValueProperty", value); }
  }

  public NaInt32 NaInt32WithNullValueProperty
  {
    get { return DataContainer.GetNaInt32 ("NaInt32WithNullValueProperty"); }
    set { DataContainer.SetValue ("NaInt32WithNullValueProperty", value); }
  }

  public ClassForRelationTest ClassForRelationTestMandatory
  {
    get { return (ClassForRelationTest) GetRelatedObject ("ClassForRelationTestMandatory"); }
    set { SetRelatedObject ("ClassForRelationTestMandatory", value); }
  }

  public ClassForRelationTest ClassForRelationTestOptional
  {
    get { return (ClassForRelationTest) GetRelatedObject ("ClassForRelationTestOptional"); }
    set { SetRelatedObject ("ClassForRelationTestOptional", value); }
  }

  public DomainObjectCollection ClassesForRelationTestMandatoryNavigateOnly
  {
    get { return GetRelatedObjects ("ClassesForRelationTestMandatoryNavigateOnly"); }
  }

  public DomainObjectCollection ClassesForRelationTestOptionalNavigateOnly
  {
    get { return GetRelatedObjects ("ClassesForRelationTestOptionalNavigateOnly"); }
  }
}
}
using System;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
{
  [MultiLingualResources ("Rubicon.Data.DomainObjects.Web.Test.Globalization.ClassWithAllDataTypes")]
  [Serializable]
  [DBTable ("TableWithAllDataTypes")]
  [Instantiable]
  [RpaTest]
  public abstract class ClassWithAllDataTypes : BindableDomainObject
  {
    // types
    [EnumDescriptionResource ("Rubicon.Data.DomainObjects.Web.Test.Globalization.ClassWithAllDataTypes")]
    public enum EnumType
    {
      Value0 = 0,
      Value1 = 1,
      Value2 = 2
    }

    // static members and constants

    public static ClassWithAllDataTypes NewObject ()
    {
      return DomainObject.NewObject<ClassWithAllDataTypes> ().With();
    }

    public new static ClassWithAllDataTypes GetObject (ObjectID id)
    {
      return DomainObject.GetObject<ClassWithAllDataTypes> (id);
    }

    public new static ClassWithAllDataTypes GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (new CurrentTransactionScope (clientTransaction))
      {
        return DomainObject.GetObject<ClassWithAllDataTypes> (id);
      }
    }

    protected ClassWithAllDataTypes ()
    {
    }

    [StorageClassNone]
    public string[] StringArray
    {
      get { return DelimitedStringArrayProperty.Split (';'); }
      set
      {
        if (value == null)
          DelimitedStringArrayProperty = string.Empty;
        else
          DelimitedStringArrayProperty = string.Join (";", value);
      }
    }

    [DBColumn ("DelimitedStringArray")]
    [StringProperty (IsNullable = false, MaximumLength = 1000)]
    protected abstract string DelimitedStringArrayProperty { get; set; }


    [StorageClassNone]
    public string[] NullStringArray
    {
      get
      {
        string delimitedNullStringArray = DelimitedNullStringArrayProperty;

        if (delimitedNullStringArray == null)
          return null;

        return delimitedNullStringArray.Split (';');
      }
      set
      {
        if (value == null)
          DelimitedNullStringArrayProperty = null;
        else
          DelimitedNullStringArrayProperty = string.Join (";", value);
      }
    }

    [DBColumn ("DelimitedNullStringArray")]
    [StringProperty (MaximumLength = 1000)]
    protected abstract string DelimitedNullStringArrayProperty { get; set; }

    [StorageClassNone]
    public new DataContainer DataContainer
    {
      get { return base.DataContainer; }
    }

    [StorageClassNone]
    public ObjectID ObjectID
    {
      get { return base.ID; }
    }

    [DBColumn ("Boolean")]
    public abstract bool BooleanProperty { get; set; }

    [DBColumn ("Byte")]
    public abstract byte ByteProperty { get; set; }

    [DBColumn ("Date")]
    [DateType (DateTypeEnum.Date)]
    public abstract DateTime DateProperty { get; set; }

    [DBColumn ("DateTime")]
    public abstract DateTime DateTimeProperty { get; set; }

    [StorageClassNone]
    public DateTime ReadOnlyDateTimeProperty { get { return DateProperty; } }

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

    [DBColumn ("String")]
    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string StringProperty { get; set; }

    [DBColumn ("StringWithoutMaxLength")]
    [StringProperty (IsNullable = false)]
    public abstract string StringPropertyWithoutMaxLength { get; set; }

    [DBColumn ("Binary")]
    [BinaryProperty (IsNullable = false)]
    public abstract byte[] BinaryProperty { get; set; }

    [DBColumn ("NaBoolean")]
    public abstract bool? NaBooleanProperty { get; set; }

    [DBColumn ("NaByte")]
    public abstract byte? NaByteProperty { get; set; }

    [DBColumn ("NaDate")]
    [DateType (DateTypeEnum.Date)]
    public abstract DateTime? NaDateProperty { get; set; }

    [DBColumn ("NaDateTime")]
    public abstract DateTime? NaDateTimeProperty { get; set; }

    [DBColumn ("NaDecimal")]
    public abstract decimal? NaDecimalProperty { get; set; }

    [DBColumn ("NaDouble")]
    public abstract double? NaDoubleProperty { get; set; }

    [DBColumn ("NaEnum")]
    public abstract EnumType? NaEnumProperty { get; set; }

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

    [DBColumn ("StringWithNullValue")]
    [StringProperty (MaximumLength = 100)]
    public abstract string StringWithNullValueProperty { get; set; }

    [DBColumn ("NaBooleanWithNullValue")]
    public abstract bool? NaBooleanWithNullValueProperty { get; set; }

    [DBColumn ("NaByteWithNullValue")]
    public abstract byte? NaByteWithNullValueProperty { get; set; }

    [DBColumn ("NaDateWithNullValue")]
    [DateType (DateTypeEnum.Date)]
    public abstract DateTime? NaDateWithNullValueProperty { get; set; }

    [DBColumn ("NaDateTimeWithNullValue")]
    public abstract DateTime? NaDateTimeWithNullValueProperty { get; set; }

    [DBColumn ("NaDecimalWithNullValue")]
    public abstract decimal? NaDecimalWithNullValueProperty { get; set; }

    [DBColumn ("NaDoubleWithNullValue")]
    public abstract double? NaDoubleWithNullValueProperty { get; set; }

    [DBColumn ("NaEnumWithNullValue")]
    public abstract EnumType? NaEnumWithNullValueProperty { get; set; }

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

    [DBColumn ("NullableBinary")]
    [BinaryProperty (MaximumLength = 1000000)]
    public abstract byte[] NullableBinaryProperty { get; set; }

    [DBColumn ("TableForRelationTestMandatory")]
    [DBBidirectionalRelation ("ClassesWithAllDataTypesMandatoryNavigateOnly")]
    [Mandatory]
    public abstract ClassForRelationTest ClassForRelationTestMandatory { get; set; }

    [DBColumn ("TableForRelationTestOptional")]
    [DBBidirectionalRelation ("ClassesWithAllDataTypesOptionalNavigateOnly")]
    public abstract ClassForRelationTest ClassForRelationTestOptional { get; set; }

    [IsReadOnly]
    [DBBidirectionalRelation ("ClassWithAllDataTypesMandatory")]
    [Mandatory]
    public abstract ObjectList<ClassForRelationTest> ClassesForRelationTestMandatoryNavigateOnly { get; }

    [IsReadOnly]
    [DBBidirectionalRelation ("ClassWithAllDataTypesOptional")]
    public abstract ObjectList<ClassForRelationTest> ClassesForRelationTestOptionalNavigateOnly { get; }
  }
}
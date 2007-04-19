using System;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
{
  [MultiLingualResources ("Rubicon.Data.DomainObjects.Web.Test.Globalization.ClassWithAllDataTypes")]
  [Serializable]
  [DBTable (Name = "TableWithAllDataTypes")]
  [NotAbstract]
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

    public ClassWithAllDataTypes ()
    {
    }

    protected ClassWithAllDataTypes (DataContainer dataContainer)
      : base (dataContainer)
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

    [DBColumn ("StringWithNullValue")]
    [StringProperty (MaximumLength = 100)]
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
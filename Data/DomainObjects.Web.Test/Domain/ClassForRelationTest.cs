using System;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Globalization;

namespace Rubicon.Data.DomainObjects.Web.Test.Domain
{
  [MultiLingualResources ("Rubicon.Data.DomainObjects.Web.Test.Globalization.ClassForRelationTest")]
  [Serializable]
  [DBTable (Name = "TableForRelationTest")]
  [NotAbstract]
  [RpaTest]
  public abstract class ClassForRelationTest: BindableDomainObject
  {
    public static ClassForRelationTest NewObject()
    {
      return DomainObject.NewObject<ClassForRelationTest> ().With ();
    }


    public ClassForRelationTest()
    {
    }

    protected ClassForRelationTest (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    public override string DisplayName
    {
      get { return Name; }
    }

    [StorageClassNone]
    public ClassWithAllDataTypes.EnumType EnumProperty
    {
      get { return ClassWithAllDataTypes.EnumType.Value0; }
    }

    [ItemType (typeof (ClassWithAllDataTypes))]
    [IsReadOnly]
    [StorageClassNone]
    public DomainObjectCollection ComputedList
    {
      get { return null; }
    }

    [DBColumn ("TableWithAllDataTypesMandatory")]
    [DBBidirectionalRelation ("ClassesForRelationTestMandatoryNavigateOnly")]
    [Mandatory]
    public abstract ClassWithAllDataTypes ClassWithAllDataTypesMandatory {get; set;}

    [DBColumn ("TableWithAllDataTypesOptional")]
    [DBBidirectionalRelation ("ClassesForRelationTestOptionalNavigateOnly")]
    public abstract ClassWithAllDataTypes ClassWithAllDataTypesOptional { get; set;}

    [DBBidirectionalRelation ("ClassForRelationTestMandatory")]
    [Mandatory]
    [IsReadOnly]
    public virtual ObjectList<ClassWithAllDataTypes> ClassesWithAllDataTypesMandatoryNavigateOnly
    {
      get { return (ObjectList<ClassWithAllDataTypes>) GetRelatedObjects(); }
    }

    [DBBidirectionalRelation ("ClassForRelationTestOptional")]
    [IsReadOnly]
    public virtual ObjectList<ClassWithAllDataTypes> ClassesWithAllDataTypesOptionalNavigateOnly
    {
      get { return (ObjectList<ClassWithAllDataTypes>)GetRelatedObjects(); }
    }
  }
}
using System;

using Remotion.Globalization;
using Remotion.NullableValueTypes;

using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.Web.Legacy.Test.Domain
{
[MultiLingualResources ("Remotion.Data.DomainObjects.Web.Legacy.Test.Globalization.ClassForRelationTest")]
[Serializable]
public class ClassForRelationTest : BindableDomainObject
{
  // types

  // static members and constants
  
  public static new ClassForRelationTest GetObject (ObjectID id)
  {
    return (ClassForRelationTest) BindableDomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing
  
  public ClassForRelationTest ()
  {
  }

  protected ClassForRelationTest (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public string Name
  {
    get { return (string) DataContainer.GetValue ("Name"); }
    set { DataContainer.SetValue ("Name", value); }
  } 

  public override string DisplayName
  {
    get { return Name; }
  }

  public ClassWithAllDataTypes.EnumType EnumProperty
  {
    get { return ClassWithAllDataTypes.EnumType.Value0; }
  }

  [ItemType(typeof(ClassWithAllDataTypes))]
  [IsReadOnly]
  public DomainObjectCollection ComputedList
  {
    get { return null; }
  }

  public ClassWithAllDataTypes ClassWithAllDataTypesMandatory
  {
    get { return (ClassWithAllDataTypes) GetRelatedObject ("ClassWithAllDataTypesMandatory"); }
    set { SetRelatedObject ("ClassWithAllDataTypesMandatory", value); }
  }

  public ClassWithAllDataTypes ClassWithAllDataTypesOptional
  {
    get { return (ClassWithAllDataTypes) GetRelatedObject ("ClassWithAllDataTypesOptional"); }
    set { SetRelatedObject ("ClassWithAllDataTypesOptional", value); }
  }

  [IsReadOnly]
  public DomainObjectCollection ClassesWithAllDataTypesMandatoryNavigateOnly
  {
    get { return GetRelatedObjects ("ClassesWithAllDataTypesMandatoryNavigateOnly"); }
  }

  [IsReadOnly]
  public DomainObjectCollection ClassesWithAllDataTypesOptionalNavigateOnly
  {
    get { return GetRelatedObjects ("ClassesWithAllDataTypesOptionalNavigateOnly"); }
  }
}
}
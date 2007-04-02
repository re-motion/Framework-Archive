using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_DerivedClassWithEntityFromBaseClassWithHierarchy")]
  public class DerivedClassWithEntityFromBaseClassWithHierarchy : DerivedClassWithEntityWithHierarchy
  {
    // types

    // static members and constants

    public static new DerivedClassWithEntityFromBaseClassWithHierarchy GetObject (ObjectID id)
    {
      return (DerivedClassWithEntityFromBaseClassWithHierarchy) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public DerivedClassWithEntityFromBaseClassWithHierarchy ()
    {
    }

    protected DerivedClassWithEntityFromBaseClassWithHierarchy (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public DerivedClassWithEntityFromBaseClassWithHierarchy ParentDerivedClassWithEntityFromBaseClassWithHierarchy
    {
      get { return (DerivedClassWithEntityFromBaseClassWithHierarchy) GetRelatedObject ("ParentDerivedClassWithEntityFromBaseClassWithHierarchy"); }
      set { SetRelatedObject ("ParentDerivedClassWithEntityFromBaseClassWithHierarchy", value); }
    }

    public DomainObjectCollection ChildDerivedClassesWithEntityFromBaseClassWithHierarchy
    {
      get { return GetRelatedObjects ("ChildDerivedClassesWithEntityFromBaseClassWithHierarchy"); }
    }

    public Client ClientFromDerivedClassWithEntityFromBaseClass
    {
      get { return (Client) GetRelatedObject ("ClientFromDerivedClassWithEntityFromBaseClass"); }
      set { SetRelatedObject ("ClientFromDerivedClassWithEntityFromBaseClass", value); }
    }

    public FileSystemItem FileSystemItemFromDerivedClassWithEntityFromBaseClass
    {
      get { return (FileSystemItem) GetRelatedObject ("FileSystemItemFromDerivedClassWithEntityFromBaseClass"); }
      set { SetRelatedObject ("FileSystemItemFromDerivedClassWithEntityFromBaseClass", value); }
    }
  }
}

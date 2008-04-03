using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class DerivedClassWithEntityFromBaseClassWithHierarchy : DerivedClassWithEntityWithHierarchy
  {
    // types

    // static members and constants

    public static new DerivedClassWithEntityFromBaseClassWithHierarchy GetObject (ObjectID id)
    {
      return (DerivedClassWithEntityFromBaseClassWithHierarchy) RepositoryAccessor.GetObject (id, false);
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

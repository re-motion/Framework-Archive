using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class DerivedClassWithEntityWithHierarchy : AbstractBaseClassWithHierarchy
  {
    // types

    // static members and constants

    public static new DerivedClassWithEntityWithHierarchy GetObject (ObjectID id)
    {
      return (DerivedClassWithEntityWithHierarchy) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public DerivedClassWithEntityWithHierarchy ()
    {
    }

    protected DerivedClassWithEntityWithHierarchy (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public DerivedClassWithEntityWithHierarchy ParentDerivedClassWithEntityWithHierarchy
    {
      get { return (DerivedClassWithEntityWithHierarchy) GetRelatedObject ("ParentDerivedClassWithEntityWithHierarchy"); }
      set { SetRelatedObject ("ParentDerivedClassWithEntityWithHierarchy", value); }
    }

    public DomainObjectCollection ChildDerivedClassesWithEntityWithHierarchy
    {
      get { return GetRelatedObjects ("ChildDerivedClassesWithEntityWithHierarchy"); }
    }

    public Client ClientFromDerivedClassWithEntity
    {
      get { return (Client) GetRelatedObject ("ClientFromDerivedClassWithEntity"); }
      set { SetRelatedObject ("ClientFromDerivedClassWithEntity", value); }
    }

    public FileSystemItem FileSystemItemFromDerivedClassWithEntity
    {
      get { return (FileSystemItem) GetRelatedObject ("FileSystemItemFromDerivedClassWithEntity"); }
      set { SetRelatedObject ("FileSystemItemFromDerivedClassWithEntity", value); }
    }
  }
}

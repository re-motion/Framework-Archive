using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  public class DerivedClassWithEntityWithHierarchy : AbstractBaseClassWithHierarchy
  {
    // types

    // static members and constants

    public static new DerivedClassWithEntityWithHierarchy GetObject (ObjectID id)
    {
      return (DerivedClassWithEntityWithHierarchy) DomainObject.GetObject (id);
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

    //TODO: change property type back to "Client" after the problem described in 
    //      IntegrationTest.SetRelationPropertyToObjectOfInvalidTypeForThisRelation is resolved 
    public DomainObject ClientFromDerivedClassWithEntity
    {
      get { return (DomainObject) GetRelatedObject ("ClientFromDerivedClassWithEntity"); }
      set { SetRelatedObject ("ClientFromDerivedClassWithEntity", value); }
    }

    public FileSystemItem FileSystemItemFromDerivedClassWithEntity
    {
      get { return (FileSystemItem) GetRelatedObject ("FileSystemItemFromDerivedClassWithEntity"); }
      set { SetRelatedObject ("FileSystemItemFromDerivedClassWithEntity", value); }
    }
  }
}

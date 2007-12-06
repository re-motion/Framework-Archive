using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public abstract class AbstractBaseClassWithHierarchy : DomainObject
  {
    // types

    // static members and constants

    public static AbstractBaseClassWithHierarchy GetObject (ObjectID id)
    {
      return (AbstractBaseClassWithHierarchy) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public AbstractBaseClassWithHierarchy ()
    {
    }

    protected AbstractBaseClassWithHierarchy (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer.GetValue ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    public AbstractBaseClassWithHierarchy ParentAbstractBaseClassWithHierarchy
    {
      get { return (AbstractBaseClassWithHierarchy) GetRelatedObject ("ParentAbstractBaseClassWithHierarchy"); }
      set { SetRelatedObject ("ParentAbstractBaseClassWithHierarchy", value); }
    }

    public DomainObjectCollection ChildAbstractBaseClassesWithHierarchy
    {
      get { return GetRelatedObjects ("ChildAbstractBaseClassesWithHierarchy"); }
    }

    public Client ClientFromAbstractBaseClass
    {
      get { return (Client) GetRelatedObject ("ClientFromAbstractBaseClass"); }
      set { SetRelatedObject ("ClientFromAbstractBaseClass", value); }
    }

    public FileSystemItem FileSystemItemFromAbstractBaseClass
    {
      get { return (FileSystemItem) GetRelatedObject ("FileSystemItemFromAbstractBaseClass"); }
      set { SetRelatedObject ("FileSystemItemFromAbstractBaseClass", value); }
    }
  }
}

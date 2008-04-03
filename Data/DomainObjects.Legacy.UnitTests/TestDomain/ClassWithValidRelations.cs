using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithValidRelations : TestDomainBase
  {
    // types

    // static members and constants

    public static ClassWithValidRelations GetObject (ObjectID id)
    {
      return (ClassWithValidRelations) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public ClassWithValidRelations ()
    {
    }

    protected ClassWithValidRelations (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public ClassWithGuidKey ClassWithGuidKeyOptional
    {
      get { return (ClassWithGuidKey) GetRelatedObject ("ClassWithGuidKeyOptional"); }
      set { SetRelatedObject ("ClassWithGuidKeyOptional", value); }
    }

    public ClassWithGuidKey ClassWithGuidKeyNonOptional
    {
      get { return (ClassWithGuidKey) GetRelatedObject ("ClassWithGuidKeyNonOptional"); }
      set { SetRelatedObject ("ClassWithGuidKeyNonOptional", value); }
    }
  }
}

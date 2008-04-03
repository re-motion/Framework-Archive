using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithInvalidRelation : TestDomainBase
  {
    // types

    // static members and constants

    public static ClassWithInvalidRelation GetObject (ObjectID id)
    {
      return (ClassWithInvalidRelation) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public ClassWithInvalidRelation ()
    {
    }

    protected ClassWithInvalidRelation (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public ClassWithGuidKey ClassWithGuidKey
    {
      get
      {
        return (ClassWithGuidKey) GetRelatedObject ("ClassWithGuidKey");
      }
      set
      {
        SetRelatedObject ("ClassWithGuidKey", value);
      }
    }
  }
}

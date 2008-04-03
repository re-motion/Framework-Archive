using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithoutRelatedClassIDColumnAndDerivation : DomainObject
  {
    // types

    // static members and constants

    public static ClassWithoutRelatedClassIDColumnAndDerivation GetObject (ObjectID id)
    {
      return (ClassWithoutRelatedClassIDColumnAndDerivation) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public ClassWithoutRelatedClassIDColumnAndDerivation ()
    {
    }

    protected ClassWithoutRelatedClassIDColumnAndDerivation (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public Company Company
    {
      get { return (Company) GetRelatedObject ("Company"); }
      set { SetRelatedObject ("Company", value); }
    }
  }
}

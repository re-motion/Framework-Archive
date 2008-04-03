using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithOptionalOneToOneRelationAndOppositeDerivedClass : DomainObject
  {
    // types

    // static members and constants

    public static ClassWithOptionalOneToOneRelationAndOppositeDerivedClass GetObject (ObjectID id)
    {
      return (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public ClassWithOptionalOneToOneRelationAndOppositeDerivedClass ()
    {
    }

    protected ClassWithOptionalOneToOneRelationAndOppositeDerivedClass (DataContainer dataContainer)
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

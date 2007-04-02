using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable (Name = "TableWithoutRelatedClassIDColumnAndDerivation")]
  public class ClassWithoutRelatedClassIDColumnAndDerivation : TestDomainBase
  {
    // types

    // static members and constants

    public static new ClassWithoutRelatedClassIDColumnAndDerivation GetObject (ObjectID id)
    {
      return (ClassWithoutRelatedClassIDColumnAndDerivation) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public ClassWithoutRelatedClassIDColumnAndDerivation ()
    {
    }

    public ClassWithoutRelatedClassIDColumnAndDerivation (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected ClassWithoutRelatedClassIDColumnAndDerivation (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    [DBBidirectionalRelation ("ClassWithoutRelatedClassIDColumnAndDerivation", ContainsForeignKey = true)]
    public Company Company
    {
      get { return (Company) GetRelatedObject ("Company"); }
      set { SetRelatedObject ("Company", value); }
    }
  }
}

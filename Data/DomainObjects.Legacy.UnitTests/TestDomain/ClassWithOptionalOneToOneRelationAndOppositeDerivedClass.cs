using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithOptionalOneToOneRelationAndOppositeDerivedClass : DomainObject
  {
    // types

    // static members and constants

    public static new ClassWithOptionalOneToOneRelationAndOppositeDerivedClass GetObject (ObjectID id)
    {
      return (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass) DomainObject.GetObject (id);
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

using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class ClassWithoutRelatedClassIDColumnAndDerivation : DomainObject
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

  protected ClassWithoutRelatedClassIDColumnAndDerivation (DataContainer dataContainer) : base (dataContainer)
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

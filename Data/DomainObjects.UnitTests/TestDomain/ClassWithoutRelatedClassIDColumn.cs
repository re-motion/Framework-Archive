using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class ClassWithoutRelatedClassIDColumn : DomainObject
{
  // types

  // static members and constants

  public static new ClassWithoutRelatedClassIDColumn GetObject (ObjectID id)
  {
    return (ClassWithoutRelatedClassIDColumn) DomainObject.GetObject (id);
  }

  // member fields

  // construction and disposing

  public ClassWithoutRelatedClassIDColumn ()
  {
  }

  protected ClassWithoutRelatedClassIDColumn (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public Partner Partner
  {
    get { return (Partner) GetRelatedObject ("Partner"); }
    set { SetRelatedObject ("Partner", value); }
  }
}
}

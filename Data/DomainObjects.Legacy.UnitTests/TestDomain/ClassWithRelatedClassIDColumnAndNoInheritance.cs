using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithRelatedClassIDColumnAndNoInheritance : TestDomainBase
  {
    // types

    // static members and constants

    public static new ClassWithRelatedClassIDColumnAndNoInheritance GetObject (ObjectID id)
    {
      return (ClassWithRelatedClassIDColumnAndNoInheritance) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public ClassWithRelatedClassIDColumnAndNoInheritance ()
    {
    }

    protected ClassWithRelatedClassIDColumnAndNoInheritance (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public ClassWithGuidKey ClassWithGuidKey
    {
      get { return (ClassWithGuidKey) GetRelatedObject ("ClassWithGuidKey"); }
      set { SetRelatedObject ("ClassWithGuidKey", value); }
    }
  }
}

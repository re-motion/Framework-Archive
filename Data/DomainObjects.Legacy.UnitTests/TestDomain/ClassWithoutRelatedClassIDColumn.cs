using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class ClassWithoutRelatedClassIDColumn : DomainObject
  {
    // types

    // static members and constants

    public static ClassWithoutRelatedClassIDColumn GetObject (ObjectID id)
    {
      return (ClassWithoutRelatedClassIDColumn) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public ClassWithoutRelatedClassIDColumn ()
    {
    }

    protected ClassWithoutRelatedClassIDColumn (DataContainer dataContainer)
      : base (dataContainer)
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

using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class IndustrialSector : TestDomainBase
  {
    // types

    // static members and constants

    public static IndustrialSector GetObject (ObjectID id)
    {
      return (IndustrialSector) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public IndustrialSector ()
    {
    }

    protected IndustrialSector (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    public DomainObjectCollection Companies
    {
      get { return GetRelatedObjects ("Companies"); }
    }
  }
}

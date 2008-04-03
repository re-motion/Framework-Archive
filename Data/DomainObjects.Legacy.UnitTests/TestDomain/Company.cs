using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  [Serializable]
  public class Company : TestDomainBase
  {
    // types

    // static members and constants

    public static Company GetObject (ObjectID id)
    {
      return (Company) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    // New customers cannot be created directly.
    protected Company ()
    {
    }

    protected Company (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    internal int NamePropertyOfInvalidType
    {
      set { DataContainer["Name"] = value; }
    }

    public string Name
    {
      get { return (string) DataContainer.GetValue ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    public Ceo Ceo
    {
      get { return (Ceo) GetRelatedObject ("Ceo"); }
      set { SetRelatedObject ("Ceo", value); }
    }

    public IndustrialSector IndustrialSector
    {
      get { return (IndustrialSector) GetRelatedObject ("IndustrialSector"); }
      set { SetRelatedObject ("IndustrialSector", value); }
    }

    private ClassWithoutRelatedClassIDColumnAndDerivation ClassWithoutRelatedClassIDColumnAndDerivation
    {
      get
      {
        return (ClassWithoutRelatedClassIDColumnAndDerivation) GetRelatedObject (
            "ClassWithoutRelatedClassIDColumnAndDerivation");
      }
      set
      {
        SetRelatedObject ("ClassWithoutRelatedClassIDColumnAndDerivation", value);
      }
    }
  }
}

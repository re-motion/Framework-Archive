using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class OrganizationalUnit : DomainBase
  {
    // types

    // static members and constants

    public static OrganizationalUnit GetObject (ObjectID id)
    {
      return (OrganizationalUnit) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public OrganizationalUnit ()
    {
    }

    protected OrganizationalUnit (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer.GetValue ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }
  }
}

using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  public class OrganizationalUnit : DomainBase
  {
    // types

    // static members and constants

    public static new OrganizationalUnit GetObject (ObjectID id)
    {
      return (OrganizationalUnit) DomainObject.GetObject (id);
    }

    public static new OrganizationalUnit GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (OrganizationalUnit) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public OrganizationalUnit ()
    {
    }

    public OrganizationalUnit (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected OrganizationalUnit (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return DataContainer.GetString ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }
  }
}

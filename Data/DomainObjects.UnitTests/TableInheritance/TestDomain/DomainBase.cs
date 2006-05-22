using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  public abstract class DomainBase : DomainObject
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    protected DomainBase ()
    {
    }

    protected DomainBase (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    protected DomainBase (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string CreatedBy
    {
      get { return DataContainer.GetString ("CreatedBy"); }
      set { DataContainer.SetValue ("CreatedBy", value); }
    }

    public DateTime CreatedAt
    {
      get { return DataContainer.GetDateTime ("CreatedAt"); }
      set { DataContainer.SetValue ("CreatedAt", value); }
    }

    public Client Client
    {
      get { return (Client) GetRelatedObject ("Client"); }
    }
  }
}

using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Security;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.TestDomain
{
  public class NonSecurableObject : DomainObject
  {
    public NonSecurableObject (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public DataContainer GetDataContainer ()
    {
      return DataContainer;
    }

    public string StringProperty
    {
      get { return (string) DataContainer["StringProperty"]; }
      set { DataContainer["StringProperty"] = value; }
    }

    public NonSecurableObject Parent
    {
      get { return (NonSecurableObject) GetRelatedObject ("Parent"); }
      set { SetRelatedObject ("Parent", value); }
    }

    public DomainObjectCollection Children
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("Children"); }
    }
  }
}

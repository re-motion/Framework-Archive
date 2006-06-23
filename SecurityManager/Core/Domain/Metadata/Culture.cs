using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  public class Culture : BaseSecurityManagerObject
  {
    // types

    // static members and constants

    public static new Culture GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Culture) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Culture GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Culture) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public Culture (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
    }

    public Culture (ClientTransaction clientTransaction, string cultureName)
      : base (clientTransaction)
    {
      DataContainer["CultureName"] = cultureName;
    }

    protected Culture (DataContainer dataContainer)
      : base (dataContainer)
    {
      // This infrastructure constructor is necessary for the DomainObjects framework.
      // Do not remove the constructor or place any code here.
    }

    // methods and properties

    public string CultureName
    {
      get { return (string) DataContainer["CultureName"]; }
    }
  }
}

using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class ObjectSecurityProvider : IObjectSecurityProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public ObjectSecurityProvider ()
    {
    }

    // methods and properties

    public bool HasAccessOnGetAccessor (ISecurableObject securableObject)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);

      SecurityClient securityClient = new SecurityClient ();
      return securityClient.HasAccess (securableObject, AccessType.Get (GeneralAccessType.Read));
    }

    public bool HasAccessOnSetAccessor (ISecurableObject securableObject)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);
      
      SecurityClient securityClient = new SecurityClient ();
      return securityClient.HasAccess (securableObject, AccessType.Get (GeneralAccessType.Edit));
    }
  }
}
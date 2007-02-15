using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  [Obsolete ("Use ObjectSecurityProvider instead. (Version: 1.7.41)", true)]
  public class ObjectSecurityProvider : ObjectSecurityAdapter, IObjectSecurityProvider
  {  
  }
  
  public class ObjectSecurityAdapter : IObjectSecurityAdapter
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public ObjectSecurityAdapter ()
    {
    }

    // methods and properties

    public bool HasAccessOnGetAccessor (ISecurableObject securableObject, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);

      if (SecurityFreeSection.IsActive)
        return true;

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return securityClient.HasPropertyReadAccess (securableObject, propertyName);
    }

    public bool HasAccessOnSetAccessor (ISecurableObject securableObject, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);

      if (SecurityFreeSection.IsActive)
        return true;

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return securityClient.HasPropertyWriteAccess (securableObject, propertyName);
    }
  }
}
using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  [AttributeUsage (AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple=true)]
  public class RequiredMethodPermissionAttribute : Attribute
  {
    private Enum _accessType;

    public RequiredMethodPermissionAttribute (object accessType)
    {
      ArgumentUtility.CheckNotNullAndType ("accessType", accessType, typeof (Enum));

      Type permissionType = accessType.GetType ();
      if (!permissionType.IsDefined (typeof (AccessTypeAttribute), false))
      {
        string message = string.Format (string.Format ("Enumerated Type '{0}' cannot be used as an access type. Valid access types must have the "
                + "Rubicon.Security.AccessTypeAttribute applied.", permissionType.FullName));

        throw new ArgumentException (message, "accessType");
      }

      _accessType = (Enum) accessType;
    }

    public Enum AccessType
    {
      get { return _accessType; }
    }
  }
}

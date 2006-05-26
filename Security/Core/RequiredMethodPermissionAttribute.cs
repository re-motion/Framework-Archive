using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  [AttributeUsage (AttributeTargets.Method, AllowMultiple=false)]
  public class RequiredMethodPermissionAttribute : Attribute
  {
    private Enum[] _accessTypes;

    public RequiredMethodPermissionAttribute (object accessType1)
        : this (new object[] { accessType1 })
    {
    }

    public RequiredMethodPermissionAttribute (object accessType1, object accessType2)
      : this (new object[] { accessType1, accessType2 })
    {
    }

    public RequiredMethodPermissionAttribute (object accessType1, object accessType2, object accessType3)
      : this (new object[] { accessType1, accessType2, accessType3 })
    {
    }

    public RequiredMethodPermissionAttribute (object accessType1, object accessType2, object accessType3, object accessType4)
      : this (new object[] { accessType1, accessType2, accessType3, accessType4 })
    {
    }

    public RequiredMethodPermissionAttribute (object accessType1, object accessType2, object accessType3, object accessType4, object accessType5)
      : this (new object[] { accessType1, accessType2, accessType3, accessType4, accessType5 })
    {
    }

    public RequiredMethodPermissionAttribute (params object[] accessTypes)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("accessTypes", accessTypes);
      ArgumentUtility.CheckItemsType ("accessTypes", accessTypes, typeof (Enum));

      Enum[] accessTypeEnums = new Enum[accessTypes.Length];

      for (int i = 0; i < accessTypes.Length; i++)
        accessTypeEnums[i] = GetAccessType (accessTypes[i]);

      _accessTypes = accessTypeEnums;
    }

    public Enum[] AccessTypes
    {
      get { return _accessTypes; }
    }

    private Enum GetAccessType (object accessType)
    {
      Type permissionType = accessType.GetType ();
      if (!permissionType.IsDefined (typeof (AccessTypeAttribute), false))
      {
        string message = string.Format (string.Format ("Enumerated Type '{0}' cannot be used as an access type. Valid access types must have the "
                + "Rubicon.Security.AccessTypeAttribute applied.", permissionType.FullName));

        throw new ArgumentException (message, "accessType");
      }

      return (Enum) accessType;
    }
  }
}

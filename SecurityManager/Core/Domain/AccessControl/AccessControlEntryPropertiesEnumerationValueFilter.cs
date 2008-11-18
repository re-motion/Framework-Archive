/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// Implementation of <see cref="IEnumerationValueFilter"/> for the <see cref="AccessControlEntry"/> type.
  /// </summary>
  /// <remarks>
  /// The service is applied to the <see cref="AccessControlEntry.TenantCondition"/>, <see cref="AccessControlEntry.GroupCondition"/>, 
  /// and <see cref="AccessControlEntry.UserCondition"/> properties via the <see cref="DisableEnumValuesAttribute"/>.
  /// </remarks>
  public class AccessControlEntryPropertiesEnumerationValueFilter : IEnumerationValueFilter
  {
    public AccessControlEntryPropertiesEnumerationValueFilter ()
    {
    }

    public bool IsEnabled (IEnumerationValueInfo value, IBusinessObject businessObject, IBusinessObjectEnumerationProperty property)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNullAndType<AccessControlEntry> ("businessObject", businessObject);
      ArgumentUtility.CheckNotNull ("property", property);

      AccessControlEntry ace = (AccessControlEntry) businessObject;
      bool isStateful = ace.AccessControlList is StatefulAccessControlList;

      switch (property.Identifier)
      {
        case "TenantCondition":
          return value.IsEnabled && IsTenantConditionEnabled ((TenantCondition) value.Value, isStateful);
        case "GroupCondition":
          return value.IsEnabled && IsGroupConditionEnabled ((GroupCondition) value.Value, isStateful);
        case "UserCondition":
          return value.IsEnabled && IsUserConditionEnabled ((UserCondition) value.Value, isStateful);
        default:
          throw CreateInvalidOperationException ("The property '{0}' is not supported by the '{1}'.", property.Identifier, typeof (AccessControlEntryPropertiesEnumerationValueFilter).FullName);
      }
    }

    private bool IsTenantConditionEnabled (TenantCondition value, bool isStateful)
    {
      switch (value)
      {
        case TenantCondition.None:
          return true;
        case TenantCondition.OwningTenant:
          return isStateful;
        case TenantCondition.SpecificTenant:
          return true;
        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'TenantCondition'.", value);
      }
    }

    private bool IsGroupConditionEnabled (GroupCondition value, bool isStateful)
    {
      switch (value)
      {
        case GroupCondition.None:
          return true;
        case GroupCondition.OwningGroup:
          return isStateful;
        case GroupCondition.SpecificGroup:
          return true;
        case GroupCondition.AnyGroupWithSpecificGroupType:
          return true;
        case GroupCondition.BranchOfOwningGroup:
          return isStateful;
        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'GroupCondition'.", value);
      }
    }

    private bool IsUserConditionEnabled (UserCondition value, bool isStateful)
    {
      switch (value)
      {
        case UserCondition.None:
          return true;
        case UserCondition.Owner:
          return isStateful;
        case UserCondition.SpecificUser:
          return true;
        case UserCondition.SpecificPosition:
          return true;
        default:
          throw CreateInvalidOperationException ("The value '{0}' is not a valid value for 'UserCondition'.", value);
      }
    }

    private InvalidOperationException CreateInvalidOperationException (string message, params object[] args)
    {
      return new InvalidOperationException (string.Format (message, args));
    }
  }
}
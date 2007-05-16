using System;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [EnumDescriptionResource ("Rubicon.SecurityManager.Globalization.Domain.AccessControl.TenantSelection")]
  public enum TenantSelection
  {
    All = 0,
    OwningTenant = 1,
    SpecificTenant = 2
  }
}

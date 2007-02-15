using System;
using Rubicon.Security;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  [SecurityState]
  [EnumDescriptionResource ("Rubicon.SecurityManager.Globalization.Domain.OrganizationalStructure.Delegation")]
  public enum Delegation
  {
    Disabled = 0,
    Enabled = 1
  }
}
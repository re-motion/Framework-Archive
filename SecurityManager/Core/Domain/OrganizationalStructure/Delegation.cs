using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Security;

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
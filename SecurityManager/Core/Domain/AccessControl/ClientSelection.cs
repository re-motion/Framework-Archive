using System;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [EnumDescriptionResource ("Rubicon.SecurityManager.Globalization.Domain.AccessControl.ClientSelection")]
  public enum ClientSelection
  {
    All = 0,
    OwningClient = 1,
    SpecificClient = 2
  }
}

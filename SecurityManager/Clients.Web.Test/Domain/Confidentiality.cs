using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;
using Rubicon.Security;

namespace Rubicon.SecurityManager.Clients.Web.Test.Domain
{
  [SecurityState]
  public enum Confidentiality
  {
    Normal = 0,
    Classified = 1,
    Secret = 2
  }
}

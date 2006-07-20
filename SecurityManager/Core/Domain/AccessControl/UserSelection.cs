using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public enum UserSelection
  {
    All = 0,
    Owner = 1,
    SpecificUser = 2,
    SpecificPosition = 3
  }
}

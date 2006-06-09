using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
public enum GroupSelection
{
  All = 0,
  GroupOfOwner,
  SpecificGroup,
  SpecificGroupType
}
}

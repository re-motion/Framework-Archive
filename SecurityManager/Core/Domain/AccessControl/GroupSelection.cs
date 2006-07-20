using System;

using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Globalization;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  [EnumDescriptionResource ("Rubicon.SecurityManager.Globalization.Domain.AccessControl.GroupSelection")]
  public enum GroupSelection
  {
    All = 0,
    OwningGroup = 1,
    //SpecificGroup = 2,
    //SpecificGroupType = 3
  }
}

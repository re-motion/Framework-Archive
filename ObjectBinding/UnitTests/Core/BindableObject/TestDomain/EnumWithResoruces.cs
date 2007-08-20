using System;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  [EnumDescriptionResource ("Rubicon.ObjectBinding.UnitTests.Core.Globalization.EnumWithResources")]
  public enum EnumWithResources
  {
    Value1 = 1,
    Value2 = 2,
    ValueWithoutResource = 3
  }
}
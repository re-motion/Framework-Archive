using System;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain
{
  [EnumDescriptionResource ("Rubicon.ObjectBinding.UnitTests.Globalization.EnumWithResources")]
  public enum EnumWithResources
  {
    Value1 = 1,
    Value2 = 2,
    ValueWithoutResource = 3
  }
}
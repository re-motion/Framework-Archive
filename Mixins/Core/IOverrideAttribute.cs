using System;

namespace Rubicon.Mixins
{
  public interface IOverrideAttribute
  {
    Type OverriddenType { get; }
  }
}
using System;
using Rubicon.Utilities;

namespace Rubicon.Mixins
{
  public class NonIntroducedAttribute : Attribute
  {
    private readonly Type _suppressedInterface;

    public NonIntroducedAttribute (Type suppressedInterface)
    {
      ArgumentUtility.CheckNotNull ("suppressedInterface", suppressedInterface);
      _suppressedInterface = suppressedInterface;
    }

    public Type SuppressedInterface
    {
      get { return _suppressedInterface; }
    }
  }
}

using System;
using Rubicon.Utilities;

namespace Rubicon.Mixins
{
  public class NotIntroducedAttribute : Attribute
  {
    private readonly Type _suppressedInterface;

    public NotIntroducedAttribute (Type suppressedInterface)
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

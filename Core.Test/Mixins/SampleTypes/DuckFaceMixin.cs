using System;
using Remotion.Mixins;

namespace Remotion.Core.UnitTests.Mixins.SampleTypes
{
  public interface IDuckFaceRequirements
  {
    string MethodImplementedOnBase ();
    string ProtectedMethodImplementedOnBase ();
  }

  public class DuckFaceMixin : Mixin<IDuckFaceRequirements>
  {
    public string CallMethodsOnThis ()
    {
      return "DuckFaceMixin.CallMethodsOnThis-" + This.MethodImplementedOnBase ();
    }

    [OverrideTarget]
    public string MethodImplementedOnBase ()
    {
      return "DuckFaceMixin.MethodImplementedOnBase-" + This.ProtectedMethodImplementedOnBase ();
    }
  }

  public class DuckFaceMixinWithoutOverrides : Mixin<IDuckFaceRequirements>
  {
  }
}

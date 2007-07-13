using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
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

    [Override]
    public string MethodImplementedOnBase ()
    {
      return "DuckFaceMixin.MethodImplementedOnBase-" + This.ProtectedMethodImplementedOnBase ();
    }
  }

  public class DuckFaceMixinWithoutOverrides : Mixin<IDuckFaceRequirements>
  {
  }
}

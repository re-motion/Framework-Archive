using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public interface IDuckBaseRequirements
  {
    string MethodImplementedOnBase ();
    string ProtectedMethodImplementedOnBase ();
  }

  public class DuckBaseMixin : Mixin<object, IDuckBaseRequirements>
  {
    [Override]
    public string MethodImplementedOnBase ()
    {
      return "DuckBaseMixin.MethodImplementedOnBase-" + Base.MethodImplementedOnBase ();
    }

    [Override]
    public string ProtectedMethodImplementedOnBase ()
    {
      return "DuckBaseMixin.ProtectedMethodImplementedOnBase-" + Base.ProtectedMethodImplementedOnBase ();
    }
  }

  public class DuckBaseMixinWithoutOverrides : Mixin<object, IDuckBaseRequirements>
  {
  }
}

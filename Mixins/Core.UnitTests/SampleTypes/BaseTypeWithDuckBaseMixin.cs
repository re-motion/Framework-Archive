using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  [Uses (typeof (DuckBaseMixin))]
  public class BaseTypeWithDuckBaseMixin
  {
    public virtual string MethodImplementedOnBase ()
    {
      return "BaseTypeWithDuckBaseMixin.MethodImplementedOnBase-" + ProtectedMethodImplementedOnBase();
    }

    protected virtual string ProtectedMethodImplementedOnBase ()
    {
      return "BaseTypeWithDuckBaseMixin.ProtectedMethodImplementedOnBase";
    }
  }
}

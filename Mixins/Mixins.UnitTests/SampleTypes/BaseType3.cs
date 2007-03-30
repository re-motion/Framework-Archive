using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBaseType3
  {
    string IfcMethod ();
  }

  public class BaseType3 : IBaseType3
  {
    public string IfcMethod ()
    {
      return "BaseType3.IfcMethod";
    }
  }
}

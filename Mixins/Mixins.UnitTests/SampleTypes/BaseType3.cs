using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBaseType31
  {
    string IfcMethod ();
  }

  public interface IBaseType32
  {
    string IfcMethod ();
  }

  public interface IBaseType33
  {
    string IfcMethod ();
  }

  public interface IBaseType34 : IBaseType33
  {
    new string IfcMethod ();
  }

  public interface IBaseType35
  {
    string IfcMethod2 ();
  }

  [Uses(typeof(BT3Mixin5))]
  public class BaseType3 : IBaseType31, IBaseType32, IBaseType34, IBaseType35
  {
    public virtual string IfcMethod ()
    {
      return "BaseType3.IfcMethod";
    }

    public string IfcMethod2 ()
    {
      return "BaseType3.IfcMethod";
    }
  }
}

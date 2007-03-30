using System;

namespace Mixins.UnitTests.SampleTypes
{
  public interface IBaseType2
  {
    string IfcMethod ();
  }
  
  public class BaseType2 : IBaseType2
  {
    public string IfcMethod ()
    {
      return "BaseType2.IfcMethod";
    }
  }
}

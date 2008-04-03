using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public interface InterfaceWithPartialProperties
  {
    int Prop1 { get; }
    int Prop2 { set; }
  }

  public class MixinImplementingFullPropertiesWithPartialIntroduction : InterfaceWithPartialProperties
  {
    public int Prop1
    {
      get { throw new Exception ("The method or operation is not implemented."); }
      set { throw new Exception ("The method or operation is not implemented."); }
    }

    public int Prop2
    {
      get { throw new Exception ("The method or operation is not implemented."); }
      set { throw new Exception ("The method or operation is not implemented."); }
    }
  }
}

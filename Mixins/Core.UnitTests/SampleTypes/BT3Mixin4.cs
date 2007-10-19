using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Mixins.UnitTests.SampleTypes
{
  public interface IBT3Mixin4
  {
    string Foo ();
  }

  [Extends (typeof (BaseType3))]
  [Serializable]
  public class BT3Mixin4 : BT3Mixin3<BaseType3, IBaseType34>, IBT3Mixin4
  {
    public string Foo ()
    {
      return "BT3Mixin4.Foo";
    }
  }
}

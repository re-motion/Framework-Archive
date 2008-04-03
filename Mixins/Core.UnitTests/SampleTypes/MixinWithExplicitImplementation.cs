using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Mixins.UnitTests.SampleTypes
{
  public interface IExplicit
  {
    string Explicit();
  }
 
  public class MixinWithExplicitImplementation : IExplicit
  {
    string IExplicit.Explicit ()
    {
      return "XXX";
    }
  }
}

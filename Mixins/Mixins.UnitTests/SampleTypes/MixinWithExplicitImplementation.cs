using System;
using System.Collections.Generic;
using System.Text;

namespace Mixins.UnitTests.SampleTypes
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

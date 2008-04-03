using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 0693

namespace Remotion.Mixins.UnitTests.CodeGeneration.SampleTypes
{
  public interface IGeneric<T>
  {
    string Generic<T> (T t);
  }

  public class MixinIntroducingGenericInterface<T> : Mixin<T>, IGeneric<T>
    where T : class
  {
    public string Generic<T> (T t)
    {
      return "Generic";
    }
  }
}

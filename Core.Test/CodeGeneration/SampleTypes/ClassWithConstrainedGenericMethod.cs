using System;

namespace Rubicon.Core.UnitTests.CodeGeneration.SampleTypes
{
  public class ClassWithConstrainedGenericMethod
  {
    public virtual string GenericMethod<T1, T2, T3> (T1 t1, T2 t2, T3 t3)
        where T1 : IConvertible
        where T2 : struct
        where T3 : T1
    {
      return string.Format ("{0}, {1}, {2}", t1, t2, t3);
    }
  }
}
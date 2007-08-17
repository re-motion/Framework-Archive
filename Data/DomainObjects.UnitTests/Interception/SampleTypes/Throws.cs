using System;

namespace Rubicon.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  public class Throws : DomainObject
  {
    public static Throws NewObject ()
    {
      return NewObject<Throws> ().With();
    }

    public Throws ()
        : base (ThrowException())
    {
    }

    private static DataContainer ThrowException ()
    {
      throw new Exception ("Thrown in ThrowException()");
    }
  }
}
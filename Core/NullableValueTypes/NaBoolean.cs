using System;

namespace Rubicon.Data.NullableValueTypes
{

public struct NaBoolean
{
  public NaBoolean (bool b)
  {
  }

  public static readonly NaBoolean Null = new NaBoolean (false);
}


}

using System;

namespace Rubicon.Security.UnitTests.SampleDomain
{

  public struct TestValueType : ISecurableObject
  {
    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }

    public Type GetSecurableType ()
    {
      throw new Exception ("The method or operation is not implemented.");
    }
  }
}
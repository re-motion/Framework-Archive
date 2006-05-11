using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.SampleDomain
{

  public struct TestValueType : ISecurableType
  {
    public ISecurityContextFactory GetSecurityContextFactory ()
    {
      throw new NotImplementedException ("The method or operation is not implemented.");
    }
  }
}
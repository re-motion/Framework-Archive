using System;
using System.Collections;
using Rubicon.ObjectBinding;

namespace Rubicon.Core.UnitTests.ObjectBidning.BusinessObjectPropertyPathTests
{
  public class StubBusinessObjectProvider : BusinessObjectProvider
  {
    protected override IDictionary ServiceDictionary
    {
      get { throw new NotImplementedException (); }
    }
  }
}
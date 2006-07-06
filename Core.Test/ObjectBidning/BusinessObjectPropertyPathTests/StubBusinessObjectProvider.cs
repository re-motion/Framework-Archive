using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using System.Collections;

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
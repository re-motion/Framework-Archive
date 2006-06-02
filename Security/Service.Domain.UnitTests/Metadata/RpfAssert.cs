using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Security.Service.Domain.UnitTests.Metadata
{
  public static class RpfAssert
  {
    static public void Contains (DomainObjectFilterCriteria expectedFilter, DomainObjectCollection actualCollection, string message, params object[] args)
    {
      Assert.DoAssert (new DomainObjectCollectionContainsAsserter (expectedFilter, actualCollection, message, args));
    }

    static public void Contains (DomainObjectFilterCriteria expectedFilter, DomainObjectCollection actualCollection, string message)
    {
      Contains (expectedFilter, actualCollection, message, null);
    }
    
    static public void Contains (DomainObjectFilterCriteria expectedFilter, DomainObjectCollection actualCollection)
    {
      Contains (expectedFilter, actualCollection, string.Empty, null);
    }
  }
}

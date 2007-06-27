using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class AccessTypeReferenceTest : DomainTest
  {
    [Test]
    public void SetAndGet_Index ()
    {
      ClientTransaction transaction = ClientTransactionScope.CurrentTransaction;
      AccessTypeReference accessTypeReference = AccessTypeReference.NewObject (transaction);

      accessTypeReference.Index = 1;
      Assert.AreEqual (1, accessTypeReference.Index);
    }
  }
}
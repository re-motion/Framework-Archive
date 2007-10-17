using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class AccessTypeReferenceTest : DomainTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      ClientTransaction.NewTransaction ().EnterNonDiscardingScope ();
    }

    [Test]
    public void SetAndGet_Index ()
    {
      AccessTypeReference accessTypeReference = AccessTypeReference.NewObject();

      accessTypeReference.Index = 1;
      Assert.AreEqual (1, accessTypeReference.Index);
    }
  }
}
using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.Linq.UnitTests
{
  [TestFixture]
  public class DataContextTest
  {
    [Test]
    public void Entity()
    {
      Assert.IsNotNull (DataContext.Entity<Order>());
    }
  }
}
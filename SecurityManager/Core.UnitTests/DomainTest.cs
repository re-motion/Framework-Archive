using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.UnitTests
{
  public abstract class DomainTest
  {
    protected DomainTest()
    {
    }

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp()
    {
    }

    [SetUp]
    public virtual void SetUp()
    {
    }

    [TearDown]
    public virtual void TearDown()
    {
      ClientTransactionScope.ResetActiveScope();
    }
  }
}
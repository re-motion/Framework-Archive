using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.UnitTests
{
  public abstract class DomainTest
  {
    // types

    // static members and constants

    // member fields

    private ClientTransactionScope _clientTransactionScope;

    // construction and disposing

    protected DomainTest()
    {
    }

    // methods and properties

    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp()
    {
    }

    [SetUp]
    public virtual void SetUp()
    {
      _clientTransactionScope = new ClientTransactionScope ();
    }

    [TearDown]
    public virtual void TearDown ()
    {
      _clientTransactionScope.Leave ();
    }
  }
}
using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.Factories;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests
{
  public class ClientTransactionBaseTest : StandardMappingTest
  {
    // types

    // static members and constants

    // member fields

    private ClientTransactionMock _clientTransactionMock;
    private ClientTransactionScope _clientTransactionScope;
    private TestDataContainerFactory _testDataContainerFactory;

    // construction and disposing

    protected ClientTransactionBaseTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      ReInitializeTransaction ();
    }

    public override void TearDown ()
    {
      base.TearDown();
      _testDataContainerFactory = null;
      _clientTransactionMock = null;
      _clientTransactionScope.Dispose ();
    }

    protected ClientTransactionMock ClientTransactionMock
    {
      get { return _clientTransactionMock; }
    }

    protected TestDataContainerFactory TestDataContainerFactory
    {
      get { return _testDataContainerFactory; }
    }

    protected void ReInitializeTransaction ()
    {
      if (_clientTransactionScope != null)
        _clientTransactionScope.Dispose ();

      _clientTransactionMock = new ClientTransactionMock ();
      _clientTransactionScope = new ClientTransactionScope (_clientTransactionMock);
      _testDataContainerFactory = new TestDataContainerFactory (_clientTransactionMock);
    }

    protected void CheckIfObjectIsDeleted (ObjectID id)
    {
      try
      {
        DomainObject domainObject = TestDomainBase.GetObject (id, true);
        Assert.IsNull (domainObject, string.Format ("Object '{0}' was not deleted.", id));
      }
      catch (ObjectNotFoundException)
      {
      }
    }
  }
}

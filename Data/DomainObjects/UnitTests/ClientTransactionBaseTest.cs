using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests
{
public class ClientTransactionBaseTest : DatabaseTest
{
  // types

  // static members and constants

  // member fields

  private ClientTransactionMock _clientTransactionMock;
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

  [TearDown]
  public virtual void TearDown ()
  {
    _testDataContainerFactory = null;
    _clientTransactionMock = null;
    ClientTransaction.SetCurrent (null);
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
    _clientTransactionMock = new ClientTransactionMock ();
    ClientTransaction.SetCurrent (_clientTransactionMock);
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

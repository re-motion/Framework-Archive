using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
[TestFixture]
public class SqlProviderTest : SqlProviderBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProviderTest ()
  {
  }

  // methods and properties

  [Test]
  public void IsConnectedFalse ()
  {
    RdbmsProvider rdbmsProvider = Provider;

    Assert.IsFalse (rdbmsProvider.IsConnected);
  }

  [Test]
  public void ConnectionHandling ()
  {
    RdbmsProvider rdbmsProvider = Provider;

    rdbmsProvider.Connect ();
    Assert.IsTrue (rdbmsProvider.IsConnected);
    rdbmsProvider.Disconnect ();
    Assert.IsFalse (rdbmsProvider.IsConnected);
  }

  [Test]
  public void Disposing ()
  {
    using (StorageProvider provider = Provider)
    {
      provider.LoadDataContainer (DomainObjectIDs.Order1);
    }

    RdbmsProvider rdbmsProvider = Provider;
    Assert.IsFalse (rdbmsProvider.IsConnected);
  }

  [Test]
  public void GetParameterName ()
  {
    Assert.AreEqual ("@parameter", Provider.GetParameterName ("parameter"));    
    Assert.AreEqual ("@parameter", Provider.GetParameterName ("@parameter"));
  }
}
}

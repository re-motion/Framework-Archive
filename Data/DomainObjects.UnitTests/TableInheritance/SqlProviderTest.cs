using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  [TestFixture]
  public class SqlProviderTest : TableInheritanceMappingTest
  {
    // types

    // static members and constants

    // member fields

    private StorageProviderManager _storageProviderManager;
    private SqlProvider _provider;

    // construction and disposing

    public SqlProviderTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _storageProviderManager = new StorageProviderManager ();
      _provider = (SqlProvider) _storageProviderManager.GetMandatory (c_testDomainProviderID);
    }

    [TearDown]
    public void TearDown ()
    {
      _storageProviderManager.Dispose ();
    }

    [Test]
    public void LoadConcreteSingle ()
    {
      DataContainer customerContainer = _provider.LoadDataContainer (DomainObjectIDs.Customer);
      Assert.IsNotNull (customerContainer);
      Assert.AreEqual (DomainObjectIDs.Customer, customerContainer.ID);
      Assert.AreEqual ("UnitTests", customerContainer.GetString ("CreatedBy"));
      Assert.AreEqual ("Zaphod", customerContainer.GetString ("FirstName"));
      Assert.AreEqual (CustomerType.Premium, customerContainer.GetValue ("CustomerType"));
    }
  }
}

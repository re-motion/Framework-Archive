using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
{
  public class SqlProviderBaseTest : TableInheritanceMappingTest
  {
    // types

    // static members and constants

    // member fields

    private StorageProviderManager _storageProviderManager;
    private SqlProvider _provider;

    // construction and disposing

    protected SqlProviderBaseTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();

      _storageProviderManager = new StorageProviderManager ();
      _provider = (SqlProvider) _storageProviderManager.GetMandatory (c_testDomainProviderID);
      _provider.Connect ();
    }

    public override void TearDown ()
    {
      base.TearDown();
      _storageProviderManager.Dispose ();
    }

    protected StorageProviderManager StorageProviderManager
    {
      get { return _storageProviderManager; }
    }

    protected SqlProvider Provider
    {
      get { return _provider; }
    }
  }
}

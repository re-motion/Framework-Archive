using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.Persistence;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
public class SqlProviderBaseTest : DatabaseTest
{
  // types

  // static members and constants

  // member fields

  private RdbmsProviderDefinition _providerDefinition;
  private SqlProvider _provider;

  // construction and disposing

  protected SqlProviderBaseTest ()
  {
  }

  // methods and properties

  public override void SetUp ()
  {
    base.SetUp ();

    _providerDefinition = new RdbmsProviderDefinition (
        c_testDomainProviderID, typeof (SqlProvider), DatabaseTest.c_connectionString);

    _provider = new SqlProvider (_providerDefinition);
  }
  
  [TearDown]
  public virtual void TearDown ()
  {
    _provider.Dispose ();
  }

  protected RdbmsProviderDefinition ProviderDefinition
  {
    get { return _providerDefinition; }
  }

  protected SqlProvider Provider
  {
    get { return _provider; }
  }
}
}

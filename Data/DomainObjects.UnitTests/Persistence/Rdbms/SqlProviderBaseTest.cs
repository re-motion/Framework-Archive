using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
public class SqlProviderBaseTest : ClientTransactionBaseTest
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
  
  public override void TearDown ()
  {
    base.TearDown ();
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

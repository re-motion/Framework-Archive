using System;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  public class SqlProviderBaseTest : ClientTransactionBaseTest
  {
    private RdbmsProviderDefinition _providerDefinition;
    private SqlProvider _provider;

    public override void SetUp ()
    {
      base.SetUp ();

      _providerDefinition = new RdbmsProviderDefinition (c_testDomainProviderID, typeof (SqlProvider), DatabaseTest.c_connectionString);

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

using System;
using NUnit.Framework;
using Rubicon.Configuration;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Development;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Mapping.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.SecurityManager.Persistence;
using Rubicon.SecurityManager.UnitTests.Configuration;

namespace Rubicon.SecurityManager.UnitTests
{
  public abstract class DomainTest
  {
    // types

    // static members and constants

    // member fields

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
      ClientTransaction.SetCurrent (null);
    }
  }
}
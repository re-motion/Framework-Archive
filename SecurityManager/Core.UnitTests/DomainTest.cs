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
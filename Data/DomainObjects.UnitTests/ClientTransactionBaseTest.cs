using System;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests
{
public class ClientTransactionBaseTest : DatabaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  protected ClientTransactionBaseTest ()
  {
  }

  // methods and properties

  [TearDown]
  public virtual void TearDown ()
  {
    ClientTransaction.SetCurrent (null);
  }
}
}

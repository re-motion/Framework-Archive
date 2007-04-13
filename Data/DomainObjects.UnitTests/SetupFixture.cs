using System;
using System.Data.SqlClient;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    [SetUp]
    public void SetUp()
    {
      SqlConnection.ClearAllPools();
    }

    [TearDown]
    public void TearDown ()
    {
      SqlConnection.ClearAllPools ();
    }
  }
}

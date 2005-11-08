using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.PerformanceTests.Database;

namespace Rubicon.Data.DomainObjects.PerformanceTests
{
public class DatabaseTest
{
  // types

  // static members and constants

  private const string c_connectionString = "Integrated Security=SSPI;Initial Catalog=PerformanceTestDomain;Data Source=localhost";

  // member fields

  // construction and disposing

  public DatabaseTest ()
  {
  }

  // methods and properties

  [SetUp]
  public virtual void SetUp ()
  {
    using (TestDataLoader loader = new TestDataLoader (c_connectionString))
    { 
      loader.Load ();
    }
  }
}
}

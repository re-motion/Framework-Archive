using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.Database;

namespace Rubicon.Data.DomainObjects.UnitTests
{
public class DatabaseTest
{
  // types

  // static members and constants

  public const string c_connectionString = 
      "Integrated Security=SSPI;Initial Catalog=TestDomain;Data Source=localhost";

  public const string c_testDomainProviderID = "TestDomain";
  public const string c_unitTestStorageProviderStubID = "UnitTestStorageProviderStub";

  // member fields

  // construction and disposing

  protected DatabaseTest ()
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

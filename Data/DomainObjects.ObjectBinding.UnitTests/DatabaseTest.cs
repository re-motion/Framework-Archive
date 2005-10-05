using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.UnitTests.Database;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests
{
public class DatabaseTest
{
  // types

  // static members and constants

  private const string c_connectionString = 
      "Integrated Security=SSPI;Initial Catalog=DomainObjects_ObjectBinding_UnitTests;Data Source=localhost";

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

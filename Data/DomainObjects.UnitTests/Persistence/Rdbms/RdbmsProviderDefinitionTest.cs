using System;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
[TestFixture]
public class RdbmsProviderDefinitionTest
{
  // types

  // static members and constants

  // member fields

  private StorageProviderDefinition _definition;

  // construction and disposing

  public RdbmsProviderDefinitionTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp ()
  {
    _definition = new RdbmsProviderDefinition ("StorageProviderID", typeof (SqlProvider), "ConnectionString");
  }

  [Test]
  public void IsIdentityTypeSupportedFalse ()
  {
    Assert.IsFalse (_definition.IsIdentityTypeSupported (typeof (int)));        
  }

  [Test]
  public void IsIdentityTypeSupportedTrue ()
  {
    Assert.IsTrue (_definition.IsIdentityTypeSupported (typeof (Guid)));        
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void IsIdentityTypeSupportedNull ()
  {
    _definition.IsIdentityTypeSupported (null);        
  }

  [Test]
  public void CheckValidIdentityType ()
  {
    _definition.CheckIdentityType (typeof (Guid));        
  }

  [Test]
  [ExpectedException (typeof (IdentityTypeNotSupportedException), 
      "The StorageProvider 'Rubicon.Data.DomainObjects.Persistence.Rdbms.SqlProvider' does not support identity values of type 'System.String'.")]
  public void CheckInvalidIdentityType ()
  {
    _definition.CheckIdentityType (typeof (string));        
  }

  [Test]
  public void CheckDetailsOfInvalidIdentityType ()
  {
    try
    {
      _definition.CheckIdentityType (typeof (string));        
    }
    catch (IdentityTypeNotSupportedException ex)
    {
      Assert.AreEqual (typeof (SqlProvider), ex.StorageProviderType);
      Assert.AreEqual (typeof (string), ex.InvalidIdentityType);
      return;
    }

    Assert.Fail ("Test expects an IdentityTypeNotSupportedException.");
  }
}
}

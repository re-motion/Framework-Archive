using System;
using System.Security.Principal;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Security.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class FilterQueryResultTest
  {
    private SecurityClientTransactionExtensionTestHelper _testHelper;
    private IClientTransactionExtension _extension;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTransactionExtensionTestHelper ();
      _extension = new SecurityClientTransactionExtension ();

      _testHelper.SetupSecurityConfiguration ();
      _testHelper.Transaction.EnterScope ();
    }

    [TearDown]
    public void TearDown ()
    {
      _testHelper.TearDownSecurityConfiguration ();
    }

    [Test]
    public void Test_WithOneAllowedObject ()
    {
      SecurableObject allowedObject = _testHelper.CreateSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (allowedObject);
      IQuery query = new Query ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (allowedObject, GeneralAccessTypes.Find, true);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (collection, query);

      _testHelper.VerifyAll ();
      Assert.AreEqual (1, collection.Count);
      Assert.Contains (allowedObject, collection);
    }

    [Test]
    public void Test_WithNoObjects ()
    {
      _extension = new SecurityClientTransactionExtension ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      IQuery query = new Query ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (collection, query);

      _testHelper.VerifyAll ();
      Assert.AreEqual (0, collection.Count);
    }

    [Test]
    public void Test_WithOneDeniedObject ()
    {
      SecurableObject deniedObject = _testHelper.CreateSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (deniedObject);
      IQuery query = new Query ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (deniedObject, GeneralAccessTypes.Find, false);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (collection, query);

      _testHelper.VerifyAll ();
      Assert.AreEqual (0, collection.Count);
    }

    [Test]
    public void Test_WithOneAllowedAndOneDeniedObject ()
    {
      SecurableObject allowedObject = _testHelper.CreateSecurableObject ();
      SecurableObject deniedObject = _testHelper.CreateSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (allowedObject);
      collection.Add (deniedObject);
      IQuery query = new Query ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (allowedObject, GeneralAccessTypes.Find, true);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (deniedObject, GeneralAccessTypes.Find, false);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (collection, query);

      _testHelper.VerifyAll ();
      Assert.AreEqual (1, collection.Count);
      Assert.Contains (allowedObject, collection);
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (nonSecurableObject);
      IQuery query = new Query ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (collection, query);

      _testHelper.VerifyAll ();
      Assert.AreEqual (1, collection.Count);
      Assert.Contains (nonSecurableObject, collection);
    }

    [Test]
    public void Test_WithinSecurityFreeSection ()
    {
      SecurableObject allowedObject = _testHelper.CreateSecurableObject ();
      SecurableObject deniedObject = _testHelper.CreateSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (allowedObject);
      collection.Add (deniedObject);
      IQuery query = new Query ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.FilterQueryResult (collection, query);
      }

      _testHelper.VerifyAll ();
      Assert.AreEqual (2, collection.Count);
      Assert.Contains (allowedObject, collection);
      Assert.Contains (deniedObject, collection);
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (securableObject);
      IQuery query = new Query ("Dummy");
      _testHelper.AddExtension (_extension);
      HasAccessDelegate hasAccess = delegate (ISecurityProvider securityProvider, IPrincipal user, AccessType[] requiredAccessTypes)
      {
        _testHelper.Transaction.QueryManager.GetCollection (new Query ("GetSecurableObjects"));
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Find, hasAccess);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (collection, query);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectSecurityProviderGetAccess (new SecurityContext (typeof (SecurableObject)), GeneralAccessTypes.Find);
      _testHelper.ReplayAll ();

      _testHelper.Transaction.QueryManager.GetCollection (new Query ("GetSecurableObjects"));

      _testHelper.VerifyAll ();
    }
  }
}
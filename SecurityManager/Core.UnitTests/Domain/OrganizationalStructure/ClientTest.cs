using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Security;
using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class ClientTest : DomainTest
  {
    private DatabaseFixtures _dbFixtures;
    private OrganisationalStructureTestHelper _testHelper;
    private ObjectID _expectedClientID;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();

      _dbFixtures = new DatabaseFixtures ();
      Client client = _dbFixtures.CreateOrganizationalStructureWithTwoClients ();
      _expectedClientID = client.ID;
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _testHelper = new OrganisationalStructureTestHelper ();
    }
    
    [Test]
    public void FindByUnqiueIdentifier_ValidClient ()
    {
      Client foundClient = Client.FindByUnqiueIdentifier (_testHelper.Transaction, "UID: testClient");

      Assert.AreEqual ("UID: testClient", foundClient.UniqueIdentifier);
    }

    [Test]
    public void FindByUnqiueIdentifier_NotExistingClient ()
    {
      Client foundClient = Client.FindByUnqiueIdentifier (_testHelper.Transaction, "UID: NotExistingClient");

      Assert.IsNull (foundClient);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UniqueIdentifier_SameidentifierTwice ()
    {
      Client client = _testHelper.CreateClient ("TestClient", "UID: testClient");

      _testHelper.Transaction.Commit ();
    }

    [Test]
    public void Get_UniqueIdentifier ()
    {
      Client client = _testHelper.CreateClient ("TestClient", "UID: testClient");

      Assert.IsNotEmpty (client.UniqueIdentifier);
    }

    #region IBusinessObjectWithIdentifier.UniqueIdentifier tests

    [Test]
    public void GetAndSet_UniqueIdentifier ()
    {
      Client client = _testHelper.CreateClient ("TestClient", string.Empty);

      client.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", client.UniqueIdentifier);
    }

    [Test]
    public void GetAndSet_UniqueIdentifierFromBusinessObjectWithIdentity ()
    {
      Client client = _testHelper.CreateClient ("TestClient", string.Empty);
      IBusinessObjectWithIdentity businessObject = client;

      client.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual (client.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetProperty_UniqueIdentifier ()
    {
      Client client = _testHelper.CreateClient ("TestClient", string.Empty);
      IBusinessObjectWithIdentity businessObject = client;

      client.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty ("UniqueIdentifier"));
      Assert.AreEqual (client.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void SetProperty_UniqueIdentifier ()
    {
      Client client = _testHelper.CreateClient ("TestClient", string.Empty);
      IBusinessObjectWithIdentity businessObject = client;

      businessObject.SetProperty ("UniqueIdentifier", "My Unique Identifier");
      Assert.AreEqual ("My Unique Identifier", client.UniqueIdentifier);
      Assert.AreEqual (client.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetPropertyDefinition_UniqueIdentifier ()
    {
      Client client = _testHelper.CreateClient ("TestClient", string.Empty);
      IBusinessObjectWithIdentity businessObject = client;
      client.UniqueIdentifier = "My Unique Identifier";

      IBusinessObjectProperty property = businessObject.BusinessObjectClass.GetPropertyDefinition ("UniqueIdentifier");

      Assert.IsInstanceOfType (typeof (IBusinessObjectStringProperty), property);
      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty (property));
    }

    [Test]
    public void GetPropertyDefinitions_CheckForUniqueIdentifier ()
    {
      Client client = _testHelper.CreateClient ("TestClient", string.Empty);
      IBusinessObjectWithIdentity businessObject = client;

      IBusinessObjectProperty[] properties = businessObject.BusinessObjectClass.GetPropertyDefinitions ();

      bool isFound = false;
      foreach (BaseProperty property in properties)
      {
        if (property.Identifier == "UniqueIdentifier" && property.PropertyInfo.DeclaringType == typeof (Client))
        {
          isFound = true;
          break;
        }
      }

      Assert.IsTrue (isFound, "Property UnqiueIdentifier declared on Client was not found.");
    }

    #endregion
  }
}

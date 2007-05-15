using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Security.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Security;
using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Development.UnitTesting;

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
    public void FindAll ()
    {
      ClientTransaction transaction = new ClientTransaction ();

      DomainObjectCollection clients = Client.FindAll (transaction);

      Assert.AreEqual (2, clients.Count);
      Assert.AreEqual (_expectedClientID, clients[1].ID);
    }

    [Test]
    public void FindByUnqiueIdentifier_ValidClient ()
    {
      Client foundClient = Client.FindByUnqiueIdentifier ("UID: testClient", _testHelper.Transaction);

      Assert.AreEqual ("UID: testClient", foundClient.UniqueIdentifier);
    }

    [Test]
    public void FindByUnqiueIdentifier_NotExistingClient ()
    {
      Client foundClient = Client.FindByUnqiueIdentifier ("UID: NotExistingClient", _testHelper.Transaction);

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

    [Test]
    public void GetDisplayName ()
    {
      Client client = _testHelper.CreateClient ("Clientname", "UID");

      Assert.AreEqual ("Clientname", client.DisplayName);
    }

    [Test]
    public void Get_Current_NotInitialized ()
    {
      Assert.IsNull (Client.Current);
    }

    [Test]
    public void SetAndGet_Current()
    {
      Client client = _testHelper.CreateClient ("Client", "UID: Client");
      
      Client.Current = client;
      Assert.AreSame (client, Client.Current);
      
      Client.Current = null;
    }

    [Test]
    public void SetAndGet_Current_Threading ()
    {
      Client client = _testHelper.CreateClient ("Client", "UID: Client");

      Client.Current = client;
      Assert.AreSame (client, Client.Current);

      ThreadRunner.Run (delegate ()
          {
            Client otherClient = _testHelper.CreateClient ("OtherClient", "UID: OtherClient");

            Assert.IsNull (Client.Current);
            Client.Current = otherClient;
            Assert.AreSame (otherClient, Client.Current);

          });

      Assert.AreSame (client, Client.Current);
    }

    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject client = _testHelper.CreateClient ("Client", "UID: Client");

      IObjectSecurityStrategy objectSecurityStrategy = client.GetSecurityStrategy ();
      Assert.IsNotNull (objectSecurityStrategy);
      Assert.IsInstanceOfType (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
      DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
      Assert.AreEqual (RequiredSecurityForStates.None, domainObjectSecurityStrategy.RequiredSecurityForStates);
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject client = _testHelper.CreateClient ("Client", "UID: Client");

      Assert.AreSame (client.GetSecurityStrategy (), client.GetSecurityStrategy ());
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      Client client = _testHelper.CreateClient ("Client", "UID: Client");
      IDomainObjectSecurityContextFactory factory = client;

      Assert.IsFalse (factory.IsDiscarded);
      Assert.IsTrue (factory.IsNew);
      Assert.IsFalse (factory.IsDeleted);

      client.Delete ();

      Assert.IsTrue (factory.IsDiscarded);
    }

    [Test]
    public void CreateSecurityContext ()
    {
      Client client = _testHelper.CreateClient ("Client", "UID: Client");

      SecurityContext securityContext = ((ISecurityContextFactory) client).CreateSecurityContext ();
      Assert.AreEqual (client.GetPublicDomainObjectType (), Type.GetType (securityContext.Class));
      Assert.IsEmpty (securityContext.Owner);
      Assert.IsEmpty (securityContext.OwnerGroup);
      Assert.AreEqual (client.UniqueIdentifier, securityContext.OwnerClient);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    [Test]
    public void GetHierachy_NoChildren ()
    {
      Client root = _testHelper.CreateClient ("Root", "UID: Root");

      DomainObjectCollection clients = root.GetHierachy ();

      Assert.AreEqual (1, clients.Count);
      Assert.Contains (root, clients);
    }

    [Test]
    public void GetHierachy_NoGrandChildren ()
    {
      Client root = _testHelper.CreateClient ("Root", "UID: Root");
      Client child1 = _testHelper.CreateClient ("Child1", "UID: Child1");
      child1.Parent = root;
      Client child2 = _testHelper.CreateClient ("Child2", "UID: Child2");
      child2.Parent = root;

      DomainObjectCollection clients = root.GetHierachy ();

      Assert.AreEqual (3, clients.Count);
      Assert.Contains (root, clients);
      Assert.Contains (child1, clients);
      Assert.Contains (child2, clients);
    }

    [Test]
    public void GetHierachy_WithGrandChildren ()
    {
      Client root = _testHelper.CreateClient ("Root", "UID: Root");
      Client child1 = _testHelper.CreateClient ("Child1", "UID: Child1");
      child1.Parent = root;
      Client child2 = _testHelper.CreateClient ("Child2", "UID: Child2");
      child2.Parent = root;
      Client grandChild1 = _testHelper.CreateClient ("GrandChild1", "UID: GrandChild1");
      grandChild1.Parent = child1;

      DomainObjectCollection clients = root.GetHierachy ();

      Assert.AreEqual (4, clients.Count);
      Assert.Contains (root, clients);
      Assert.Contains (child1, clients);
      Assert.Contains (child2, clients);
      Assert.Contains (grandChild1, clients);
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

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Security;
using Rubicon.Security.Data.DomainObjects;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  [TestFixture]
  public class GroupTest : DomainTest
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
    public void FindByUnqiueIdentifier_ValidGroup ()
    {
      Group foundGroup = Group.FindByUnqiueIdentifier ("UID: testGroup", _testHelper.Transaction);

      Assert.AreEqual ("UID: testGroup", foundGroup.UniqueIdentifier);
    }

    [Test]
    public void FindByUnqiueIdentifier_NotExistingGroup ()
    {
      Group foundGroup = Group.FindByUnqiueIdentifier ("UID: NotExistingGroup", _testHelper.Transaction);

      Assert.IsNull (foundGroup);
    }

    [Test]
    public void Find_GroupsByClientID ()
    {
      DomainObjectCollection groups = Group.FindByClientID (_expectedClientID, _testHelper.Transaction);

      Assert.AreEqual (9, groups.Count);
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UniqueIdentifier_SameIdentifierTwice ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = _testHelper.CreateClient (transaction, "NewClient2", "UID: testClient");
      _testHelper.CreateGroup (transaction, "NewGroup2", "UID: testGroup", null, client);

      transaction.Commit ();
    }

    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject group = CreateGroup ();

      IObjectSecurityStrategy objectSecurityStrategy = group.GetSecurityStrategy ();
      Assert.IsNotNull (objectSecurityStrategy);
      Assert.IsInstanceOfType (typeof (DomainObjectSecurityStrategy), objectSecurityStrategy);
      DomainObjectSecurityStrategy domainObjectSecurityStrategy = (DomainObjectSecurityStrategy) objectSecurityStrategy;
      Assert.AreEqual (RequiredSecurityForStates.None, domainObjectSecurityStrategy.RequiredSecurityForStates);
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject group = CreateGroup ();

      Assert.AreSame (group.GetSecurityStrategy (), group.GetSecurityStrategy ());
    }

    [Test]
    public void DomainObjectSecurityContextFactoryImplementation ()
    {
      Group group = CreateGroup ();
      IDomainObjectSecurityContextFactory factory = group;

      Assert.IsFalse (factory.IsDiscarded);
      Assert.IsTrue (factory.IsNew);
      Assert.IsFalse (factory.IsDeleted);

      group.Delete ();

      Assert.IsTrue (factory.IsDiscarded);
    }

    [Test]
    public void CreateSecurityContext ()
    {
      Group group = CreateGroup ();

      SecurityContext securityContext = ((ISecurityContextFactory) group).CreateSecurityContext ();
      Assert.AreEqual (group.GetType (), Type.GetType (securityContext.Class));
      Assert.IsEmpty (securityContext.Owner);
      Assert.AreEqual (group.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.AreEqual (group.Client.UniqueIdentifier, securityContext.OwnerClient);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    [Test]
    public void CreateSecurityContext_WithNoClient ()
    {
      Group group = CreateGroup ();
      group.Client = null;

      SecurityContext securityContext = ((ISecurityContextFactory) group).CreateSecurityContext ();
      Assert.AreEqual (group.GetType (), Type.GetType (securityContext.Class));
      Assert.IsEmpty (securityContext.Owner);
      Assert.AreEqual (group.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.IsEmpty (securityContext.OwnerClient);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    [Test]
    public void Get_UniqueIdentifier ()
    {
      OrganizationalStructureFactory factory = new OrganizationalStructureFactory();
      Group group = factory.CreateGroup (_testHelper.Transaction);

      Assert.IsNotEmpty (group.UniqueIdentifier);
    }

    #region IBusinessObjectWithIdentifier.UniqueIdentifier tests

    [Test]
    public void SetAndGet_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty, string.Empty));

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", group.UniqueIdentifier);
    }

    [Test]
    public void SetAndGet_UniqueIdentifierFromBusinessObjectWithIdentity ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual (group.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetProperty_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty ("UniqueIdentifier"));
      Assert.AreEqual (group.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void SetProperty_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      businessObject.SetProperty ("UniqueIdentifier", "My Unique Identifier");
      Assert.AreEqual ("My Unique Identifier", group.UniqueIdentifier);
      Assert.AreEqual (group.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetPropertyDefinition_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;
      group.UniqueIdentifier = "My Unique Identifier";

      IBusinessObjectProperty property = businessObject.BusinessObjectClass.GetPropertyDefinition ("UniqueIdentifier");

      Assert.IsInstanceOfType (typeof (IBusinessObjectStringProperty), property);
      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty (property));
    }

    [Test]
    public void GetPropertyDefinitions_CheckForUniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      IBusinessObjectProperty[] properties = businessObject.BusinessObjectClass.GetPropertyDefinitions ();

      bool isFound = false;
      foreach (BaseProperty property in properties)
      {
        if (property.Identifier == "UniqueIdentifier" && property.PropertyInfo.DeclaringType == typeof (Group))
        {
          isFound = true;
          break;
        }
      }

      Assert.IsTrue (isFound, "Property UnqiueIdentifier declared on Group was not found.");
    }

    #endregion

    private Group CreateGroup ()
    {
      Client client = _testHelper.CreateClient ("TestClient", "UID: testClient");
      Group group = _testHelper.CreateGroup ("TestGroup", "UID: TestGroup", null, client);

      return group;
    }
  }
}

using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.ObjectBinding;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.ObjectBinding;
using Rubicon.Security;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class Test : DomainTest
  {
    private OrganisationalStructureTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();

      _testHelper = new OrganisationalStructureTestHelper ();
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException))]
    public void UniqueIdentifier_SameIdentifierTwice ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();

      ClientTransaction transaction1 = new ClientTransaction ();
      Client client1 = _testHelper.CreateClient (transaction1, "NewClient1");
      Group group1 = _testHelper.CreateGroup (transaction1, "NewGroup1", "UID: NewGroup", null, client1);

      transaction1.Commit ();

      ClientTransaction transaction2 = new ClientTransaction ();
      Client client2 = _testHelper.CreateClient (transaction2, "NewClient2");
      Group group2 = _testHelper.CreateGroup (transaction2, "NewGroup2", "UID: NewGroup", null, client2);

      transaction2.Commit ();
    }

    [Test]
    public void GetAndSet_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty));

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", group.UniqueIdentifier);
    }

    [Test]
    public void GetAndSet_UniqueIdentifierFromBusinessObjectWithIdentity ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual (group.ID.ToString(), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetProperty_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty));
      IBusinessObjectWithIdentity businessObject = group;
      
      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty ("UniqueIdentifier"));
      Assert.AreEqual (group.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void SetProperty_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      businessObject.SetProperty ("UniqueIdentifier", "My Unique Identifier");
      Assert.AreEqual ("My Unique Identifier", group.UniqueIdentifier);
      Assert.AreEqual (group.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetPropertyDefinition_UniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup ( string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty));
      IBusinessObjectWithIdentity businessObject = group;
      group.UniqueIdentifier = "My Unique Identifier";

      IBusinessObjectProperty property = businessObject.BusinessObjectClass.GetPropertyDefinition ("UniqueIdentifier");

      Assert.IsInstanceOfType (typeof (IBusinessObjectStringProperty), property);
      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty (property));
    }

    [Test]
    public void GetPropertyDefinitions_CheckForUniqueIdentifier ()
    {
      Group group = _testHelper.CreateGroup (string.Empty, string.Empty, null, _testHelper.CreateClient (string.Empty));
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

    [Test]
    public void GetSecurityStrategy ()
    {
      ISecurableObject group = CreateGroup ();

      Assert.IsNotNull (group.GetSecurityStrategy ());
    }

    [Test]
    public void GetSecurityStrategy_SameTwice ()
    {
      ISecurableObject group = CreateGroup ();

      Assert.AreSame (group.GetSecurityStrategy (), group.GetSecurityStrategy ());
    }

    [Test]
    public void CreateSecurityContext ()
    {
      Group group = CreateGroup ();

      SecurityContext securityContext = ((ISecurityContextFactory) group).CreateSecurityContext ();
      Assert.AreEqual (group.GetType (), Type.GetType (securityContext.Class));
      Assert.IsEmpty (securityContext.Owner);
      Assert.AreEqual (group.UniqueIdentifier, securityContext.OwnerGroup);
      Assert.IsEmpty (securityContext.OwnerClient);
      Assert.IsEmpty (securityContext.AbstractRoles);
      Assert.IsTrue (securityContext.IsStateless);
    }

    private Group CreateGroup ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Group group = _testHelper.CreateGroup ("TestGroup", "UID: TestGroup", null, client);
      
      return group;
    }
  }
}

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.ObjectBinding;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
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
      Group group1 = _testHelper.CreateGroup (transaction1, "NewGroup1", "UnqiueIdentifier: NewGroup", null, client1);

      transaction1.Commit ();

      ClientTransaction transaction2 = new ClientTransaction ();
      Client client2 = _testHelper.CreateClient (transaction2, "NewClient2");
      Group group2 = _testHelper.CreateGroup (transaction2, "NewGroup2", "UnqiueIdentifier: NewGroup", null, client2);

      transaction2.Commit ();
    }

    [Test]
    public void GetAndSet_UniqueIdentifier ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Group group = _testHelper.CreateGroup (transaction, string.Empty, string.Empty, null, _testHelper.CreateClient (transaction, string.Empty));

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", group.UniqueIdentifier);
    }

    [Test]
    public void GetAndSet_UniqueIdentifierFromBusinessObjectWithIdentity ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Group group = _testHelper.CreateGroup (transaction, string.Empty, string.Empty, null, _testHelper.CreateClient (transaction, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual (group.ID.ToString(), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetProperty_UniqueIdentifier ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Group group = _testHelper.CreateGroup (transaction, string.Empty, string.Empty, null, _testHelper.CreateClient (transaction, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;
      
      group.UniqueIdentifier = "My Unique Identifier";

      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty ("UniqueIdentifier"));
      Assert.AreEqual (group.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void SetProperty_UniqueIdentifier ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Group group = _testHelper.CreateGroup (transaction, string.Empty, string.Empty, null, _testHelper.CreateClient (transaction, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;

      businessObject.SetProperty ("UniqueIdentifier", "My Unique Identifier");
      Assert.AreEqual ("My Unique Identifier", group.UniqueIdentifier);
      Assert.AreEqual (group.ID.ToString (), businessObject.UniqueIdentifier);
    }

    [Test]
    public void GetPropertyDefinition_UniqueIdentifier ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Group group = _testHelper.CreateGroup (transaction, string.Empty, string.Empty, null, _testHelper.CreateClient (transaction, string.Empty));
      IBusinessObjectWithIdentity businessObject = group;
      group.UniqueIdentifier = "My Unique Identifier";

      IBusinessObjectProperty property = businessObject.BusinessObjectClass.GetPropertyDefinition ("UniqueIdentifier");

      Assert.IsInstanceOfType (typeof (IBusinessObjectStringProperty), property);
      Assert.AreEqual ("My Unique Identifier", businessObject.GetProperty (property));
    }

    [Test]
    public void GetPropertyDefinitions_CheckForUniqueIdentifier ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Group group = _testHelper.CreateGroup (transaction, string.Empty, string.Empty, null, _testHelper.CreateClient (transaction, string.Empty));
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
  }
}

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.UnitTests.Domain.AccessControl;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class SecurableClassDefinitionTest : DomainTest
  {
    private ClientTransaction _transaction;
    private SecurableClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new ClientTransaction ();
      _classDefinition = new SecurableClassDefinition (_transaction);
    }

    [Test]
    public void AddAccessType ()
    {
      AccessTypeDefinition accessType = new AccessTypeDefinition (_transaction);

      _classDefinition.AddAccessType (accessType);

      Assert.AreEqual (1, _classDefinition.AccessTypes.Count);
      Assert.AreSame (accessType, _classDefinition.AccessTypes[0]);
    }

    [Test]
    public void AddStateProperty ()
    {
      StatePropertyDefinition stateProperty = new StatePropertyDefinition (_transaction);

      _classDefinition.AddStateProperty (stateProperty);

      Assert.AreEqual (1, _classDefinition.StateProperties.Count);
      Assert.AreSame (stateProperty, _classDefinition.StateProperties[0]);
    }

    [Test]
    public void FindStateCombination_ValidStates ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      StateCombination expectedCombination = testHelper.GetStateCombinationForDeliveredAndUnpaidOrder ();
      SecurableClassDefinition orderClass = expectedCombination.ClassDefinition;
      List<StateDefinition> states = testHelper.GetDeliveredAndUnpaidStateList (orderClass);

      StateCombination stateCombination = orderClass.FindStateCombination (states);
      Assert.AreSame (expectedCombination, stateCombination);
    }

    [Test]
    public void StateProperties_Empty ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();

      Assert.IsEmpty (orderClass.StateProperties);
    }

    [Test]
    public void StateProperties_OrderStateAndPaymentState ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinitionWithProperties ();

      Assert.AreEqual (AccessControlTestHelper.OrderClassPropertyCount, orderClass.StateProperties.Count);
    }

    [Test]
    public void StateProperties_IsCached ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinitionWithProperties ();

      DomainObjectCollection firstCollection = orderClass.StateProperties;
      DomainObjectCollection secondCollection = orderClass.StateProperties;

      Assert.AreSame (firstCollection, secondCollection);
    }

    [Test]
    public void StateProperties_IsReadOnly ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinitionWithProperties ();

      Assert.IsTrue (orderClass.StateProperties.IsReadOnly);
    }

    [Test]
    public void StateProperties_IsResetByAddStateProperty ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinitionWithProperties ();

      DomainObjectCollection firstCollection = orderClass.StateProperties;
      orderClass.AddStateProperty (testHelper.CreateTestProperty ());
      DomainObjectCollection secondCollection = orderClass.StateProperties;

      Assert.AreNotSame (firstCollection, secondCollection);
    }

    [Test]
    public void AccessTypes_Empty ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();

      Assert.IsEmpty (orderClass.AccessTypes);
    }

    [Test]
    public void AccessTypes_OneAccessType ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      testHelper.AttachJournalizeAccessType (orderClass);

      Assert.AreEqual (1, orderClass.AccessTypes.Count);
    }

    [Test]
    public void AccessTypes_IsCached ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      testHelper.AttachJournalizeAccessType (orderClass);

      DomainObjectCollection firstCollection = orderClass.AccessTypes;
      DomainObjectCollection secondCollection = orderClass.AccessTypes;

      Assert.AreSame (firstCollection, secondCollection);
    }

    [Test]
    public void AccessTypes_IsReadOnly ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      testHelper.AttachJournalizeAccessType (orderClass);

      Assert.IsTrue (orderClass.AccessTypes.IsReadOnly);
    }

    [Test]
    public void AccessTypes_IsResetByAddAccessType ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();

      DomainObjectCollection firstCollection = orderClass.AccessTypes;
      orderClass.AddAccessType (testHelper.CreateJournalizeAccessType ());
      DomainObjectCollection secondCollection = orderClass.AccessTypes;

      Assert.AreNotSame (firstCollection, secondCollection);
    }

    [Test]
    public void FindByName_ValidClassName ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();

      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      SecurableClassDefinition invoiceClass = testHelper.CreateInvoiceClassDefinition ();
      testHelper.Transaction.Commit ();

      ClientTransaction transaction = new ClientTransaction ();
      SecurableClassDefinition foundClass = SecurableClassDefinition.FindByName (transaction, "Rubicon.SecurityManager.UnitTests.TestDomain.Invoice");

      MetadataObjectAssert.AreEqual (invoiceClass, foundClass);
    }

    [Test]
    public void FindByName_InvalidClassName ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();

      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      SecurableClassDefinition invoiceClass = testHelper.CreateInvoiceClassDefinition ();
      testHelper.Transaction.Commit ();

      ClientTransaction transaction = new ClientTransaction ();
      SecurableClassDefinition foundClass = SecurableClassDefinition.FindByName (transaction, "Invce");

      Assert.IsNull (foundClass);
    }
  }
}

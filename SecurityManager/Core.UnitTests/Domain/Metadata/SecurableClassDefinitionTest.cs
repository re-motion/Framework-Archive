using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.UnitTests.Domain.AccessControl;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain;
using System.Threading;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class SecurableClassDefinitionTest : DomainTest
  {
    [Test]
    public void AddAccessType_TwoNewAccessTypes ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AccessTypeDefinition accessType0 = new AccessTypeDefinition (transaction);
      AccessTypeDefinition accessType1 = new AccessTypeDefinition (transaction);
      SecurableClassDefinitionWrapper classDefinitionWrapper = new SecurableClassDefinitionWrapper (new SecurableClassDefinition (transaction));
      DateTime changedAt = classDefinitionWrapper.SecurableClassDefinition.ChangedAt;
      Thread.Sleep (50);

      classDefinitionWrapper.SecurableClassDefinition.AddAccessType (accessType0);
      classDefinitionWrapper.SecurableClassDefinition.AddAccessType (accessType1);

      Assert.AreEqual (2, classDefinitionWrapper.SecurableClassDefinition.AccessTypes.Count);
      Assert.AreSame (accessType0, classDefinitionWrapper.SecurableClassDefinition.AccessTypes[0]);
      Assert.AreSame (accessType1, classDefinitionWrapper.SecurableClassDefinition.AccessTypes[1]);
      DomainObjectCollection references = classDefinitionWrapper.AccessTypeReferences;
      Assert.AreEqual (0, ((AccessTypeReference) references[0]).Index);
      Assert.AreEqual (1, ((AccessTypeReference) references[1]).Index);
      Assert.Greater ((decimal) classDefinitionWrapper.SecurableClassDefinition.ChangedAt.Ticks, (decimal) changedAt.Ticks);
    }

    [Test]
    public void AddStateProperty ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      StatePropertyDefinition stateProperty = new StatePropertyDefinition (transaction);
      SecurableClassDefinition classDefinition = new SecurableClassDefinition (transaction);

      classDefinition.AddStateProperty (stateProperty);

      Assert.AreEqual (1, classDefinition.StateProperties.Count);
      Assert.AreSame (stateProperty, classDefinition.StateProperties[0]);
    }

    [Test]
    public void FindStateCombination_ValidStates ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      StateCombination expectedCombination = testHelper.GetStateCombinationForDeliveredAndUnpaidOrder ();
      SecurableClassDefinition orderClass = expectedCombination.Class;
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

    [Test]
    public void FindAll_EmptyResult ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();

      ClientTransaction transaction = new ClientTransaction ();
      DomainObjectCollection result = SecurableClassDefinition.FindAll (transaction);

      Assert.AreEqual (0, result.Count);
    }

    [Test]
    public void FindAll_TwoFound ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();

      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      SecurableClassDefinition invoiceClass = testHelper.CreateInvoiceClassDefinition ();
      testHelper.Transaction.Commit ();

      ClientTransaction transaction = new ClientTransaction ();
      DomainObjectCollection result = SecurableClassDefinition.FindAll (transaction);

      Assert.AreEqual (2, result.Count);
    }

    [Test]
    public void Get_DisplayName ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurableClassDefinition classDefinition = new SecurableClassDefinition (transaction);
      classDefinition.Name = "Namespace.TypeName, AssemblyName";

      Assert.AreEqual ("Namespace.TypeName, AssemblyName", classDefinition.DisplayName);
    }

    [Test]
    public void CreateAccessControlList ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurableClassDefinition classDefinition = new SecurableClassDefinition (transaction);
      DateTime changedAt = classDefinition.ChangedAt;
      Thread.Sleep (50);

      AccessControlList accessControlList = classDefinition.CreateAccessControlList ();

      Assert.AreSame (classDefinition, accessControlList.Class);
      Assert.IsNotEmpty (accessControlList.AccessControlEntries);
      Assert.IsNotEmpty (accessControlList.StateCombinations);
      Assert.Greater ((decimal) classDefinition.ChangedAt.Ticks, (decimal) changedAt.Ticks);
    }

    [Test]
    public void CreateAccessControlList_TwoNewAcls ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurableClassDefinition classDefinition = new SecurableClassDefinition (transaction);
      DateTime changedAt = classDefinition.ChangedAt;
      Thread.Sleep (50);

      AccessControlList acccessControlList0 = classDefinition.CreateAccessControlList ();
      AccessControlList acccessControlListl = classDefinition.CreateAccessControlList ();

      Assert.AreEqual (2, classDefinition.AccessControlLists.Count);
      Assert.AreSame (acccessControlList0, classDefinition.AccessControlLists[0]);
      Assert.AreEqual (0, acccessControlList0.Index);
      Assert.AreSame (acccessControlListl, classDefinition.AccessControlLists[1]);
      Assert.AreEqual (1, acccessControlListl.Index);
      Assert.Greater ((decimal) classDefinition.ChangedAt.Ticks, (decimal) changedAt.Ticks);
    }

    [Test]
    public void Get_AccessTypesFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      SecurableClassDefinition expectedClassDefinition = dbFixtures.CreateSecurableClassDefinitionWith10AccessTypes ();

      ClientTransaction transaction = new ClientTransaction ();
      SecurableClassDefinition actualClassDefinition = SecurableClassDefinition.GetObject (expectedClassDefinition.ID, transaction);

      Assert.AreEqual (10, actualClassDefinition.AccessTypes.Count);
      for (int i = 0; i < 10; i++)
        Assert.AreEqual (expectedClassDefinition.AccessTypes[i].ID, actualClassDefinition.AccessTypes[i].ID);
    }

    [Test]
    public void Get_AccessControlListsFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      SecurableClassDefinition expectedClassDefinition = dbFixtures.CreateSecurableClassDefinitionWith10AccessControlLists ();

      ClientTransaction transaction = new ClientTransaction ();
      SecurableClassDefinition actualClassDefinition = SecurableClassDefinition.GetObject (expectedClassDefinition.ID, transaction);

      Assert.AreEqual (10, actualClassDefinition.AccessControlLists.Count);
      for (int i = 0; i < 10; i++)
        Assert.AreEqual (expectedClassDefinition.AccessControlLists[i].ID, actualClassDefinition.AccessControlLists[i].ID);
    }

    [Test]
    public void GetChangedAt_AfterCreation ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurableClassDefinition classDefinition = new SecurableClassDefinition (transaction);

      Assert.AreNotEqual (DateTime.MinValue, classDefinition.ChangedAt);
    }

    [Test]
    public void Touch_AfterCreation ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      SecurableClassDefinition classDefinition = new SecurableClassDefinition (transaction);

      DateTime creationDate = classDefinition.ChangedAt;

      Thread.Sleep (50);
      classDefinition.Touch ();

      Assert.Greater ((decimal) classDefinition.ChangedAt.Ticks, (decimal) creationDate.Ticks);
    }

    [Test]
    public void Validate_Valid ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      List<StateCombination> stateCombinations = testHelper.CreateOrderStateAndPaymentStateCombinations (orderClass);

      SecurableClassValidationResult result = orderClass.Validate ();

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void Validate_DoubleStateCombination ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      List<StateCombination> stateCombinations = testHelper.CreateOrderStateAndPaymentStateCombinations (orderClass);
      StatePropertyDefinition orderStateProperty = ((StateUsage) stateCombinations[0].StateUsages[0]).StateDefinition.StateProperty;
      StatePropertyDefinition paymentProperty = ((StateUsage) stateCombinations[0].StateUsages[1]).StateDefinition.StateProperty;
      testHelper.CreateStateCombination (orderClass, orderStateProperty["Received"], paymentProperty["Paid"]);

      SecurableClassValidationResult result = orderClass.Validate ();

      Assert.IsFalse (result.IsValid);
    }

    [Test]
    public void ValidateUniqueStateCombinations_NoStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();

      SecurableClassValidationResult result = new SecurableClassValidationResult ();
      orderClass.ValidateUniqueStateCombinations (result);

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void ValidateUniqueStateCombinations_TwoStatelessCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      StateCombination statelessCombination1 = testHelper.CreateStateCombination (orderClass);
      StateCombination statelessCombination2 = testHelper.CreateStateCombination (orderClass);

      SecurableClassValidationResult result = new SecurableClassValidationResult ();
      orderClass.ValidateUniqueStateCombinations (result);

      Assert.IsFalse (result.IsValid);
      Assert.Contains (statelessCombination1, result.InvalidStateCombinations);
      Assert.Contains (statelessCombination2, result.InvalidStateCombinations);
    }

    [Test]
    public void ValidateUniqueStateCombinations_TwoStateCombinations ()
    {
      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination paidCombination1 = testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);
      StateCombination paidCombination2 = testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);
      StateCombination notPaidCombination = testHelper.CreateStateCombination (orderClass, paymentProperty["None"]);

      SecurableClassValidationResult result = new SecurableClassValidationResult ();
      orderClass.ValidateUniqueStateCombinations (result);

      Assert.IsFalse (result.IsValid);
      Assert.AreEqual (2, result.InvalidStateCombinations.Count);
      Assert.Contains (paidCombination1, result.InvalidStateCombinations);
      Assert.Contains (paidCombination2, result.InvalidStateCombinations);
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException),
       "The securable class definition 'Rubicon.SecurityManager.UnitTests.TestDomain.Order' contains at least one state combination, which has been defined twice.")]
    public void Commit_TwoStateCombinations ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();

      AccessControlTestHelper testHelper = new AccessControlTestHelper ();
      SecurableClassDefinition orderClass = testHelper.CreateOrderClassDefinition ();
      StatePropertyDefinition paymentProperty = testHelper.CreatePaymentStateProperty (orderClass);
      StateCombination paidCombination1 = testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);
      StateCombination paidCombination2 = testHelper.CreateStateCombination (orderClass, paymentProperty["Paid"]);
      StateCombination notPaidCombination = testHelper.CreateStateCombination (orderClass, paymentProperty["None"]);

      testHelper.Transaction.Commit ();
    }
  }
}

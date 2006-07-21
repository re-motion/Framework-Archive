using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class StatePropertyDefinitionTest : DomainTest
  {
    private MetadataTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();

      _testHelper = new MetadataTestHelper ();
    }

    [Test]
    public void GetState_ValidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      StateDefinition actualState = stateProperty.GetState (MetadataTestHelper.Confidentiality_ConfidentialName);

      StateDefinition expectedState = _testHelper.CreateConfidentialState ();
      MetadataObjectAssert.AreEqual (expectedState, actualState, "Confidential state");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The state 'New' is not defined for the property 'Confidentiality'.\r\nParameter name: name")]
    public void GetState_InvalidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      StateDefinition actualState = stateProperty.GetState ("New");
    }

    [Test]
    public void ContainsState_ValidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);
      Assert.IsTrue (stateProperty.ContainsState (MetadataTestHelper.Confidentiality_ConfidentialName));
    }

    [Test]
    public void ContainsState_InvalidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);
      Assert.IsFalse (stateProperty.ContainsState ("New"));
    }

    [Test]
    public void GetState_ValidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      StateDefinition actualState = stateProperty.GetState (MetadataTestHelper.Confidentiality_PrivateValue);

      StateDefinition expectedState = _testHelper.CreatePrivateState ();
      MetadataObjectAssert.AreEqual (expectedState, actualState, "Private state");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "A state with the value 42 is not defined for the property 'Confidentiality'.\r\nParameter name: stateValue")]
    public void GetState_InvalidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      StateDefinition actualState = stateProperty.GetState (42);
    }

    [Test]
    public void ContainsState_ValidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);
      Assert.IsTrue (stateProperty.ContainsState (MetadataTestHelper.Confidentiality_PrivateValue));
    }

    [Test]
    public void ContainsState_InvalidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);
      Assert.IsFalse (stateProperty.ContainsState (42));
    }

    [Test]
    public void Indexer_ValidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty (0);

      StateDefinition actualState = stateProperty[MetadataTestHelper.Confidentiality_ConfidentialName];

      StateDefinition expectedState = _testHelper.CreateConfidentialState ();
      MetadataObjectAssert.AreEqual (expectedState, actualState, "Confidential state");
    }

    [Test]
    public void AddState_WithNameAndValueAndImpliedIndex ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty ("NewProperty");

      stateProperty.AddState ("NewState", 42);

      Assert.AreEqual (1, stateProperty.DefinedStates.Count);
      StateDefinition expectedState = _testHelper.CreateState ("NewState", 42);
      MetadataObjectAssert.AreEqual (expectedState, stateProperty.GetState ("NewState"));
    }

    [Test]
    public void AddState_AsStateDefinition ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty ("NewProperty");
      StateDefinition newState = _testHelper.CreateState ("NewState", 42);

      stateProperty.AddState (newState);

      Assert.AreEqual (1, stateProperty.DefinedStates.Count);
      MetadataObjectAssert.AreEqual (newState, stateProperty.GetState ("NewState"));
    }

    [Test]
    public void GetDefinedStates ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateEmptyDomain ();
      StatePropertyDefinition expectdPropertyDefinition = _testHelper.CreateConfidentialityProperty (0);
      _testHelper.Transaction.Commit ();

      ClientTransaction transaction = new ClientTransaction ();
      StatePropertyDefinition actualStatePropertyDefinition = StatePropertyDefinition.GetObject (expectdPropertyDefinition.ID, transaction);

      Assert.AreEqual (3, actualStatePropertyDefinition.DefinedStates.Count);
      for (int i = 0; i < actualStatePropertyDefinition.DefinedStates.Count; i++)
        Assert.AreEqual (expectdPropertyDefinition.DefinedStates[i].ID, actualStatePropertyDefinition.DefinedStates[i].ID);
    }

  }
}

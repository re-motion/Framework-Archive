using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.Domain.UnitTests.Metadata
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
    public void GetStateByName_ValidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty ();

      StateDefinition actualState = stateProperty.GetStateByName (MetadataTestHelper.Confidentiality_ConfidentialName);

      StateDefinition expectedState = _testHelper.CreateConfidentialState ();
      MetadataObjectAssert.AreEqual (expectedState, actualState, "Confidential state");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The state 'New' is not defined for the property 'Confidentiality'.\r\nParameter name: name")]
    public void GetStateByName_InvalidName ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty ();

      StateDefinition actualState = stateProperty.GetStateByName ("New");
    }

    [Test]
    public void GetStateByValue_ValidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty ();

      StateDefinition actualState = stateProperty.GetStateByValue (MetadataTestHelper.Confidentiality_PrivateValue);

      StateDefinition expectedState = _testHelper.CreatePrivateState ();
      MetadataObjectAssert.AreEqual (expectedState, actualState, "Private state");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "A state with the value 42 is not defined for the property 'Confidentiality'.\r\nParameter name: stateValue")]
    public void GetStateByValue_InvalidValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateConfidentialityProperty ();

      StateDefinition actualState = stateProperty.GetStateByValue (42);
    }

    [Test]
    public void AddState_WithNameAndValue ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty ("NewProperty");

      stateProperty.AddState ("NewState", 42);

      Assert.AreEqual (1, stateProperty.DefinedStates.Count);
      StateDefinition expectedState = _testHelper.CreateState ("NewState", 42);
      MetadataObjectAssert.AreEqual (expectedState, stateProperty.GetStateByName ("NewState"));
    }

    [Test]
    public void AddState_AsStateDefinition ()
    {
      StatePropertyDefinition stateProperty = _testHelper.CreateNewStateProperty ("NewProperty");
      StateDefinition newState = _testHelper.CreateState ("NewState", 42);

      stateProperty.AddState (newState);

      Assert.AreEqual (1, stateProperty.DefinedStates.Count);
      MetadataObjectAssert.AreEqual (newState, stateProperty.GetStateByName ("NewState"));
    }
  }
}

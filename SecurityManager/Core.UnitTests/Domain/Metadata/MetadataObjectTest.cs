using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using System.Globalization;
using System.Threading;

namespace Rubicon.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class MetadataObjectTest
  {
    [Test]
    public void Find_ValidSimpleMetadataObjectID ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();
      string metadataObjectID = "b8621bc9-9ab3-4524-b1e4-582657d6b420";

      ClientTransaction transaction = new ClientTransaction ();
      MetadataObject metadataObject = MetadataObject.Find (transaction, metadataObjectID);

      Assert.IsInstanceOfType (typeof (SecurableClassDefinition), metadataObject);
    }

    [Test]
    public void Find_NotExistingSimpleMetadataObjectID ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();
      string metadataObjectID = "38777218-cd4d-45ca-952d-c10b1104996a";

      ClientTransaction transaction = new ClientTransaction ();
      MetadataObject metadataObject = MetadataObject.Find (transaction, metadataObjectID);

      Assert.IsNull (metadataObject);
    }

    [Test]
    public void Find_ValidStateByMetadataObjectID ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();
      string metadataObjectID = "9e689c4c-3758-436e-ac86-23171289fa5e|2";

      ClientTransaction transaction = new ClientTransaction ();
      MetadataObject metadataObject = MetadataObject.Find (transaction, metadataObjectID);

      Assert.IsInstanceOfType (typeof (StateDefinition), metadataObject);
      StateDefinition state = (StateDefinition) metadataObject;
      Assert.AreEqual ("Reaccounted", state.Name);
      Assert.AreEqual (2, state.Value);
    }

    [Test]
    public void Find_NotExistingStateDefinition ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();
      string metadataObjectID = "9e689c4c-3758-436e-ac86-23171289fa5e|42";

      ClientTransaction transaction = new ClientTransaction ();
      MetadataObject metadataObject = MetadataObject.Find (transaction, metadataObjectID);

      Assert.IsNull (metadataObject);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The metadata ID 'Hello|42' is invalid.\r\nParameter name: metadataID")]
    public void Find_InvalidMetadataItemID ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();
      string metadataObjectID = "Hello|42";

      ClientTransaction transaction = new ClientTransaction ();
      MetadataObject metadataObject = MetadataObject.Find (transaction, metadataObjectID);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The metadata ID '9e689c4c-3758-436e-ac86-23171289fa5e|Hello' is invalid.\r\nParameter name: metadataID")]
    public void Find_InvalidStateValue ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();
      string metadataObjectID = "9e689c4c-3758-436e-ac86-23171289fa5e|Hello";

      ClientTransaction transaction = new ClientTransaction ();
      MetadataObject metadataObject = MetadataObject.Find (transaction, metadataObjectID);
    }

    [Test]
    public void DisplayName_German ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      dbFixtures.CreateSecurableClassDefinitionWithStates ();
      string metadataObjectID = "b8621bc9-9ab3-4524-b1e4-582657d6b420";
      ClientTransaction transaction = new ClientTransaction ();
      MetadataObject metadataObject = MetadataObject.Find (transaction, metadataObjectID);
      Culture culture = new Culture (transaction, "de");
      LocalizedName name = new LocalizedName (transaction, "Testklasse", culture, metadataObject);
      transaction.Commit ();

      CultureInfo threadCulture = Thread.CurrentThread.CurrentCulture;
      CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture ("de");
      Thread.CurrentThread.CurrentCulture = cultureInfo;

      transaction = new ClientTransaction ();
      metadataObject = MetadataObject.Find (transaction, metadataObjectID);

      Assert.AreEqual ("Testklasse", metadataObject.DisplayName);
    }
  }
}

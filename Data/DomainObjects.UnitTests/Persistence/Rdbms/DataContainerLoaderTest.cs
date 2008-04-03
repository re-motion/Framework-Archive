using System;
using System.Data;
using NUnit.Framework;
using System.Collections.Generic;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TableInheritance;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class DataContainerLoaderTest : SqlProviderBaseTest
  {
    private DataContainerLoader _loader;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp ();
      _loader = new DataContainerLoader (Provider);
      _mockRepository = new MockRepository ();
      Provider.Connect ();
    }

    public override void TearDown ()
    {
      base.TearDown ();
      Provider.Disconnect ();
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (Provider, _loader.Provider);
    }

    [Test]
    public void LoadDataContainerFromID ()
    {
      DataContainer dataContainer = _loader.LoadDataContainerFromID (DomainObjectIDs.Order1);
      Assert.AreEqual (DomainObjectIDs.Order1, dataContainer.ID);

      Assert.AreEqual (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)), dataContainer.ClassDefinition);
      Assert.AreEqual (dataContainer.ClassDefinition.GetPropertyDefinitions ().Count, dataContainer.PropertyValues.Count);

      Assert.AreEqual (1, dataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Order), "OrderNumber")].Value);
      Assert.AreEqual (DomainObjectIDs.Customer1,
          dataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Order), "Customer")].Value);
    }

    [Test]
    public void LoadDataContainersFromID_CallsProviderExecuteReader ()
    {
      CreateLoaderAndExpectProviderExecuteReader (delegate (DataContainerLoader loader)
          {
            loader.Provider.Connect ();

            loader.LoadDataContainerFromID (DomainObjectIDs.OrderItem1);
          });
    }

    [Test]
    public void LoadDataContainersFromIDs_Simple ()
    {
      IEnumerable<ObjectID> objectIDs = new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3 };
      DataContainerCollection dataContainers = _loader.LoadDataContainersFromIDs (objectIDs);
      Assert.AreEqual (3, dataContainers.Count);

      Assert.AreEqual (DomainObjectIDs.Order1, dataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.Order2, dataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.Order3, dataContainers[2].ID);

      Assert.AreEqual (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)), dataContainers[0].ClassDefinition);
      Assert.AreEqual (dataContainers[0].ClassDefinition.GetPropertyDefinitions().Count, dataContainers[0].PropertyValues.Count);

      Assert.AreEqual (1, dataContainers[0].PropertyValues[ReflectionUtility.GetPropertyName (typeof (Order), "OrderNumber")].Value);
      Assert.AreEqual (DomainObjectIDs.Customer1,
          dataContainers[0].PropertyValues[ReflectionUtility.GetPropertyName (typeof (Order), "Customer")].Value);
    }

    [Test]
    public void LoadDataContainersFromIDs_NonExistingID ()
    {
      IEnumerable<ObjectID> objectIDs = new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2,
          new ObjectID (typeof (Order), Guid.NewGuid()), DomainObjectIDs.Order3 };
      DataContainerCollection dataContainers = _loader.LoadDataContainersFromIDs (objectIDs);
      Assert.AreEqual (3, dataContainers.Count);

      Assert.AreEqual (DomainObjectIDs.Order1, dataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.Order2, dataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.Order3, dataContainers[2].ID);
    }

    [Test]
    public void LoadDataContainersFromIDs_DifferentEntities_Ordering ()
    {
      IEnumerable<ObjectID> objectIDs = new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.OrderItem1, DomainObjectIDs.Order2,
          DomainObjectIDs.Customer1, DomainObjectIDs.Customer2, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem2 };
      DataContainerCollection dataContainers = _loader.LoadDataContainersFromIDs (objectIDs);
      Assert.AreEqual (7, dataContainers.Count);

      Assert.AreEqual (DomainObjectIDs.Order1, dataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderItem1, dataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.Order2, dataContainers[2].ID);
      Assert.AreEqual (DomainObjectIDs.Customer1, dataContainers[3].ID);
      Assert.AreEqual (DomainObjectIDs.Customer2, dataContainers[4].ID);
      Assert.AreEqual (DomainObjectIDs.Order3, dataContainers[5].ID);
      Assert.AreEqual (DomainObjectIDs.OrderItem2, dataContainers[6].ID);
    }

    [Test]
    public void LoadDataContainersFromIDs_DifferentEntities_Grouping ()
    {
      IEnumerable<ObjectID> objectIDs = new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.OrderItem1, DomainObjectIDs.Order2,
          DomainObjectIDs.Company1, DomainObjectIDs.Company2, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem2 };

      IDataContainerLoaderHelper loaderHelperMock = _mockRepository.CreateMock<DataContainerLoaderHelper>();

      DataContainerLoader loader = new DataContainerLoader (loaderHelperMock, Provider);

      using (_mockRepository.Unordered ())
      {
        Expect.Call (loaderHelperMock.GetSelectCommandBuilder (null, null, null))
            .Constraints (Mocks_Is.Same (Provider), Mocks_Is.Equal ("Order"),
                Mocks_List.Equal (new ObjectID[] {DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3}))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        Expect.Call (loaderHelperMock.GetSelectCommandBuilder (null, null, null))
            .Constraints (Mocks_Is.Same (Provider), Mocks_Is.Equal ("OrderItem"),
            Mocks_List.Equal (new ObjectID[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 }))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        Expect.Call (loaderHelperMock.GetSelectCommandBuilder (null, null, null))
            .Constraints (Mocks_Is.Same (Provider), Mocks_Is.Equal ("Company"),
            Mocks_List.Equal (new ObjectID[] { DomainObjectIDs.Company1, DomainObjectIDs.Company2 }))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      }

      _mockRepository.ReplayAll ();

      loader.LoadDataContainersFromIDs (objectIDs);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void LoadDataContainersFromIDs_CallsProviderExecuteReader ()
    {
      CreateLoaderAndExpectProviderExecuteReader (delegate (DataContainerLoader loader)
          {
            loader.Provider.Connect();

            loader.LoadDataContainersFromIDs (new ObjectID[] {DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2});
          });
    }

    [Test]
    public void LoadDataContainersFromCommandBuilder ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      SelectCommandBuilder builder =
          SelectCommandBuilder.CreateForIDLookup (Provider, "OrderItem", new ObjectID[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 });
      
      List<DataContainer> sortedContainers = GetSortedContainers(_loader.LoadDataContainersFromCommandBuilder (builder));

      Assert.AreEqual (2, sortedContainers.Count);
      Assert.AreEqual (DomainObjectIDs.OrderItem1, sortedContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderItem2, sortedContainers[1].ID);

      Assert.AreEqual (classDefinition.GetPropertyDefinitions ().Count, sortedContainers[0].PropertyValues.Count);
      Assert.AreEqual (1, sortedContainers[0].PropertyValues[ReflectionUtility.GetPropertyName (typeof (OrderItem), "Position")].Value);
      Assert.AreEqual (DomainObjectIDs.Order1, sortedContainers[0].PropertyValues[ReflectionUtility.GetPropertyName (typeof (OrderItem), "Order")].Value);
    }

    [Test]
    public void LoadDataContainersFromCommandBuilder_CallsProviderExecuteReader ()
    {
      CreateLoaderAndExpectProviderExecuteReader (delegate (DataContainerLoader loader)
          {
            loader.Provider.Connect ();

            SelectCommandBuilder builder = SelectCommandBuilder.CreateForIDLookup (loader.Provider, "OrderItem",
              new ObjectID[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 });

            loader.LoadDataContainersFromCommandBuilder (builder);
          });
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithEntityName ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      Assert.IsNotNull (classDefinition.GetEntityName ());

      DataContainerCollection dataContainers = _loader.LoadDataContainersByRelatedID (
          classDefinition, ReflectionUtility.GetPropertyName (typeof (OrderItem), "Order"), DomainObjectIDs.Order1);
      List<DataContainer> sortedContainers = GetSortedContainers (dataContainers);

      Assert.AreEqual (2, sortedContainers.Count);
      Assert.AreEqual (DomainObjectIDs.OrderItem1, sortedContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderItem2, sortedContainers[1].ID);

      Assert.AreEqual (classDefinition.GetPropertyDefinitions ().Count, sortedContainers[0].PropertyValues.Count);
      Assert.AreEqual (1, sortedContainers[0].PropertyValues[ReflectionUtility.GetPropertyName (typeof (OrderItem), "Position")].Value);
      Assert.AreEqual (DomainObjectIDs.Order1, sortedContainers[0].PropertyValues[ReflectionUtility.GetPropertyName (typeof (OrderItem), "Order")].Value);
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithEntityName_CallsProviderLoadDataContainers ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      Assert.IsNotNull (classDefinition.GetEntityName ());

      CreateLoaderAndExpectProviderCall (
          delegate (DataContainerLoader loader)
          {
            loader.Provider.Connect ();

            loader.LoadDataContainersByRelatedID (
                classDefinition, ReflectionUtility.GetPropertyName (typeof (OrderItem), "Order"), DomainObjectIDs.Order1);
          },
          delegate (RdbmsProvider provider)
          {
            Expect.Call (PrivateInvoke.InvokeNonPublicMethod (provider, "LoadDataContainers", new object[] { null }))
                .IgnoreArguments().CallOriginalMethod(OriginalCallOptions.CreateExpectation);
          });
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithoutEntityName ()
    {
      Provider.Disconnect ();

      using (RdbmsProvider provider = new SqlProvider (
          (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory ("TableInheritanceTestDomain")))
      {
        provider.Connect ();

        ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TableInheritance.TestDomain.DomainBase));
        Assert.IsNull (classDefinition.GetEntityName());

        DataContainerLoader loader = new DataContainerLoader (provider);
        DomainObjectIDs domainObjectIDs = new TableInheritance.DomainObjectIDs();
        DataContainerCollection dataContainers = loader.LoadDataContainersByRelatedID (
            classDefinition,
            ReflectionUtility.GetPropertyName (typeof (TableInheritance.TestDomain.DomainBase), "Client"),
            domainObjectIDs.Client);

        Assert.AreEqual (4, dataContainers.Count);
        Assert.IsTrue (dataContainers.Contains (domainObjectIDs.Customer));
        Assert.IsTrue (dataContainers.Contains (domainObjectIDs.Person));
        Assert.IsTrue (dataContainers.Contains (domainObjectIDs.OrganizationalUnit));
        Assert.IsTrue (dataContainers.Contains (domainObjectIDs.PersonForUnidirectionalRelationTest));
      }
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithoutEntityName_UsesConcreteTableInheritanceRelationLoader ()
    {
      Provider.Disconnect ();

      using (RdbmsProvider provider = new SqlProvider (
          (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory ("TableInheritanceTestDomain")))
      {
        provider.Connect ();

        ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (TableInheritance.TestDomain.DomainBase));
        Assert.IsNull (classDefinition.GetEntityName ());

        IDataContainerLoaderHelper loaderHelperMock = _mockRepository.CreateMock<DataContainerLoaderHelper> ();

        Expect.Call (loaderHelperMock.GetConcreteTableInheritanceRelationLoader (null, null, null, null)).IgnoreArguments ()
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);

        _mockRepository.ReplayAll ();

        DataContainerLoader loader = new DataContainerLoader (loaderHelperMock, provider);

        DomainObjectIDs domainObjectIDs = new TableInheritance.DomainObjectIDs ();
        loader.LoadDataContainersByRelatedID (
            classDefinition,
            ReflectionUtility.GetPropertyName (typeof (TableInheritance.TestDomain.DomainBase), "Client"),
            domainObjectIDs.Client);

        _mockRepository.VerifyAll ();
      }
    }

    private void CreateLoaderAndExpectProviderCall (Proc<DataContainerLoader> action, Proc<RdbmsProvider> expectationSetup)
    {
      Provider.Disconnect ();

      using (RdbmsProvider providerMock = _mockRepository.PartialMock<SqlProvider> (ProviderDefinition))
      {
        DataContainerLoader loader = new DataContainerLoader (providerMock);

        expectationSetup (providerMock);

        _mockRepository.ReplayAll();

        action (loader);
      }
      _mockRepository.VerifyAll ();
    }

    private void CreateLoaderAndExpectProviderExecuteReader (Proc<DataContainerLoader> action)
    {
      CreateLoaderAndExpectProviderCall (action, delegate (RdbmsProvider providerMock)
      {
        Expect.Call (providerMock.ExecuteReader (null, CommandBehavior.Default)).IgnoreArguments ()
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      });
    }

    private List<DataContainer> GetSortedContainers (DataContainerCollection dataContainers)
    {
      List<DataContainer> sortedContainers = new List<DataContainer> ();
      foreach (DataContainer dc in dataContainers)
        sortedContainers.Add (dc);
      sortedContainers.Sort (delegate (DataContainer one, DataContainer two) { return one.ID.Value.ToString ().CompareTo (two.ID.Value.ToString ()); });
      return sortedContainers;
    }
  }
} // expect
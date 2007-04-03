using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance
{
  [TestFixture]
  public class SqlProviderTest : SqlProviderBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SqlProviderTest ()
    {
    }

    // methods and properties

    [Test]
    public void LoadConcreteSingle ()
    {
      DataContainer customerContainer = Provider.LoadDataContainer (DomainObjectIDs.Customer);
      Assert.IsNotNull (customerContainer);
      Assert.AreEqual (DomainObjectIDs.Customer, customerContainer.ID);
      Assert.AreEqual ("UnitTests", customerContainer.GetString ("CreatedBy"));
      Assert.AreEqual ("Zaphod", customerContainer.GetString ("FirstName"));
      Assert.AreEqual (CustomerType.Premium, customerContainer.GetValue ("CustomerType"));
    }

    [Test]
    public void LoadDataContainersByRelatedIDWithAbstractBaseClass ()
    {
      ClassDefinition domainBaseClass = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainBase));

      DataContainerCollection loadedDataContainers = Provider.LoadDataContainersByRelatedID (domainBaseClass, "Client", DomainObjectIDs.Client);

      Assert.IsNotNull (loadedDataContainers);
      Assert.AreEqual (3, loadedDataContainers.Count);
      Assert.AreEqual (DomainObjectIDs.OrganizationalUnit, loadedDataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.Person, loadedDataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.Customer, loadedDataContainers[2].ID);
    }

    [Test]
    public void GetColumnsFromSortExpression ()
    {
      RdbmsProvider rdbmsProvider = Provider;
      Assert.AreEqual ("Column", rdbmsProvider.GetColumnsFromSortExpression ("Column"));
      Assert.AreEqual ("Column", rdbmsProvider.GetColumnsFromSortExpression (" Column"));
      Assert.AreEqual ("Asc", rdbmsProvider.GetColumnsFromSortExpression ("Asc"));
      Assert.AreEqual ("Asc", rdbmsProvider.GetColumnsFromSortExpression (" Asc"));
      Assert.AreEqual ("asc", rdbmsProvider.GetColumnsFromSortExpression (" asc"));
      Assert.AreEqual ("[Asc]", rdbmsProvider.GetColumnsFromSortExpression ("[Asc]"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1, Column2"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("    Column1   ,    Column2   "));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1,Column2"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1 asc,Column2 desc"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1 asc, Column2 desc"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1 ASC, Column2 DESC"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1 \tASC, Column2  \nDESC"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1\tASC,\r\nColumn2\nDESC"));
      Assert.AreEqual ("[ASC], [desc]", rdbmsProvider.GetColumnsFromSortExpression ("[ASC] ASC, [desc] DESC"));
      Assert.AreEqual ("[Collate]", rdbmsProvider.GetColumnsFromSortExpression ("[Collate] asc"));
      Assert.AreEqual ("AscColumnAsc1Asc, DescColumn2Desc", rdbmsProvider.GetColumnsFromSortExpression ("AscColumnAsc1Asc ASC, DescColumn2Desc DESC"));
      Assert.IsNull (rdbmsProvider.GetColumnsFromSortExpression (null));
      Assert.AreEqual (string.Empty, rdbmsProvider.GetColumnsFromSortExpression (string.Empty));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        ExpectedMessage = "Collations cannot be used in sort expressions. Sort expression: 'Column1 collate German_PhoneBook_CI_AI'.\r\nParameter name: sortExpression")]
    public void GetColumnsWithCollate ()
    {
      Provider.GetColumnsFromSortExpression ("Column1 collate German_PhoneBook_CI_AI");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Collations cannot be used in sort expressions. Sort expression: 'Column1\t\tcollate German_PhoneBook_CI_AI'.\r\nParameter name: sortExpression")]
    public void GetColumnsWithCollateAfterMultipleTabs ()
    {
      Provider.GetColumnsFromSortExpression ("Column1\t\tcollate German_PhoneBook_CI_AI");
    }

  }
}

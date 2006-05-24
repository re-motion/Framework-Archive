using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance
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
    public void GetColumnsFromSortExpression ()
    {
      RdbmsProvider rdbmsProvider = Provider;
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1, Column2"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1 asc, Column2 desc"));
      Assert.AreEqual ("Column1, Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1 ASC, Column2 DESC"));
      Assert.AreEqual ("Column1 , Column2  ", rdbmsProvider.GetColumnsFromSortExpression ("Column1 \tASC, Column2  \nDESC"));
      Assert.AreEqual ("Column1,  Column2", rdbmsProvider.GetColumnsFromSortExpression ("Column1\tASC,\r\nColumn2\nDESC"));
      Assert.AreEqual ("[ASC], [desc]", rdbmsProvider.GetColumnsFromSortExpression ("[ASC] ASC, [desc] DESC"));
      Assert.AreEqual ("[Collate]", rdbmsProvider.GetColumnsFromSortExpression ("[Collate] asc"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        "Collations cannot be used in sort expressions. Sort expression: 'Column1 collate German_PhoneBook_CI_AI'.\r\nParameter name: sortExpression")]
    public void GetColumnsWithCollate ()
    {
      Provider.GetColumnsFromSortExpression ("Column1 collate German_PhoneBook_CI_AI");
    }

  }
}

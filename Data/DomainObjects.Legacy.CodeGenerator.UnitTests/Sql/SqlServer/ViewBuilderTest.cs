using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Legacy.CodeGenerator.Sql.SqlServer;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Legacy.CodeGenerator.UnitTests.Sql.SqlServer
{
  [TestFixture]
  public class ViewBuilderTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private ViewBuilder _createViewBuilder;

    // construction and disposing

    public ViewBuilderTest ()
    {
    }

    // methods and properties

    public override void SetUp ()
    {
      base.SetUp ();
      _createViewBuilder = new ViewBuilder ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual (string.Empty, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddView ()
    {
      _createViewBuilder.AddView (OrderClass);

      string expectedScript = "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "    WHERE [ClassID] IN ('Order')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithConcreteDerivedClass ()
    {
      _createViewBuilder.AddView (CustomerClass);

      string expectedScript = "CREATE VIEW [dbo].[CustomerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID]\r\n"
          + "    FROM [dbo].[Customer]\r\n"
          + "    WHERE [ClassID] IN ('Customer')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithConcreteBaseClass ()
    {
      _createViewBuilder.AddView (ConcreteClass);

      string expectedScript = "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "    WHERE [ClassID] IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithAbstractClass ()
    {
      _createViewBuilder.AddView (CompanyClass);

      string expectedScript = "CREATE VIEW [dbo].[CompanyView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], null, null, null\r\n"
          + "    FROM [dbo].[Customer]\r\n"
          + "    WHERE [ClassID] IN ('Customer', 'DevelopmentPartner')\r\n"
          + "  UNION ALL\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], null, null, null, [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]\r\n"
          + "    FROM [dbo].[DevelopmentPartner]\r\n"
          + "    WHERE [ClassID] IN ('Customer', 'DevelopmentPartner')\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithAbstractClassWithSingleConcreteConcrete ()
    {
      _createViewBuilder.AddView (PartnerClass);

      string expectedScript = "CREATE VIEW [dbo].[PartnerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])\r\n"
        + "  WITH SCHEMABINDING AS\r\n"
        + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]\r\n"
        + "    FROM [dbo].[DevelopmentPartner]\r\n"
        + "    WHERE [ClassID] IN ('DevelopmentPartner')\r\n"
        + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithDerivedClass ()
    {
      _createViewBuilder.AddView (DerivedClass);

      string expectedScript = "CREATE VIEW [dbo].[DerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "    WHERE [ClassID] IN ('DerivedClass', 'DerivedOfDerivedClass')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithAbstractWithoutConcreteTable()
    {
      _createViewBuilder.AddView (AbstractWithoutConcreteClass);

      Assert.IsEmpty (_createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewTwice()
    {
      _createViewBuilder.AddView (OrderClass);
      _createViewBuilder.AddView (ConcreteClass);

      string expectedScript = "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "    WHERE [ClassID] IN ('Order')\r\n"
          + "  WITH CHECK OPTION\r\n"
          + "GO\r\n\r\n"
          + "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "    WHERE [ClassID] IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViews ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (OrderClass);
      classes.Add (ConcreteClass);

      _createViewBuilder.AddViews (classes);

      string expectedScript = "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "    WHERE [ClassID] IN ('Order')\r\n"
          + "  WITH CHECK OPTION\r\n"
          + "GO\r\n\r\n"
          + "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "    WHERE [ClassID] IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void GetDropViewScriptWithConcreteClass ()
    {
      _createViewBuilder.AddView (OrderClass);

      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[OrderView]\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetDropViewScript ());
    }

    [Test]
    public void GetDropViewScriptWithAbstractClass ()
    {
      _createViewBuilder.AddView (CompanyClass);

      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[CompanyView]\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetDropViewScript ());
    }

    [Test]
    public void GetDropViewScriptWithTwoClasses ()
    {
      _createViewBuilder.AddView (OrderClass);
      _createViewBuilder.AddView (CompanyClass);

      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[OrderView]\r\n\r\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[CompanyView]\r\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetDropViewScript ());
    }
  }
}

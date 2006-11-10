using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.CodeGenerator.Sql.SqlServer;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.Sql.SqlServer
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

      string expectedScript = "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])\n"
          + "  WITH SCHEMABINDING AS\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]\n"
          + "    FROM [dbo].[Order]\n"
          + "    WHERE [ClassID] IN ('Order')\n"
          + "  WITH CHECK OPTION\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithConcreteDerivedClass ()
    {
      _createViewBuilder.AddView (CustomerClass);

      string expectedScript = "CREATE VIEW [dbo].[CustomerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID])\n"
          + "  WITH SCHEMABINDING AS\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID]\n"
          + "    FROM [dbo].[Customer]\n"
          + "    WHERE [ClassID] IN ('Customer')\n"
          + "  WITH CHECK OPTION\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithConcreteBaseClass ()
    {
      _createViewBuilder.AddView (ConcreteClass);

      string expectedScript = "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID])\n"
          + "  WITH SCHEMABINDING AS\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID]\n"
          + "    FROM [dbo].[ConcreteClass]\n"
          + "    WHERE [ClassID] IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\n"
          + "  WITH CHECK OPTION\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithAbstractClass ()
    {
      _createViewBuilder.AddView (CompanyClass);

      string expectedScript = "CREATE VIEW [dbo].[CompanyView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])\n"
          + "  WITH SCHEMABINDING AS\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [CustomerType], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [PrimaryOfficialID], null, null, null\n"
          + "    FROM [dbo].[Customer]\n"
          + "    WHERE [ClassID] IN ('Customer', 'DevelopmentPartner')\n"
          + "  UNION ALL\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], null, null, null, [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]\n"
          + "    FROM [dbo].[DevelopmentPartner]\n"
          + "    WHERE [ClassID] IN ('Customer', 'DevelopmentPartner')\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithAbstractClassWithSingleConcreteConcrete ()
    {
      _createViewBuilder.AddView (PartnerClass);

      string expectedScript = "CREATE VIEW [dbo].[PartnerView] ([ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])\n"
        + "  WITH SCHEMABINDING AS\n"
        + "  SELECT [ID], [ClassID], [Timestamp], [Name], [PhoneNumber], [AddressID], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]\n"
        + "    FROM [dbo].[DevelopmentPartner]\n"
        + "    WHERE [ClassID] IN ('DevelopmentPartner')\n"
        + "  WITH CHECK OPTION\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViewWithDerivedClass ()
    {
      _createViewBuilder.AddView (DerivedClass);

      string expectedScript = "CREATE VIEW [dbo].[DerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID])\n"
          + "  WITH SCHEMABINDING AS\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID]\n"
          + "    FROM [dbo].[ConcreteClass]\n"
          + "    WHERE [ClassID] IN ('DerivedClass', 'DerivedOfDerivedClass')\n"
          + "  WITH CHECK OPTION\n";

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

      string expectedScript = "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])\n"
          + "  WITH SCHEMABINDING AS\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]\n"
          + "    FROM [dbo].[Order]\n"
          + "    WHERE [ClassID] IN ('Order')\n"
          + "  WITH CHECK OPTION\n"
          + "GO\n\n"
          + "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID])\n"
          + "  WITH SCHEMABINDING AS\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID]\n"
          + "    FROM [dbo].[ConcreteClass]\n"
          + "    WHERE [ClassID] IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\n"
          + "  WITH CHECK OPTION\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void AddViews ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (OrderClass);
      classes.Add (ConcreteClass);

      _createViewBuilder.AddViews (classes);

      string expectedScript = "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID])\n"
          + "  WITH SCHEMABINDING AS\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [Number], [Priority], [CustomerID], [CustomerIDClassID], [OfficialID]\n"
          + "    FROM [dbo].[Order]\n"
          + "    WHERE [ClassID] IN ('Order')\n"
          + "  WITH CHECK OPTION\n"
          + "GO\n\n"
          + "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID])\n"
          + "  WITH SCHEMABINDING AS\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInSecondDerivedClass], [ClassWithRelationsInSecondDerivedClassID]\n"
          + "    FROM [dbo].[ConcreteClass]\n"
          + "    WHERE [ClassID] IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\n"
          + "  WITH CHECK OPTION\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetCreateViewScript ());
    }

    [Test]
    public void GetDropViewScriptWithConcreteClass ()
    {
      _createViewBuilder.AddView (OrderClass);

      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')\n"
          + "  DROP VIEW [dbo].[OrderView]\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetDropViewScript ());
    }

    [Test]
    public void GetDropViewScriptWithAbstractClass ()
    {
      _createViewBuilder.AddView (CompanyClass);

      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')\n"
          + "  DROP VIEW [dbo].[CompanyView]\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetDropViewScript ());
    }

    [Test]
    public void GetDropViewScriptWithTwoClasses ()
    {
      _createViewBuilder.AddView (OrderClass);
      _createViewBuilder.AddView (CompanyClass);

      string expectedScript = "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')\n"
          + "  DROP VIEW [dbo].[OrderView]\n\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')\n"
          + "  DROP VIEW [dbo].[CompanyView]\n";

      Assert.AreEqual (expectedScript, _createViewBuilder.GetDropViewScript ());
    }
  }
}

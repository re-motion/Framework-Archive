using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.RdbmsTools.SchemaGeneration.SqlServer;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.SchemaGeneration.SqlServer
{
  [TestFixture]
  public class ViewBuilderTest : StandardMappingTest
  {
    private ViewBuilder _viewBuilder;

    public override void SetUp ()
    {
      base.SetUp();
      _viewBuilder = new ViewBuilder();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual (string.Empty, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddView ()
    {
      _viewBuilder.AddView (OrderClass);

      string expectedScript =
          "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [CustomerID], [CustomerIDClassID], [Number], [OfficialID], [Priority])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [CustomerID], [CustomerIDClassID], [Number], [OfficialID], [Priority]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "    WHERE [ClassID] IN ('Order')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewWithConcreteDerivedClass ()
    {
      _viewBuilder.AddView (CustomerClass);

      string expectedScript =
          "CREATE VIEW [dbo].[CustomerView] ([ID], [ClassID], [Timestamp], [AddressID], [Name], [PhoneNumber], [PrimaryOfficialID], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [CustomerType])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [AddressID], [Name], [PhoneNumber], [PrimaryOfficialID], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [CustomerType]\r\n"
          + "    FROM [dbo].[Customer]\r\n"
          + "    WHERE [ClassID] IN ('Customer')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewWithConcreteBaseClass ()
    {
      _viewBuilder.AddView (ConcreteClass);

      string expectedScript =
          "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [PropertyInSecondDerivedClass])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [PropertyInSecondDerivedClass]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "    WHERE [ClassID] IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewWithAbstractClass ()
    {
      _viewBuilder.AddView (CompanyClass);

      string expectedScript =
          "CREATE VIEW [dbo].[CompanyView] ([ID], [ClassID], [Timestamp], [AddressID], [Name], [PhoneNumber], [PrimaryOfficialID], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [CustomerType], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [AddressID], [Name], [PhoneNumber], [PrimaryOfficialID], [CustomerPropertyWithIdenticalNameInDifferentInheritanceBranches], [CustomerType], null, null, null\r\n"
          + "    FROM [dbo].[Customer]\r\n"
          + "    WHERE [ClassID] IN ('Customer', 'DevelopmentPartner')\r\n"
          + "  UNION ALL\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [AddressID], [Name], [PhoneNumber], null, null, null, [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]\r\n"
          + "    FROM [dbo].[DevelopmentPartner]\r\n"
          + "    WHERE [ClassID] IN ('Customer', 'DevelopmentPartner')\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewWithAbstractClassWithSingleConcreteConcrete ()
    {
      _viewBuilder.AddView (PartnerClass);

      string expectedScript =
          "CREATE VIEW [dbo].[PartnerView] ([ID], [ClassID], [Timestamp], [AddressID], [Name], [PhoneNumber], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [AddressID], [Name], [PhoneNumber], [Description], [PartnerPropertyWithIdenticalNameInDifferentInheritanceBranches], [Competences]\r\n"
          + "    FROM [dbo].[DevelopmentPartner]\r\n"
          + "    WHERE [ClassID] IN ('DevelopmentPartner')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewWithDerivedClass ()
    {
      _viewBuilder.AddView (DerivedClass);

      string expectedScript =
          "CREATE VIEW [dbo].[DerivedClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInDerivedOfDerivedClass])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInDerivedOfDerivedClass]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "    WHERE [ClassID] IN ('DerivedClass', 'DerivedOfDerivedClass')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewWithAbstractWithoutConcreteTable ()
    {
      _viewBuilder.AddView (AbstractWithoutConcreteClass);

      Assert.IsEmpty (_viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViewTwice ()
    {
      _viewBuilder.AddView (OrderClass);
      _viewBuilder.AddView (ConcreteClass);

      string expectedScript =
          "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [CustomerID], [CustomerIDClassID], [Number], [OfficialID], [Priority])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [CustomerID], [CustomerIDClassID], [Number], [OfficialID], [Priority]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "    WHERE [ClassID] IN ('Order')\r\n"
          + "  WITH CHECK OPTION\r\n"
          + "GO\r\n\r\n"
          + "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [PropertyInSecondDerivedClass])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [PropertyInSecondDerivedClass]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "    WHERE [ClassID] IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void AddViews ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (OrderClass);
      classes.Add (ConcreteClass);

      _viewBuilder.AddViews (classes);

      string expectedScript =
          "CREATE VIEW [dbo].[OrderView] ([ID], [ClassID], [Timestamp], [CustomerID], [CustomerIDClassID], [Number], [OfficialID], [Priority])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [CustomerID], [CustomerIDClassID], [Number], [OfficialID], [Priority]\r\n"
          + "    FROM [dbo].[Order]\r\n"
          + "    WHERE [ClassID] IN ('Order')\r\n"
          + "  WITH CHECK OPTION\r\n"
          + "GO\r\n\r\n"
          + "CREATE VIEW [dbo].[ConcreteClassView] ([ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [PropertyInSecondDerivedClass])\r\n"
          + "  WITH SCHEMABINDING AS\r\n"
          + "  SELECT [ID], [ClassID], [Timestamp], [PropertyInConcreteClass], [PropertyInDerivedClass], [ClassWithRelationsInDerivedOfDerivedClassID], [PropertyInDerivedOfDerivedClass], [ClassWithRelationsInSecondDerivedClassID], [PropertyInSecondDerivedClass]\r\n"
          + "    FROM [dbo].[ConcreteClass]\r\n"
          + "    WHERE [ClassID] IN ('ConcreteClass', 'DerivedClass', 'DerivedOfDerivedClass', 'SecondDerivedClass')\r\n"
          + "  WITH CHECK OPTION\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetCreateViewScript());
    }

    [Test]
    public void GetDropViewScriptWithConcreteClass ()
    {
      _viewBuilder.AddView (OrderClass);

      string expectedScript =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[OrderView]\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetDropViewScript());
    }

    [Test]
    public void GetDropViewScriptWithAbstractClass ()
    {
      _viewBuilder.AddView (CompanyClass);

      string expectedScript =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[CompanyView]\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetDropViewScript());
    }

    [Test]
    public void GetDropViewScriptWithTwoClasses ()
    {
      _viewBuilder.AddView (OrderClass);
      _viewBuilder.AddView (CompanyClass);

      string expectedScript =
          "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'OrderView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[OrderView]\r\n\r\n"
          + "IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.Views WHERE TABLE_NAME = 'CompanyView' AND TABLE_SCHEMA = 'dbo')\r\n"
          + "  DROP VIEW [dbo].[CompanyView]\r\n";

      Assert.AreEqual (expectedScript, _viewBuilder.GetDropViewScript());
    }
  }
}
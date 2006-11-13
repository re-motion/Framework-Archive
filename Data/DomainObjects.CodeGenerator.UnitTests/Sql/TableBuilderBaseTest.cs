using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.CodeGenerator.Sql;

namespace Rubicon.Data.DomainObjects.CodeGenerator.UnitTests.Sql.SqlServer
{
  [TestFixture]
  public class TableBuilderBaseTest : MappingBaseTest
  {
    // types

    // static members and constants

    // member fields

    private MockRepository _mocks;
    private TableBuilderBase _mockTableBuilder;

    // construction and disposing

    public TableBuilderBaseTest ()
    {
    }

    // methods and properties
    
    public override void SetUp ()
    {
      base.SetUp ();

      _mocks = new MockRepository ();
      _mockTableBuilder = _mocks.CreateMock<TableBuilderBase> ();
    }

    [Test]
    public void AddTable ()
    {
      using (_mocks.Ordered ())
      {
        _mockTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull ()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _mockTableBuilder.AddToDropTableScript (null, null);
        LastCall.IgnoreArguments ();
      }
      _mocks.ReplayAll ();

      _mockTableBuilder.AddTable (OrderClass);
      string actualScript = _mockTableBuilder.GetCreateTableScript ();

      _mocks.VerifyAll ();
      Assert.AreEqual ("CREATE TABLE [dbo].[Order] ()", actualScript);
    }

    [Test]
    public void AddTableTwice ()
    {
      using (_mocks.Ordered ())
      {
        _mockTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull ()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _mockTableBuilder.AddToDropTableScript (null, null);
        LastCall.IgnoreArguments ();
        _mockTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull ()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _mockTableBuilder.AddToDropTableScript (null, null);
        LastCall.IgnoreArguments ();
      }
      _mocks.ReplayAll ();

      _mockTableBuilder.AddTable (OrderClass);
      _mockTableBuilder.AddTable (OrderClass);
      string actualScript = _mockTableBuilder.GetCreateTableScript ();

      _mocks.VerifyAll ();
      Assert.AreEqual ("CREATE TABLE [dbo].[Order] ()\nCREATE TABLE [dbo].[Order] ()", actualScript);
    }

    [Test]
    public void GetDropTableScript ()
    {
      using (_mocks.Ordered ())
      {
        _mockTableBuilder.AddToCreateTableScript (null, null);
        LastCall.IgnoreArguments ();
        _mockTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull ()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Order]"));
      }
      _mocks.ReplayAll ();

      _mockTableBuilder.AddTable (OrderClass);
      string actualScript = _mockTableBuilder.GetDropTableScript ();

      _mocks.VerifyAll ();
      Assert.AreEqual ("DROP TABLE [dbo].[Order]", actualScript);
    }

    [Test]
    public void GetDropTableScriptWithMultipleTables ()
    {
      using (_mocks.Ordered ())
      {
        _mockTableBuilder.AddToCreateTableScript (null, null);
        LastCall.IgnoreArguments ();
        _mockTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (CustomerClass), Is.NotNull ()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Customer]"));
        _mockTableBuilder.AddToCreateTableScript (null, null);
        LastCall.IgnoreArguments ();
        _mockTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull ()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Order]"));
      }
      _mocks.ReplayAll ();

      _mockTableBuilder.AddTable (CustomerClass);
      _mockTableBuilder.AddTable (OrderClass);
      string actualScript = _mockTableBuilder.GetDropTableScript ();

      _mocks.VerifyAll ();
      Assert.AreEqual ("DROP TABLE [dbo].[Customer]\nDROP TABLE [dbo].[Order]", actualScript);
    }

    [Test]
    public void AddTables ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (CustomerClass);
      classes.Add (OrderClass);

      using (_mocks.Ordered ())
      {
        _mockTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (CustomerClass), Is.NotNull ()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Customer] ()"));
        _mockTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (CustomerClass), Is.NotNull ()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Customer]"));
        _mockTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull ()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _mockTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull ()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Order]"));
      }
      _mocks.ReplayAll ();

      _mockTableBuilder.AddTables (classes);
      string actualCreateTableScript = _mockTableBuilder.GetCreateTableScript ();
      string actualDropTableScript = _mockTableBuilder.GetDropTableScript ();

      _mocks.VerifyAll ();
      Assert.AreEqual ("CREATE TABLE [dbo].[Customer] ()\nCREATE TABLE [dbo].[Order] ()", actualCreateTableScript);
      Assert.AreEqual ("DROP TABLE [dbo].[Customer]\nDROP TABLE [dbo].[Order]", actualDropTableScript);
    }

    [Test]
    public void AddTableWithAbstractClass ()
    {
      _mockTableBuilder.AddTable (CompanyClass);
      _mocks.ReplayAll ();

      string actualCreateTableScript = _mockTableBuilder.GetCreateTableScript ();
      string actualDropTableScript = _mockTableBuilder.GetDropTableScript ();

      _mocks.VerifyAll ();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    [Test]
    public void AddTableWithDerivedClassWithoutEntityName ()
    {
      _mockTableBuilder.AddTable (DerivedClass);
      _mocks.ReplayAll ();

      string actualCreateTableScript = _mockTableBuilder.GetCreateTableScript ();
      string actualDropTableScript = _mockTableBuilder.GetDropTableScript ();

      _mocks.VerifyAll ();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    [Test]
    public void AddTableWithDerivedClassWithEntityName ()
    {
      _mockTableBuilder.AddTable (SecondDerivedClass);
      _mocks.ReplayAll ();

      string actualCreateTableScript = _mockTableBuilder.GetCreateTableScript ();
      string actualDropTableScript = _mockTableBuilder.GetDropTableScript ();

      _mocks.VerifyAll ();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    [Test]
    public void AddTableWithDerivedOfDerivedClassWithEntityName ()
    {
      _mockTableBuilder.AddTable (DerivedOfDerivedClass);
      _mocks.ReplayAll ();

      string actualCreateTableScript = _mockTableBuilder.GetCreateTableScript ();
      string actualDropTableScript = _mockTableBuilder.GetDropTableScript ();

      _mocks.VerifyAll ();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    private Proc<ClassDefinition, StringBuilder> CreateAddToDropTableScriptDelegate (string statement)
    {
      return (Proc<ClassDefinition, StringBuilder>) delegate (ClassDefinition classDefinition, StringBuilder stringBuilder)
      {
        stringBuilder.Append (statement);
      };
    }

    private Proc<ClassDefinition, StringBuilder> CreateAddToCreateTableScriptDelegate (string statement)
    {
      return (Proc<ClassDefinition, StringBuilder>) delegate (ClassDefinition classDefinition, StringBuilder stringBuilder)
      {
        stringBuilder.Append (statement);
      };
    }
  }
}

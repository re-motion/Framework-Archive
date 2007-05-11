using System;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.RdbmsTools.SchemaGeneration;

namespace Rubicon.Data.DomainObjects.RdbmsTools.UnitTests.SchemaGeneration
{
  //TODO: Cover execution path from GetColumnList (ClassDefinition) to GetColumn (PropertyDefinition, bool)
  [TestFixture]
  public class TableBuilderBaseTest : StandardMappingTest
  {
    private MockRepository _mocks;
    private TableBuilderBase _stubTableBuilder;

    public override void SetUp ()
    {
      base.SetUp();

      _mocks = new MockRepository();
      _stubTableBuilder = _mocks.CreateMock<TableBuilderBase>();
    }

    [Test]
    public void AddTable ()
    {
      using (_mocks.Ordered())
      {
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.IgnoreArguments();
      }
      _mocks.ReplayAll();

      _stubTableBuilder.AddTable (OrderClass);
      string actualScript = _stubTableBuilder.GetCreateTableScript();

      _mocks.VerifyAll();
      Assert.AreEqual ("CREATE TABLE [dbo].[Order] ()", actualScript);
    }

    [Test]
    public void AddTableTwice ()
    {
      using (_mocks.Ordered())
      {
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.IgnoreArguments();
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.IgnoreArguments();
      }
      _mocks.ReplayAll();

      _stubTableBuilder.AddTable (OrderClass);
      _stubTableBuilder.AddTable (OrderClass);
      string actualScript = _stubTableBuilder.GetCreateTableScript();

      _mocks.VerifyAll();
      Assert.AreEqual ("CREATE TABLE [dbo].[Order] ()\r\nCREATE TABLE [dbo].[Order] ()", actualScript);
    }

    [Test]
    public void GetDropTableScript ()
    {
      using (_mocks.Ordered())
      {
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.IgnoreArguments();
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Order]"));
      }
      _mocks.ReplayAll();

      _stubTableBuilder.AddTable (OrderClass);
      string actualScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.AreEqual ("DROP TABLE [dbo].[Order]", actualScript);
    }

    [Test]
    public void GetDropTableScriptWithMultipleTables ()
    {
      using (_mocks.Ordered())
      {
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.IgnoreArguments();
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (CustomerClass), Is.NotNull()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Customer]"));
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.IgnoreArguments();
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Order]"));
      }
      _mocks.ReplayAll();

      _stubTableBuilder.AddTable (CustomerClass);
      _stubTableBuilder.AddTable (OrderClass);
      string actualScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.AreEqual ("DROP TABLE [dbo].[Customer]\r\nDROP TABLE [dbo].[Order]", actualScript);
    }

    [Test]
    public void AddTables ()
    {
      ClassDefinitionCollection classes = new ClassDefinitionCollection (false);
      classes.Add (CustomerClass);
      classes.Add (OrderClass);

      using (_mocks.Ordered())
      {
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (CustomerClass), Is.NotNull()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Customer] ()"));
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (CustomerClass), Is.NotNull()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Customer]"));
        _stubTableBuilder.AddToCreateTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull()).Do (CreateAddToCreateTableScriptDelegate ("CREATE TABLE [dbo].[Order] ()"));
        _stubTableBuilder.AddToDropTableScript (null, null);
        LastCall.Constraints (Is.Equal (OrderClass), Is.NotNull()).Do (CreateAddToDropTableScriptDelegate ("DROP TABLE [dbo].[Order]"));
      }
      _mocks.ReplayAll();

      _stubTableBuilder.AddTables (classes);
      string actualCreateTableScript = _stubTableBuilder.GetCreateTableScript();
      string actualDropTableScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.AreEqual ("CREATE TABLE [dbo].[Customer] ()\r\nCREATE TABLE [dbo].[Order] ()", actualCreateTableScript);
      Assert.AreEqual ("DROP TABLE [dbo].[Customer]\r\nDROP TABLE [dbo].[Order]", actualDropTableScript);
    }

    [Test]
    public void AddTableWithAbstractClass ()
    {
      _stubTableBuilder.AddTable (CompanyClass);
      _mocks.ReplayAll();

      string actualCreateTableScript = _stubTableBuilder.GetCreateTableScript();
      string actualDropTableScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    [Test]
    public void AddTableWithDerivedClassWithoutEntityName ()
    {
      _stubTableBuilder.AddTable (DerivedClass);
      _mocks.ReplayAll();

      string actualCreateTableScript = _stubTableBuilder.GetCreateTableScript();
      string actualDropTableScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    [Test]
    public void AddTableWithDerivedClassWithEntityName ()
    {
      _stubTableBuilder.AddTable (SecondDerivedClass);
      _mocks.ReplayAll();

      string actualCreateTableScript = _stubTableBuilder.GetCreateTableScript();
      string actualDropTableScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    [Test]
    public void AddTableWithDerivedOfDerivedClassWithEntityName ()
    {
      _stubTableBuilder.AddTable (DerivedOfDerivedClass);
      _mocks.ReplayAll();

      string actualCreateTableScript = _stubTableBuilder.GetCreateTableScript();
      string actualDropTableScript = _stubTableBuilder.GetDropTableScript();

      _mocks.VerifyAll();
      Assert.IsEmpty (actualCreateTableScript);
      Assert.IsEmpty (actualDropTableScript);
    }

    private Proc<ClassDefinition, StringBuilder> CreateAddToDropTableScriptDelegate (string statement)
    {
      return delegate (ClassDefinition classDefinition, StringBuilder stringBuilder) { stringBuilder.Append (statement); };
    }

    private Proc<ClassDefinition, StringBuilder> CreateAddToCreateTableScriptDelegate (string statement)
    {
      return delegate (ClassDefinition classDefinition, StringBuilder stringBuilder) { stringBuilder.Append (statement); };
    }
  }
}
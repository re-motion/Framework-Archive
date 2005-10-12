using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using System.Xml.Schema;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine.Mapping;
using Rubicon.Utilities;
using Rubicon.Web.UnitTests.AspNetFramework;

namespace Rubicon.Web.UnitTests.ExecutionEngine.Mapping
{

[TestFixture]
public class WxeMappingSchemaTest
{
  [SetUp]
  public virtual void SetUp()
  {
  }

  [TearDown]
  public virtual void TearDown()
  {
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaException))]
  public void LoadMappingWithMissingPath()
  {
    MappingConfiguration mapping = MappingConfiguration.LoadMappingFromFile ("UrlMappingWithMissingPath.xml");
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaException))]
  public void LoadMappingWithEmptyPath()
  {
    MappingConfiguration mapping = MappingConfiguration.LoadMappingFromFile ("UrlMappingWithEmptyPath.xml");
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaException))]
  public void LoadMappingWithMissingFunctionType()
  {
    MappingConfiguration mapping = MappingConfiguration.LoadMappingFromFile ("UrlMappingWithMissingFunctionType.xml");
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaException))]
  public void LoadMappingWithEmptyFunctionType()
  {
    MappingConfiguration mapping = MappingConfiguration.LoadMappingFromFile ("UrlMappingWithEmptyFunctionType.xml");
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaException))]
  public void LoadMappingWithFunctionTypeHavingNoAssembly()
  {
    MappingConfiguration mapping = MappingConfiguration.LoadMappingFromFile ("UrlMappingWithFunctionTypeHavingNoAssembly.xml");
    Assert.Fail();
  }
}

}

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
using Rubicon.Web.ExecutionEngine.UrlMapping;
using Rubicon.Utilities;
using Rubicon.Web.UnitTests.AspNetFramework;

namespace Rubicon.Web.UnitTests.ExecutionEngine.UrlMapping
{

[TestFixture]
public class UrlMappingSchemaTest
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
    UrlMappingConfiguration mapping = UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithMissingPath.xml");
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (Exception))]
  public void LoadMappingWithEmptyPath()
  {
    UrlMappingConfiguration mapping = UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithEmptyPath.xml");
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaException))]
  public void LoadMappingWithMissingFunctionType()
  {
    UrlMappingConfiguration mapping = UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithMissingFunctionType.xml");
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (Exception))]
  public void LoadMappingWithEmptyFunctionType()
  {
    UrlMappingConfiguration mapping = UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithEmptyFunctionType.xml");
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (Exception))]
  public void LoadMappingWithFunctionTypeHavingNoAssembly()
  {
    UrlMappingConfiguration mapping = UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithFunctionTypeHavingNoAssembly.xml");
    Assert.Fail();
  }
}

}

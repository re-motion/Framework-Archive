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
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithMissingPath.xml");
  }

  [Test]
  [ExpectedException (typeof (Exception))]
  public void LoadMappingWithEmptyPath()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithEmptyPath.xml");
  }

  [Test]
  [ExpectedException (typeof (XmlSchemaException))]
  public void LoadMappingWithMissingFunctionType()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithMissingFunctionType.xml");
  }

  [Test]
  [ExpectedException (typeof (Exception))]
  public void LoadMappingWithEmptyFunctionType()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithEmptyFunctionType.xml");
  }

  [Test]
  [ExpectedException (typeof (Exception))]
  public void LoadMappingWithFunctionTypeHavingNoAssembly()
  {
    UrlMappingConfiguration.CreateUrlMappingConfiguration (@"Res\UrlMappingWithFunctionTypeHavingNoAssembly.xml");
  }
}

}

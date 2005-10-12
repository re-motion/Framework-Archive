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
using Rubicon.Web.Configuration;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Web.ExecutionEngine.Mapping;
using Rubicon.Utilities;
using Rubicon.Web.UnitTests.AspNetFramework;
using Rubicon.Web.UnitTests.Configuration;

namespace Rubicon.Web.UnitTests.ExecutionEngine.Mapping
{

[TestFixture]
public class WxeMappingTest
{
  [SetUp]
  public virtual void SetUp()
  {
    MappingConfiguration.SetCurrent (null);
  }

  [TearDown]
  public virtual void TearDown()
  {
  }

  [Test]
  public void LoadMappingFromFile()
  {
    MappingConfiguration mapping = MappingConfiguration.LoadMappingFromFile ("UrlMapping.xml");

    Assert.IsNotNull (mapping, "Mapping is null.");
    
    Assert.IsNotNull (mapping.Rules, "Rules are null.");
    Assert.AreEqual (2, mapping.Rules.Count);

    Assert.IsNotNull (mapping.Rules[0], "First rule is null.");
    Assert.IsNotNull (mapping.Rules[1], "Second rule is null.");
    
    Assert.AreEqual ("Rubicon.Web.UnitTests.ExecutionEngine.Mapping.FirstMappedFunction, Rubicon.Web.UnitTests", mapping.Rules[0].FunctionType);
    Assert.AreEqual ("First.wxe", mapping.Rules[0].Path);
    Assert.AreEqual ("Rubicon.Web.UnitTests.ExecutionEngine.Mapping.SecondMappedFunction, Rubicon.Web.UnitTests", mapping.Rules[1].FunctionType);
    Assert.AreEqual ("Second.wxe", mapping.Rules[1].Path);
  }

  [Test]
  [ExpectedException (typeof (FileNotFoundException))]
  public void LoadMappingFromFileWithInvalidFilename()
  {
    MappingConfiguration mapping = MappingConfiguration.LoadMappingFromFile ("InvalidFilename.xml");
    Assert.Fail();
  }

  [Test]
  public void GetCurrentMapping()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineMapping();
    MappingConfiguration mapping = MappingConfiguration.Current;
    Assert.IsNotNull (mapping);
  }

  [Test]
  public void GetCurrentMappingFromConfiguration()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineMapping();
    MappingConfiguration mapping = MappingConfiguration.Current;

    Assert.IsNotNull (mapping, "Mapping is null.");
    
    Assert.IsNotNull (mapping.Rules, "Rules are null.");
    Assert.AreEqual (2, mapping.Rules.Count);

    Assert.IsNotNull (mapping.Rules[0], "First rule is null.");
    Assert.IsNotNull (mapping.Rules[1], "Second rule is null.");
    
    Assert.AreEqual ("Rubicon.Web.UnitTests.ExecutionEngine.Mapping.FirstMappedFunction, Rubicon.Web.UnitTests", mapping.Rules[0].FunctionType);
    Assert.AreEqual ("First.wxe", mapping.Rules[0].Path);
    Assert.AreEqual ("Rubicon.Web.UnitTests.ExecutionEngine.Mapping.SecondMappedFunction, Rubicon.Web.UnitTests", mapping.Rules[1].FunctionType);
    Assert.AreEqual ("Second.wxe", mapping.Rules[1].Path);
  }

  [Test]
  public void GetCurrentMappingFromConfigurationWithNoFilemane()
  {
    WebConfigurationMock.Current = WebConfigurationFactory.GetExecutionEngineMappingWithNoFilename();
    MappingConfiguration mapping = MappingConfiguration.Current;

    Assert.IsNotNull (mapping, "Mapping is null.");
    
    Assert.IsNotNull (mapping.Rules, "Rules are null.");
    Assert.AreEqual (0, mapping.Rules.Count);
  }
}

}

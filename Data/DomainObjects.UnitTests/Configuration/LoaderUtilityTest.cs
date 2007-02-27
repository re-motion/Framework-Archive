using System;
using System.Configuration;
using System.IO;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.ConfigurationLoader.FileBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration
{
  [TestFixture]
  public class LoaderUtilityTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public LoaderUtilityTest ()
    {
    }

    // methods and properties

    [Test]
    public void GetConfigurationFileName ()
    {
      ConfigurationManager.AppSettings["ConfigurationFileThatDoesNotExist"] = @"C:\NonExistingConfigurationFile.xml";

      Assert.AreEqual (
          ConfigurationManager.AppSettings["ConfigurationFileThatDoesNotExist"],
          LoaderUtility.GetConfigurationFileName ("ConfigurationFileThatDoesNotExist", "Mapping.xml"));
    }

    [Test]
    public void GetEmptyConfigurationFileName ()
    {
      ConfigurationManager.AppSettings["EmptyConfigurationFileName"] = string.Empty;

      Assert.AreEqual (string.Empty, LoaderUtility.GetConfigurationFileName ("EmptyConfigurationFileName", "Mapping.xml"));
    }

    [Test]
    public void GetConfigurationFileNameForNonExistingAppSettingsKey ()
    {
      Assert.AreEqual (
          Path.Combine (ReflectionUtility.GetExecutingAssemblyPath (), "Mapping.xml"),
          LoaderUtility.GetConfigurationFileName ("AppSettingKeyDoesNotExist", "Mapping.xml"));
    }
  }
}

using System.Configuration;
using Rubicon.Development.UnitTesting.IO;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public class ConfigurationFactory
  {
    public static System.Configuration.Configuration LoadConfigurationFromFile (TempFile tempFile, byte[] content)
    {
      tempFile.WriteAllBytes (content);

      ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
      fileMap.ExeConfigFilename = tempFile.FileName;
      return ConfigurationManager.OpenMappedExeConfiguration (fileMap, ConfigurationUserLevel.None);
    }
  }
}
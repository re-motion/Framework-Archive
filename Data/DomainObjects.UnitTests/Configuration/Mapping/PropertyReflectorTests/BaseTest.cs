using System.Reflection;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  public class BaseTest: ReflectionBasedMappingTest
  {
    protected PropertyReflector CreatePropertyReflector<T> (string property)
    {
      PropertyInfo propertyInfo = typeof (T).GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      return new PropertyReflector (propertyInfo);
    }
  }
}
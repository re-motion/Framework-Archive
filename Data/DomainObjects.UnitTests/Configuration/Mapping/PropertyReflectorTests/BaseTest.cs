using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTests
{
  public class BaseTest: StandardMappingTest
  {
    protected PropertyReflector CreatePropertyReflector<T> (string property)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("property", property);

      Type type = typeof (T);
      PropertyInfo propertyInfo = type.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ReflectionBasedClassDefinition classDefinition;
      if (typeof (DomainObject).IsAssignableFrom (type))
        classDefinition = new ReflectionBasedClassDefinition (type.Name, type.Name, c_testDomainProviderID, type, true);
      else
        classDefinition = new ReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false);

      return new PropertyReflector (classDefinition, propertyInfo);
    }
  }
}
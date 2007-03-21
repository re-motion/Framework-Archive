using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Apply the <see cref="BinaryAttribute"/> to properties of type <see cref="byte"/> array.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class BinaryAttribute : NullableLengthConstrainedPropertyAttribute
  {
    public BinaryAttribute ()
    {
    }
  }
}
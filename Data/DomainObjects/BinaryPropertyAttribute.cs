using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Apply the <see cref="BinaryPropertyAttribute"/> to properties of type <see cref="byte"/> array.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class BinaryPropertyAttribute : NullableLengthConstrainedPropertyAttribute
  {
    public BinaryPropertyAttribute ()
    {
    }
  }
}
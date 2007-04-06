using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Apply the <see cref="StringPropertyAttribute"/> to properties of type <see cref="string"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class StringPropertyAttribute: NullableLengthConstrainedPropertyAttribute
  {
    public StringPropertyAttribute()
    {
    }
  }
}
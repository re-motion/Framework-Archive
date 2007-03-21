using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Apply the <see cref="MandatoryAttribute"/> to properties of type <see cref="DomainObject"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class MandatoryAttribute: Attribute, INullablePropertyAttribute
  {
    public MandatoryAttribute()
    {
    }

    bool INullablePropertyAttribute.IsNullable
    {
      get { return false; }
    }
  }
}
using System;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping
{
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class StringPropertyAttribute: NullableLengthConstrainedPropertyAttribute
  {
    public StringPropertyAttribute()
    {
    }
  }
}
using System;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping
{
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class BinaryPropertyAttribute : NullableLengthConstrainedPropertyAttribute
  {
    public BinaryPropertyAttribute ()
    {
    }
  }
}
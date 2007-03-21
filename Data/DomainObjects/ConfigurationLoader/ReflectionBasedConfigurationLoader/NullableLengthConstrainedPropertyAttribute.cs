using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public abstract class NullableLengthConstrainedPropertyAttribute: NullablePropertyAttribute, ILengthConstrainedPropertyAttribute
  {
    private int? _maximumLength = null;

    protected NullableLengthConstrainedPropertyAttribute ()
    {
    }

    public int MaximumLength
    {
      get { return _maximumLength.Value; }
      set { _maximumLength = value; }
    }

    public bool HasMaximumLength
    {
      get { return _maximumLength.HasValue; }
    }

    int? ILengthConstrainedPropertyAttribute.MaximumLength
    {
      get { return _maximumLength; }
    }
  }
}
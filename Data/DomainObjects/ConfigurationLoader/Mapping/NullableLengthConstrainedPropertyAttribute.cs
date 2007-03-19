using System;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping
{
  public abstract class NullableLengthConstrainedPropertyAttribute: NullablePropertyAttribute, ILengthConstrainedPropertyAttribute
  {
    private int? _maximumLength;

    protected NullableLengthConstrainedPropertyAttribute ()
    {
    }

    public int MaximumLength
    {
      get { return _maximumLength ?? -1; }
      set { _maximumLength = value; }
    }

    int? ILengthConstrainedPropertyAttribute.MaximumLength
    {
      get { return _maximumLength; }
    }
  }
}
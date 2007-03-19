using System;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping
{
  public abstract class NullablePropertyAttribute: Attribute, INullablePropertyAttribute
  {
    private bool _isNullable = false;

    protected NullablePropertyAttribute()
    {
    }

    public bool IsNullable
    {
      get { return _isNullable; }
      set { _isNullable = value; }
    }
  }
}
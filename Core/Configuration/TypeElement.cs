using System;
using System.Configuration;

namespace Rubicon.Configuration
{
  public class TypeElement<T> : ConfigurationElement
  {
    private readonly ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _typeProperty;

    public TypeElement ()
    {
      _typeProperty = new ConfigurationProperty (
          "type",
          typeof (Type),
          null,
          new Rubicon.Utilities.TypeNameConverter (),
          new SubclassTypeValidator (typeof (T)),
          ConfigurationPropertyOptions.IsRequired);

      _properties = new ConfigurationPropertyCollection ();
      _properties.Add (_typeProperty);
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public Type Type
    {
      get { return (Type) base[_typeProperty]; }
      set { base[_typeProperty] = value; }
    }
  }
}

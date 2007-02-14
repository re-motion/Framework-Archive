using System;
using System.Configuration;

namespace Rubicon.Configuration
{
  public class TypeElement<TBase> : ConfigurationElement
      where TBase : class
  {
    private readonly ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _typeProperty;

    public TypeElement ()
      : this (null)
    {
    }

    protected TypeElement (Type defaultValue)
    {
      _typeProperty = new ConfigurationProperty (
          "type",
          typeof (Type),
          defaultValue,
          new Rubicon.Utilities.TypeNameConverter (),
          new SubclassTypeValidator (typeof (TBase)),
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

    public TBase CreateInstance ()
    {
      if (Type == null)
        return null;
        
      return (TBase) Activator.CreateInstance (Type);
    }
  }

  public class TypeElement<TBase, TDefault> : TypeElement<TBase>
    where TBase : class
    where TDefault : TBase
  {
    public TypeElement ()
      : base (typeof (TDefault))
    {
    }
  }
}

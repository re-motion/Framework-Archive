using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Rubicon.Security.Configuration
{
  public class UserProviderElement : ConfigurationElement
  {
    private readonly ConfigurationPropertyCollection _properties;
    private readonly ConfigurationProperty _typeProperty;

    public UserProviderElement ()
    {
      _typeProperty = new ConfigurationProperty (
          "type", 
          typeof (Type), 
          null,
          new Rubicon.Utilities.TypeNameConverter (), 
          new SubclassTypeValidator (typeof (IUserProvider)), 
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

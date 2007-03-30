using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
  //TODO: Doc
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class DBTableAttribute: Attribute, IStorageSpecificIdentifierAttribute
  {
    private string _name;

    public DBTableAttribute()
    {
    }

    public string Name
    {
      get { return _name; }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty ("value", value);
        _name = value;
      }
    }

    string IStorageSpecificIdentifierAttribute.Identifier
    {
      get { return Name; }
    }
  }
}
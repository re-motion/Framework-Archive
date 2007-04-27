using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  //TODO: Doc
  public class RdbmsClassReflector: ClassReflector
  {
    public RdbmsClassReflector (Type type)
        : base (type)
    {
    }

    public override string GetStorageSpecificIdentifier()
    {
      if (IsTable())
        return base.GetStorageSpecificIdentifier();
      return null;
    }

    private bool IsTable()
    {
      return Attribute.IsDefined (Type, typeof (DBTableAttribute), false);
    }
  }
}
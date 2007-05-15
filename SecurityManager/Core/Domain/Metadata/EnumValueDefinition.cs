using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.Metadata
{
  [Serializable]
  [DBTable]
  public abstract class EnumValueDefinition : MetadataObject
  {
    protected EnumValueDefinition ()
    {
    }

    public abstract int Value { get; set; }
  }
}

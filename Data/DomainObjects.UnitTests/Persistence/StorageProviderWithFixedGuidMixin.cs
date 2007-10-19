using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
  public class StorageProviderWithFixedGuidMixin : IStorageProviderWithFixedGuid
  {
    private Guid _fixedGuid = Guid.NewGuid ();

    [OverrideTarget]
    public ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      return new ObjectID (classDefinition, FixedGuid);
    }

    public Guid FixedGuid
    {
      get { return _fixedGuid; }
      set { _fixedGuid = value; }
    }
  }

  public interface IStorageProviderWithFixedGuid
  {
    Guid FixedGuid { get; set; }
  }
}
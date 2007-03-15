using System.Configuration;
using Rubicon.Data.DomainObjects.Persistence.Configuration;

namespace Rubicon.Data.DomainObjects.Configuration
{
  public interface IDomainObjectsConfiguration
  {
    PersistenceConfiguration Storage { get; }
  }
}
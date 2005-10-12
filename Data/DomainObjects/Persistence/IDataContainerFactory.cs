using System;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.Persistence
{
public interface IDataContainerFactory
{
  DataContainer CreateDataContainer ();
  DataContainerCollection CreateCollection ();
}
}

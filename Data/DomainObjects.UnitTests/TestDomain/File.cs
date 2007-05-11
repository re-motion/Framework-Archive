using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  public abstract class File : FileSystemItem
  {
    protected File()
    {
    }

    protected File (DataContainer dataContainer)
        : base (dataContainer)
    {
    }
  }
}

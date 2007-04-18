using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [NotAbstract]
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

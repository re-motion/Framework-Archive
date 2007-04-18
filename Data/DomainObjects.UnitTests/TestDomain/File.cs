using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [NotAbstract]
  public abstract class File : FileSystemItem
  {
    public static new File NewObject ()
    {
      return DomainObject.NewObject<File> ().With();
    }

    protected File()
    {
    }

    protected File (DataContainer dataContainer)
        : base (dataContainer)
    {
    }
  }
}

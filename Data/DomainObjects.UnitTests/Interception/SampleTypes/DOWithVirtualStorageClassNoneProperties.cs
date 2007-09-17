using System;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  public class DOWithVirtualStorageClassNoneProperties : DomainObject
  {
    [StorageClassNone]
    public virtual int PropertyWithGetterAndSetter
    {
      get { return 0; }
      set { Dev.Null = value; }
    }
  }
}
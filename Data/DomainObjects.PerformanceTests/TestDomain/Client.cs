using System;

namespace Rubicon.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class Client : DomainObject
  {
    public static Client NewObject()
    {
      return NewObject<Client>().With();
    }

    protected  Client()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength= 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Client")]
    public abstract ObjectList<File> Files { get; }

    [DBBidirectionalRelation ("Client")]
    public abstract ObjectList<ClientBoundBaseClass> ClientBoundBaseClasses { get; }
  }
}